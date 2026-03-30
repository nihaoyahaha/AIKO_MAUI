using DI.DiNetWinServiceObject;
using System.Collections;

namespace DI.DiNetWinServiceObject
{
    /// <summary>
    /// 階マスターリモートサービス
    /// </summary>
    public class HM05KAIMLISTDC : CMObjectDC
    {
        // 階マスター
        private ArrayList _HM05KAIM = new ArrayList();

        /// <summary>
        /// 階マスター
        /// </summary>
        public ArrayList HM05KAIM {
            get { return _HM05KAIM; }
            set { _HM05KAIM = value; }
        }

       
    }
}

