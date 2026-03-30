using DI.DiNetWinServiceObject;
using System.Collections;

namespace DI.DiNetWinServiceObject
{
    /// <summary>
    /// メモマスターリモートサービス
    /// </summary>
    public class HM11MEMOLISTDC : CMObjectDC
    {
        // 部位マスタリスト
        private ArrayList _HM11BUIM = new ArrayList();

        /// <summary>
        /// 部位マスタリスト
        /// </summary>
        public ArrayList HM11BUIM
        {
            get { return _HM11BUIM; }
            set { _HM11BUIM = value; }
        }

       
    }
}

