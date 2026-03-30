using DI.DiNetWinServiceObject;

namespace DI.DiNetWinServiceObject
{
    /// <summary>
    /// マップガイドマスターリモートサービス
    /// </summary>
    public class HM14GUIDDC : CMObjectDC
    {
        // 工事コード
        private string _HM14001;
        // マップコード
        private string _HM14002;
        // ガイドコード
        private string _HM14003;
        // ガイドタイプ
        private int _HM14004;
        // 並び順
        private string _HM14005;
        // 座標種別
        private string _HM14006;
        // 間隔
        private int _HM14007;
        // 論理座標
        private int _HM14008;
        // 作成日時
        private DateTime _HM14009;
        // 作成オペレータ
        private string _HM14010;
        // 更新日時
        private DateTime _HM14011;
        // 更新オペレータ
        private string _HM14012;
        // 同期日時
        private DateTime _HM14013;
        // 同期オペレータ
        private string _HM14014;

        //ガイド番号
        private int _HM14015;
        ////角度
        //private decimal _HM14016;
        ////制御点1X
        //private int _HM14017;
        //// 制御点1Y
        //private int _HM14018;
        //// 制御点2X
        //private int _HM14019;
        //// 制御点2Y
        //private int _HM14020;

        ////制御点1論理座標X
        //private int _HM14021;
        ////制御点1論理座標Y
        //private int _HM14022;
        ////制御点2論理座標X
        //private int _HM14023;
        ////制御点2論理座標Y
        //private int _HM14024;

        // 変更フラグ
        private string _HMCHANGE;

        /// <summary>
        /// 工事コード
        /// </summary>
        public string HM14001
        {
            get { return _HM14001; }

            set { _HM14001 = value; }
        }

        /// <summary>
        /// マップコード
        /// </summary>
        public string HM14002
        {
            get { return _HM14002; }

            set { _HM14002 = value; }
        }

        /// <summary>
        /// ガイドコード
        /// </summary>
        public string HM14003
        {
            get { return _HM14003; }

            set { _HM14003 = value; }
        }

        /// <summary>
        /// ガイドタイプ
        /// </summary>
        public int HM14004
        {
            get { return _HM14004; }

            set { _HM14004 = value; }
        }

        /// <summary>
        /// 並び順
        /// </summary>
        public string HM14005
        {
            get { return _HM14005; }

            set { _HM14005 = value; }
        }

        /// <summary>
        /// 座標種別
        /// </summary>
        public string HM14006
        {
            get { return _HM14006; }

            set { _HM14006 = value; }
        }

        /// <summary>
        /// 間隔
        /// </summary>
        public int HM14007
        {
            get { return _HM14007; }

            set { _HM14007 = value; }
        }

        /// <summary>
        /// 論理座標
        /// </summary>
        public int HM14008
        {
            get { return _HM14008; }

            set { _HM14008 = value; }
        }

        /// <summary>
        /// 作成日時
        /// </summary>
        public DateTime HM14009
        {
            get { return _HM14009; }

            set { _HM14009 = value; }
        }

        /// <summary>
        /// 作成オペレータ
        /// </summary>
        public string HM14010
        {
            get { return _HM14010; }

            set { _HM14010 = value; }
        }

        /// <summary>
        /// 更新日時
        /// </summary>
        public DateTime HM14011
        {
            get { return _HM14011; }

            set { _HM14011 = value; }
        }

        /// <summary>
        /// 更新オペレータ
        /// </summary> 
        public string HM14012
        {
            get { return _HM14012; }

            set { _HM14012 = value; }
        }

        /// <summary>
        /// 同期日時
        /// </summary>
        public DateTime HM14013
        {
            get { return _HM14013; }

            set { _HM14013 = value; }
        }

        /// <summary>
        /// 同期オペレータ
        /// </summary> 
        public string HM14014
        {
            get { return _HM14014; }

            set { _HM14014 = value; }
        }
        //ガイド番号
        public int HM14015
        {
            get { return _HM14015; }

            set { _HM14015 = value; }
        }
        ////角度
        //public decimal HM14016
        //{
        //    get { return _HM14016; }

        //    set { _HM14016 = value; }
        //}
        ////制御点1X
        //public int HM14017
        //{
        //    get { return _HM14017; }

        //    set { _HM14017 = value; }
        //}
        ////制御点1Y
        //public int HM14018
        //{
        //    get { return _HM14018; }

        //    set { _HM14018 = value; }
        //}
        ////制御点2X
        //public int HM14019
        //{
        //    get { return _HM14019; }

        //    set { _HM14019 = value; }
        //}
        ////制御点2Y
        //public int HM14020
        //{
        //    get { return _HM14020; }

        //    set { _HM14020 = value; }
        //}
        ////制御点1論理座標X
        //public int HM14021
        //{
        //    get { return _HM14021; }

        //    set { _HM14021 = value; }
        //}
        ////制御点1論理座標Y
        //public int HM14022
        //{
        //    get { return _HM14022; }

        //    set { _HM14022 = value; }
        //}
        ////制御点2論理座標X

        //public int HM14023
        //{
        //    get { return _HM14023; }

        //    set { _HM14023 = value; }
        //}
        ////制御点2論理座標Y
        //public int HM14024
        //{
        //    get { return _HM14024; }

        //    set { _HM14024 = value; }
        //}


        /// <summary>
        /// 変更フラグ
        /// </summary> 
        public string HMCHANGE {
            get { return _HMCHANGE; }

            set { _HMCHANGE = value; }
        }
    }
}

