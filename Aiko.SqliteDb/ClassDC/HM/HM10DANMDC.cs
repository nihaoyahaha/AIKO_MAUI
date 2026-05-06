using DI.DiNetWinServiceObject;

namespace DI.DiNetWinServiceObject
{
    /// <summary>
    /// 断面マスターリモートサービス
    /// </summary>
    public class HM10DANMDC : CMObjectDC
    {

        /// <summary>
        /// 工事コード
        /// </summary>
        public string HM10001 { get; set; } = string.Empty;

        /// <summary>
        ///  グループコード
        /// </summary>
        public string HM10002 { get; set; } = string.Empty;

        /// <summary>
        ///  断面コード
        /// </summary>
        public string HM10003 { get; set; } = string.Empty;

        /// <summary>
        ///  キャプション
        /// </summary>
        public string HM10004 { get; set; } = string.Empty;

        /// <summary>
        ///  並び順
        /// </summary>
        public string HM10005 { get; set; } = string.Empty;

        /// <summary>
        ///  ファイルコード
        /// </summary>
        public string HM10006 { get; set; } = string.Empty;

        /// <summary>
        ///  表示範囲座標X
        /// </summary>
        public int HM10007 { get; set; }

        /// <summary>
        ///  表示範囲座標Y
        /// </summary>
        public int HM10008 { get; set; }

        /// <summary>
        ///  表示範囲Width
        /// </summary>
        public int HM10009 { get; set; }

        /// <summary>
        ///  表示範囲Height
        /// </summary>
        public int HM10010 { get; set; }

        /// <summary>
        ///  非表示フラグ
        /// </summary>
        public int HM10011 { get; set; }

        /// <summary>
        ///  説明
        /// </summary>
        public string HM10012 { get; set; } = string.Empty;

        /// <summary>
        /// 作成日時
        /// </summary>
        public DateTime HM10013 { get; set; }

        /// <summary>
        /// 作成オペレータ
        /// </summary>
        public string HM10014 { get; set; } = string.Empty;

        /// <summary>
        ///  更新日時
        /// </summary>
        public DateTime HM10015 { get; set; }

        /// <summary>
        ///  更新オペレータ
        /// </summary>
        public string HM10016 { get; set; } = string.Empty;

        /// <summary>
        ///  同期日時
        /// </summary>
        public DateTime HM10017 { get; set; }
        /// <summary>
        ///  同期オペレータ
        /// </summary>
        public string HM10018 { get; set; } = string.Empty;
        /// <summary>
        /// 座標X
        /// </summary>
        public int HM10019 { get; set; }

        /// <summary>
        ///  座標Y
        /// </summary>
        public int HM10020 { get; set; }

        /// <summary>
        ///  Width
        /// </summary>
        public int HM10021 { get; set; }

        /// <summary>
        ///  Height
        /// </summary>
        public int HM10022 { get; set; }
        /// <summary>
        ///  変更フラグ
        /// </summary>
        public string HMCHANGE { get; set; } = string.Empty;

        /// <summary>
        /// 三合一断面図
        /// </summary>
        public string HM10023 { get; set; } = string.Empty;

        /// <summary>
        /// 左領域下の絶対Y座標
        /// </summary>
        public int HM10024 { get; set; }

        /// <summary>
        /// 断面ファイルコード
        /// </summary>
        public string HM10025 { get; set; } = string.Empty;
    }
}
