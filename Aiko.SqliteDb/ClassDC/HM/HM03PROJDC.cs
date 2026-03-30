using DI.DiNetWinServiceObject;

namespace DI.DiNetWinServiceObject
{
    /// <summary>
    /// 工事マスターリモートサービス
    /// </summary>
    public class HM03PROJDC : CMObjectDC
    {
        // 工事コード
        private string _HM03001;
        // 工事名
        private string _HM03002;
        // 工事事務所
        private string _HM03003;
        // 会社コード
        private string _HM03004;
        // 工事完了区分
        private int _HM03005;
        // 作成日時
        private DateTime _HM03006;
        // 作成オペレータ
        private string _HM03007;
        // 更新日時
        private DateTime _HM03008;
        // 更新オペレータ
        private string _HM03009;
		// 同期日時
		private DateTime _HM03010;
		// 同期オペレータ
		private string _HM03011;
		// 竣工日付
		private int _HM03012;
		//ゼネコンコード
		private string _HM03013;
		// 現場別IP制限
		private int _HM03014;

		/// <summary>
		/// 工事コード
		/// </summary>
		public string HM03001 {
            get { return _HM03001; }
            set { _HM03001 = value; }
        }
        /// <summary>
        /// 工事名
        /// </summary>
        public string HM03002 {
            get { return _HM03002; }
            set { _HM03002 = value; }
        }
        /// <summary>
        /// 工事事務所
        /// </summary>
        public string HM03003 {
            get { return _HM03003; }
            set { _HM03003 = value; }
        }
        /// <summary>
        /// 会社コード
        /// </summary>
        public string HM03004 {
            get { return _HM03004; }
            set { _HM03004 = value; }
        }
        /// <summary>
        /// 工事完了区分
        /// </summary>
        public int HM03005 {
            get { return _HM03005; }
            set { _HM03005 = value; }
        }
        /// <summary>
        /// 作成日時
        /// </summary>
        public DateTime HM03006 {
            get { return _HM03006; }
            set { _HM03006 = value; }
        }
        /// <summary>
        /// 作成オペレータ
        /// </summary>
        public string HM03007 {
            get { return _HM03007; }
            set { _HM03007 = value; }
        }
        /// <summary>
        /// 更新日時
        /// </summary>
        public DateTime HM03008 {
            get { return _HM03008; }
            set { _HM03008 = value; }
        }
        /// <summary>
        /// 更新オペレータ
        /// </summary>
        public string HM03009 {
            get { return _HM03009; }
            set { _HM03009 = value; }
        }
		/// <summary>
		/// 同期日時
		/// </summary>
		public DateTime HM03010
		{
			get { return _HM03010; }
			set { _HM03010 = value; }
		}
		/// <summary>
		/// 同期オペレータ
		/// </summary>
		public string HM03011
		{
			get { return _HM03011; }
			set { _HM03011 = value; }
		}
		/// <summary>
		/// 竣工日付
		/// </summary>
		public int HM03012
        {
            get { return _HM03012; }
            set { _HM03012 = value; }
        }

		/// <summary>
		///ゼネコンコード
		/// </summary>
		public string HM03013
		{
			get { return _HM03013; }
			set { _HM03013 = value; }
		}

		/// <summary>
		/// 現場別IP制限
		/// </summary>
		public int HM03014
		{
			get { return _HM03014; }
			set { _HM03014 = value; }
		}
	}
}
