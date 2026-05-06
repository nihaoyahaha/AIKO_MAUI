using DI.DiNetWinServiceObject;

namespace DI.DiNetWinServiceObject
{
    /// <summary>
    /// グループマスターリモートサービス
    /// </summary>
    public class HM08GRPMDC : CMObjectDC
    {

        /// <summary>
        /// 工事コード
        /// </summary>
        public string HM08001 { get; set; } = string.Empty;

        /// <summary>
        /// グループコード
        /// </summary>
        public string HM08002 { get; set; } = string.Empty;

        /// <summary>
        /// グループ名
        /// </summary>
        public string HM08003 { get; set; } = string.Empty;

        /// <summary>
        /// 階コード
        /// </summary>
        public string HM08004 { get; set; } = string.Empty;

        /// <summary>
        /// 部位コード
        /// </summary>
        public string HM08005 { get; set; } = string.Empty;

        /// <summary>
        /// 並び順
        /// </summary>
        public string HM08006 { get; set; } = string.Empty;

        /// <summary>
        /// 親グループコード
        /// </summary>
        public string HM08007 { get; set; } = string.Empty;

        /// <summary>
        /// 非表示フラグ
        /// </summary>
        public int HM08008 { get; set; }

        /// <summary>
        /// 作成日時
        /// </summary>
        public DateTime HM08009 { get; set; }

        /// <summary>
        /// 作成オペレータ
        /// </summary>
        public string HM08010 { get; set; } = string.Empty;

        /// <summary>
        /// 更新日時
        /// </summary>
        public DateTime HM08011 { get; set; }

        /// <summary>
        /// 更新オペレータ
        /// </summary>
        public string HM08012 { get; set; } = string.Empty;

        /// <summary>
        /// 同期日時
        /// </summary>
        public DateTime HM08013 { get; set; }

        /// <summary>
        /// 同期オペレータ
        /// </summary>
        public string HM08014 { get; set; } = string.Empty;

        /// <summary>
        /// 変更フラグ
        /// </summary>
        public string HMCHANGE { get; set; } = string.Empty;
    }
}
