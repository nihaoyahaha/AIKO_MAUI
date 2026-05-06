using DI.DiNetWinServiceObject;
using System.Collections;

namespace DI.DiNetWinServiceObject
{
    /// <summary>
    /// マップガイドマスターリモートサービス
    /// </summary>
   public class HR03SYASLISTDC: CMObjectDC
    {

        /// <summary>
        /// 写真テーブルタリスト
        /// </summary>
        public ArrayList HR03ITEM { get; set; } = new ArrayList();

    }
}
