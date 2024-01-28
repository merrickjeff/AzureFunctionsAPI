using System;
using System.Collections.Generic;
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
            // Create a new in-memory Sqlite database and the Customers table
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            var createCommand = _connection.CreateCommand();
            createCommand.CommandText =
                @"
                CREATE TABLE Customers (
                    CustomerId TEXT,
                    FullName TEXT
                    DateOfBirth TEXT,
                    ProfileImage TEXT
                )
            ";
            createCommand.ExecuteNonQuery();
        }

        public Customer? GetCustomer(string customerId)
        {
            return _connection.QueryFirstOrDefault<Customer>("SELECT * from Customers WHERE CustomerId = @id", customerId);
            
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
}
