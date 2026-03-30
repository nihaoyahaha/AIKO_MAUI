using DI.DiNetWinServiceObject;

namespace DI.DiNetWinServiceObject
{
    /// <summary>
    /// 工区多辺形情報
    /// </summary>
    public class HR05KOKUMINFODC : CMObjectDC
    {
        // 工事コード
        private string _HR05001;
        // アイテムコード
        private string _HR05002;
        // 頂点No
        private int _HR05003;
        // X座標
        private int _HR05004;
        // Y座標
        private int _HR05005;
        // 作成日時
        private DateTime _HR05006;
        // 作成オペレータ
        private string _HR05007;
        // 更新日時
        private DateTime _HR05008;
        // 更新オペレータ
        private string _HR05009;

        /// <summary>
        /// 工事コード
        /// </summary>
        public string HR05001 { get { return _HR05001; } set { _HR05001 = value; } }
        /// <summary>
        /// アイテムコード
        /// </summary>
        public string HR05002 { get { return _HR05002; } set { _HR05002 = value; } }
        /// <summary>
        /// 頂点No
        /// </summary>
        public int HR05003 { get { return _HR05003; } set { _HR05003 = value; } }
        /// <summary>
        /// X座標
        /// </summary>
        public int HR05004 { get { return _HR05004; } set { _HR05004 = value; } }
        /// <summary>
        /// Y座標
        /// </summary>
        public int HR05005 { get { return _HR05005; } set { _HR05005 = value; } }
        /// <summary>
        /// 作成日時
        /// </summary>
        public DateTime HR05006 { get { return _HR05006; } set { _HR05006 = value; } }
        /// <summary>
        /// 作成オペレータ
        /// </summary>
        public string HR05007 { get { return _HR05007; } set { _HR05007 = value; } }
        /// <summary>
        /// 更新日時
        /// </summary>
        public DateTime HR05008 { get { return _HR05008; } set { _HR05008 = value; } }
        /// <summary>
        /// 更新オペレータ
        /// </summary>
        public string HR05009 { get { return _HR05009; } set { _HR05009 = value; } }
        
    }
}
