using DI.DiNetWinServiceObject;
using System.Collections;

namespace DI.DiNetWinServiceObject
{
    /// <summary>
    /// 工区マスターリモートサービス
    /// </summary>
    public class HM07KOKULISTDC : CMObjectDC
    {

        /// <summary>
        /// 部位マスタリスト
        /// </summary>
        public ArrayList HM07KOKU { get; set; } = new ArrayList();
    }
}
