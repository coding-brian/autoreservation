using Model;
using Model.Line;
using Service.GenerateMessage;
using Service.MessageFatcory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Service.WordProcess
{
    public class ReservationWordProcess : IWordProcess
    {
        private readonly IChangeUserCoachProcess _changeUserCoachProcess;

        private readonly IGenerateMessage _generateMessage;

        private readonly string _dateString;

        public ReservationWordProcess(IChangeUserCoachProcess changeUserCoachProcess, IGenerateMessage generateMessage, string dateString)
        {
            _changeUserCoachProcess = changeUserCoachProcess;
            _dateString = dateString;
            _generateMessage = generateMessage;
        }

        public async Task<object> ProcessWord(string userId)
        {
            var message = new object();

            UserReservation.Initial();
            if (UserReservation.IsExist(userId) && UserReservation.GetUserReservationProcession(userId) != null)
            {
                switch (UserReservation.GetUserReservationProcession(userId))
                {
                    case ReservationProcession.ChooseingCoaches:
                        //要進入輸入開始時間
                        UserReservation.ChangeUserProcessing(userId, ReservationProcession.InputStartTime);

                        message = _generateMessage.GenerateTextMessage("請輸入開始時間");

                        break;
                    case ReservationProcession.InputStartTime:
                        //要進入結束時間

                        _changeUserCoachProcess.UserStartTime(_dateString, userId);

                        UserReservation.ChangeUserProcessing(userId, ReservationProcession.InputEndTime);

                        message = _generateMessage.GenerateTextMessage("請輸入結束時間");
                        break;
                    case ReservationProcession.InputEndTime:

                        _changeUserCoachProcess.UserEndTime(_dateString, userId);

                        await _changeUserCoachProcess.InsertUserCoah(userId);

                        //要進入流程結束
                        UserReservation.ChangeUserProcessing(userId, ReservationProcession.EndProcessing);

                        message = _generateMessage.GenerateTextMessage("謝謝你");
                        UserReservation.Clear(userId);
                        break;
                }
            }
            else
            {
                UserReservationProcession userReservationProcession = new UserReservationProcession();
                userReservationProcession.UserId = userId;
                UserReservation.Add(userReservationProcession);
                message = await _generateMessage.GenerateImageCarourselMessage();
            }

            return message;
        }

        //private async Task<ImageCarouselMessage> GenerateImageCarourselMessage()
        //{
        //    var result = new ImageCarouselMessage();

        //    var columns = new List<Column>();

        //    var coaches = await _coachService.GetCoaches();

        //    foreach (var coach in coaches)
        //    {
        //        var column = new Column();
        //        column.imageUrl = coach.ImageUrl;
        //        var messageObjct = new PostBackAction
        //        {
        //            type = "postback",
        //            label = coach.Name,
        //            //text = $"我想查看{coach.Name}可以預約的時間",
        //            data = $"showreservation=true&&coach={coach.id}"
        //        };
        //        column.action = _messageFactory.ActinoGenerate("postback", messageObjct);
        //        columns.Add(column);
        //    }

        //    result = _messageFactory.GenerateImageCarouselMessage("歡迎你選擇", columns);

        //    return result;
        //}
    }
}
