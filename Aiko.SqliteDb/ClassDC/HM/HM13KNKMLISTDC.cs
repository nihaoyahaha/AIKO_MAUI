using DI.DiNetWinServiceObject;
using System.Collections;

namespace DI.DiNetWinServiceObject
{
    /// <summary>
    /// 確認項目リストリモートサービス
    /// </summary>
    public class HM13KNKMLISTDC : CMObjectDC
    {
        // 確認項目リスト
        private ArrayList _HM13KNKM = new ArrayList();

        /// <summary>
        /// 確認項目リスト
        /// </summary>
        public ArrayList HM13KNKM {
            get { return _HM13KNKM; }
            set { _HM13KNKM = value; }
        }
    }
}

