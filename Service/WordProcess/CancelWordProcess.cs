using Model;
using Model.Enum;
using Repository;
using Service.GenerateMessage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Service.WordProcess
{
    public class CancelWordProcess : IWordProcess
    {
        private readonly ICoachRepository _coachRepository;

        private readonly IGenerateMessage _generateMessage;

        private string _coachId;

        public CancelWordProcess(ICoachRepository coachRepository, IGenerateMessage generateMessage, string coachId = "")
        {
            _coachRepository = coachRepository;
            _coachId = coachId;
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
                    case ReservationProcession.InputCoachId:
                        //目前是輸入教練代碼的階段
                        var result = await _coachRepository.DeleteUserCoachTime(userId, Convert.ToInt32(_coachId));

                        if (result)
                        {
                            message = _generateMessage.GenerateTextMessage("取消完成");
                        }
                        else
                        {
                            message = _generateMessage.GenerateTextMessage("取消失敗");
                        }
                        UserReservation.Clear(userId);
                        break;
                    default:
                        UserReservation.ChangeUserProcessing(userId, ReservationProcession.InputCoachId);
                        message = _generateMessage.GenerateTextMessage("請你輸入教練代碼");
                        break;
                }
            }
            else
            {
                UserReservationProcession userReservationProcession = new UserReservationProcession();
                userReservationProcession.UserId = userId;
                userReservationProcession.ReservationProcession = ReservationProcession.InputCoachId;
                UserReservation.Add(userReservationProcession);
                message = _generateMessage.GenerateTextMessage("請你輸入教練代碼");
            }

            return message;
        }
    }
}
