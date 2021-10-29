using System;
using System.Collections.Generic;
using System.Text;

namespace Model.Enum
{
    public enum CancelReservation
    {
        /// <summary>
        /// 選擇教練階段
        /// </summary>
        ChooseingCoach,

        /// <summary>
        /// 輸入教練代號
        /// </summary>
        InputCoachId,

        /// <summary>
        /// 結束取消預約
        /// </summary>
        EndCancel
    }
}
