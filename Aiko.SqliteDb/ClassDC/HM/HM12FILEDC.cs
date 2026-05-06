using DI.DiNetWinServiceObject;

namespace DI.DiNetWinServiceObject
{
    /// <summary>
    /// ファイルマスターリモートサービス
    /// </summary>
    public class HM12FILEDC : CMObjectDC
    {

        /// <summary>
        /// 工事コード
        /// </summary>
        public string HM12001 { get; set; } = string.Empty;

        /// <summary>
        /// ファイルコード
        /// </summary>
        public string HM12002 { get; set; } = string.Empty;

        /// <summary>
        /// ファイル名
        /// </summary>
        public string HM12003 { get; set; } = string.Empty;

        /// <summary>
        /// ファイルパス
        /// </summary>
        public string HM12004 { get; set; } = string.Empty;

        /// <summary>
        /// 作成日時
        /// </summary>
        public DateTime HM12005 { get; set; }

        /// <summary>
        /// 作成オペレータ
        /// </summary>
        public string HM12006 { get; set; } = string.Empty;

        /// <summary>
        /// 更新日時
        /// </summary>
        public DateTime HM12007 { get; set; }

        /// <summary>
        /// 更新オペレータ
        /// </summary>
        public string HM12008 { get; set; } = string.Empty;

        /// <summary>
        /// 同期日時
        /// </summary>
        public DateTime HM12009 { get; set; }

        /// <summary>
        /// 同期オペレータ
        /// </summary>
        public string HM12010 { get; set; } = string.Empty;

        /// <summary>
        /// 同期分?名称
        /// </summary>
        public string HM12011 { get; set; } = string.Empty;

        /// <summary>
        /// 幅
        /// </summary>
        public int HM12012 { get; set; }

        /// <summary>
        /// 高さ
        /// </summary>
        public int HM12013 { get; set; }

        /// <summary>
        /// 解像度
        /// </summary>
        public int HM12014 { get; set; }

        /// <summary>
        /// 分類コード
        /// </summary>
        public string HM12015 { get; set; } = string.Empty;
    }
}
