using Model;
using Model.Enum;
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

                        message = _generateMessage.GenerateTextMessage("請輸入你要預約的日期與時間\n(例:2020/10/10 10:00:00)");

                        break;

                    case ReservationProcession.InputStartTime:
                        //要進入結束時間
                        DateTimeOffset dateTime;

                        var parseResult = DateTimeOffset.TryParse(_dateString, out dateTime);

                        if (parseResult)
                        {
                            _changeUserCoachProcess.UserStartTime(dateTime.ToString(), userId);

                            UserReservation.ChangeUserProcessing(userId, ReservationProcession.InputEndTime);

                            message = _generateMessage.GenerateTextMessage("請輸入結束時間");
                        }
                        else
                        {
                            message = _generateMessage.GenerateTextMessage("請輸入正確時間格式\n(例如:12:00:00)");
                        }

                        break;

                    case ReservationProcession.InputEndTime:
                        DateTimeOffset endTime;
                        var endTimeParseResult = DateTimeOffset.TryParse(_dateString, out endTime);

                        if (endTimeParseResult)
                        {
                            var userCoach = UserReservation.GetUserCoach(userId);
                            endTime = new DateTimeOffset(userCoach.StartTime.Year, userCoach.StartTime.Month, userCoach.StartTime.Day, endTime.Hour, endTime.Minute, endTime.Second, endTime.Millisecond, endTime.Offset);

                            _changeUserCoachProcess.UserEndTime(endTime.ToString(), userId);

                            await _changeUserCoachProcess.InsertUserCoah(userId);

                            //要進入流程結束
                            UserReservation.ChangeUserProcessing(userId, ReservationProcession.EndProcessing);

                            message = _generateMessage.GenerateTextMessage("謝謝你");
                            UserReservation.Clear(userId);
                        }
                        else
                        {
                            message = _generateMessage.GenerateTextMessage("請輸入正確時間格式\n(例如:12:00:00)");
                        }

                        break;

                    default:
                        //要進入選擇教練階段
                        UserReservation.ChangeUserProcessing(userId, ReservationProcession.ChooseingCoaches);

                        message = await _generateMessage.GenerateImageCarourselMessage();
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
    }
}