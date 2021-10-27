using Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository
{
    public interface ICoachRepository
    {
        Task CreateCoaChTimeTable();
        Task CreateTable();
        Task<bool> InsertCoachData(List<CoachDTO> coaches);
        Task<List<CoachDTO>> SelectCoaches();
        Task<List<CoachTimeDTO>> SelectCoachesTime(int id);
        Task<bool> UpdateDate(List<CoachDTO> coaches);

        Task<int> InsertCoachTime(CoachDTO coach);

        Task CreateUserCoachTimeTable();

        Task<bool> InsertUserCoachTime(UserCoachTimeDTO userCoachTimeDTO);

        Task<List<CoachUserTimeDTO>> SelectUserCoachTime(string userId);
    }
}