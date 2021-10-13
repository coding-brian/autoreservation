using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Model.Line;
using Newtonsoft.Json.Linq;
using Service.WebAPIRequest;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Encodings.Web;
using System.Text.Json;
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

            if (messages.events.Count > 0)
            {
                foreach (var messageevent in messages.events)
                {

                    if (messageevent.message.type == "text")
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
                            else
                            {
                                messgae = GenerateMessage("一般性回覆");

                            }
                        }
                    }
                    await ReplyMessage(messageevent.replyToken, messgae);
                }
            }

            return Ok();
        }

        [HttpGet("test")]
        public async Task PostBackMessage(string data, string message)
        {
            var imageCarouselMessage = new ImageCarouselMessage();

            var messageObjct = new
            {
                type = "message",
                label = "",
                text = message
            };

            var column = new Colums();
            column.imageUrl = "https://i.imgur.com/YH04t4q_d.webp?maxwidth=760&fidelity=grand";
            column.action = ActinoGenerate("message", messageObjct);
            //column.action["label"] = "教練1";
            //column.action["text"] = "";

            imageCarouselMessage.template.columns.Add(column);

            var bbb = JsonSerializer.Serialize(column);

            var aaa = JsonSerializer.Serialize(imageCarouselMessage);

            var body = new ReplyMessageRequest();
            body.replyToken = "";
            body.messages.Add(imageCarouselMessage);

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Authorization", $"Bearer {_lineaccesstoken}");
            var result = await _webAPIRequest.WebRequest<ReplyMessageRequest>(_configuration["Line:ReplyMessageURL"].ToString(), HttpMethod.Post, headers, body);
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

            var result = await _webAPIRequest.WebRequest<ReplyMessageRequest>(_configuration["Line:ReplyMessageURL"].ToString(), HttpMethod.Post, headers, body);

            Console.Out.WriteLine("ReplyMessage:" + JsonSerializer.Serialize(result));
        }


        private ImageCarouselMessage GenerateMessage(string text)
        {
            var imageCarouselMessage = new ImageCarouselMessage();

            var column = new Colums();
            column.imageUrl = "https://i.imgur.com/YH04t4q_d.webp?maxwidth=760&fidelity=grand";
            var messageObjct = new PostBackAction
            {
                type = "postback",
                label = "教練1",
                text = "你好，我是教練1，以下是我目前可以預約的時間",
                data="showreservation=true&&coach=1"
            };
            column.action = ActinoGenerate("postback", messageObjct);


            var column2 = new Colums();
            column2.imageUrl = "https://i.imgur.com/q7u49Mj.jpg";
            var messageObjct2 = new PostBackAction
            {
                type = "postback",
                label = "教練2",
                text = "你好，我是教練2，以下是我目前可以預約的時間",
                data = "showreservation=true&&coach=2"
            };
            column2.action = ActinoGenerate("postback", messageObjct2);


            var column3 = new Colums();
            column3.imageUrl = "https://i.imgur.com/lAAOAL2.jpg";
            var messageObject3 = new PostBackAction
            {
                type = "postback",
                label = "教練3",
                text = "你好，我是教練3，以下是我目前可以預約的時間",
                data = "showreservation=true&&coach=3"
            };
            column3.action = ActinoGenerate("postback", messageObject3);


            var column4 = new Colums();
            column4.imageUrl = "https://i.imgur.com/swePqYQ_d.webp?maxwidth=760&fidelity=grand";
            var messageObject4 = new PostBackAction
            {
                type = "postback",
                label = "教練4",
                text = "你好，我是教練4，以下是我目前可以預約的時間",
                data = "showreservation=true&&coach=4"
            };
            column4.action = ActinoGenerate("postback", messageObject4);


            imageCarouselMessage.template.columns.Add(column);
            imageCarouselMessage.template.columns.Add(column2);
            imageCarouselMessage.template.columns.Add(column3);
            imageCarouselMessage.template.columns.Add(column4);
            imageCarouselMessage.altText = "歡迎你選擇";

            return imageCarouselMessage;
        }

        private Object ActinoGenerate(string actiontype, Object dataObject)
        {
            var action = new Object();
            var type = dataObject.GetType();
            try
            {
                switch (actiontype)
                {
                    case "message":
                        action = new MessageAction();
                        break;
                    case "postback":
                        action = new PostBackAction();
                        break;
                }

                Parallel.ForEach(type.GetProperties(), (property) =>
                {

                    var value = property.GetValue(dataObject);
                    Parallel.ForEach((action.GetType().GetProperties()), (test) =>
                    {
                        if (test.Name == property.Name)
                        {
                            test.SetValue(action, value);
                        }
                    });
                });
            }
            catch (Exception e)
            {
                throw;
            }


            return action;
        }
    }
}
