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

        public async Task<List<CoachDTO>> GetCoachTime(int id)
        {
            var coaches = await _coachRepository.SelectCoachesTime(id);

            var result = new List<CoachDTO>();

            foreach (var caoch in coaches)
            {
                var coachObj = new CoachDTO();
                var startTime = DateTimeOffset.FromUnixTimeMilliseconds(caoch.StartTime);

                var endTime = DateTimeOffset.FromUnixTimeMilliseconds(caoch.EndTime);

                coachObj.Id = id;
                coachObj.StartTime = startTime.ToLocalTime();
                coachObj.EndTime = endTime.ToLocalTime();

                result.Add(coachObj);
            }

            return result;
        }

        public async Task<List<CoachDTO>> GetCoaches()
        {
            var result = await _coachRepository.SelectCoaches();

            return result;
        }
    }
}
