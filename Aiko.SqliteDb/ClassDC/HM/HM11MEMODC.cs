using DI.DiNetWinServiceObject;

namespace DI.DiNetWinServiceObject
{
    /// <summary>
    /// メモマスターリモートサービス
    /// </summary>
    public class HM11MEMODC : CMObjectDC
    {
        // 工事コード
        private string _HM11001;
        // メモコード
        private string _HM11002;
        // メモ本文
        private string _HM11003;
        //  並び順
        private string _HM11004;
        // 作成日時
        private DateTime _HM11005;
        // 作成オペレータ
        private string _HM11006;
        // 更新日時
        private DateTime _HM11007;
        // 更新オペレータ
        private string _HM11008;
        // 変更フラグ
        private string _HMCHANGE;

        /// <summary>
        /// 工事コード
        /// </summary>
        public string HM11001
        {
            get { return _HM11001; }
            set { _HM11001 = value; }
        }

        /// <summary>
        /// メモコード
        /// </summary>
        public string HM11002
        {
            get { return _HM11002; }
            set { _HM11002 = value; }
        }

        /// <summary>
        /// メモ本文
        /// </summary>
        public string HM11003
        {
            get { return _HM11003; }
            set { _HM11003 = value; }
        }

        /// <summary>
        /// 並び順
        /// </summary>
        public string HM11004
        {
            get { return _HM11004; }
            set { _HM11004 = value; }
        }

        /// <summary>
        /// 作成日時
        /// </summary>
        public DateTime HM11005
        {
            get { return _HM11005; }
            set { _HM11005 = value; }
        }

        /// <summary>
        /// 作成オペレータ
        /// </summary>
        public string HM11006
        {
            get { return _HM11006; }
            set { _HM11006 = value; }
        }

        /// <summary>
        /// 更新日時
        /// </summary>
        public DateTime HM11007
        {
            get { return _HM11007; }
            set { _HM11007 = value; }
        }

        /// <summary>
        /// 更新オペレータ
        /// </summary>
        public string HM11008
        {
            get { return _HM11008; }
            set { _HM11008 = value; }
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
