using DI.DiNetWinServiceObject;

namespace DI.DiNetWinServiceObject
{
    public class HM19GUIDCOLORDC : CMObjectDC
    {

        /// <summary>
        /// 工事コード
        /// </summary>
        public string HM19001 { get; set; } = string.Empty;

        /// <summary>
        /// マップコード
        /// </summary>
        public string HM19002 { get; set; } = string.Empty;

        /// <summary>
        /// ガイドタイプ
        /// </summary>
        public decimal HM19003 { get; set; }

        /// <summary>
        /// ガイド番号
        /// </summary>
        public decimal HM19004 { get; set; }

        /// <summary>
        /// 背景色
        /// </summary>
        public int HM19005 { get; set; }

        /// <summary>
        /// 文字色
        /// </summary>
        public int HM19006 { get; set; }

        /// <summary>
        /// フォント
        /// </summary>
        public string HM19007 { get; set; } = string.Empty;

        /// <summary>
        /// フォントサイズ
        /// </summary>
        public decimal HM19008 { get; set; }

        /// <summary>
        /// 作成日時
        /// </summary>
        public DateTime HM19009 { get; set; }

        /// <summary>
        /// 作成オペレータ
        /// </summary>
        public string HM19010 { get; set; } = string.Empty;

        /// <summary>
        /// 更新日時
        /// </summary>
        public DateTime HM19011 { get; set; }

        /// <summary>
        /// 更新オペレータ
        /// </summary>
        public string HM19012 { get; set; } = string.Empty;

        /// <summary>
        /// 同期日時
        /// </summary>
        public DateTime HM19013 { get; set; }

        /// <summary>
        /// 同期オペレータ
        /// </summary>
        public string HM19014 { get; set; } = string.Empty;
    }
}
