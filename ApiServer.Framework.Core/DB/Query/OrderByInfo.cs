using System;
using System.Globalization;

namespace ApiServer.Framework.Core.DB.Query
{
    public class OrderByInfo
    {
        private string field = "";

        /// <summary>
        /// 0=无排序 1= 正序 2=倒序
        /// </summary>
        public int OrderType { get; set; }

        /// <summary>
        /// 字段名
        /// </summary>
        public string Field { get => field; set{
                if (value != null ) {
                    if (value.Length > 2)
                        field = value.Substring(0, 1).ToUpper() + value.Substring(1);
                    else field = value.ToUpper();
                }
                
            }
        }

    }
}
