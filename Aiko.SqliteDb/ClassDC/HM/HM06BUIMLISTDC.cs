using DI.DiNetWinServiceObject;
using System.Collections;

namespace DI.DiNetWinServiceObject
{
    /// <summary>
    /// 部位リストリモートサービス
    /// </summary>
    public class HM06BUIMLISTDC : CMObjectDC
    {
        // 部位マスタリスト
        private ArrayList _HM06BUIM = new ArrayList();

        /// <summary>
        /// 部位マスタリスト
        /// </summary>
        public ArrayList HM06BUIM
        {
            get { return _HM06BUIM; }
            set { _HM06BUIM = value; }
        }

       
    }
}

