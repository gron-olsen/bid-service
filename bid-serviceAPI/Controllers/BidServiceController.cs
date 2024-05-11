using Microsoft.AspNetCore.Mvc;
using bidService.Models;

namespace bidserviceAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class BidServiceController : ControllerBase
{

    private readonly ILogger<BidServiceController> _logger;

    public BidServiceController(ILogger<BidServiceController> logger)
    {
        _logger = logger;
    }

      [HttpGet("version")]
    public IEnumerable<string> Get()
    {
        var properties = new List<string>();
        var assembly = typeof(Program).Assembly;
        foreach (var attribute in assembly.GetCustomAttributesData())
        {
            properties.Add($"{attribute.AttributeType.Name} - {attribute.ToString()}");
        }
        return properties;
    
    }
}
