using DI.DiNetWinServiceObject;

namespace DI.DiNetWinServiceObject
{
    /// <summary>
    /// オペレータサブテーブルリモートサービス
    /// </summary>
    public class HM15OSUBDC : CMObjectDC
    {
        // オペレータコード
        private string _HM15001;
        // 工事コード
        private string _HM15002;
        // 作成日時
        private DateTime _HM15003;
        // 作成オペレータ
        private string _HM15004;
        // 更新日時
        private DateTime _HM15005;
        // 更新オペレータ
        private string _HM15006;
		// 同期日時
		private DateTime _HM15007;
		// 同期オペレータ
		private string _HM15008;
		//会社コード
		private string _HM15009;

		/// <summary>
		/// オペレータコード
		/// </summary>
		public string HM15001 {
            get { return _HM15001; }
            set { _HM15001 = value; }
        }

        /// <summary>
        /// 工事コード
        /// </summary>
        public string HM15002 {
            get { return _HM15002; }
            set { _HM15002 = value; }
        }

        /// <summary>
        /// 作成日時
        /// </summary>
        public DateTime HM15003 {
            get { return _HM15003; }
            set { _HM15003 = value; }
        }

        /// <summary>
        /// 作成オペレータ
        /// </summary>
        public string HM15004 {
            get { return _HM15004; }
            set { _HM15004 = value; }
        }

        /// <summary>
        /// 更新日時
        /// </summary>
        public DateTime HM15005 {
            get { return _HM15005; }
            set { _HM15005 = value; }
        }

        /// <summary>
        /// 更新オペレータ
        /// </summary>
        public string HM15006 {
            get { return _HM15006; }
            set { _HM15006 = value; }
        }
		/// <summary>
		/// 同期日時
		/// </summary>
		public DateTime HM15007
		{
			get { return _HM15007; }
			set { _HM15007 = value; }
		}

		/// <summary>
		/// 同期オペレータ
		/// </summary>
		public string HM15008
		{
			get { return _HM15008; }
			set { _HM15008 = value; }
		}
		/// <summary>
		/// 会社コード
		/// </summary>
		public string HM15009
		{
			get { return _HM15009; }
			set { _HM15009 = value; }
		}

	}
}

