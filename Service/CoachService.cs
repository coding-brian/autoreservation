using Model;
using Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class CoachService : ICoachService
    {
        private ICoachRepository _coachRepository;

        public CoachService(ICoachRepository coachRepository)
        {
            _coachRepository = coachRepository;
        }

        public async Task<CoachDTO> GetCoachTime(int id)
        {
            var coaches = await _coachRepository.SelectCoachesTime(id);

            var startTime = DateTimeOffset.FromUnixTimeMilliseconds(coaches.StartTime);

            var endTime = DateTimeOffset.FromUnixTimeMilliseconds(coaches.EndTime);

            var result = new CoachDTO();

            result.id = id;
            result.StartTime = startTime.ToLocalTime();
            result.EndTime = endTime.ToLocalTime();

            return result;
        }
    }
}
