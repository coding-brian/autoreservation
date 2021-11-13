using Model.Enum;
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
            if (!string.IsNullOrEmpty(userId))
            {
                if (IsExist(userId))
                {
                    var user = instance.TakeWhile(x => x.UserId == userId).ToList();

                    user.ForEach(x =>
                    {
                        if (x.UserId == userId)
                        {
                            x.ReservationProcession = reservationProcession;
                        }
                    });
                }
            }
        }

        public static ReservationProcession? GetUserReservationProcession(string userId)
        {
            var user = instance?.TakeWhile(x => x.UserId == userId)?.FirstOrDefault();

            return user?.ReservationProcession;
        }

        public static UserStage GetUserStage(string userId)
        {
            if (IsExist(userId))
            {
                switch (GetUserReservationProcession(userId))
                {
                    case ReservationProcession.ChooseingCoaches:
                    case ReservationProcession.InputEndTime:
                    case ReservationProcession.InputStartTime:
                    case ReservationProcession.EndProcessing:
                        return UserStage.ReservationStage;

                    case ReservationProcession.EndCancel:
                    case ReservationProcession.InputCoachId:
                        return UserStage.CancelStage;
                }
            }

            return UserStage.NoStage;
        }

        public static void UpdateCoachTime(CoachDTO coachDTO, string userId)
        {
            var user = instance.TakeWhile(x => x.UserId == userId).ToList();

            user.ForEach(x =>
            {
                if (x.UserId == userId)
                {
                    x.coachDTO = coachDTO;
                }
            });
        }

        public static void Clear(string userId)
        {
            instance.RemoveAll(x => x.UserId == userId);
        }
    }

    public class UserReservationProcession
    {
        public string UserId { get; set; }

        public CoachDTO coachDTO { get; set; } = new CoachDTO();

        public ReservationProcession ReservationProcession { get; set; }
    }
}