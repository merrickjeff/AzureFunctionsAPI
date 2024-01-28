using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AzureFunctionsAPI_isolated
{
    public class CustomersGetEndpoint
    {
        private readonly ILogger<CustomersGetEndpoint> _logger;
        private readonly ICustomerRepository _customerRepository;

        public CustomersGetEndpoint(ILogger<CustomersGetEndpoint> logger, ICustomerRepository customerRepository)
        {
            _logger = logger;
            _customerRepository = customerRepository;
        }

        [Function("GetCustomers")]
        public IActionResult GetCustomers([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Customers/{id}")] HttpRequest req, string? id, FunctionContext executionContext)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            Debug.Assert(id != null, nameof(id) + " != null"); // TODO - return an error if null
            var customer = _customerRepository.GetCustomer(id);
            Debug.Assert(customer != null, nameof(customer) + " != null"); // TODO - return an error if null
            return new OkObjectResult("Welcome to Azure Functions!" + $" customer.CustomerId = {customer.CustomerId}");
            //return new OkObjectResult("Welcome to Azure Functions!" + $" customer.CustomerId = {customer.CustomerId}");
        }

        [Function("PostCustomers")]
        public IActionResult PostCustomers([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Customers")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult("Welcome to Azure Functions!");
        }

    }
}
