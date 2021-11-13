using Model;
using Model.Enum;
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

        private readonly string _cancel = "\u53d6\u6d88";//取消

        private IWordProcess _wordProcess;

        private readonly ICoachRepository _coachRepository;

        private readonly IChangeUserCoachProcess _changeUserCoachProcess;

        private readonly IGenerateMessage _generateMessage;

        private readonly ICoachService _coachService;

        public WordPrcoessFactory(ICoachRepository coachRepository, IChangeUserCoachProcess changeUserCoachProcess, IGenerateMessage generateMessage, ICoachService coachService)
        {
            _coachRepository = coachRepository;
            _changeUserCoachProcess = changeUserCoachProcess;
            _generateMessage = generateMessage;
            _coachService = coachService;
        }

        public IWordProcess Create(string word, string userId)
        {
            if (word.Contains(KeyWords.select))
            {
                _wordProcess = new SelectWordProcess(_coachRepository, _generateMessage);
            }
            else if (word.Contains(KeyWords.reservation))
            {
                if (word.Contains(KeyWords.cancel))
                {
                    _wordProcess = new CancelWordProcess(_coachRepository, _generateMessage, word);
                }
                else
                {
                    _wordProcess = new ReservationWordProcess(_changeUserCoachProcess, _generateMessage, word, _coachService);
                }
            }
            else
            {
                if (UserReservation.IsExist(userId) && UserReservation.GetUserReservationProcession(userId) != null)
                {
                    switch (UserReservation.GetUserStage(userId))
                    {
                        case UserStage.ReservationStage:
                            _wordProcess = new ReservationWordProcess(_changeUserCoachProcess, _generateMessage, word, _coachService);
                            break;

                        case UserStage.CancelStage:
                            _wordProcess = new CancelWordProcess(_coachRepository, _generateMessage, word);
                            break;

                        default:
                            _wordProcess = new NoWordProcess(_generateMessage);
                            break;
                    }
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