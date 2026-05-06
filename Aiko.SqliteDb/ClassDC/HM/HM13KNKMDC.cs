using DI.DiNetWinServiceObject;

namespace DI.DiNetWinServiceObject
{
    /// <summary>
    /// 確認項目リストリモートサービス
    /// </summary>
    public class HM13KNKMDC : CMObjectDC
    {

        /// <summary>
        /// 工事コード
        /// </summary>
        public string HM13001 { get; set; } = string.Empty;

        /// <summary>
        ///  部位コード
        /// </summary>
        public string HM13002 { get; set; } = string.Empty;

        /// <summary>
        /// 工程コード
        /// </summary>
        public string HM13003 { get; set; } = string.Empty;

        /// <summary>
        ///  確認項目コード
        /// </summary>
        public string HM13004 { get; set; } = string.Empty;

        /// <summary>
        ///  確認項目名
        /// </summary>
        public string HM13005 { get; set; } = string.Empty;

        /// <summary>
        ///  並び順
        /// </summary>
        public string HM13006 { get; set; } = string.Empty;

        /// <summary>
        ///  参考項目(検査不要)
        /// </summary>
        public int HM13007 { get; set; }

        /// <summary>
        ///  個別(全体検査)
        /// </summary>
        public int HM13008 { get; set; }

        /// <summary>
        ///  写真タイプ
        /// </summary>
        public int HM13009 { get; set; }

        /// <summary>
        ///  値の入力可能
        /// </summary>
        public int HM13010 { get; set; }

        /// <summary>
        /// 値の単位
        /// </summary>
        public string HM13011 { get; set; } = string.Empty;

        /// <summary>
        ///  説明
        /// </summary>
        public string HM13012 { get; set; } = string.Empty;

        /// <summary>
        ///  特記事項断面コード
        /// </summary>
        public string HM13013 { get; set; } = string.Empty;

        /// <summary>
        /// 非表示フラグ
        /// </summary>
        public int HM13014 { get; set; }

        /// <summary>
        /// 作成日時
        /// </summary>
        public DateTime HM13015 { get; set; }

        /// <summary>
        /// 作成オペレータ
        /// </summary>
        public string HM13016 { get; set; } = string.Empty;

        /// <summary>
        /// 更新日時
        /// </summary>
        public DateTime HM13017 { get; set; }

        /// <summary>
        ///  更新オペレータ
        /// </summary>
        public string HM13018 { get; set; } = string.Empty;

        /// <summary>
        ///  変更フラグ
        /// </summary>
        public string HMCHANGE { get; set; } = string.Empty;
    }
}
