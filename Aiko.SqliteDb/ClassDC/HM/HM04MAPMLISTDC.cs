using DI.DiNetWinServiceObject;
using System.Collections;

namespace DI.DiNetWinServiceObject
{
    /// <summary>
    /// マップマスターリスト
    /// </summary>
    public class HM04MAPMLISTDC : CMObjectDC
    {

        /// <summary>
        /// マップマスターリスト
        /// </summary>
        public ArrayList HM04MAPMLIST { get; set; } = new ArrayList();

        /// <summary>
        /// 変更フラグ
        /// </summary>
        public string HMCHANGE { get; set; } = string.Empty;
    }
}
