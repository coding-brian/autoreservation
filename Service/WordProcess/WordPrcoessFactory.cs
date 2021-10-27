using Model;
using Repository;
using Service.GenerateMessage;
using Service.MessageFatcory;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.WordProcess
{
    public class WordPrcoessFactory : IWordPrcoessFactory
    {
        private readonly string _reservation = "\u9810\u7D04";//預約

        private readonly string _select = "\u67e5\u8a62";//查詢

        private IWordProcess _wordProcess;

        private readonly ICoachRepository _coachRepository;

        private readonly IChangeUserCoachProcess _changeUserCoachProcess;

        private readonly IGenerateMessage _generateMessage;

        public WordPrcoessFactory(ICoachRepository coachRepository, IChangeUserCoachProcess changeUserCoachProcess, IGenerateMessage generateMessage)
        {
            _coachRepository = coachRepository;
            _changeUserCoachProcess = changeUserCoachProcess;
            _generateMessage = generateMessage;
        }

        public IWordProcess Create(string word,string userId)
        {
            if (word.Contains(_select))
            {
                _wordProcess = new SelectWordProcess(_coachRepository, _generateMessage);
            }
            else if (word.Contains(_reservation))
            {
                _wordProcess = new ReservationWordProcess(_changeUserCoachProcess, _generateMessage, word);
            }
            else
            {
                if (UserReservation.IsExist(userId) && UserReservation.GetUserReservationProcession(userId) != null)
                {
                    _wordProcess = new ReservationWordProcess(_changeUserCoachProcess, _generateMessage, word);
                }
                else 
                {
                    _wordProcess = new NoWordProcess(_generateMessage);
                }
            }

            return _wordProcess;
        }
    }
}
