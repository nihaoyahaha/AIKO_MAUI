using DI.DiNetWinServiceObject;

namespace DI.DiNetWinServiceObject
{
    /// <summary>
    /// 工区多辺形情報
    /// </summary>
    public class HR05KOKUMINFODC : CMObjectDC
    {

        /// <summary>
        /// 工事コード
        /// </summary>
        public string HR05001 { get; set; } = string.Empty;
        /// <summary>
        /// アイテムコード
        /// </summary>
        public string HR05002 { get; set; } = string.Empty;
        /// <summary>
        /// 頂点No
        /// </summary>
        public int HR05003 { get; set; }
        /// <summary>
        /// X座標
        /// </summary>
        public int HR05004 { get; set; }
        /// <summary>
        /// Y座標
        /// </summary>
        public int HR05005 { get; set; }
        /// <summary>
        /// 作成日時
        /// </summary>
        public DateTime HR05006 { get; set; }
        /// <summary>
        /// 作成オペレータ
        /// </summary>
        public string HR05007 { get; set; } = string.Empty;
        /// <summary>
        /// 更新日時
        /// </summary>
        public DateTime HR05008 { get; set; }
        /// <summary>
        /// 更新オペレータ
        /// </summary>
        public string HR05009 { get; set; } = string.Empty;
        
    }
}
