using DI.DiNetWinServiceObject;
using System.Collections;

namespace DI.DiNetWinServiceObject
{
    public class HR04KSHISLISTDC:CMObjectDC
    {
        // 写真テーブルリスト
        private ArrayList _HR04ITEM = new ArrayList();

        /// <summary>
        /// 写真テーブルタリスト
        /// </summary>
        public ArrayList HR04ITEM
        {
            get { return _HR04ITEM; }
            set { _HR04ITEM = value; }
        }



    }
}
