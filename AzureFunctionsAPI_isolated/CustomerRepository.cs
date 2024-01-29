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
                DateOfBirth = DateOnly.Parse("1978-01-28"),
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

        public Customer? GetCustomerById(string customerId)
        {
            var result = new Customer(); // todo - make customer null if not found
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
        public Customer? GetCustomerByAge(int age)
        {
            var lowDate = DateTime.Today.AddYears(-(age+1)).AddDays(1).ToString("yyyy-MM-dd");
            var highDate = DateTime.Today.AddYears(-age).ToString("yyyy-MM-dd");
            var result = new Customer(); // todo - make customer null if not found
            var command = _connection.CreateCommand();
            command.CommandText = "SELECT * from Customers WHERE DateOfBirth BETWEEN @lowDob AND @highDob LIMIT 1";
            command.Parameters.AddWithValue("lowDob", lowDate);
            command.Parameters.AddWithValue("highDob", highDate);
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

        public void CreateCustomer(Customer customerToInsert)
        {
            var command = _connection.CreateCommand();
            command.CommandText = "INSERT INTO Customers (CustomerId, FullName, DateOfBirth, ProfileImage) VALUES (@CustomerId, @FullName, @DateOfBirth, @ProfileImage)";
            command.Parameters.AddWithValue("@CustomerId", customerToInsert.CustomerId);
            command.Parameters.AddWithValue("@FullName", customerToInsert.FullName);
            command.Parameters.AddWithValue("@DateOfBirth", customerToInsert.DateOfBirth);
            command.Parameters.AddWithValue("@ProfileImage", customerToInsert.ProfileImage);
            command.ExecuteNonQuery();
        }
    }

    public interface ICustomerRepository
    {
        public Customer? GetCustomerById(string customerId);
        public Customer? GetCustomerByAge(int age);
        public void CreateCustomer(Customer customer);
    }
}
