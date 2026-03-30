using DI.DiNetWinServiceObject;

namespace DI.DiNetWinServiceObject
{
    /// <summary>
    /// ファイルマスターリモートサービス
    /// </summary>
    public class HM12FILEDC : CMObjectDC
    {
        // 工事コード
        private string _HM12001;
        // ファイルコード
        private string _HM12002;
        // ファイル名
        private string _HM12003;
        // ファイルパス
        private string _HM12004;
        // 作成日時
        private DateTime _HM12005;
        // 作成オペレータ
        private string _HM12006;
        // 更新日時
        private DateTime _HM12007;
        // 更新オペレータ
        private string _HM12008;
        // 同期日時
        private DateTime _HM12009;
        // 同期オペレータ
        private string _HM12010;

        private string _HM12011;


        // 幅
        private int _HM12012;
        // 高さ
        private int _HM12013;
        // 解像度
        private int _HM12014;
        // 分類コード
        private string _HM12015;


        /// <summary>
        /// 工事コード
        /// </summary>
        public string HM12001
        {
            get { return _HM12001; }
            set { _HM12001 = value; }
        }

        /// <summary>
        /// ファイルコード
        /// </summary>
        public string HM12002
        {
            get { return _HM12002; }
            set { _HM12002 = value; }
        }

        /// <summary>
        /// ファイル名
        /// </summary>
        public string HM12003
        {
            get { return _HM12003; }
            set { _HM12003 = value; }
        }

        /// <summary>
        /// ファイルパス
        /// </summary>
        public string HM12004
        {
            get { return _HM12004; }
            set { _HM12004 = value; }
        }

        /// <summary>
        /// 作成日時
        /// </summary>
        public DateTime HM12005
        {
            get { return _HM12005; }
            set { _HM12005 = value; }
        }

        /// <summary>
        /// 作成オペレータ
        /// </summary>
        public string HM12006
        {
            get { return _HM12006; }
            set { _HM12006 = value; }
        }

        /// <summary>
        /// 更新日時
        /// </summary>
        public DateTime HM12007
        {
            get { return _HM12007; }
            set { _HM12007 = value; }
        }

        /// <summary>
        /// 更新オペレータ
        /// </summary>
        public string HM12008
        {
            get { return _HM12008; }
            set { _HM12008 = value; }
        }

        /// <summary>
        /// 同期日時
        /// </summary>
        public DateTime HM12009
        {
            get { return _HM12009; }
            set { _HM12009 = value; }
        }

        /// <summary>
        /// 同期オペレータ
        /// </summary>
        public string HM12010
        {
            get { return _HM12010; }
            set { _HM12010 = value; }
        }

        //**** 1309  sunjiawei 20171225 start
        /// <summary>
        /// 同期分类名称
        /// </summary>
        public string HM12011
        {
            get { return _HM12011; }
            set { _HM12011 = value; }
        }
        //**** 1309  sunjiawei 20171225 END

        /// <summary>
        /// 幅
        /// </summary>
        public int HM12012
        {
            get { return _HM12012; }
            set { _HM12012 = value; }
        }

        /// <summary>
        /// 高さ
        /// </summary>
        public int HM12013
        {
            get { return _HM12013; }
            set { _HM12013 = value; }
        }

        /// <summary>
        /// 解像度
        /// </summary>
        public int HM12014
        {
            get { return _HM12014; }
            set { _HM12014 = value; }
        }

        /// <summary>
        /// 分類コード
        /// </summary>
        public string HM12015
        {
            get { return _HM12015; }
            set { _HM12015 = value; }
        }
    }
}

