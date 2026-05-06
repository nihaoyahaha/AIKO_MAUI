using DI.DiNetWinServiceObject;
using System.Collections;

namespace DI.DiNetWinServiceObject
{
    /// <summary>
    /// グループリストリモートサービス
    /// </summary>
    public class HM08GRPMLISTDC : CMObjectDC
    {

        /// <summary>
        /// リグループマスタリスト
        /// </summary>
        public ArrayList HM08GRPM { get; set; } = new ArrayList();
    }
}
