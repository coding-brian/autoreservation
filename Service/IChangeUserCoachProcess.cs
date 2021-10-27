using System.Threading.Tasks;

namespace Service
{
    public interface IChangeUserCoachProcess
    {
        Task InsertUserCoah(string userId);
        void UserEndTime(string dateTimeString, string userId);
        void UserStartTime(string dateTiemString, string userId);
    }
}