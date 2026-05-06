using DI.DiNetWinServiceObject;

namespace DI.DiNetWinServiceObject
{
    /// <summary>
    /// オペレータサブテーブルリモートサービス
    /// </summary>
    public class HM15OSUBDC : CMObjectDC
    {

		/// <summary>
		/// オペレータコード
		/// </summary>
		public string HM15001 { get; set; } = string.Empty;

        /// <summary>
        /// 工事コード
        /// </summary>
        public string HM15002 { get; set; } = string.Empty;

        /// <summary>
        /// 作成日時
        /// </summary>
        public DateTime HM15003 { get; set; }

        /// <summary>
        /// 作成オペレータ
        /// </summary>
        public string HM15004 { get; set; } = string.Empty;

        /// <summary>
        /// 更新日時
        /// </summary>
        public DateTime HM15005 { get; set; }

        /// <summary>
        /// 更新オペレータ
        /// </summary>
        public string HM15006 { get; set; } = string.Empty;
		/// <summary>
		/// 同期日時
		/// </summary>
		public DateTime HM15007 { get; set; }

		/// <summary>
		/// 同期オペレータ
		/// </summary>
		public string HM15008 { get; set; } = string.Empty;
		/// <summary>
		/// 会社コード
		/// </summary>
		public string HM15009 { get; set; } = string.Empty;

	}
}
