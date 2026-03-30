using DI.DiNetWinServiceObject;

namespace DI.DiNetWinServiceObject
{
    /// <summary>
    /// 検査結果テーブルリモートサービス
    /// </summary>
    public class HR02KSKKDC: CMObjectDC
    {
        //工事コード
        private string _HR02001;
        //アイテムコード
        private string _HR02002;
        //確認項目コード
        private string _HR02003;
        //値
        private string _HR02004;
        //判定
        private int _HR02005;
        //確認日
        private int _HR02006;
        //確認オペレータ
        private string _HR02007;
        //指摘日
        private int _HR02008;
        //指摘オペレータ
        private string _HR02009;
        //メモ
        private string _HR02010;
        //方法
        private string _HR02011;
        //写真枚数
        private int _HR02012;
        //作成日時
        private DateTime _HR02013;
        //作成オペレータ
        private string _HR02014;
        //更新日時
        private DateTime _HR02015;
        //更新オペレータ
        private string _HR02016;
        //同期日時
        private DateTime _HR02017;
        //同期オペレータ
        private string _HR02018;
        //確認方法
        private int _HR02019;
        // 変更フラグ
        private string _HMCHANGE;

        /// <summary>
        /// 工事コード
        /// </summary>
        public string HR02001
        {
            get
            {
                return _HR02001;
            }
            set
            {
                _HR02001 = value;
            }
        }

        /// <summary>
        /// アイテムコード
        /// </summary>
        public string HR02002
        {
            get
            {
                return _HR02002;
            }
            set
            {
                _HR02002 = value;
            }
        }

        /// <summary>
        /// 確認項目コード
        /// </summary>
        public string HR02003
        {
            get
            {
                return _HR02003;
            }
            set
            {
                _HR02003 = value;
            }
        }

        /// <summary>
        /// 値
        /// </summary>
        public string HR02004
        {
            get
            {
                return _HR02004;
            }
            set
            {
                _HR02004 = value;
            }
        }

        /// <summary>
        /// 判定
        /// </summary>
        public int HR02005
        {
            get
            {
                return _HR02005;
            }
            set
            {
                _HR02005 = value;
            }
        }

        /// <summary>
        /// 確認日
        /// </summary>
        public int HR02006
        {
            get
            {
                return _HR02006;
            }
            set
            {
                _HR02006 = value;
            }
        }

        /// <summary>
        /// 確認オペレータ
        /// </summary>
        public string HR02007
        {
            get
            {
                return _HR02007;
            }
            set
            {
                _HR02007 = value;
            }
        }

        /// <summary>
        /// 指摘日
        /// </summary>
        public int HR02008
        {
            get
            {
                return _HR02008;
            }
            set
            {
                _HR02008 = value;
            }
        }

        /// <summary>
        /// 指摘オペレータ
        /// </summary>
        public string HR02009
        {
            get
            {
                return _HR02009;
            }
            set
            {
                _HR02009 = value;
            }
        }

        /// <summary>
        /// メモ
        /// </summary>
        public string HR02010
        {
            get
            {
                return _HR02010;
            }
            set
            {
                _HR02010 = value;
            }
        }

        /// <summary>
        /// 備考
        /// </summary>
        public string HR02011
        {
            get
            {
                return _HR02011;
            }
            set
            {
                _HR02011 = value;
            }
        }

        /// <summary>
        /// 写真枚数
        /// </summary>
        public int HR02012
        {
            get
            {
                return _HR02012;
            }
            set
            {
                _HR02012 = value;
            }
        }

        /// <summary>
        /// 作成日時
        /// </summary>
        public DateTime HR02013
        {
            get
            {
                return _HR02013;
            }
            set
            {
                _HR02013 = value;
            }
        }

        /// <summary>
        /// 作成オペレータ
        /// </summary>
        public string HR02014
        {
            get
            {
                return _HR02014;
            }
            set
            {
                _HR02014 = value;
            }
        }

        /// <summary>
        /// 更新日時
        /// </summary>
        public DateTime HR02015
        {
            get
            {
                return _HR02015;
            }
            set
            {
                _HR02015 = value;
            }
        }

        /// <summary>
        /// 更新オペレータ
        /// </summary>
        public string HR02016
        {
            get
            {
                return _HR02016;
            }
            set
            {
                _HR02016 = value;
            }
        }

        /// <summary>
        /// 同期日時
        /// </summary>
        public DateTime HR02017
        {
            get
            {
                return _HR02017;
            }
            set
            {
                _HR02017 = value;
            }
        }

        /// <summary>
        /// 同期オペレータ
        /// </summary>
        public string HR02018
        {
            get
            {
                return _HR02018;
            }
            set
            {
                _HR02018 = value;
            }
        }
        //確認方法
        public int HR02019
        {
            get { return _HR02019; }

            set { _HR02019 = value; }

        }

        /// <summary>
        ///  変更フラグ
        /// </summary>
        public string HMCHANGE
        {
            get { return _HMCHANGE; }

            set { _HMCHANGE = value; }
        }


       
    }
}
