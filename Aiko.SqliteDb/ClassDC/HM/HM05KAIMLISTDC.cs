using DI.DiNetWinServiceObject;
using System.Collections;

namespace DI.DiNetWinServiceObject
{
    /// <summary>
    /// 階マスターリモートサービス
    /// </summary>
    public class HM05KAIMLISTDC : CMObjectDC
    {

        /// <summary>
        /// 階マスター
        /// </summary>
        public ArrayList HM05KAIM { get; set; } = new ArrayList();

    }
}
