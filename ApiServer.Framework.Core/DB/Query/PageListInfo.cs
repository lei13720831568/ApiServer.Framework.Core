using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace ApiServer.Framework.Core.DB.Query
{
    /// <summary>
    /// 分页请求信息
    /// </summary>
    public class PageListInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Fast.Server.Dto.PageListInfo"/> class.
        /// </summary>
        public PageListInfo()
        {
            PageSize = 20;
            PageIndex = 0;
        }
        /// <summary>
        /// 分页尺寸
        /// </summary>
        /// <value>The size of the page.</value>
        public int PageSize { get; set; }

        /// <summary>
        /// 第N页
        /// </summary>
        /// <value>The index of the page.</value>
        
        public int PageIndex { get; set; }

        [JsonIgnore]
        public int SkipCount
        {
            get {
                return PageIndex * PageSize;
            }
        }
    }
}
