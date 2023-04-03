using System;
using Microsoft.AspNetCore.Mvc;

namespace PaymentService.Controllers
{
    [ApiController]
    [Route("/api/v1/helloworld")]
    public class HelloWorldController : ControllerBase
    {

        [HttpGet]
        public String Get()
        {
            return "Hello World";
        }
    }
}
