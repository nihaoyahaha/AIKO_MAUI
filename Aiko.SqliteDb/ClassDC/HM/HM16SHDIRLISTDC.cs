using DI.DiNetWinServiceObject;
using System.Collections;

namespace DI.DiNetWinServiceObject
{
    /// <summary>
    /// マップガイドマスターリモートサービス
    /// </summary>
    public class HM16SHDIRLISTDC : CMObjectDC
    {

        /// <summary>
        /// 部位マスタリスト
        /// </summary>
        public ArrayList HM16SHDIR { get; set; } = new ArrayList();

    }
}
