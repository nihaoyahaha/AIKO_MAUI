using DI.DiNetWinServiceObject;

namespace DI.DiNetWinServiceObject
{
    /// <summary>
    /// メモマスターリモートサービス
    /// </summary>
    public class HM11MEMODC : CMObjectDC
    {

        /// <summary>
        /// 工事コード
        /// </summary>
        public string HM11001 { get; set; } = string.Empty;

        /// <summary>
        /// メモコード
        /// </summary>
        public string HM11002 { get; set; } = string.Empty;

        /// <summary>
        /// メモ本文
        /// </summary>
        public string HM11003 { get; set; } = string.Empty;

        /// <summary>
        /// 並び順
        /// </summary>
        public string HM11004 { get; set; } = string.Empty;

        /// <summary>
        /// 作成日時
        /// </summary>
        public DateTime HM11005 { get; set; }

        /// <summary>
        /// 作成オペレータ
        /// </summary>
        public string HM11006 { get; set; } = string.Empty;

        /// <summary>
        /// 更新日時
        /// </summary>
        public DateTime HM11007 { get; set; }

        /// <summary>
        /// 更新オペレータ
        /// </summary>
        public string HM11008 { get; set; } = string.Empty;

        /// <summary>
        /// 変更フラグ
        /// </summary>
        public string HMCHANGE { get; set; } = string.Empty;

    }
}
