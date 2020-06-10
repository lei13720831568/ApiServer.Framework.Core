using System;
using System.Globalization;

namespace ApiServer.Framework.Core.DB.Query
{
    public class OrderByInfo
    {
        private string field = "";
        private TextInfo myTI = new CultureInfo("en-US", false).TextInfo;

        /// <summary>
        /// 1= 正序 2=倒序
        /// </summary>
        public int OrderType { get; set; } = 1;

        /// <summary>
        /// 字段名
        /// </summary>
        public string Field { get => field; set => field = myTI.ToTitleCase(value); }

    }
}
