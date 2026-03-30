using DI.DiNetWinServiceObject;
using System.Collections;

namespace DI.DiNetWinServiceObject
{
    /// <summary>
    /// グループリストリモートサービス
    /// </summary>
    public class HM08GRPMLISTDC : CMObjectDC
    {
        // リグループマスタリスト
        private ArrayList _HM08GRPM = new ArrayList();

        /// <summary>
        /// リグループマスタリスト
        /// </summary>
        public ArrayList HM08GRPM
        {
            get { return _HM08GRPM; }
            set { _HM08GRPM = value; }
        }
    }
}

