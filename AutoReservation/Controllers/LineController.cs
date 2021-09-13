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
using Action = Model.Line.Action;

namespace AutoReservation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LineController : ControllerBase
    {

        private string _lineaccesstoken = "dGRpItttHICbmITq95Lii7tw67AfhAVj4EJJeuDHwC4EHWGWZeIjDAyH+wst0IPikKvLSoJN7J/N1m7sgWiiX6M5aMGGHgDsT5KgnR0LjEDLEleI4M+lQ3f4lNVmenXmV3TtT6WFO7csubyJSPYC0gdB04t89/1O/w1cDnyilFU=";

        private readonly IConfiguration _configuration;

        private readonly IWebAPIRequest _webAPIRequest;

        private readonly string keywords = "\u9810\u7D04";

        public LineController(IConfiguration configuration, IWebAPIRequest webAPIRequest) 
        {

            _configuration = configuration;
            _webAPIRequest = webAPIRequest;
        }

        [HttpGet("testlog")]
        public string TestLog()
        {
            Console.Out.Write("testestsets");
            Console.Out.WriteLine("測試測試");
            return "OK";
        }

        [HttpPost("webhook")]

        public async Task<IActionResult> LineWebhook([FromBody] LineMessage messages) 
        {
            Console.Out.WriteLine("Receive:" + JsonSerializer.Serialize(messages));

            var messgae = new ImageCarouselMessage();

            if (messages.events.Count>0) 
            {
                foreach (var messageevent in messages.events) 
                {

                    if (messageevent.message.type=="text") 
                    {
                        if (string.IsNullOrEmpty(messageevent.replyToken))
                        {
                            PushMessage();
                        }
                        else
                        {

                            if (messageevent.message.text.Contains(keywords))
                            {
                                 messgae = GenerateMessage("好的馬上提給您我們的教練");
                            }
                            else {
                                messgae= GenerateMessage("一般性回覆");
                                
                            }
                        }
                    }
                    await ReplyMessage(messageevent.replyToken, messgae);
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
        private async Task ReplyMessage(string replytoken, Object replymessage) 
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Authorization", $"Bearer {_lineaccesstoken}");

            var body = new ReplyMessageRequest();
            body.replyToken = replytoken;
            body.messages.Add(replymessage);

            var result=await _webAPIRequest.WebRequest<ReplyMessageRequest>(_configuration["Line:ReplyMessageURL"].ToString(),HttpMethod.Post ,headers, body);

            Console.Out.WriteLine("ReplyMessage:" + JsonSerializer.Serialize(result));
        }


        private ImageCarouselMessage GenerateMessage(string text) 
        {
            var imageCarouselMessage = new ImageCarouselMessage();

            var column = new Colums();
            column.imageUrl = "https://i.imgur.com/YH04t4q_d.webp?maxwidth=760&fidelity=grand";
            column.action.label = "教練1";
            column.action.text= text;

            imageCarouselMessage.template.columns.Add(column);
            imageCarouselMessage.altText = "教練1教練1";

            return imageCarouselMessage;
        }
    }
}
