using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Model;
using Model.Line;
using Repository;
using Service;
using Service.MessageFatcory;
using Service.WebAPIRequest;
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

        private readonly ICoachRepository _coachRepository;

        private readonly ICoachService _coachService;

        private readonly IMessageFactory _messageFactory;
        public LineController(IConfiguration configuration, IWebAPIRequest webAPIRequest, ICoachRepository coachRepository, ICoachService coachService, IMessageFactory messageFactory)
        {

            _configuration = configuration;
            _webAPIRequest = webAPIRequest;
            _coachRepository = coachRepository;
            _coachService = coachService;
            _messageFactory = messageFactory;
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> LineWebhook([FromBody] LineMessage messages)
        {
            var messgae = new object();

            if (messages.events.Count > 0)
            {
                foreach (var messageevent in messages.events)
                {
                    var userId = messageevent.source.userId;
                    UserReservationProcession userReservationProcession = new UserReservationProcession();

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

                                    if (messageevent.message.text.Contains(keywords))
                                    {
                                        UserReservation.Initial();
                                        userReservationProcession.UserId = userId;
                                        UserReservation.Add(userReservationProcession);
                                        ChangeUserReservationProcessing(userId, ReservationProcession.ChooseingCoaches);
                                        messgae = await GenerateImageCarourselMessage();
                                    }
                                    else
                                    {
                                        var coach = UserReservation.GetUserCoach(userId);
                                        switch (UserReservation.GetUserReservationProcession(userId)?.ReservationProcession)
                                        {
                                            case ReservationProcession.ChooseingCoaches:
                                                //要進入輸入開始時間
                                                ChangeUserReservationProcessing(messageevent.source.userId, ReservationProcession.InputStartTime);

                                                messgae = GenerateTextMessage("請輸入開始時間");
                                                break;
                                            case ReservationProcession.InputStartTime:
                                                var inputStartTime = DateTimeOffset.Parse(messageevent.message.text);
                                                coach.StartTime = inputStartTime;
                                                UserReservation.InsertCoachTime(coach, userId);

                                                //要進入結束時間
                                                ChangeUserReservationProcessing(messageevent.source.userId, ReservationProcession.InputEndTime);
                                                messgae = GenerateTextMessage("請輸入結束時間");
                                                break;
                                            case ReservationProcession.InputEndTime:
                                                var inputEndTime = DateTimeOffset.Parse(messageevent.message.text);
                                                coach.EndTime = inputEndTime;
                                                UserReservation.InsertCoachTime(coach, userId);

                                                var userCoach=UserReservation.GetUserCoach(userId);
                                                var coachTimeNo=await _coachRepository.InsertCoachTime(userCoach);
                                                UserCoachTimeDTO userCoachTimeDTO = new UserCoachTimeDTO();
                                                userCoachTimeDTO.coachTiemNo = coachTimeNo;
                                                userCoachTimeDTO.userId = userId;
                                                await _coachRepository.InsertUserCoachTime(userCoachTimeDTO);

                                                //要進入流程結束
                                                ChangeUserReservationProcessing(messageevent.source.userId, ReservationProcession.EndProcessing);
                                                messgae = GenerateTextMessage("謝謝你");
                                                break;
                                            default:
                                                messgae = GenerateTextMessage(messageevent.message.text);
                                                break;
                                        }
                                    }
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
                            coachdto.id = coachId;
                            UserReservation.InsertCoachTime(coachdto, userId);
                            ChangeUserReservationProcessing(messageevent.source.userId, ReservationProcession.InputStartTime);

                            var timeList = new List<string>();
                            foreach (var coach in coachTime)
                            {
                                var aaa = coach.StartTime.ToString("yyyy/MM/dd HH:mm:ss") + "~" + coach.EndTime.ToString("yyyy/MM/dd HH:mm:ss");
                                timeList.Add(aaa);
                            }

                            var temp = "請排除以下時間，再輸入您要預約的時間:\n" + string.Join("\n", timeList);

                            messgae = GenerateTextMessage(temp);

                            break;
                    }
                    await ReplyMessage(messageevent.replyToken, messgae);
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

        private async Task<ImageCarouselMessage> GenerateImageCarourselMessage()
        {
            var result = new ImageCarouselMessage();

            var columns = new List<Column>();

            var coaches = await GetCoaches();

            foreach (var coach in coaches)
            {
                var column = new Column();
                column.imageUrl = coach.ImageUrl;
                var messageObjct = new PostBackAction
                {
                    type = "postback",
                    label = coach.Name,
                    //text = $"我想查看{coach.Name}可以預約的時間",
                    data = $"showreservation=true&&coach={coach.id}"
                };
                column.action = _messageFactory.ActinoGenerate("postback", messageObjct);
                columns.Add(column);
            }

            result = _messageFactory.GenerateImageCarouselMessage("歡迎你選擇", columns);

            return result;
        }

        private TextMessage GenerateTextMessage(string text)
        {
            var result = _messageFactory.GenerateTextMessageAsyc(text);

            return result;
        }

        private void ChangeUserReservationProcessing(string userId, ReservationProcession reservationProcession)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                if (UserReservation.IsExist(userId))
                {
                    UserReservation.ChangeUserProcessing(userId, reservationProcession);
                }
            }
        }
    }
}
