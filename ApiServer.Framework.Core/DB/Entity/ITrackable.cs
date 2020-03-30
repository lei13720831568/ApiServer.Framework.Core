using System;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ApiServer.Framework.Core.DB.Entity
{
    /// <summary>
    /// 可跟踪的实体接口
    /// </summary>
    public interface ITrackable
    {
        /// <summary>
        /// 跟踪接口
        /// </summary>
        /// <param name="entry">Entry.</param>
        void DoTrack(EntityEntry entry);
    }
}
