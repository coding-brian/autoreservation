using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model.Line;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AutoReservation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LineController : ControllerBase
    {

        [HttpGet("testlog")]
        public string TestLog()
        {
            Console.Out.Write("testestsets");
            Console.Out.WriteLine("123456789");
            return "OK";
        }

        [HttpPost("webhook")]

        public IActionResult LineWebhook([FromBody] LineMessage messages) 
        {
            Console.Out.WriteLine(JsonSerializer.Serialize(messages));
            return Ok();
        }


        /// <summary>
        /// call message api 回復
        /// </summary>
        private void LineResponse() {
        
        
        }
    }
}
