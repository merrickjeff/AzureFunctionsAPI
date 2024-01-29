using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using FromBodyAttribute = Microsoft.Azure.Functions.Worker.Http.FromBodyAttribute;

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
        public IActionResult GetCustomers([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Customers/{idOrAge}")] HttpRequest req, string? idOrAge, FunctionContext executionContext)
        {
            int age;
            Debug.Assert(idOrAge != null, nameof(idOrAge) + " != null");
            Customer? customer;
            var isGuid = Guid.TryParse(idOrAge, out _);

            var isAge = int.TryParse(idOrAge, out age);

            if (isGuid)
            {
                customer = _customerRepository.GetCustomerById(idOrAge);
            }
            else if (isAge)
            {
                customer = _customerRepository.GetCustomerByAge(age);
            }
            else
            {
                return new BadRequestObjectResult("Id or Age provided could not be parsed");
            }

            _logger.LogInformation("C# HTTP trigger function processed a request.");
            Debug.Assert(idOrAge != null, nameof(idOrAge) + " != null"); // TODO - return an error if null
            if (customer is null)
            {
                return new NotFoundObjectResult("The customer could not be found");
            }

            return new JsonResult(customer);
        }

        [Function("PostCustomers")]
        public IActionResult PostCustomers([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Customers")] HttpRequest req, [FromBody] Customer customer)
        {
            //string requestBody = new StreamReader(req.Body).ReadToEndAsync().Result;
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            _customerRepository.CreateCustomer(customer);
            return new OkObjectResult("Welcome to Azure Functions!");
        }

    }
}
