using DI.DiNetWinServiceObject;

namespace DI.DiNetWinServiceObject
{
    /// <summary>
    /// 工程マスターリモートサービス
    /// </summary>
    public class HM09PROCDC : CMObjectDC
    {

        /// <summary>
        /// 工事コード
        /// </summary>
        public string HM09001 { get; set; } = string.Empty;

        /// <summary>
        /// 工程コード
        /// </summary>
        public string HM09002 { get; set; } = string.Empty;

        /// <summary>
        /// 工程名
        /// </summary>
        public string HM09003 { get; set; } = string.Empty;

        /// <summary>
        /// 並び順
        /// </summary>
        public string HM09004 { get; set; } = string.Empty;

        /// <summary>
        /// 非表示フラグ
        /// </summary>
        public int HM09005 { get; set; }

        /// <summary>
        /// 作成日時
        /// </summary>
        public DateTime HM09006 { get; set; }

        /// <summary>
        /// 作成オペレータ
        /// </summary>
        public string HM09007 { get; set; } = string.Empty;

        /// <summary>
        /// 更新日時
        /// </summary>
        public DateTime HM09008 { get; set; }

        /// <summary>
        /// 更新オペレータ
        /// </summary>
        public string HM09009 { get; set; } = string.Empty;

        /// <summary>
        /// 変更フラグ
        /// </summary>
        public string HMCHANGE { get; set; } = string.Empty;
    }
}
