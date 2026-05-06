using DI.DiNetWinServiceObject;

namespace DI.DiNetWinServiceObject
{
    /// <summary>
    /// 写真テーブルリモートサービス
    /// </summary>
   public class HR03SYASDC: CMObjectDC
    {

        /// <summary>
        /// 工事コード
        /// </summary>
        public string HR03001 { get; set; } = string.Empty;
        /// <summary>
        /// 写真コード
        /// </summary>
        public string HR03002 { get; set; } = string.Empty;
        /// <summary>
        /// アイテムコード
        /// </summary>
        public string HR03003 { get; set; } = string.Empty;
        /// <summary>
        /// 確認項目コード
        /// </summary>
        public string HR03004 { get; set; } = string.Empty;
        /// <summary>
        /// 並び順
        /// </summary>
        public string HR03005 { get; set; } = string.Empty;
        /// <summary>
        /// 分類
        /// </summary>
        public int HR03006 { get; set; }
        /// <summary>
        /// 方向
        /// </summary>
        public int HR03007 { get; set; }
        /// <summary>
        /// コメント
        /// </summary>
        public string HR03008 { get; set; } = string.Empty;
        /// <summary>
        /// 撮影日付
        /// </summary>
        public int HR03009 { get; set; }
        /// <summary>
        /// 撮影時刻
        /// </summary>
        public int HR03010 { get; set; }
        /// <summary>
        /// 作成日時
        /// </summary>
        public DateTime HR03011 { get; set; }
        /// <summary>
        /// 作成オペレータ
        /// </summary>
        public string HR03012 { get; set; } = string.Empty;
        /// <summary>
        /// 更新日時
        /// </summary>
        public DateTime HR03013 { get; set; }
        /// <summary>
        /// 更新オペレータ
        /// </summary>
        public string HR03014 { get; set; } = string.Empty;
        /// <summary>
        /// 同期日時
        /// </summary>
        public DateTime HR03015 { get; set; }
        /// <summary>
        /// 同期オペレータ
        /// </summary>
        public string HR03016 { get; set; } = string.Empty;

		/// <summary>
		/// 写真方式 0：JPG 1：SVG
		/// </summary>
		public int HR03017 { get; set; }
		/// <summary>
		/// //SVGレイヤ表示
		/// </summary>
		public int HR03018 { get; set; }

		/// <summary>
		/// 撮影方向
		/// </summary>
		public string HR03019 { get; set; } = string.Empty;
		/// <summary>
		/// 同期日時
		/// </summary>
		public DateTime HR03020 { get; set; }
		/// <summary>
		/// 変更フラグ
		/// </summary>
		public string HMCHANGE { get; set; } = string.Empty;

    }
}
