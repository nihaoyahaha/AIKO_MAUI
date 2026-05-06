using DI.DiNetWinServiceObject;
using System.Collections;

namespace DI.DiNetWinServiceObject
{
    public class HR04KSHISLISTDC:CMObjectDC
    {

        /// <summary>
        /// 写真テーブルタリスト
        /// </summary>
        public ArrayList HR04ITEM { get; set; } = new ArrayList();

    }
}
