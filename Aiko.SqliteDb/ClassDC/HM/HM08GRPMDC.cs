using DI.DiNetWinServiceObject;

namespace DI.DiNetWinServiceObject
{
    /// <summary>
    /// グループマスターリモートサービス
    /// </summary>
    public class HM08GRPMDC : CMObjectDC
    {
        // 工事コード
        private string _HM08001;
        // グループコード
        private string _HM08002;
        // グループ名
        private string _HM08003;
        // 階コード
        private string _HM08004;
        // 部位コード
        private string _HM08005;
        // 並び順
        private string _HM08006;
        // 親グループコード
        private string _HM08007;
        // 非表示フラグ
        private int _HM08008;
        // 作成日時
        private DateTime _HM08009;
        // 作成オペレータ
        private string _HM08010;
        // 更新日時
        private DateTime _HM08011;
        // 更新オペレータ
        private string _HM08012;
        // 同期日時
        private DateTime _HM08013;
        //同期オペレータ
        private string _HM08014;
        // 変更フラグ
        private string _HMCHANGE;

        /// <summary>
        /// 工事コード
        /// </summary>
        public string HM08001 {
            get { return _HM08001; }

            set { _HM08001 = value; }
        }

        /// <summary>
        /// グループコード
        /// </summary>
        public string HM08002 {
            get { return _HM08002; }

            set { _HM08002 = value; }
        }

        /// <summary>
        /// グループ名
        /// </summary>
        public string HM08003 {
            get { return _HM08003; }

            set { _HM08003 = value; }
        }

        /// <summary>
        /// 階コード
        /// </summary>
        public string HM08004 {
            get { return _HM08004; }

            set { _HM08004 = value; }
        }

        /// <summary>
        /// 部位コード
        /// </summary>
        public string HM08005 {
            get { return _HM08005; }

            set { _HM08005 = value; }
        }

        /// <summary>
        /// 並び順
        /// </summary>
        public string HM08006 {
            get { return _HM08006; }

            set { _HM08006 = value; }
        }

        /// <summary>
        /// 親グループコード
        /// </summary>
        public string HM08007 {
            get { return _HM08007; }

            set { _HM08007 = value; }
        }

        /// <summary>
        /// 非表示フラグ
        /// </summary>
        public int HM08008 {
            get { return _HM08008; }

            set { _HM08008 = value; }
        }

        /// <summary>
        /// 作成日時
        /// </summary>
        public DateTime HM08009 {
            get { return _HM08009; }

            set { _HM08009 = value; }
        }

        /// <summary>
        /// 作成オペレータ
        /// </summary>
        public string HM08010 {
            get { return _HM08010; }

            set { _HM08010 = value; }
        }

        /// <summary>
        /// 更新日時
        /// </summary>
        public DateTime HM08011 {
            get { return _HM08011; }

            set { _HM08011 = value; }
        }

        /// <summary>
        /// 更新オペレータ
        /// </summary>
        public string HM08012 {
            get { return _HM08012; }

            set { _HM08012 = value; }
        }

        /// <summary>
        /// 同期日時
        /// </summary>
        public DateTime HM08013 {
            get { return _HM08013; }

            set { _HM08013 = value; }
        }

        /// <summary>
        /// 同期オペレータ
        /// </summary>
        public string HM08014 {
            get { return _HM08014; }

            set { _HM08014 = value; }
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

