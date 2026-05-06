using DI.DiNetWinServiceObject;

namespace DI.DiNetWinServiceObject
{
    public class HM23BUNRUIDC : CMObjectDC
    {
        /// <summary>
        /// 工事コード
        /// </summary>
        public string HM23001 { get; set; } = string.Empty;
        /// <summary>
        ///分類コード 
        /// </summary>
        public string HM23002 { get; set; } = string.Empty;
        /// <summary>
        /// 分類名
        /// </summary>
        public string HM23003 { get; set; } = string.Empty;
        /// <summary>
        /// 並び順
        /// </summary>
        public string HM23004 { get; set; } = string.Empty;
        /// <summary>
        /// 作成日時
        /// </summary>
        public DateTime HM23005 { get; set; }
        /// <summary>
        /// 作成オペレータ
        /// </summary>
        public string HM23006 { get; set; } = string.Empty;
        /// <summary>
        /// 更新日時
        /// </summary>
        public DateTime HM23007 { get; set; }
        /// <summary>
        /// 更新オペレータ
        /// </summary>
        public string HM23008 { get; set; } = string.Empty;
        /// <summary>
        /// 同期日時
        /// </summary>
        public DateTime HM23009 { get; set; }
        /// <summary>
        /// 同期オペレータ
        /// </summary>
        public string HM23010 { get; set; } = string.Empty;
    }
}
