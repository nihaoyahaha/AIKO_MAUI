using DI.DiNetWinServiceObject;

namespace DI.DiNetWinServiceObject
{
    /// <summary>
    /// 工程マスターリモートサービス
    /// </summary>
    public class HM09PROCDC : CMObjectDC
    {
        // 工事コード
        private string _HM09001;
        // 工程コード
        private string _HM09002;
        // 工程名
        private string _HM09003;
        // 並び順
        private string _HM09004;
        // 非表示フラグ
        private int _HM09005;
        // 作成日時
        private DateTime _HM09006;
        // 作成オペレータ
        private string _HM09007;
        // 更新日時
        private DateTime _HM09008;
        // 更新オペレータ
        private string _HM09009;
        // 変更フラグ
        private string _HMCHANGE;

        /// <summary>
        /// 工事コード
        /// </summary>
        public string HM09001 {
            get { return _HM09001; }
            set { _HM09001 = value; }
        }

        /// <summary>
        /// 工程コード
        /// </summary>
        public string HM09002 {
            get { return _HM09002; }
            set { _HM09002 = value; }
        }

        /// <summary>
        /// 工程名
        /// </summary>
        public string HM09003 {
            get { return _HM09003; }
            set { _HM09003 = value; }
        }

        /// <summary>
        /// 並び順
        /// </summary>
        public string HM09004 {
            get { return _HM09004; }
            set { _HM09004 = value; }
        }

        /// <summary>
        /// 非表示フラグ
        /// </summary>
        public int HM09005 {
            get { return _HM09005; }
            set { _HM09005 = value; }
        }

        /// <summary>
        /// 作成日時
        /// </summary>
        public DateTime HM09006 {
            get { return _HM09006; }
            set { _HM09006 = value; }
        }

        /// <summary>
        /// 作成オペレータ
        /// </summary>
        public string HM09007 {
            get { return _HM09007; }
            set { _HM09007 = value; }
        }

        /// <summary>
        /// 更新日時
        /// </summary>
        public DateTime HM09008 {
            get { return _HM09008; }
            set { _HM09008 = value; }
        }

        /// <summary>
        /// 更新オペレータ
        /// </summary>
        public string HM09009 {
            get { return _HM09009; }
            set { _HM09009 = value; }
        }

        /// <summary>
        /// 変更フラグ
        /// </summary>
        public string HMCHANGE {
            get { return _HMCHANGE; }
            set { _HMCHANGE = value; }
        }
    }
}
