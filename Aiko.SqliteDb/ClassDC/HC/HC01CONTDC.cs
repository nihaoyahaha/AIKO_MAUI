using DI.DiNetWinServiceObject;

namespace DI.DiNetWinServiceObject
{
    /// <summary>
    /// コントロールファイル
    /// </summary>
    public class HC01CONTDC : CMObjectDC
    {
		/// <summary>
		/// コード
		/// </summary>
		public string HC01001 { get; set; } = string.Empty;

        /// <summary>
        /// 会社名
        /// </summary>
        public string HC01002 { get; set; } = string.Empty;

        /// <summary>
        /// 郵便番号
        /// </summary>
        public string HC01003 { get; set; } = string.Empty;

        /// <summary>
        /// 都道府県
        /// </summary>
        public string HC01004 { get; set; } = string.Empty;

        /// <summary>
        /// 市区町村
        /// </summary>
        public string HC01005 { get; set; } = string.Empty;

        /// <summary>
        /// 番地
        /// </summary>
        public string HC01006 { get; set; } = string.Empty;

        /// <summary>
        /// 電話番号
        /// </summary>
        public string HC01007 { get; set; } = string.Empty;

        /// <summary>
        /// FAX番号
        /// </summary>
        public string HC01008 { get; set; } = string.Empty;

        /// <summary>
        /// ファイルサーバタイプ
        /// </summary>
        public int HC01009 { get; set; }

        /// <summary>
        /// ファイルサーバアドレス
        /// </summary>
        public string HC01010 { get; set; } = string.Empty;

        /// <summary>
        /// ファイルサーバユーザID
        /// </summary>
        public string HC01011 { get; set; } = string.Empty;

        /// <summary>
        /// ファイルサーバパスワード
        /// </summary>
        public string HC01012 { get; set; } = string.Empty;

        /// <summary>
        /// 図面ファイルフォル
        /// </summary>
        public string HC01013 { get; set; } = string.Empty;
        /// <summary>
        /// 作成日時
        /// </summary>
        public DateTime HC01014 { get; set; }
        /// <summary>
        /// 作成オペレータ
        /// </summary>
        public string HC01015 { get; set; } = string.Empty;
        /// <summary>
        /// 更新日時
        /// </summary>
        public DateTime HC01016 { get; set; }
        /// <summary>
        /// 更新オペレータ
        /// </summary>
        public string HC01017 { get; set; } = string.Empty;

		/// <summary>
		/// ファイルサーバポート
		/// </summary>
		public int HC01020 { get; set; }

		/// <summary>
		/// ファイルサーバの暗号化
		/// </summary>
		public int HC01021 { get; set; }
	}
}