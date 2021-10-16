using Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository
{
    public interface ICoachRepository
    {
        Task CreateTable();
        Task<bool> InsertCoachData(List<CoachDTO> coaches);
        Task<List<CoachDTO>> SelectCoaches();
        Task<bool> UpdateDate(List<CoachDTO> coaches);
    }
}