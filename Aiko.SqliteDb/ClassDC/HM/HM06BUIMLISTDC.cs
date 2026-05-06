using DI.DiNetWinServiceObject;
using System.Collections;

namespace DI.DiNetWinServiceObject
{
    /// <summary>
    /// 部位リストリモートサービス
    /// </summary>
    public class HM06BUIMLISTDC : CMObjectDC
    {

        /// <summary>
        /// 部位マスタリスト
        /// </summary>
        public ArrayList HM06BUIM { get; set; } = new ArrayList();

    }
}
