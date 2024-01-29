using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
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
            var result = new Customer();
            var command = _connection.CreateCommand();
            command.CommandText = "SELECT * from Customers WHERE CustomerId = @customerId LIMIT 1";
            var guidParameter = new SqliteParameter("@customerId", customerId.ToUpper()) { DbType = DbType.String };
            command.Parameters.Add(guidParameter);
            var sqlReader = command.ExecuteReader()!;
            if (sqlReader.HasRows)
            {
                while (sqlReader.Read())
                {
                    result.CustomerId = Guid.Parse(sqlReader.GetString("CustomerId"));
                    result.FullName = sqlReader.GetString("FullName");
                    result.DateOfBirth = DateOnly.Parse(sqlReader.GetString("DateOfBirth"));
                    result.ProfileImage = sqlReader.GetString("ProfileImage");
                }
                return result;
            }

            return null;
        }
        public Customer? GetCustomerByAge(int age)
        {
            var result = new Customer();

            var lowDate = DateTime.Today.AddYears(-(age + 1)).AddDays(1).ToString("yyyy-MM-dd");
            var highDate = DateTime.Today.AddYears(-age).ToString("yyyy-MM-dd");
            var command = _connection.CreateCommand();
            command.CommandText = "SELECT * from Customers WHERE DateOfBirth BETWEEN @lowDob AND @highDob LIMIT 1";
            command.Parameters.AddWithValue("lowDob", lowDate);
            command.Parameters.AddWithValue("highDob", highDate);
            var sqlReader = command.ExecuteReader()!;
            if (sqlReader.HasRows)
            {
                while (sqlReader.Read())
                {
                    result.CustomerId = Guid.Parse(sqlReader.GetString("CustomerId"));
                    result.FullName = sqlReader.GetString("FullName");
                    result.DateOfBirth = DateOnly.Parse(sqlReader.GetString("DateOfBirth"));
                    result.ProfileImage = sqlReader.GetString("ProfileImage");
                }
                return result;
            }

            return null;
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
