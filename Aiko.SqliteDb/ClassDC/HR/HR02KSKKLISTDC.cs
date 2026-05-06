using DI.DiNetWinServiceObject;
using System.Collections;

namespace DI.DiNetWinServiceObject
{
    /// <summary>
    /// マップガイドマスターリモートサービス
    /// </summary>
    public class HR02KSKKLISTDC : CMObjectDC
    {

        /// <summary>
        /// 検査結果テーブルタリスト
        /// </summary>
        public ArrayList HR02KSKK { get; set; } = new ArrayList();

    }

}
