using DI.DiNetWinServiceObject;

namespace DI.DiNetWinServiceObject
{
    /// <summary>
    /// オペレータマスターリモートサービス
    /// </summary>
    public class HM02OPERDC : CMObjectDC
    {
        // オペレータコード
        private string _HM02001;
        // オペレータ名称
        private string _HM02002;
        // パスワード
        private string _HM02003;
        // 会社コード
        private string _HM02004;
        // 種別
        private int _HM02005;
        // 在職状態
        private int _HM02006;
        // 並び順
        private string _HM02007;
        // 最終ログイン日時
        private DateTime _HM02008;
        // 作成日時
        private DateTime _HM02009;
        // 作成オペレータ
        private string _HM02010;
        // 更新日時
        private DateTime _HM02011;
        // 更新オペレータ
        private string _HM02012;
		// 同期日時
		private DateTime _HM02013;
		//  同期オペレータ
		private string _HM02014;
		// 変更フラグ
		private string _HMCHANGE;
		//オペレータID
		private string _HM02015;
		// ユーザ別IP制限 0:無効　1:有効
		private int _HM02017 = 0;
		// ユーザ別端末制限 0:無効　1:有効
		private int _HM02018 = 0;
		// 自働取得期間
		private DateTime _HM02019;

		/// <summary>
		/// オペレータコード
		/// </summary>
		public string HM02001
        {
            get { return _HM02001; }
            set { _HM02001 = value; }
        }
        /// <summary>
        /// オペレータ名称
        /// </summary>
        public string HM02002
        {
            get { return _HM02002; }
            set { _HM02002 = value; }
        }
        /// <summary>
        /// パスワード
        /// </summary>
        public string HM02003
        {
            get { return _HM02003; }
            set { _HM02003 = value; }
        }
        /// <summary>
        /// 会社コード
        /// </summary>
        public string HM02004
        {
            get { return _HM02004; }
            set { _HM02004 = value; }
        }
        /// <summary>
        /// 種別
        /// </summary>
        public int HM02005
        {
            get { return _HM02005; }
            set { _HM02005 = value; }
        }
        /// <summary>
        /// 在職状態
        /// </summary>
        public int HM02006
        {
            get { return _HM02006; }
            set { _HM02006 = value; }
        }
        /// <summary>
        /// 並び順
        /// </summary>
        public string HM02007
        {
            get { return _HM02007; }
            set { _HM02007 = value; }
        }
        /// <summary>
        /// 最終ログイン日時
        /// </summary>
        public DateTime HM02008
        {
            get { return _HM02008; }
            set { _HM02008 = value; }
        }
        /// <summary>
        /// 作成日時
        /// </summary>
        public DateTime HM02009
        {
            get { return _HM02009; }
            set { _HM02009 = value; }
        }
        /// <summary>
        /// 作成オペレータ
        /// </summary>
        public string HM02010
        {
            get { return _HM02010; }
            set { _HM02010 = value; }
        }
        /// <summary>
        /// 更新日時
        /// </summary>
        public DateTime HM02011
        {
            get { return _HM02011; }
            set { _HM02011 = value; }
        }
        /// <summary>
        /// 更新オペレータ
        /// </summary>
        public string HM02012
        {
            get { return _HM02012; }
            set { _HM02012 = value; }
        }
		/// <summary>
		/// 同期日時
		/// </summary>
		public DateTime HM02013
		{
			get { return _HM02013; }
			set { _HM02013 = value; }
		}
		/// <summary>
		/// 同期オペレータ
		/// </summary>
		public string HM02014
		{
			get { return _HM02014; }
			set { _HM02014 = value; }
		}
		/// <summary>
		/// 変更フラグ
		/// </summary>
		public string HMCHANGE
        {
            get { return _HMCHANGE; }

            set { _HMCHANGE = value; }
        }

		/// <summary>
		/// オペレータID
		/// </summary>
		public string HM02015
		{
			get { return _HM02015; }
			set { _HM02015 = value; }
		}

		/// <summary>
		/// ユーザ別IP制限 0:無効　1:有効
		/// </summary>
		public int HM02017
		{
			get { return _HM02017; }
			set { _HM02017 = value; }
		}

		/// <summary>
		/// ユーザ別端末制限 0:無効　1:有効
		/// </summary>
		public int HM02018
		{
			get { return _HM02018; }
			set { _HM02018 = value; }
		}

		/// <summary>
		/// 自働取得期間
		/// </summary>
		public DateTime HM02019
		{
			get { return _HM02019; }
			set { _HM02019 = value; }
		}
	}
}

