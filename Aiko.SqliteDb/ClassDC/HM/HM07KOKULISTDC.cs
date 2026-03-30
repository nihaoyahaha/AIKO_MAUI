using DI.DiNetWinServiceObject;
using System.Collections;

namespace DI.DiNetWinServiceObject
{
    /// <summary>
    /// 工区マスターリモートサービス
    /// </summary>
    public class HM07KOKULISTDC : CMObjectDC
    {
        // 部位マスタリスト
        private ArrayList _HM07KOKU = new ArrayList();

        /// <summary>
        /// 部位マスタリスト
        /// </summary>
        public ArrayList HM07KOKU
        {
            get { return _HM07KOKU; }
            set { _HM07KOKU = value; }
        }
    }
}

