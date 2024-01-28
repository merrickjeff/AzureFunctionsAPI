using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace AzureFunctionsAPI_isolated
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly SqliteConnection _connection;
        public CustomerRepository()
        {
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
            var command = _connection.CreateCommand();
            command.CommandText = "INSERT INTO Customers (CustomerId, FullName, DateOfBirth, ProfileImage) VALUES (@CustomerId, @FullName, @DateOfBirth, @ProfileImage)";
            command.Parameters.AddWithValue("@CustomerId", customerToInsert.CustomerId);
            command.Parameters.AddWithValue("@FullName", customerToInsert.FullName);
            command.Parameters.AddWithValue("@DateOfBirth", customerToInsert.DateOfBirth);
            command.Parameters.AddWithValue("@ProfileImage", customerToInsert.ProfileImage);
            command.ExecuteNonQuery();

        }

        public Customer? GetCustomer(string customerId)
        {
            var result = new Customer();
            var command = _connection.CreateCommand();
            command.CommandText = "SELECT * from Customers WHERE CustomerId = @customerId LIMIT 1";
            var guidParameter = new SqliteParameter("@customerId", customerId.ToUpper()) { DbType = DbType.String };
            command.Parameters.Add(guidParameter);
            var sqReader = command.ExecuteReader()!;
            while (sqReader.Read())
            {
                result.CustomerId = Guid.Parse(sqReader.GetString("CustomerId"));
                result.FullName = sqReader.GetString("FullName");
                result.DateOfBirth = DateOnly.Parse(sqReader.GetString("DateOfBirth"));
                result.ProfileImage = sqReader.GetString("ProfileImage");
            }
            return result;



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
            //_connection.Execute(sql, customerToInsert);
            return customerToInsert;

        }
    }

    public interface ICustomerRepository
    {
        public Customer? GetCustomer(string customerId);
        public Customer CreateCustomer(string customerId, string fullName, string dateOfBirth, string profileImage);
    }
}
