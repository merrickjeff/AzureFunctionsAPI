using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.Sqlite;

namespace AzureFunctionsAPI_isolated
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly SqliteConnection _connection;
        public CustomerRepository()
        {
            SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());
            SqlMapper.AddTypeHandler(new GuidTypeHandler());
            SqlMapper.RemoveTypeMap(typeof(Guid));
            SqlMapper.RemoveTypeMap(typeof(Guid?));
            // Create a new in-memory Sqlite database and the Customers table
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            var createCommand = _connection.CreateCommand();
            createCommand.CommandText =
                @"
                CREATE TABLE Customers (
                    CustomerId TEXT,
                    FullName TEXT,
                    DateOfBirth TEXT,
                    ProfileImage TEXT
                )
            ";
            var result = createCommand.ExecuteNonQuery();

            var customerToInsert = new Customer()
            {
                CustomerId = Guid.Parse("e4e7d865-4720-4e61-a625-56f7a182c3da"),
                FullName = "Test Person",
                DateOfBirth = DateOnly.Parse("02-03-1978"),
                ProfileImage = "http://urltoimage.com/asdf"
            };
            var sql = "INSERT INTO Customers (CustomerId, FullName, DateOfBirth, ProfileImage) VALUES (@CustomerId, @FullName, @DateOfBirth, @ProfileImage)";
            _connection.Execute(sql, customerToInsert);
        }

        public Customer? GetCustomer(string customerId)
        {
            return _connection.QueryFirstOrDefault<Customer>("SELECT * from Customers WHERE CustomerId = @customerId", new {customerId});
            
        }

        public Customer CreateCustomer(string customerId, string fullName, string dateOfBirth, string profileImage)
        {
            var customerToInsert = new Customer()
            {
                CustomerId = Guid.Parse(customerId),
                FullName = fullName,
                DateOfBirth = DateOnly.Parse(dateOfBirth),
                ProfileImage = profileImage
            };
            var sql = "INSERT INTO Customers (CustomerId, FullName, DateOfBirth, ProfileImage) VALUES (@CustomerId, @FullName, @DateOfBirth, @ProfileImage)";
            _connection.Execute(sql, customerToInsert);
            return customerToInsert;

        }
    }

    public interface ICustomerRepository
    {
        public Customer? GetCustomer(string customerId);
        public Customer CreateCustomer(string customerId, string fullName, string dateOfBirth, string profileImage);
    }
    public class DateOnlyTypeHandler : SqlMapper.TypeHandler<DateOnly>
    {
        public override DateOnly Parse(object value) => DateOnly.FromDateTime((DateTime)value);

        public override void SetValue(IDbDataParameter parameter, DateOnly value)
        {
            parameter.DbType = DbType.Date;
            parameter.Value = value;
        }
    }
    public class GuidTypeHandler : SqlMapper.TypeHandler<Guid>
    {
        public override void SetValue(IDbDataParameter parameter, Guid guid)
        {
            parameter.Value = guid.ToString();
        }

        public override Guid Parse(object value)
        {
            return new Guid((string)value);
        }
    }
}
