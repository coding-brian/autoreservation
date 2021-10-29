using System;
using System.Collections.Generic;
using System.Text;

namespace Model.Enum
{
    public enum ReservationProcession
    {
        /// <summary>
        /// 正在選擇教練流程
        /// </summary>
        ChooseingCoaches,

        /// <summary>
        /// 正在輸入開始時間流程
        /// </summary>
        InputStartTime,

        /// <summary>
        /// 正在輸入結束時間流程
        /// </summary>
        InputEndTime,

        /// <summary>
        /// 結束預約流程
        /// </summary>
        EndProcessing,

        /// <summary>
        /// 輸入教練代號流程
        /// </summary>
        InputCoachId,

        /// <summary>
        /// 結束取消預約流程
        /// </summary>
        EndCancel
    }
}
