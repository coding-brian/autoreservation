using Model.Line;
using Service.MessageFatcory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Service.GenerateMessage
{
    public class GenerateMessage : IGenerateMessage
    {
        private readonly ICoachService _coachService;

        private readonly IMessageFactory _messageFactory;

        public GenerateMessage(ICoachService coachService, IMessageFactory messageFactory)
        {
            _coachService = coachService;
            _messageFactory = messageFactory;
        }

        public async Task<ImageCarouselMessage> GenerateImageCarourselMessage()
        {
            var result = new ImageCarouselMessage();

            var columns = new List<Column>();

            var coaches = await _coachService.GetCoaches();

            foreach (var coach in coaches)
            {
                var column = new Column();
                column.imageUrl = coach.ImageUrl;
                var messageObjct = new PostBackAction
                {
                    type = "postback",
                    label = coach.Name,
                    //text = $"我想查看{coach.Name}可以預約的時間",
                    data = $"showreservation=true&&coach={coach.Id}"
                };
                column.action = _messageFactory.ActinoGenerate("postback", messageObjct);
                columns.Add(column);
            }

            result = _messageFactory.GenerateImageCarouselMessage("歡迎你選擇", columns);

            return result;
        }

        public TextMessage GenerateTextMessage(string text)
        {
            var result = _messageFactory.GenerateTextMessageAsyc(text);

            return result;
        }
    }
}
