using DI.DiNetWinServiceObject;

namespace DI.DiNetWinServiceObject
{
    /// <summary>
    /// 工事マスターリモートサービス
    /// </summary>
    public class HM03PROJDC : CMObjectDC
    {

		/// <summary>
		/// 工事コード
		/// </summary>
		public string HM03001 { get; set; } = string.Empty;
        /// <summary>
        /// 工事名
        /// </summary>
        public string HM03002 { get; set; } = string.Empty;
        /// <summary>
        /// 工事事務所
        /// </summary>
        public string HM03003 { get; set; } = string.Empty;
        /// <summary>
        /// 会社コード
        /// </summary>
        public string HM03004 { get; set; } = string.Empty;
        /// <summary>
        /// 工事完了区分
        /// </summary>
        public int HM03005 { get; set; }
        /// <summary>
        /// 作成日時
        /// </summary>
        public DateTime HM03006 { get; set; }
        /// <summary>
        /// 作成オペレータ
        /// </summary>
        public string HM03007 { get; set; } = string.Empty;
        /// <summary>
        /// 更新日時
        /// </summary>
        public DateTime HM03008 { get; set; }
        /// <summary>
        /// 更新オペレータ
        /// </summary>
        public string HM03009 { get; set; } = string.Empty;
		/// <summary>
		/// 同期日時
		/// </summary>
		public DateTime HM03010 { get; set; }
		/// <summary>
		/// 同期オペレータ
		/// </summary>
		public string HM03011 { get; set; } = string.Empty;
		/// <summary>
		/// 竣工日付
		/// </summary>
		public int HM03012 { get; set; }

		/// <summary>
		///ゼネコンコード
		/// </summary>
		public string HM03013 { get; set; } = string.Empty;

		/// <summary>
		/// 現場別IP制限
		/// </summary>
		public int HM03014 { get; set; }
	}
}
