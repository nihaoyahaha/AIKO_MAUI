using DI.DiNetWinServiceObject;

namespace DI.DiNetWinServiceObject
{
    /// <summary>
    /// 写真テーブルリモートサービス
    /// </summary>
   public class HR03SYASDC: CMObjectDC
    {
        //工事コード
        private string _HR03001;
        //写真コード
        private string _HR03002;
        //アイテムコード
        private string _HR03003;
        //確認項目コード
        private string _HR03004;
        //並び順
        private string _HR03005;
        //分類
        private int _HR03006;
        //方向
        private int _HR03007;
        //コメント
        private string _HR03008;
        //撮影日付
        private int _HR03009;
        //撮影時刻
        private int _HR03010;
        //作成日時
        private DateTime _HR03011;
        //作成オペレータ
        private string _HR03012;
        //更新日時
        private DateTime _HR03013;
        //更新オペレータ
        private string _HR03014;
        //同期日時
        private DateTime _HR03015;
        //同期オペレータ
        private string _HR03016;
		//写真方式 0：JPG 1：SVG
		private int _HR03017;
		//SVGレイヤ表示
		//      写真    黒板   注釈
		//7：　　〇      〇     〇
		//3：　　〇      〇     ×
		//5：　　〇      ×     〇
		//1：　　〇      ×     ×
		private int _HR03018;
		// 撮影方向
		private string _HR03019;
		// 撮影方向
		private DateTime _HR03020;
		// 変更フラグ
		private string _HMCHANGE;

        /// <summary>
        /// 工事コード
        /// </summary>
        public string HR03001 { get { return _HR03001; } set { _HR03001 = value; } }
        /// <summary>
        /// 写真コード
        /// </summary>
        public string HR03002 { get { return _HR03002; } set { _HR03002 = value; } }
        /// <summary>
        /// アイテムコード
        /// </summary>
        public string HR03003 { get { return _HR03003; } set { _HR03003 = value; } }
        /// <summary>
        /// 確認項目コード
        /// </summary>
        public string HR03004 { get { return _HR03004; } set { _HR03004 = value; } }
        /// <summary>
        /// 並び順
        /// </summary>
        public string HR03005 { get { return _HR03005; } set { _HR03005 = value; } }
        /// <summary>
        /// 分類
        /// </summary>
        public int HR03006 { get { return _HR03006; } set { _HR03006 = value; } }
        /// <summary>
        /// 方向
        /// </summary>
        public int HR03007 { get { return _HR03007; } set { _HR03007 = value; } }
        /// <summary>
        /// コメント
        /// </summary>
        public string HR03008 { get { return _HR03008; } set { _HR03008 = value; } }
        /// <summary>
        /// 撮影日付
        /// </summary>
        public int HR03009 { get { return _HR03009; } set { _HR03009 = value; } }
        /// <summary>
        /// 撮影時刻
        /// </summary>
        public int HR03010 { get { return _HR03010; } set { _HR03010 = value; } }
        /// <summary>
        /// 作成日時
        /// </summary>
        public DateTime HR03011 { get { return _HR03011; } set { _HR03011 = value; } }
        /// <summary>
        /// 作成オペレータ
        /// </summary>
        public string HR03012 { get { return _HR03012; } set { _HR03012 = value; } }
        /// <summary>
        /// 更新日時
        /// </summary>
        public DateTime HR03013 { get { return _HR03013; } set { _HR03013 = value; } }
        /// <summary>
        /// 更新オペレータ
        /// </summary>
        public string HR03014 { get { return _HR03014; } set { _HR03014 = value; } }
        /// <summary>
        /// 同期日時
        /// </summary>
        public DateTime HR03015 { get { return _HR03015; } set { _HR03015 = value; } }
        /// <summary>
        /// 同期オペレータ
        /// </summary>
        public string HR03016 { get { return _HR03016; } set { _HR03016 = value; } }

		/// <summary>
		/// 写真方式 0：JPG 1：SVG
		/// </summary>
		public int HR03017 { get { return _HR03017; } set { _HR03017 = value; } }
		/// <summary>
		/// //SVGレイヤ表示
		//      写真    黒板   注釈
		//7：　　〇      〇     〇
		//3：　　〇      〇     ×
		//5：　　〇      ×     〇
		//1：　　〇      ×     ×
		/// </summary>
		public int HR03018 { get { return _HR03018; } set { _HR03018 = value; } }

		/// <summary>
		/// 撮影方向
		/// </summary>
		public string HR03019 { get { return _HR03019; } set { _HR03019 = value; } }
		/// <summary>
		/// 同期日時
		/// </summary>
		public DateTime HR03020 { get { return _HR03020; } set { _HR03020 = value; } }
		/// <summary>
		/// 変更フラグ
		/// </summary>
		public string HMCHANGE { get { return _HMCHANGE; } set { _HMCHANGE = value; } }

        
    }
}
