using DI.DiNetWinServiceObject;

namespace DI.DiNetWinServiceObject
{
    /// <summary>
    /// マップ管理
    /// </summary>
    public class HM04MAPMDC : CMObjectDC
    {

        /// <summary>
        /// 工事コード
        /// </summary>
        public string HM04001 { get; set; } = string.Empty;

        /// <summary>
        /// マップコード
        /// </summary>
        public string HM04002 { get; set; } = string.Empty;

        /// <summary>
        /// マップ名
        /// </summary>
        public string HM04003 { get; set; } = string.Empty;

        /// <summary>
        /// マップグループコード
        /// </summary>
        public string HM04004 { get; set; } = string.Empty;

        /// <summary>
        /// ファイルコード
        /// </summary>
        public string HM04005 { get; set; } = string.Empty;

        /// <summary>
        /// 並び順
        /// </summary>
        public string HM04006 { get; set; } = string.Empty;

        /// <summary>
        /// 表示範囲X
        /// </summary>
        public int HM04007 { get; set; }

        /// <summary>
        /// 表示範囲Y
        /// </summary>
        public int HM04008 { get; set; }

        /// <summary>
        /// 表示範囲Width
        /// </summary>
        public int HM04009 { get; set; }

        /// <summary>
        /// 表示範囲Height
        /// </summary>
        public int HM04010 { get; set; }

        /// <summary>
        /// 制御点1X
        /// </summary>
        public int HM04011 { get; set; }

        /// <summary>
        /// 制御点1Y
        /// </summary>
        public int HM04012 { get; set; }

        /// <summary>
        /// 制御点2X
        /// </summary>
        public int HM04013 { get; set; }

        /// <summary>
        /// 制御点2Y
        /// </summary>
        public int HM04014 { get; set; }

        /// <summary>
        /// Xガイド起用
        /// </summary>
        public int HM04015 { get; set; }

        /// <summary>
        /// XガイドX
        /// </summary>
        public int HM04016 { get; set; }

        /// <summary>
        /// XガイドY
        /// </summary>
        public int HM04017 { get; set; }

        /// <summary>
        /// XガイドWidth
        /// </summary>
        public int HM04018 { get; set; }

        /// <summary>
        /// XガイドHeight
        /// </summary>
        public int HM04019 { get; set; }

        /// <summary>
        /// Yガイド起用
        /// </summary>
        public int HM04020 { get; set; }

        /// <summary>
        /// YガイドX
        /// </summary>
        public int HM04021 { get; set; }

        /// <summary>
        /// YガイドY
        /// </summary>
        public int HM04022 { get; set; }

        /// <summary>
        /// YガイドWidth
        /// </summary>
        public int HM04023 { get; set; }

        /// <summary>
        /// YガイドHeight
        /// </summary>
        public int HM04024 { get; set; }

        /// <summary>
        /// Z範囲From
        /// </summary>
        public int HM04025 { get; set; }

        /// <summary>
        /// Z範囲To
        /// </summary>
        public int HM04026 { get; set; }

        /// <summary>
        /// 表示倍率
        /// </summary>
        public int HM04027 { get; set; }

        /// <summary>
        /// XY表示
        /// </summary>
        public int HM04028 { get; set; }

        /// <summary>
        /// 作成日時
        /// </summary>
        public DateTime HM04029 { get; set; }

        /// <summary>
        /// 作成オペレータ
        /// </summary>
        public string HM04030 { get; set; } = string.Empty;

        /// <summary>
        /// 更新日時
        /// </summary>
        public DateTime HM04031 { get; set; }

        /// <summary>
        /// 更新オペレータ
        /// </summary>
        public string HM04032 { get; set; } = string.Empty;

        /// <summary>
        /// 制御点1論理座標X
        /// </summary>
        public int? HM04035 { get; set; }

        /// <summary>
        /// 制御点1論理座標Y
        /// </summary>
        public int? HM04036 { get; set; }

        /// <summary>
        /// 制御点2論理座標X
        /// </summary>
        public int? HM04037 { get; set; }

        /// <summary>
        /// 制御点2論理座標Y
        /// </summary>
        public int? HM04038 { get; set; }

        /// <summary>
        /// 階コード
        /// </summary>
        public string HM04039 { get; set; } = string.Empty;

        /// <summary>
        /// 落書きの表示範囲X
        /// </summary>
        public int? HM04040 { get; set; }

        /// <summary>
        /// 落書きの表示範囲Y
        /// </summary>
        public int? HM04041 { get; set; }

        /// <summary>
        /// 落書きのマップコード
        /// </summary>
        public string HM04042 { get; set; } = string.Empty;

        /// <summary>
        /// 変更フラグ
        /// </summary>
        public string HMCHANGE { get; set; } = string.Empty;
    }
}
