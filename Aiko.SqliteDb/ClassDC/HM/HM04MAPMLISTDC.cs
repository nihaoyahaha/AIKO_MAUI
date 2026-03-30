using DI.DiNetWinServiceObject;
using System.Collections;

namespace DI.DiNetWinServiceObject
{
    /// <summary>
    /// マップマスターリスト
    /// </summary>
    public class HM04MAPMLISTDC : CMObjectDC
    {
        // マップマスターリスト
        private ArrayList _HM04MAPMLIST = new ArrayList();

        /// <summary>
        /// マップマスターリスト
        /// </summary>
        public ArrayList HM04MAPMLIST
        {
            get { return _HM04MAPMLIST; }
            set { _HM04MAPMLIST = value; }
        }

        // 変更フラグ
        private string _HMCHANGE;

        /// <summary>
        /// 変更フラグ
        /// </summary>
        public string HMCHANGE
        {
            get { return _HMCHANGE; }

            set { _HMCHANGE = value; }
        }
    }
}

