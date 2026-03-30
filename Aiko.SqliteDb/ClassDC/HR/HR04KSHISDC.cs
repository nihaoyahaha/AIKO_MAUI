using DI.DiNetWinServiceObject;

namespace DI.DiNetWinServiceObject
{
    /// <summary>
    /// 検査履歴テーブルリモートサービス
    /// </summary>
    public class HR04KSHISDC : CMObjectDC
    {
        //工事コード
        private string _HR04001;
        //アイテムコード
        private string _HR04002;
        //確認項目コード
        private string _HR04003;
        //履歴No
        private int _HR04004;
        // 値
        private string _HR04005;
        // 判定
        private int _HR04006;
        //確認日
        private int _HR04007;
        //確認オペレータ
        private string _HR04008;
        //指摘日
        private int _HR04009;
        //指摘オペレータ
        private string _HR04010;
        //メモ
        private string _HR04011;
        //処理方法
        private string _HR04012;
        //写真枚数
        private int _HR04013;
        //確認方法
        private int _HR04014;
        //作成日時
        private DateTime _HR04015;
        //作成オペレータ
        private string _HR04016;
        // 変更フラグ
        private string _HMCHANGE;

        /// <summary>
        /// 工事コード
        /// </summary>
        public string HR04001  {   get {   return _HR04001; }   set {  _HR04001 = value; } }
        /// <summary>
        /// アイテムコード
        /// </summary>
        public string HR04002 { get { return _HR04002; } set { _HR04002 = value; } }
        /// <summary>
        /// 確認項目コード
        /// </summary>
        public string HR04003 { get { return _HR04003; } set { _HR04003 = value; } }
        /// <summary>
        ///  履歴No
        /// </summary>
        public int HR04004 { get { return _HR04004; } set { _HR04004 = value; } }
        /// <summary>
        ///  値
        /// </summary>
        public string HR04005 { get { return _HR04005; } set { _HR04005 = value; } }
        /// <summary>
        /// 判定
        /// </summary>
        public int HR04006 { get { return _HR04006; } set { _HR04006 = value; } }
        /// <summary>
        /// 確認日
        /// </summary>
        public int HR04007 { get { return _HR04007; } set { _HR04007 = value; } }
        /// <summary>
        /// 確認オペレータ
        /// </summary>
        public string HR04008 { get { return _HR04008; } set { _HR04008 = value; } }
        /// <summary>
        /// 指摘日
        /// </summary>
        public int HR04009 { get { return _HR04009; } set { _HR04009 = value; } }
        /// <summary>
        /// 指摘オペレータ
        /// </summary>
        public string HR04010 { get { return _HR04010; } set { _HR04010 = value; } }
        /// <summary>
        /// メモ
        /// </summary>
        public string HR04011 { get { return _HR04011; } set { _HR04011 = value; } }
        /// <summary>
        /// 処理方法
        /// </summary>
        public string HR04012 { get { return _HR04012; } set { _HR04012 = value; } }
        /// <summary>
        /// 写真枚数
        /// </summary>
        public int HR04013 { get { return _HR04013; } set { _HR04013 = value; } }
        /// <summary>
        /// 確認方法
        /// </summary>
        public int HR04014 { get { return _HR04014; } set { _HR04014 = value; } }
        /// <summary>
        /// 作成日時
        /// </summary>
        public DateTime HR04015 { get { return _HR04015; } set { _HR04015 = value; } }
        /// <summary>
        /// 作成オペレータ
        /// </summary>
        public string HR04016 { get { return _HR04016; } set { _HR04016 = value; } }
        /// <summary>
        /// 変更フラグ
        /// </summary>
        public string HMCHANGE { get { return _HMCHANGE; } set { _HMCHANGE = value; } }

       

    }
}