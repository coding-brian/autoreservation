using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    public class UserReservation
    {
        private static List<UserReservationProcession> instance;

        private UserReservation()
        {
            instance = new List<UserReservationProcession>();
        }

        public static List<UserReservationProcession> Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new List<UserReservationProcession>();
                }
                return instance;
            }
        }

        public static void Initial() 
        {
            instance = Instance;
        }

        public static void Add(UserReservationProcession userReservationProcession)
        {
            if (!IsExist(userReservationProcession.UserId))
            {
                instance.Add(userReservationProcession);
            }
        }

        public static bool IsExist(string userId)
        {
            if (instance != null)
            {
                return instance.Where(x => x.UserId == userId).Count() > 0;
            }

            return false;
        }

        public static CoachDTO GetUserCoach(string userId)
        {
            try
            {
                return instance?.Where(x => x.UserId == userId)?.Select(x => x.coachDTO)?.FirstOrDefault();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void ChangeUserProcessing(string userId, ReservationProcession reservationProcession)
        {
            var user = instance.TakeWhile(x => x.UserId == userId).FirstOrDefault();

            instance.ForEach(x =>
            {
                if (x.UserId == userId)
                {
                    x.ReservationProcession = reservationProcession;
                }
            });
        }

        public static UserReservationProcession GetUserReservationProcession(string userId)
        {

            var user = instance?.TakeWhile(x => x.UserId == userId)?.FirstOrDefault();

            return user;
        }

        public static void InsertCoachTime(CoachDTO coachDTO, string userId)
        {
            instance.ForEach(x =>
            {
                if (x.UserId == userId)
                {
                    x.coachDTO = coachDTO;
                }
            });
        }
    }

    public class UserReservationProcession
    {
        public string UserId { get; set; }

        public CoachDTO coachDTO { get; set; } = new CoachDTO();

        public ReservationProcession ReservationProcession { get; set; }
    }

    public enum ReservationProcession
    {
        /// <summary>
        /// 正在選擇教練階段
        /// </summary>
        ChooseingCoaches,

        /// <summary>
        /// 正在輸入開始時間階段
        /// </summary>
        InputStartTime,

        /// <summary>
        /// 正在輸入結束時間階段
        /// </summary>
        InputEndTime,

        /// <summary>
        /// 結束預約流程
        /// </summary>
        EndProcessing,
    }
}
