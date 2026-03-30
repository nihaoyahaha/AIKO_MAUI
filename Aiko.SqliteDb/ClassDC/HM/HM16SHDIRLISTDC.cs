using DI.DiNetWinServiceObject;
using System.Collections;

namespace DI.DiNetWinServiceObject
{
    /// <summary>
    /// マップガイドマスターリモートサービス
    /// </summary>
    public class HM16SHDIRLISTDC : CMObjectDC
    {
        // 部位マスタリスト
        private ArrayList _HM16SHDIR = new ArrayList();

        /// <summary>
        /// 部位マスタリスト
        /// </summary>
        public ArrayList HM16SHDIR
        {
            get { return _HM16SHDIR; }
            set { _HM16SHDIR = value; }
        }

      
    }
}

