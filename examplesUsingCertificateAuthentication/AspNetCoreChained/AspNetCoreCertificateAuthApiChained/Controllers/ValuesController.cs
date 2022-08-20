using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreCertificateAuthApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ValuesController : ControllerBase
{
    // GET api/values
    [HttpGet]
    public ActionResult<IEnumerable<string>> Get()
    {
        return new string[] { "value1", "value2" };
    }

    // GET api/values/5
    [HttpGet("{id}")]
    public ActionResult<string> Get(int id)
    {
        return $"value:{id}";
    }

    // POST api/values
    [HttpPost]
    public void Post([FromBody] string value)
    {
        Console.WriteLine($"post api/values {value}");
    }

    // PUT api/values/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value)
    {
        Console.WriteLine($"put api/values {value}:{id}");
    }

    // DELETE api/values/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
        Console.WriteLine($"delete api/values value:{id}");
    }
}
