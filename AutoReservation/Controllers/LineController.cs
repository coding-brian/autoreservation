using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Model;
using Model.Enum;
using Model.Line;
using Repository;
using Service;
using Service.GenerateMessage;
using Service.MessageFatcory;
using Service.WebAPIRequest;
using Service.WordProcess;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

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

        private readonly string select = "\u67e5\u8a62";

        private readonly ICoachRepository _coachRepository;

        private readonly ICoachService _coachService;

        private readonly IMessageFactory _messageFactory;

        private readonly IChangeUserCoachProcess _changeUserCoachProcess;

        private readonly IGenerateMessage _generateMessage;

        public LineController(IConfiguration configuration, IWebAPIRequest webAPIRequest, ICoachRepository coachRepository, ICoachService coachService, IMessageFactory messageFactory, IChangeUserCoachProcess changeUserCoachProcess, IWordPrcoessFactory wordPrcoessFactory, IGenerateMessage generateMessage)
        {

            _configuration = configuration;
            _webAPIRequest = webAPIRequest;
            _coachRepository = coachRepository;
            _coachService = coachService;
            _messageFactory = messageFactory;
            _changeUserCoachProcess = changeUserCoachProcess;
            _generateMessage = generateMessage;
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> LineWebhook([FromBody] LineMessage messages)
        {
            var returnMessage = new object();

            if (messages.events.Count > 0)
            {
                foreach (var messageevent in messages.events)
                {
                    var userId = messageevent.source.userId;                    

                    switch (messageevent.type)
                    {
                        case "message":
                            if (messageevent.message.type == "text")
                            {
                                if (string.IsNullOrEmpty(messageevent.replyToken))
                                {
                                    PushMessage();
                                }
                                else
                                {
                                    var factory = new WordPrcoessFactory(_coachRepository, _changeUserCoachProcess, _generateMessage);
                                    var wordProcess = factory.Create(messageevent.message.text, userId);
                                    returnMessage = await wordProcess.ProcessWord(userId);
                                }
                            }
                            break;
                        case "postback":
                            var request = Request.Scheme + "://" + Request.Host;
                            Uri uri = new Uri(request + "?" + messageevent.postback.data);
                            var uriQuery = HttpUtility.ParseQueryString(uri.Query);
                            var showReservation = uriQuery.Get("showreservation");
                            var coachId = Convert.ToInt32(uriQuery.Get("coach"));
                            var coachTime = await _coachService.GetCoachTime(coachId);

                            CoachDTO coachdto = new CoachDTO();
                            coachdto.Id = coachId;
                            UserReservation.UpdateCoachTime(coachdto, userId);
                            UserReservation.ChangeUserProcessing(messageevent.source.userId, ReservationProcession.InputStartTime);

                            var timeList = new List<string>();
                            foreach (var coach in coachTime)
                            {
                                var aaa = coach.StartTime.ToString("yyyy/MM/dd HH:mm:ss") + "~" + coach.EndTime.ToString("yyyy/MM/dd HH:mm:ss");
                                timeList.Add(aaa);
                            }

                            var temp = "請排除以下時間，再輸入您要預約的時間:\n" + string.Join("\n", timeList);

                            returnMessage = _generateMessage.GenerateTextMessage(temp);

                            break;
                    }
                    await ReplyMessage(messageevent.replyToken, returnMessage);
                }
            }

            return Ok();
        }

        [HttpPost("coach")]
        public async Task<bool> InsertCoach([FromBody] List<CoachDTO> coaches)
        {
            var result = await _coachRepository.InsertCoachData(coaches);

            return result;
        }

        [HttpPut("coach")]
        public async Task<bool> UpdateCoach([FromBody] List<CoachDTO> coaches)
        {
            var result = await _coachRepository.InsertCoachData(coaches);

            return result;
        }

        [HttpGet("create/table")]
        public void CreateTable()
        {
            _coachRepository.CreateTable();
        }

        [HttpGet("create/CoashTime")]
        public async Task CreateCoachTime()
        {
            await _coachRepository.CreateCoaChTimeTable();
        }

        [HttpGet("create/UserCoachTime")]
        public async Task CreateUserCoachTime()
        {
            await _coachRepository.CreateUserCoachTimeTable();
        }

        [HttpGet("coach")]
        public async Task<List<CoachDTO>> GetCoaches()
        {
            var result = await _coachRepository.SelectCoaches();

            return result;
        }

        [HttpPost("coachtime")]
        public async Task InsertCoachTime([FromBody] List<CoachDTO> coachTimeDTOs)
        {
            foreach (var coach in coachTimeDTOs)
            {
                var result = await _coachRepository.InsertCoachTime(coach);
            }
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
    }
}
