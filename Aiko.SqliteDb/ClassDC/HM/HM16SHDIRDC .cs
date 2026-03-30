using DI.DiNetWinServiceObject;

namespace DI.DiNetWinServiceObject
{
    /// <summary>
    /// オペレータサブテーブルリモートサービス
    /// </summary>
    public class HM16SHDIRDC : CMObjectDC
    {
        // 工事コード
        private string _HM16001;
        // 撮影方向コード
        private int _HM16002;
        // 撮影方向名
        private string _HM16003;
        // 削除可能フラグ
        private int _HM16004;
        // 並び順
        private string _HM16005;
        // 非表示フラグ
        private int _HM16006;
        // 作成日時
        private DateTime _HM16007;
        // 作成オペレータ
        private string _HM16008;
        // 更新日時
        private DateTime _HM16009;
        // 更新オペレータ
        private string _HM16010;
        // 変更フラグ
        private string _HMCHANGE;

        /// <summary>
        /// 工事コード
        /// </summary>
        public string HM16001
        {
            get { return _HM16001; }
            set { _HM16001 = value; }
        }

        /// <summary>
        /// 撮影方向コード
        /// </summary>
        public int HM16002
        {
            get { return _HM16002; }
            set { _HM16002 = value; }
        }

        /// <summary>
        /// 撮影方向名
        /// </summary>
        public string HM16003
        {
            get { return _HM16003; }
            set { _HM16003 = value; }
        }

        /// <summary>
        /// 削除可能フラグ
        /// </summary>
        public int HM16004
        {
            get { return _HM16004; }
            set { _HM16004 = value; }
        }

        /// <summary>
        /// 並び順
        /// </summary>
        public string HM16005
        {
            get { return _HM16005; }
            set { _HM16005 = value; }
        }

        /// <summary>
        /// 非表示フラグ
        /// </summary>
        public int HM16006
        {
            get { return _HM16006; }
            set { _HM16006 = value; }
        }

        /// <summary>
        /// 作成日時
        /// </summary>
        public DateTime HM16007
        {
            get { return _HM16007; }
            set { _HM16007 = value; }
        }

        /// <summary>
        /// 作成オペレータ
        /// </summary>
        public string HM16008
        {
            get { return _HM16008; }
            set { _HM16008 = value; }
        }

        /// <summary>
        /// 更新日時
        /// </summary>
        public DateTime HM16009
        {
            get { return _HM16009; }
            set { _HM16009 = value; }
        }

        /// <summary>
        /// 更新オペレータ
        /// </summary>
        public string HM16010
        {
            get { return _HM16010; }
            set { _HM16010 = value; }
        }

        /// <summary>
        /// 変更フラグ
        /// </summary> 
        public string HMCHANGE
        {
            get { return _HMCHANGE; }

            set { _HMCHANGE = value; }
        }
    }
}

