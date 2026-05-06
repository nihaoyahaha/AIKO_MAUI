using DI.DiNetWinServiceObject;
using System.Collections;

namespace DI.DiNetWinServiceObject
{
    public class HR06SYASDELLISTDC:CMObjectDC
    {

        /// <summary>
        /// 写真テーブルタリスト
        /// </summary>
        public ArrayList HR06ITEM { get; set; } = new ArrayList();
    }
}
