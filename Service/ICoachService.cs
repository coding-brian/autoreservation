using Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service
{
    public interface ICoachService
    {
        Task<List<CoachDTO>> GetCoachTime(int id);

        Task<List<CoachDTO>> GetCoaches();
    }
}