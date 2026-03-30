

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
        // 工事コード
        private string _HR01001;
        // マップコード
        private string _HR01002;
        // アイテムコード
        private string _HR01003;
        // アイテムタイプ
        private int _HR01004;
        // 配筋表断面コード
        private string _HR01005;
        // 位置
        private string _HR01006;
        // 工区コード
        private string _HR01007;
        // マップに表示位置X
        private int _HR01008;
        // マップに表示位置Y
        private int _HR01009;
        // マップに表示Width
        private int _HR01010;
        // マップに表示Height
        private int _HR01011;
        // 非表示フラグ
        private int _HR01012;
        // 作成日時
        private DateTime _HR01013;
        // 作成オペレータ
        private string _HR01014;
        // 更新日時
        private DateTime _HR01015;
        // 更新オペレータ
        private string _HR01016;
        // 同期日時
        private DateTime _HR01017;
        // 同期オペレータ
        private string _HR01018;
        // 部位コード
        private string _HR01019;
        // 断面コード
        private string _HR01020;
        // 表示色
        private int _HR01021;

        //***** 1361 配筋検査システムーPC版断面リスト特記対応  modified by liuchunming  20180222 start
        //***** １．配筋確認画面　「確定」ボタンを押下するとき、断面特記をアイテムマスタに保存する。
        //***** ２．アイテムコピー機能も対応する。
        //断面特記
        private string _HR01022;
        //***** 1361 配筋検査システムーPC版断面リスト特記対応  modified by liuchunming  20180222 end

        // チェンジフラグ
        private string _HMCHANGE;

        /// <summary>
        /// 工事コード
        /// </summary>
        public string HR01001 { get { return _HR01001; } set { _HR01001 = value; } }
        /// <summary>
        /// マップコード
        /// </summary>
        public string HR01002 { get { return _HR01002; } set { _HR01002 = value; } }
        /// <summary>
        /// アイテムコード
        /// </summary>
        public string HR01003 { get { return _HR01003; } set { _HR01003 = value; } }
        /// <summary>
        /// アイテムタイプ
        /// </summary>
        public int HR01004 { get { return _HR01004; } set { _HR01004 = value; } }
        /// <summary>
        /// 配筋表断面コード
        /// </summary>
        public string HR01005 { get { return _HR01005; } set { _HR01005 = value; } }
        /// <summary>
        /// 位置
        /// </summary>
        public string HR01006 { get { return _HR01006; } set { _HR01006 = value; } }
        /// <summary>
        /// 工区コード
        /// </summary>
        public string HR01007 { get { return _HR01007; } set { _HR01007 = value; } }
        /// <summary>
        /// マップに表示位置X
        /// </summary>
        public int HR01008 { get { return _HR01008; } set { _HR01008 = value; } }
        /// <summary>
        /// マップに表示位置Y
        /// </summary>
        public int HR01009 { get { return _HR01009; } set { _HR01009 = value; } }
        /// <summary>
        /// マップに表示Width
        /// </summary>
        public int HR01010 { get { return _HR01010; } set { _HR01010 = value; } }
        /// <summary>
        /// マップに表示Height
        /// </summary>
        public int HR01011 { get { return _HR01011; } set { _HR01011 = value; } }
        /// <summary>
        /// 非表示フラグ
        /// </summary>
        public int HR01012 { get { return _HR01012; } set { _HR01012 = value; } }
        /// <summary>
        /// 作成日時
        /// </summary>
        public DateTime HR01013 { get { return _HR01013; } set { _HR01013 = value; } }
        /// <summary>
        /// 作成オペレータ
        /// </summary>
        public string HR01014 { get { return _HR01014; } set { _HR01014 = value; } }
        /// <summary>
        /// 更新日時
        /// </summary>
        public DateTime HR01015 { get { return _HR01015; } set { _HR01015 = value; } }
        /// <summary>
        /// 更新オペレータ
        /// </summary>
        public string HR01016 { get { return _HR01016; } set { _HR01016 = value; } }
        /// <summary>
        /// 同期日時
        /// </summary>
        public DateTime HR01017 { get { return _HR01017; } set { _HR01017 = value; } }
        /// <summary>
        /// 同期オペレータ
        /// </summary>
        public string HR01018 { get { return _HR01018; } set { _HR01018 = value; } }
        /// <summary>
        /// 部位コード
        /// </summary>
        public string HR01019 { get { return _HR01019; } set { _HR01019 = value; } }
        /// <summary>
        /// 断面コード
        /// </summary>
        public string HR01020 { get { return _HR01020; } set { _HR01020 = value; } }
        /// <summary>
        /// 表示色
        /// </summary>
        public int HR01021 { get { return _HR01021; } set { _HR01021 = value; } }

        //***** 1361 配筋検査システムーPC版断面リスト特記対応  modified by liuchunming  20180222 start
        //***** １．配筋確認画面　「確定」ボタンを押下するとき、断面特記をアイテムマスタに保存する。
        //***** ２．アイテムコピー機能も対応する。
        /// <summary>
        /// 断面特記
        /// </summary>
        public string HR01022 { get { return _HR01022; } set { _HR01022 = value; } }
        //***** 1361 配筋検査システムーPC版断面リスト特記対応  modified by liuchunming  20180222 end

        /// <summary>
        /// チェンジフラグ
        /// </summary>
        public string HMCHANGE { get { return _HMCHANGE; } set { _HMCHANGE = value; } }

       
    }
}
