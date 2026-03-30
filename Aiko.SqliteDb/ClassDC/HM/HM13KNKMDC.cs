using DI.DiNetWinServiceObject;

namespace DI.DiNetWinServiceObject
{
    /// <summary>
    /// 確認項目リストリモートサービス
    /// </summary>
    public class HM13KNKMDC : CMObjectDC
    {
        // 工事コード
        private string _HM13001;
        // 部位コード
        private string _HM13002;
        // 工程コード
        private string _HM13003;
        // 確認項目コード
        private string _HM13004;
        // 確認項目名
        private string _HM13005;
        // 並び順
        private string _HM13006;
        // 参考項目(検査不要)
        private int _HM13007;
        // 個別(全体検査)
        private int _HM13008;
        // 写真タイプ
        private int _HM13009;
        // 値の入力可能
        private int _HM13010;
        // 値の単位
        private string _HM13011;
        // 説明
        private string _HM13012;
        // 特記事項断面コード
        private string _HM13013;
        // 非表示フラグ
        private int _HM13014;
        // 作成日時
        private DateTime _HM13015;
        // 作成オペレータ
        private string _HM13016;
        // 更新日時
        private DateTime _HM13017;
        //更新オペレータ
        private string _HM13018;
        // 変更フラグ
        private string _HMCHANGE;

        /// <summary>
        /// 工事コード
        /// </summary>
        public string HM13001 {
            get { return _HM13001; }

            set { _HM13001 = value; }
        }

        /// <summary>
        ///  部位コード
        /// </summary>
        public string HM13002 {
            get { return _HM13002; }

            set { _HM13002 = value; }
        }

        /// <summary>
        /// 工程コード
        /// </summary>
        public string HM13003 {
            get { return _HM13003; }

            set { _HM13003 = value; }
        }

        /// <summary>
        ///  確認項目コード
        /// </summary>
        public string HM13004 {
            get { return _HM13004; }

            set { _HM13004 = value; }
        }

        /// <summary>
        ///  確認項目名
        /// </summary>
        public string HM13005 {
            get { return _HM13005; }

            set { _HM13005 = value; }
        }

        /// <summary>
        ///  並び順
        /// </summary>
        public string HM13006 {
            get { return _HM13006; }

            set { _HM13006 = value; }
        }

        /// <summary>
        ///  参考項目(検査不要)
        /// </summary>
        public int HM13007 {
            get { return _HM13007; }

            set { _HM13007 = value; }
        }

        /// <summary>
        ///  個別(全体検査)
        /// </summary>
        public int HM13008 {
            get { return _HM13008; }

            set { _HM13008 = value; }
        }

        /// <summary>
        ///  写真タイプ
        /// </summary>
        public int HM13009 {
            get { return _HM13009; }

            set { _HM13009 = value; }
        }

        /// <summary>
        ///  値の入力可能
        /// </summary>
        public int HM13010 {
            get { return _HM13010; }

            set { _HM13010 = value; }
        }

        /// <summary>
        /// 値の単位
        /// </summary>
        public string HM13011 {
            get { return _HM13011; }

            set { _HM13011 = value; }
        }

        /// <summary>
        ///  説明
        /// </summary>
        public string HM13012 {
            get { return _HM13012; }

            set { _HM13012 = value; }
        }

        /// <summary>
        ///  特記事項断面コード
        /// </summary>
        public string HM13013 {
            get { return _HM13013; }

            set { _HM13013 = value; }
        }

        /// <summary>
        /// 非表示フラグ
        /// </summary>
        public int HM13014 {
            get { return _HM13014; }

            set { _HM13014 = value; }
        }

        /// <summary>
        /// 作成日時
        /// </summary>
        public DateTime HM13015 {
            get { return _HM13015; }

            set { _HM13015 = value; }
        }

        /// <summary>
        /// 作成オペレータ
        /// </summary>
        public string HM13016 {
            get { return _HM13016; }

            set { _HM13016 = value; }
        }

        /// <summary>
        /// 更新日時
        /// </summary>
        public DateTime HM13017 {
            get { return _HM13017; }

            set { _HM13017 = value; }
        }

        /// <summary>
        ///  更新オペレータ
        /// </summary>
        public string HM13018 {
            get { return _HM13018; }

            set { _HM13018 = value; }
        }

        /// <summary>
        ///  変更フラグ
        /// </summary>
        public string HMCHANGE {
            get { return _HMCHANGE; }

            set { _HMCHANGE = value; }
        }
    }
}

