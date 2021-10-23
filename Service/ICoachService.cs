using Model;
using System.Threading.Tasks;

namespace Service
{
    public interface ICoachService
    {
        Task<CoachDTO> GetCoachTime(int id);
    }
}