
using DI.DiNetWinServiceObject;
using System;
using System.Text;

namespace DI.DiNetWinServiceObject
{
    /// <summary>
    /// アイテムテーブルリモートサービス
    /// </summary>
    public class HR01ITEMDC : CMObjectDC
    {

        /// <summary>
        /// 工事コード
        /// </summary>
        public string HR01001 { get; set; } = string.Empty;
        /// <summary>
        /// マップコード
        /// </summary>
        public string HR01002 { get; set; } = string.Empty;
        /// <summary>
        /// アイテムコード
        /// </summary>
        public string HR01003 { get; set; } = string.Empty;
        /// <summary>
        /// アイテムタイプ
        /// </summary>
        public int HR01004 { get; set; }
        /// <summary>
        /// 配筋表断面コード
        /// </summary>
        public string HR01005 { get; set; } = string.Empty;
        /// <summary>
        /// 位置
        /// </summary>
        public string HR01006 { get; set; } = string.Empty;
        /// <summary>
        /// 工区コード
        /// </summary>
        public string HR01007 { get; set; } = string.Empty;
        /// <summary>
        /// マップに表示位置X
        /// </summary>
        public int HR01008 { get; set; }
        /// <summary>
        /// マップに表示位置Y
        /// </summary>
        public int HR01009 { get; set; }
        /// <summary>
        /// マップに表示Width
        /// </summary>
        public int HR01010 { get; set; }
        /// <summary>
        /// マップに表示Height
        /// </summary>
        public int HR01011 { get; set; }
        /// <summary>
        /// 非表示フラグ
        /// </summary>
        public int HR01012 { get; set; }
        /// <summary>
        /// 作成日時
        /// </summary>
        public DateTime HR01013 { get; set; }
        /// <summary>
        /// 作成オペレータ
        /// </summary>
        public string HR01014 { get; set; } = string.Empty;
        /// <summary>
        /// 更新日時
        /// </summary>
        public DateTime HR01015 { get; set; }
        /// <summary>
        /// 更新オペレータ
        /// </summary>
        public string HR01016 { get; set; } = string.Empty;
        /// <summary>
        /// 同期日時
        /// </summary>
        public DateTime HR01017 { get; set; }
        /// <summary>
        /// 同期オペレータ
        /// </summary>
        public string HR01018 { get; set; } = string.Empty;
        /// <summary>
        /// 部位コード
        /// </summary>
        public string HR01019 { get; set; } = string.Empty;
        /// <summary>
        /// 断面コード
        /// </summary>
        public string HR01020 { get; set; } = string.Empty;
        /// <summary>
        /// 表示色
        /// </summary>
        public int HR01021 { get; set; }

        /// <summary>
        /// 断面特記
        /// </summary>
        public string HR01022 { get; set; } = string.Empty;

        /// <summary>
        /// チェンジフラグ
        /// </summary>
        public string HMCHANGE { get; set; } = string.Empty;

    }
}
