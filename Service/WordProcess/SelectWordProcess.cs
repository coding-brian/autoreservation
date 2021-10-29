using Repository;
using Service.GenerateMessage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Service.WordProcess
{
    public class SelectWordProcess : IWordProcess
    {
        private readonly ICoachRepository _coachRepository;

        private readonly IGenerateMessage _generateMessage;

        public SelectWordProcess(ICoachRepository coachRepository, IGenerateMessage generateMessage)
        {
            _coachRepository = coachRepository;
            _generateMessage = generateMessage;
        }

        public async Task<object> ProcessWord(string userId)
        {
            var result = new object();

            var coachUserTimes = await _coachRepository.SelectUserCoachTime(userId);

            var messageObjectList = new List<string>();

            if (coachUserTimes.Count > 0)
            {
                foreach (var coachUserTime in coachUserTimes)
                {
                    var messageObject = $"教練名稱:{coachUserTime.Name}，" +
                        $"開始時間:{DateTimeOffset.FromUnixTimeMilliseconds(coachUserTime.StartTime).ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss")}，" +
                        $"結束時間:{DateTimeOffset.FromUnixTimeMilliseconds(coachUserTime.EndTime).ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss")}";

                    messageObjectList.Add(messageObject);
                }
                result = _generateMessage.GenerateTextMessage(string.Join("\n", messageObjectList));
            }
            else
            {
                result = _generateMessage.GenerateTextMessage("目前沒有任何預約");
            }

            return result;
        }
    }
}
