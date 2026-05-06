using DI.DiNetWinServiceObject;

namespace DI.DiNetWinServiceObject
{
    /// <summary>
    /// 工区マスターリモートサービス
    /// </summary>
    public class HM07KOKUDC : CMObjectDC
    {

        /// <summary>
        /// 工事コード
        /// </summary>
        public string HM07001 { get; set; } = string.Empty;

        /// <summary>
        /// 工区コード
        /// </summary>
        public string HM07002 { get; set; } = string.Empty;

        /// <summary>
        /// 工区名
        /// </summary>
        public string HM07003 { get; set; } = string.Empty;

        /// <summary>
        /// マップコード
        /// </summary>
        public string HM07004 { get; set; } = string.Empty;

        /// <summary>
        /// 並び順
        /// </summary>
        public string HM07005 { get; set; } = string.Empty;

        /// <summary>
        /// 打設予定日
        /// </summary>
        public int HM07006 { get; set; }

        /// <summary>
        /// 非表示フラグ
        /// </summary>
        public int HM07007 { get; set; }

        //作成日時
        public DateTime HM07008 { get; set; }

        /// <summary>
        /// 作成オペレータ
        /// </summary>
        public string HM07009 { get; set; } = string.Empty;

        /// <summary>
        /// 更新日時
        /// </summary>
        public DateTime HM07010 { get; set; }

        /// <summary>
        /// 更新オペレータ
        /// </summary>
        public string HM07011 { get; set; } = string.Empty;

        /// <summary>
        /// 同期日時
        /// </summary>
        public DateTime HM07012 { get; set; }

        /// <summary>
        /// 同期オペレータ
        /// </summary>
        public string HM07013 { get; set; } = string.Empty;

        /// <summary>
        /// マップに表示位置X
        /// </summary>
        public int HM07014 { get; set; }

        /// <summary>
        /// マップに表示位置Y
        /// </summary>
        public int HM07015 { get; set; }

        /// <summary>
        /// 変更フラグ
        /// </summary>
        public string HMCHANGE { get; set; } = string.Empty;
    }
}
