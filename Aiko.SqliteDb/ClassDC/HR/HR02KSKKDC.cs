using DI.DiNetWinServiceObject;

namespace DI.DiNetWinServiceObject
{
    /// <summary>
    /// 検査結果テーブルリモートサービス
    /// </summary>
    public class HR02KSKKDC: CMObjectDC
    {

        /// <summary>
        /// 工事コード
        /// </summary>
        public string HR02001 { get; set; } = string.Empty;

        /// <summary>
        /// アイテムコード
        /// </summary>
        public string HR02002 { get; set; } = string.Empty;

        /// <summary>
        /// 確認項目コード
        /// </summary>
        public string HR02003 { get; set; } = string.Empty;

        /// <summary>
        /// 値
        /// </summary>
        public string HR02004 { get; set; } = string.Empty;

        /// <summary>
        /// 判定
        /// </summary>
        public int HR02005 { get; set; }

        /// <summary>
        /// 確認日
        /// </summary>
        public int HR02006 { get; set; }

        /// <summary>
        /// 確認オペレータ
        /// </summary>
        public string HR02007 { get; set; } = string.Empty;

        /// <summary>
        /// 指摘日
        /// </summary>
        public int HR02008 { get; set; }

        /// <summary>
        /// 指摘オペレータ
        /// </summary>
        public string HR02009 { get; set; } = string.Empty;

        /// <summary>
        /// メモ
        /// </summary>
        public string HR02010 { get; set; } = string.Empty;

        /// <summary>
        /// 備考
        /// </summary>
        public string HR02011 { get; set; } = string.Empty;

        /// <summary>
        /// 写真枚数
        /// </summary>
        public int HR02012 { get; set; }

        /// <summary>
        /// 作成日時
        /// </summary>
        public DateTime HR02013 { get; set; }

        /// <summary>
        /// 作成オペレータ
        /// </summary>
        public string HR02014 { get; set; } = string.Empty;

        /// <summary>
        /// 更新日時
        /// </summary>
        public DateTime HR02015 { get; set; }

        /// <summary>
        /// 更新オペレータ
        /// </summary>
        public string HR02016 { get; set; } = string.Empty;

        /// <summary>
        /// 同期日時
        /// </summary>
        public DateTime HR02017 { get; set; }

        /// <summary>
        /// 同期オペレータ
        /// </summary>
        public string HR02018 { get; set; } = string.Empty;
        //確認方法
        public int HR02019 { get; set; }

        /// <summary>
        ///  変更フラグ
        /// </summary>
        public string HMCHANGE { get; set; } = string.Empty;

    }
}
