using DI.DiNetWinServiceObject;
using System.Collections;

namespace DI.DiNetWinServiceObject
{
    /// <summary>
    /// マップガイドマスターリモートサービス
    /// </summary>
    public class HR02KSKKLISTDC : CMObjectDC
    {
        // 検査結果テーブルリスト
        private ArrayList _HR02KSKK = new ArrayList();

        /// <summary>
        /// 検査結果テーブルタリスト
        /// </summary>
        public ArrayList HR02KSKK
        {
            get { return _HR02KSKK; }
            set { _HR02KSKK = value; }
        }

        
    }

}
