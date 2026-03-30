using DI.DiNetWinServiceObject;
using System.Collections;

namespace DI.DiNetWinServiceObject
{
    /// <summary>
    /// 工程マスター
    /// </summary>
    public class HM09PROCLISTDC : CMObjectDC
    {
        // 工程マスター
        private ArrayList _HM09PROC = new ArrayList();

        /// <summary>
        /// 工程マスター
        /// </summary>
        public ArrayList HM09PROC {
            get { return _HM09PROC; }
            set { _HM09PROC = value; }
        }
    }
}

