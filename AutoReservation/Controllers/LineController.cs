using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Model.Line;
using Service.WebAPIRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AutoReservation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LineController : ControllerBase
    {

        private string _lineaccesstoken = "dGRpItttHICbmITq95Lii7tw67AfhAVj4EJJeuDHwC4EHWGWZeIjDAyH+wst0IPikKvLSoJN7J/N1m7sgWiiX6M5aMGGHgDsT5KgnR0LjEDLEleI4M+lQ3f4lNVmenXmV3TtT6WFO7csubyJSPYC0gdB04t89/1O/w1cDnyilFU=";

        private readonly IConfiguration _configuration;

        private readonly IWebAPIRequest _webAPIRequest;

        public LineController(IConfiguration configuration, IWebAPIRequest webAPIRequest) {

            _configuration = configuration;
            _webAPIRequest = webAPIRequest;
        }

        [HttpGet("testlog")]
        public string TestLog()
        {
            Console.Out.Write("testestsets");
            Console.Out.WriteLine("123456789");
            return "OK";
        }

        [HttpPost("webhook")]

        public async Task<IActionResult> LineWebhook([FromBody] LineMessage messages) 
        {
            Console.Out.WriteLine("Receive:" + JsonSerializer.Serialize(messages));

            if (messages.events.Count>0) 
            {
                foreach (var messageevent in messages.events) 
                {
                    if (string.IsNullOrEmpty(messageevent.replyToken))
                    {
                        PushMessage();
                    }
                    else {
                        await ReplyMessage(messageevent.replyToken, messageevent.message.text);
                    }
                }
            }

            return Ok();
        }


        /// <summary>
        /// 用push api 回復
        /// </summary>
        private void PushMessage() 
        {
        
        
        }

        /// <summary>
        /// 用reply api 回復
        /// </summary>
        private async Task ReplyMessage(string replytoken,string replymessage) 
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Authorization", $"Bearer {_lineaccesstoken}");

            var body = new ReplyMessageRequest();
            body.replyToken = replytoken;
            var temp = new ReplyMessages();
            temp.type = "text";
            temp.text = replymessage;
            body.messages.Add(temp);

            var result=await _webAPIRequest.WebRequest<ReplyMessageRequest>(_configuration["Line:ReplyMessageURL"].ToString(),HttpMethod.Post ,headers, body);

            Console.Out.WriteLine("ReplyMessage:" + JsonSerializer.Serialize(result));
        }
    }
}
