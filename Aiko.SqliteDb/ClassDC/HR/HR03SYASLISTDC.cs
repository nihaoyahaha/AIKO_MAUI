using DI.DiNetWinServiceObject;
using System.Collections;

namespace DI.DiNetWinServiceObject
{
    /// <summary>
    /// マップガイドマスターリモートサービス
    /// </summary>
   public class HR03SYASLISTDC: CMObjectDC
    {
        // 写真テーブルリスト
        private ArrayList _HR03ITEM = new ArrayList();

        /// <summary>
        /// 写真テーブルタリスト
        /// </summary>
        public ArrayList HR03ITEM
        {
            get { return _HR03ITEM; }
            set { _HR03ITEM = value; }
        }

       
    }
}
