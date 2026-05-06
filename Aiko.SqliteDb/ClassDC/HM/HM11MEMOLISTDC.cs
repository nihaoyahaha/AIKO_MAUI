using DI.DiNetWinServiceObject;
using System.Collections;

namespace DI.DiNetWinServiceObject
{
    /// <summary>
    /// メモマスターリモートサービス
    /// </summary>
    public class HM11MEMOLISTDC : CMObjectDC
    {

        /// <summary>
        /// 部位マスタリスト
        /// </summary>
        public ArrayList HM11BUIM { get; set; } = new ArrayList();

    }
}
