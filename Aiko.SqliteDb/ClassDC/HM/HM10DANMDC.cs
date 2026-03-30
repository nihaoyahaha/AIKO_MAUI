using DI.DiNetWinServiceObject;

namespace DI.DiNetWinServiceObject
{
    /// <summary>
    /// 断面マスターリモートサービス
    /// </summary>
    public class HM10DANMDC : CMObjectDC
    {
        // 工事コード
        private string _HM10001;
        // グループコード
        private string _HM10002;
        // 断面コード
        private string _HM10003;
        // キャプション
        private string _HM10004;
        // 並び順
        private string _HM10005;
        // ファイルコード
        private string _HM10006;
        // 表示範囲座標X
        private int _HM10007;
        // 表示範囲座標Y
        private int _HM10008;
        // 表示範囲Width
        private int _HM10009;
        // 表示範囲Height
        private int _HM10010;
        // 非表示フラグ
        private int _HM10011;
        // 説明
        private string _HM10012;
        // 作成日時
        private DateTime _HM10013;
        // 作成オペレータ
        private string _HM10014;
        // 更新日時
        private DateTime _HM10015;
        // 更新オペレータ
        private string _HM10016;
        // 同期日時
        private DateTime _HM10017;
        // 同期オペレータ
        private string _HM10018;

        //*****  1232 配筋検査システムーUWP開発配筋確認画面の断面図対応   zhangcheng  20171013  start
        //*****   １．配筋確認画面、タブの断面図合併対応。
        // 座標X
        private int _HM10019;
        // 座標Y
        private int _HM10020;
        // Width
        private int _HM10021;
        // Height
        private int _HM10022;
        //*****  1232 配筋検査システムーUWP開発配筋確認画面の断面図対応   zhangcheng  20171013  end
        // 変更フラグ
        private string _HMCHANGE;
        //***** 1281 配筋検査システムーUWP開発図形表示遅延　対応  zhangcheng  2017/11/10 start
        //***** 配筋検査画面、断面表示が遅いですので、改善しましょう
        //三合一断面図
        private string _HM10023;
        //***** 1281 配筋検査システムーUWP開発図形表示遅延　対応  zhangcheng  2017/11/10 end

        // ピング四角形の座標
        private int _HM10024;

        // ファイルコード
        private string _HM10025;

        /// <summary>
        /// 工事コード
        /// </summary>
        public string HM10001 {
            get { return _HM10001; }

            set { _HM10001 = value; }
        }

        /// <summary>
        ///  グループコード
        /// </summary>
        public string HM10002 {
            get { return _HM10002; }

            set { _HM10002 = value; }
        }

        /// <summary>
        ///  断面コード
        /// </summary>
        public string HM10003 {
            get { return _HM10003; }

            set { _HM10003 = value; }
        }

        /// <summary>
        ///  キャプション
        /// </summary>
        public string HM10004 {
            get { return _HM10004; }

            set { _HM10004 = value; }
        }

        /// <summary>
        ///  並び順
        /// </summary>
        public string HM10005 {
            get { return _HM10005; }

            set { _HM10005 = value; }
        }

        /// <summary>
        ///  ファイルコード
        /// </summary>
        public string HM10006 {
            get { return _HM10006; }

            set { _HM10006 = value; }
        }

        /// <summary>
        ///  表示範囲座標X
        /// </summary>
        public int HM10007 {
            get { return _HM10007; }

            set { _HM10007 = value; }
        }

        /// <summary>
        ///  表示範囲座標Y
        /// </summary>
        public int HM10008 {
            get { return _HM10008; }

            set { _HM10008 = value; }
        }

        /// <summary>
        ///  表示範囲Width
        /// </summary>
        public int HM10009 {
            get { return _HM10009; }

            set { _HM10009 = value; }
        }

        /// <summary>
        ///  表示範囲Height
        /// </summary>
        public int HM10010 {
            get { return _HM10010; }

            set { _HM10010 = value; }
        }

        /// <summary>
        ///  非表示フラグ
        /// </summary>
        public int HM10011 {
            get { return _HM10011; }

            set { _HM10011 = value; }
        }

        /// <summary>
        ///  説明
        /// </summary>
        public string HM10012 {
            get { return _HM10012; }

            set { _HM10012 = value; }
        }

        /// <summary>
        /// 作成日時
        /// </summary>
        public DateTime HM10013 {
            get { return _HM10013; }

            set { _HM10013 = value; }
        }

        /// <summary>
        /// 作成オペレータ
        /// </summary>
        public string HM10014 {
            get { return _HM10014; }

            set { _HM10014 = value; }
        }

        /// <summary>
        ///  更新日時
        /// </summary>
        public DateTime HM10015 {
            get { return _HM10015; }

            set { _HM10015 = value; }
        }

        /// <summary>
        ///  更新オペレータ
        /// </summary>
        public string HM10016 {
            get { return _HM10016; }

            set { _HM10016 = value; }
        }

        /// <summary>
        ///  同期日時
        /// </summary>
        public DateTime HM10017
        {
            get { return _HM10017; }

            set { _HM10017 = value; }
        }
        /// <summary>
        ///  同期オペレータ
        /// </summary>
        public string HM10018
        {
            get { return _HM10018; }

            set { _HM10018 = value; }
        }
        //*****  1232 配筋検査システムーUWP開発配筋確認画面の断面図対応   zhangcheng  20171013  start
        //*****   １．配筋確認画面、タブの断面図合併対応。
        /// <summary>
        /// 座標X
        /// </summary>
        public int HM10019
        {
            get { return _HM10019; }

            set { _HM10019 = value; }
        }

        /// <summary>
        ///  座標Y
        /// </summary>
        public int HM10020
        {
            get { return _HM10020; }

            set { _HM10020 = value; }
        }

        /// <summary>
        ///  Width
        /// </summary>
        public int HM10021
        {
            get { return _HM10021; }

            set { _HM10021 = value; }
        }

        /// <summary>
        ///  Height
        /// </summary>
        public int HM10022
        {
            get { return _HM10022; }

            set { _HM10022 = value; }
        }
        //*****  1232 配筋検査システムーUWP開発配筋確認画面の断面図対応   zhangcheng  20171013  end
        /// <summary>
        ///  変更フラグ
        /// </summary>
        public string HMCHANGE {
            get { return _HMCHANGE; }

            set { _HMCHANGE = value; }
        }

        //***** 1281 配筋検査システムーUWP開発図形表示遅延　対応  zhangcheng  2017/11/10 start
        //***** 配筋検査画面、断面表示が遅いですので、改善しましょう
        //三合一断面図
        public string HM10023
        {
            get { return _HM10023; }

            set { _HM10023 = value; }
        }
        //***** 1281 配筋検査システムーUWP開発図形表示遅延　対応  zhangcheng  2017/11/10 end


        public int HM10024
        {
            get { return _HM10024; }

            set { _HM10024 = value; }
        }

        public string HM10025
        {
            get { return _HM10025; }

            set { _HM10025 = value; }
        }
    }
}

