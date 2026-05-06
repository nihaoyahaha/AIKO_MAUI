using DI.DiNetWinServiceObject;
using System.Collections;

namespace DI.DiNetWinServiceObject
{
    /// <summary>
    /// 確認項目リストリモートサービス
    /// </summary>
    public class HM13KNKMLISTDC : CMObjectDC
    {

        /// <summary>
        /// 確認項目リスト
        /// </summary>
        public ArrayList HM13KNKM { get; set; } = new ArrayList();
    }
}
