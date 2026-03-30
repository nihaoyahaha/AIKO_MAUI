using DI.DiNetWinServiceObject;
using System.Collections;

namespace DI.DiNetWinServiceObject
{
    /// <summary>
    /// メモマスターリモートサービス
    /// </summary>
    public class HM17VERSIONLISTDC : CMObjectDC
    {
        // 部位マスタリスト
        private ArrayList _HM17VERSION = new ArrayList();

        /// <summary>
        /// 部位マスタリスト
        /// </summary>
        public ArrayList HM17VERSION
        {
            get { return _HM17VERSION; }
            set { _HM17VERSION = value; }
        }

       
    }
}

