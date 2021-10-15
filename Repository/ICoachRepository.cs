using Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository
{
    public interface ICoachRepository
    {
        Task CreateTable();
        Task<bool> InsertCoachData(List<CoachDTO> coaches);
    }
}