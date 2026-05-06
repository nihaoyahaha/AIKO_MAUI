using DI.DiNetWinServiceObject;

namespace DI.DiNetWinServiceObject
{
    public class HM20GUIDHEADDC : CMObjectDC
    {

        /// <summary>
        /// 工事コード
        /// </summary>
        public string HM20001 { get; set; } = string.Empty;

        /// <summary>
        ///  マップコード
        /// </summary>
        public string HM20002 { get; set; } = string.Empty;

        /// <summary>
        /// ガイド番号
        /// </summary>
        public decimal HM20003 { get; set; }

        /// <summary>
        /// ガイド名
        /// </summary>
        public string HM20004 { get; set; } = string.Empty;

        /// <summary>
        /// 角度
        /// </summary>
        public decimal HM20005 { get; set; }

        /// <summary>
        /// 制御点1X
        /// </summary>
        public decimal HM20006 { get; set; }

        /// <summary>
        /// 制御点1Y
        /// </summary>
        public decimal HM20007 { get; set; }

        /// <summary>
        /// 制御点2X
        /// </summary>
        public decimal HM20008 { get; set; }

        /// <summary>
        /// 制御点2Y
        /// </summary>
        public decimal HM20009 { get; set; }

        /// <summary>
        /// 制御点1論理座標X
        /// </summary>
        public decimal HM20010 { get; set; }

        /// <summary>
        /// 制御点1論理座標Y
        /// </summary>
        public decimal HM20011 { get; set; }

        /// <summary>
        /// 制御点2論理座標X
        /// </summary>
        public decimal HM20012 { get; set; }

        /// <summary>
        /// 制御点2論理座標Y
        /// </summary>
        public decimal HM20013 { get; set; }

        /// <summary>
        /// 作成日時
        /// </summary>
        public DateTime HM20014 { get; set; }

        /// <summary>
        /// 作成オペレータ
        /// </summary>
        public string HM20015 { get; set; } = string.Empty;

        /// <summary>
        /// 更新日時
        /// </summary>
        public DateTime HM20016 { get; set; }

        /// <summary>
        /// 更新オペレータ
        /// </summary>
        public string HM20017 { get; set; } = string.Empty;

        /// <summary>
        /// 同期日時
        /// </summary>
        public DateTime HM20018 { get; set; }

        /// <summary>
        /// 同期オペレータ
        /// </summary>
        public string HM20019 { get; set; } = string.Empty;
    }
}
