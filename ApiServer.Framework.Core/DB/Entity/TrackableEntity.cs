using System;
using ApiServer.Framework.Core.DB.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ApiServer.Framework.Core.DB.Entity
{
    /// <summary>
    /// 跟踪对象实体
    /// </summary>
    public abstract class TrackableEntity:EntityBase,ITrackable
    {
        /// <summary>
        /// 跟踪方法
        /// </summary>
        /// <param name="entry">Entry.</param>
        public virtual void DoTrack(EntityEntry entry)
        {
            //var now = DateTime.Now;
            //switch (entry.State)
            //{
            //    case EntityState.Modified:
            //        entry.CurrentValues["LastUpdatedTime"] = now;
            //        //entry.CurrentValues["LastUpdatedBy"] = user; 
            //        break;
            //    case EntityState.Added:
            //        entry.CurrentValues["CreatedTime"] = now;
            //        //entry.CurrentValues["CreatedBy"] = user;
            //        entry.CurrentValues["LastUpdatedTime"] = now;
            //        //entry.CurrentValues["LastUpdatedBy"] = user; 
            //        break;
            //}
        }
    }
}
