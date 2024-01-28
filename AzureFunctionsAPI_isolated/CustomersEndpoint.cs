using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AzureFunctionsAPI_isolated
{
    public class CustomersGetEndpoint
    {
        private readonly ILogger<CustomersGetEndpoint> _logger;

        public CustomersGetEndpoint(ILogger<CustomersGetEndpoint> logger)
        {
            _logger = logger;
        }

        [Function("GetCustomers")]
        public IActionResult GetCustomers([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Customer")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult("Welcome to Azure Functions!");
        }

        [Function("PostCustomers")]
        public IActionResult PostCustomers([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Customer")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult("Welcome to Azure Functions!");
        }

    }
}
