using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    public static class KeyWords
    {
        /// <summary>
        /// 查詢的Unicode
        /// </summary>
        public static string select { get; } = "\u67e5\u8a62";

        /// <summary>
        /// 預約的Unicode
        /// </summary>
        public static string reservation { get; } = "\u9810\u7D04";

        /// <summary>
        /// 取消的Unicode
        /// </summary>
        public static string cancel { get; } = "\u53d6\u6d88";
    }
}