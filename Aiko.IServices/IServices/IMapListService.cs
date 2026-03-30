using Aiko.SqliteDb;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aiko.IServices.IServices
{
    public interface IMapListService : IServiceBase
    {
        /// <summary>
        /// 获取底图
        /// </summary>
        /// <param name="classCode">分類コード</param>
        /// <returns></returns>
        public Task<List<HM12FILE>> GetHM12FILEID(string classCode);
    }
}
