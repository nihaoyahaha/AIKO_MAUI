using DI.DiNetWinServiceObject;
using System.Collections;

namespace DI.DiNetWinServiceObject
{
    public class HR06SYASDELLISTDC:CMObjectDC
    {
        // 写真テーブルリスト
        private ArrayList _HR06ITEM = new ArrayList();

        /// <summary>
        /// 写真テーブルタリスト
        /// </summary>
        public ArrayList HR06ITEM
        {
            get { return _HR06ITEM; }
            set { _HR06ITEM = value; }
        }
    }
}
