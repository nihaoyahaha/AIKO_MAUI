using DI.DiNetWinServiceObject;

namespace DI.DiNetWinServiceObject
{
    /// <summary>
    /// メモマスターリモートサービス
    /// </summary>
    public class HM17VERSIONDC : CMObjectDC
    {
        // コンピューター名
        private string _HM17001;
        // オペレータコード
        private string _HM17002;
        // オペレータ名称
        private string _HM17003;
        // PC利用バージョン
        private string _HM17004;
        // UWP利用バージョン
        private string _HM17005;
        // 作成日時
        private DateTime _HM17006;
        // 作成オペレータ
        private string _HM17007;
        // 更新日時
        private DateTime _HM17008;
        // 更新オペレータ
        private string _HM17009;
        // 同期日時
        private DateTime _HM17010;
        // 同期オペレータ
        private string _HM17011;
        // 変更フラグ
        private string _HMCHANGE;

		//会社コード
		private string _HM17012;
		//オペレータID
		private string _HM17013;
		// 端末IPアドレス
		private string _HM17014;
		// 端末のserial numberまたはIMEI
		private string _HM17015;
		// 最終ログイン日時
		private DateTime _HM17016;
		// ログイン中
		private int _HM17017;
		// 工事コード
		private string _HM17018;

		/// <summary>
		/// コンピューター名
		/// </summary>
		public string HM17001
        {
            get { return _HM17001; }
            set { _HM17001 = value; }
        }

        /// <summary>
        /// オペレータコード
        /// </summary>
        public string HM17002
        {
            get { return _HM17002; }
            set { _HM17002 = value; }
        }

        /// <summary>
        /// オペレータ名称
        /// </summary>
        public string HM17003
        {
            get { return _HM17003; }
            set { _HM17003 = value; }
        }

        /// <summary>
        /// PC利用バージョン
        /// </summary>
        public string HM17004
        {
            get { return _HM17004; }
            set { _HM17004 = value; }
        }

        /// <summary>
        /// UWP利用バージョン
        /// </summary>
        public string HM17005
        {
            get { return _HM17005; }
            set { _HM17005 = value; }
        }

        /// <summary>
        /// 作成日時
        /// </summary>
        public DateTime HM17006
        {
            get { return _HM17006; }
            set { _HM17006 = value; }
        }

        /// <summary>
        /// 作成オペレータ
        /// </summary>
        public string HM17007
        {
            get { return _HM17007; }
            set { _HM17007 = value; }
        }

        /// <summary>
        /// 更新日時
        /// </summary>
        public DateTime HM17008
        {
            get { return _HM17008; }
            set { _HM17008 = value; }
        }

        /// <summary>
        /// 更新オペレータ
        /// </summary>
        public string HM17009
        {
            get { return _HM17009; }
            set { _HM17009 = value; }
        }

        /// <summary>
        /// 同期日時
        /// </summary>
        public DateTime HM17010
        {
            get { return _HM17010; }
            set { _HM17010 = value; }
        }

        /// <summary>
        /// 同期オペレータ
        /// </summary>
        public string HM17011
        {
            get { return _HM17011; }
            set { _HM17011 = value; }
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
		/// 会社コード
		/// </summary>
		public string HM17012
		{
			get { return _HM17012; }
			set { _HM17012 = value; }
		}

		/// <summary>
		/// オペレータID
		/// </summary>
		public string HM17013
		{
			get { return _HM17013; }
			set { _HM17013 = value; }
		}

		/// <summary>
		/// 端末IPアドレス
		/// </summary>
		public string HM17014
		{
			get { return _HM17014; }
			set { _HM17014 = value; }
		}

		/// <summary>
		/// 端末のserial numberまたはIMEI
		/// </summary>
		public string HM17015
		{
			get { return _HM17015; }
			set { _HM17015 = value; }
		}

		/// <summary>
		/// 最終ログイン日時
		/// </summary>
		public DateTime HM17016
		{
			get { return _HM17016; }
			set { _HM17016 = value; }
		}

		/// <summary>
		/// ログイン中
		/// </summary>
		public int HM17017
		{
			get { return _HM17017; }
			set { _HM17017 = value; }
		}

		/// <summary>
		/// ログイン中
		/// </summary>
		public string HM17018
		{
			get { return _HM17018; }
			set { _HM17018 = value; }
		}
	}
}
