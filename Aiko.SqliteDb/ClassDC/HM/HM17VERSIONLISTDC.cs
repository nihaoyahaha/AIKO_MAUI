using DI.DiNetWinServiceObject;
using System.Collections;

namespace DI.DiNetWinServiceObject
{
    /// <summary>
    /// メモマスターリモートサービス
    /// </summary>
    public class HM17VERSIONLISTDC : CMObjectDC
    {

        /// <summary>
        /// 部位マスタリスト
        /// </summary>
        public ArrayList HM17VERSION { get; set; } = new ArrayList();

    }
}
