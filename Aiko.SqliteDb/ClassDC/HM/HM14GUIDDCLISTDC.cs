using DI.DiNetWinServiceObject;
using System.Collections;

namespace DI.DiNetWinServiceObject
{
    /// <summary>
    /// マップガイドマスターリモートサービス
    /// </summary>
    public class HM14GUIDDCLISTDC : CMObjectDC
    {

        /// <summary>
        /// 部位マスタリスト
        /// </summary>
        public ArrayList HM14GUID { get; set; } = new ArrayList();

    }
}
