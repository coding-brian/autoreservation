using Model;
using Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class ChangeUserCoachProcess : IChangeUserCoachProcess
    {
        private readonly ICoachRepository _coachRepository;

        public ChangeUserCoachProcess(ICoachRepository coachRepository)
        {
            _coachRepository = coachRepository;
        }

        public void UserStartTime(string dateTiemString, string userId)
        {
            var coach = UserReservation.GetUserCoach(userId);
            var inputStartTime = DateTimeOffset.Parse(dateTiemString);
            coach.StartTime = inputStartTime;
            UserReservation.UpdateCoachTime(coach, userId);
        }

        public void UserEndTime(string dateTimeString, string userId)
        {
            var coach = UserReservation.GetUserCoach(userId);
            var inputEndTime = DateTimeOffset.Parse(dateTimeString);
            coach.EndTime = inputEndTime;
            UserReservation.UpdateCoachTime(coach, userId);
        }

        public async Task InsertUserCoah(string userId)
        {
            var userCoach = UserReservation.GetUserCoach(userId);
            var coachTimeNo = await _coachRepository.InsertCoachTime(userCoach);
            UserCoachTimeDTO userCoachTimeDTO = new UserCoachTimeDTO();
            userCoachTimeDTO.coachTiemNo = coachTimeNo;
            userCoachTimeDTO.userId = userId;
            await _coachRepository.InsertUserCoachTime(userCoachTimeDTO);
        }
    }
}
