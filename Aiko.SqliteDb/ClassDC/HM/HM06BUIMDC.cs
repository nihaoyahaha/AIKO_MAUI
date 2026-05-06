using DI.DiNetWinServiceObject;

namespace DI.DiNetWinServiceObject
{
    /// <summary>
    /// 部位マスターリモートサービス
    /// </summary>
    public class HM06BUIMDC : CMObjectDC
    {

        /// <summary>
        /// 工事コード
        /// </summary>
        public string HM06001 { get; set; } = string.Empty;

        /// <summary>
        /// 部位コード
        /// </summary>
        public string HM06002 { get; set; } = string.Empty;

        /// <summary>
        /// 部位名
        /// </summary>
        public string HM06003 { get; set; } = string.Empty;

        /// <summary>
        /// 記号
        /// </summary>
        public string HM06004 { get; set; } = string.Empty;

        /// <summary>
        /// 階指定
        /// </summary>
        public int HM06005 { get; set; }

        /// <summary>
        /// 並び順
        /// </summary>
        public string HM06006 { get; set; } = string.Empty;

        /// <summary>
        /// 共通事項断面コード
        /// </summary>
        public string HM06007 { get; set; } = string.Empty;

        /// <summary>
        /// タイトル断面コード
        /// </summary>
        public string HM06008 { get; set; } = string.Empty;

        /// <summary>
        /// 非表示フラグ
        /// </summary>
        public int HM06009 { get; set; }

        /// <summary>
        /// 作成日時
        /// </summary>
        public DateTime HM06010 { get; set; }

        /// <summary>
        /// 作成オペレータ
        /// </summary>
        public string HM06011 { get; set; } = string.Empty;

        /// <summary>
        /// 更新日時
        /// </summary>
        public DateTime HM06012 { get; set; }

        /// <summary>
        /// 更新オペレータ
        /// </summary>
        public string HM06013 { get; set; } = string.Empty;

        /// <summary>
        /// 同期日時
        /// </summary>
        public DateTime HM06014 { get; set; }

        /// <summary>
        /// 同期オペレータ
        /// </summary>
        public string HM06015 { get; set; } = string.Empty;

        /// <summary>
        /// アイコンリソースファイル
        /// </summary>
        public string HM06016 { get; set; } = string.Empty;

        /// <summary>
        /// 参照
        /// </summary>
        public int HM06017 { get; set; }

        /// <summary>
        /// 命名規則
        /// </summary>
        public int HM06018 { get; set; }

        /// <summary>
        /// 変更フラグ
        /// </summary>
        public string HMCHANGE { get; set; } = string.Empty;

    }
}
