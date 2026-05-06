using DI.DiNetWinServiceObject;

namespace DI.DiNetWinServiceObject
{
    /// <summary>
    /// マップガイドマスターリモートサービス
    /// </summary>
    public class HM14GUIDDC : CMObjectDC
    {

        /// <summary>
        /// 工事コード
        /// </summary>
        public string HM14001 { get; set; } = string.Empty;

        /// <summary>
        /// マップコード
        /// </summary>
        public string HM14002 { get; set; } = string.Empty;

        /// <summary>
        /// ガイドコード
        /// </summary>
        public string HM14003 { get; set; } = string.Empty;

        /// <summary>
        /// ガイドタイプ
        /// </summary>
        public int HM14004 { get; set; }

        /// <summary>
        /// 並び順
        /// </summary>
        public string HM14005 { get; set; } = string.Empty;

        /// <summary>
        /// 座標種別
        /// </summary>
        public string HM14006 { get; set; } = string.Empty;

        /// <summary>
        /// 間隔
        /// </summary>
        public int HM14007 { get; set; }

        /// <summary>
        /// 論理座標
        /// </summary>
        public int HM14008 { get; set; }

        /// <summary>
        /// 作成日時
        /// </summary>
        public DateTime HM14009 { get; set; }

        /// <summary>
        /// 作成オペレータ
        /// </summary>
        public string HM14010 { get; set; } = string.Empty;

        /// <summary>
        /// 更新日時
        /// </summary>
        public DateTime HM14011 { get; set; }

        /// <summary>
        /// 更新オペレータ
        /// </summary> 
        public string HM14012 { get; set; } = string.Empty;

        /// <summary>
        /// 同期日時
        /// </summary>
        public DateTime HM14013 { get; set; }

        /// <summary>
        /// 同期オペレータ
        /// </summary> 
        public string HM14014 { get; set; } = string.Empty;

        /// <summary>
        /// ガイド番号
        /// </summary>
        public int HM14015 { get; set; }

        /// <summary>
        /// 変更フラグ
        /// </summary> 
        public string HMCHANGE { get; set; } = string.Empty;
    }
}
