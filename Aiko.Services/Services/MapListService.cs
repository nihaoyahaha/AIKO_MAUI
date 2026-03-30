using Aiko.Common;
using Aiko.IServices.IServices;
using Aiko.SqliteDb;
using DI.DiNetWinServiceObject;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aiko.Services.Services
{
    public class MapListService : BaseService<MapListService>, IMapListService
    {
        public MapListService(ServiceContext<MapListService> context)
            : base(context) { }

        /// <summary>
        /// aikoApp実行中のアプリケーションコンテキスト
        /// </summary>
        public AikoAppContext AppContext => AikoAppContext;

        /// <summary>
        /// 获取底图
        /// </summary>
        /// <param name="classCode">分類コード</param>
        /// <returns></returns>
        public async Task<List<HM12FILE>> GetHM12FILEID(string classCode)
        {
            HM12FILE hm12DC = new HM12FILE();
            hm12DC.HM12001 = AppContext.WorkCD;
            hm12DC.HM12015 = classCode;
            List<HM12FILE> result = await HkksDb.GetHM12FILEIDAsync(hm12DC);

            return result;
        }
    }
}
