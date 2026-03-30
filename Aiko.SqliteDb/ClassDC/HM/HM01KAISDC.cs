using DI.DiNetWinServiceObject;

namespace DI.DiNetWinServiceObject
{
    /// <summary>
    /// 会社マスターリモートサービス
    /// </summary>
    public class HM01KAISDC : CMObjectDC
    {
        // 会社コード
        private string _HM01001;
        // 会社名
        private string _HM01002;
        // 郵便番号
        private string _HM01003;
        // 都道府県
        private string _HM01004;
        // 市区町村
        private string _HM01005;
        // 番地
        private string _HM01006;
        // 電話番号
        private string _HM01007;
        // FAX
        private string _HM01008;
        // 並び順
        private string _HM01009;
        // 作成日時
        private DateTime _HM01010;
        // 作成オペレータ
        private string _HM01011;
        // 更新日時
        private DateTime _HM01012;
        // 更新オペレータ
        private string _HM01013;
		// 同期日時
		private DateTime _HM01014;
		// 同期オペレータ
		private string _HM01015;
		//会社ID
		private string _HM01016;
		// 会社別IP制限 0:無効　1:有効
		private int _HM01017 = 0;
		// 会社別端末制限 0:無効　1:有効
		private int _HM01018 = 0;

        /// <summary>
        /// 会社コード
        /// </summary>
        public string HM01001
        {
            get { return _HM01001; }
            set { _HM01001 = value; }
        }

        /// <summary>
        /// 会社名
        /// </summary>
        public string HM01002
        {
            get { return _HM01002; }
            set { _HM01002 = value; }
        }

        /// <summary>
        /// 郵便番号
        /// </summary>
        public string HM01003
        {
            get { return _HM01003; }
            set { _HM01003 = value; }
        }

        /// <summary>
        /// 都道府県
        /// </summary>
        public string HM01004
        {
            get { return _HM01004; }
            set { _HM01004 = value; }
        }

        /// <summary>
        /// 市区町村
        /// </summary>
        public string HM01005
        {
            get { return _HM01005; }
            set { _HM01005 = value; }
        }

        /// <summary>
        /// 番地
        /// </summary>
        public string HM01006
        {
            get { return _HM01006; }
            set { _HM01006 = value; }
        }

        /// <summary>
        /// 電話番号
        /// </summary>
        public string HM01007
        {
            get { return _HM01007; }
            set { _HM01007 = value; }
        }

        /// <summary>
        /// FAX
        /// </summary>
        public string HM01008
        {
            get { return _HM01008; }
            set { _HM01008 = value; }
        }

        /// <summary>
        /// 並び順
        /// </summary>
        public string HM01009
        {
            get { return _HM01009; }
            set { _HM01009 = value; }
        }

        /// <summary>
        /// 作成日時
        /// </summary>
        public DateTime HM01010
        {
            get { return _HM01010; }
            set { _HM01010 = value; }
        }

        /// <summary>
        /// 作成オペレータ
        /// </summary>
        public string HM01011
        {
            get { return _HM01011; }
            set { _HM01011 = value; }
        }

        /// <summary>
        /// 更新日時
        /// </summary>
        public DateTime HM01012
        {
            get { return _HM01012; }
            set { _HM01012 = value; }
        }

        /// <summary>
        /// 更新オペレータ
        /// </summary>
        public string HM01013
        {
            get { return _HM01013; }
            set { _HM01013 = value; }
        }
		/// <summary>
		/// 同期日時
		/// </summary>
		public DateTime HM01014
		{
			get { return _HM01014; }
			set { _HM01014 = value; }
		}

		/// <summary>
		/// 同期オペレータ
		/// </summary>
		public string HM01015
		{
			get { return _HM01015; }
			set { _HM01015 = value; }
		}
		/// <summary>
		/// 会社ID
		/// </summary>
		public string HM01016
		{
			get { return _HM01016; }
			set { _HM01016 = value; }
		}

		/// <summary>
		/// 会社別IP制限 0:無効　1:有効
		/// </summary>
		public int HM01017
		{
			get { return _HM01017; }
			set { _HM01017 = value; }
		}

		/// <summary>
		/// 会社別端末制限 0:無効　1:有効
		/// </summary>
		public int HM01018
		{
			get { return _HM01018; }
			set { _HM01018 = value; }
		}
	}
}

