using DI.DiNetWinServiceObject;

namespace DI.DiNetWinServiceObject
{
    /// <summary>
    /// 部位マスターリモートサービス
    /// </summary>
    public class HM06BUIMDC : CMObjectDC
    {
        // 工事コード
        private string _HM06001;
        // 部位コード
        private string _HM06002;
        // 部位名
        private string _HM06003;
        // 記号
        private string _HM06004;
        // 階指定
        private int _HM06005;
        // 並び順
        private string _HM06006;
        // 共通事項断面コード
        private string _HM06007;
        // タイトル断面コード
        private string _HM06008;
        // 非表示フラグ
        private int _HM06009;
        // 作成日時
        private DateTime _HM06010;
        // 作成オペレータ
        private string _HM06011;
        // 更新日時
        private DateTime _HM06012;
        // 更新オペレータ
        private string _HM06013;
        // 同期日時
        private DateTime _HM06014;
        // 同期オペレータ
        private string _HM06015;
        // アイコンリソースファイル
        private string _HM06016;
        // 参照
        private int _HM06017;
        // 命名規則
        private int _HM06018;
        // 変更フラグ
        private string _HMCHANGE;

        /// <summary>
        /// 工事コード
        /// </summary>
        public string HM06001
        {
            get { return _HM06001; }
            set { _HM06001 = value; }
        }

        /// <summary>
        /// 部位コード
        /// </summary>
        public string HM06002
        {
            get { return _HM06002; }
            set { _HM06002 = value; }
        }

        /// <summary>
        /// 部位名
        /// </summary>
        public string HM06003
        {
            get { return _HM06003; }
            set { _HM06003 = value; }
        }

        /// <summary>
        /// 記号
        /// </summary>
        public string HM06004
        {
            get { return _HM06004; }
            set { _HM06004 = value; }
        }

        /// <summary>
        /// 階指定
        /// </summary>
        public int HM06005
        {
            get { return _HM06005; }
            set { _HM06005 = value; }
        }

        /// <summary>
        /// 並び順
        /// </summary>
        public string HM06006
        {
            get { return _HM06006; }
            set { _HM06006 = value; }
        }

        /// <summary>
        /// 共通事項断面コード
        /// </summary>
        public string HM06007
        {
            get { return _HM06007; }
            set { _HM06007 = value; }
        }

        /// <summary>
        /// タイトル断面コード
        /// </summary>
        public string HM06008
        {
            get { return _HM06008; }
            set { _HM06008 = value; }
        }

        /// <summary>
        /// 非表示フラグ
        /// </summary>
        public int HM06009
        {
            get { return _HM06009; }
            set { _HM06009 = value; }
        }

        /// <summary>
        /// 作成日時
        /// </summary>
        public DateTime HM06010
        {
            get { return _HM06010; }
            set { _HM06010 = value; }
        }

        /// <summary>
        /// 作成オペレータ
        /// </summary>
        public string HM06011
        {
            get { return _HM06011; }
            set { _HM06011 = value; }
        }

        /// <summary>
        /// 更新日時
        /// </summary>
        public DateTime HM06012
        {
            get { return _HM06012; }
            set { _HM06012 = value; }
        }

        /// <summary>
        /// 更新オペレータ
        /// </summary>
        public string HM06013
        {
            get { return _HM06013; }
            set { _HM06013 = value; }
        }

        /// <summary>
        /// 同期日時
        /// </summary>
        public DateTime HM06014
        {
            get { return _HM06014; }
            set { _HM06014 = value; }
        }

        /// <summary>
        /// 同期オペレータ
        /// </summary>
        public string HM06015
        {
            get { return _HM06015; }
            set { _HM06015 = value; }
        }

        /// <summary>
        /// アイコンリソースファイル
        /// </summary>
        public string HM06016
        {
            get { return _HM06016; }
            set { _HM06016 = value; }
        }

        /// <summary>
        /// 参照
        /// </summary>
        public int HM06017
        {
            get { return _HM06017; }
            set { _HM06017 = value; }
        }

        /// <summary>
        /// 命名規則
        /// </summary>
        public int HM06018
        {
            get { return _HM06018; }
            set { _HM06018 = value; }
        }

        /// <summary>
        /// 変更フラグ
        /// </summary>
        public string HMCHANGE
        {
            get { return _HMCHANGE; }
            set { _HMCHANGE = value; }
        }

        ///// <summary>
        ///// 部位マスターテーブルを更新する
        ///// </summary>
        ///// <param name="connection"></param>
        ///// <param name="transaction"></param>
        ///// <returns></returns>
        //public override bool UpdateTable(NpgsqlConnection connection, NpgsqlTransaction transaction)
        //{
        //    StringBuilder strB = new StringBuilder();
        //    NCArrayList list = new NCArrayList();
        //    NCPara para = null;

        //    // 新規の場合
        //    if (IsNew)
        //    {
        //        // 指定したデータが存在しない
        //        if (IsExists(connection, transaction) == false)
        //        {
        //            strB.Append("INSERT INTO HM06BUIM( ");
        //            strB.Append("HM06001 ,HM06002 ,HM06003 ,HM06004 ,HM06005 ,HM06006 ,HM06007 ,HM06008, )");
        //            strB.Append("HM06009 ,HM06010 ,HM06011 ,HM06012 ,HM06013 ,HM06016 ,HM06017 )");
        //            strB.Append("VALUES( ");
        //            strB.Append(":HM06001 ,:HM06002 ,:HM06003 ,:HM06004 ,:HM06005 ,:HM06006 ,:HM06007 ,:HM06008, )");
        //            strB.Append(":HM06009 ,CURRENT_TIMESTAMP ,:HM06011 ,CURRENT_TIMESTAMP ,:HM06013 ,:HM06016 ,:HM06017 )");
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //    // 修正の場合
        //    else
        //    {
        //        strB.Append("UPDATE HM06BUIM SET ");
        //        strB.Append("HM06001 = :HM06001, ");
        //        strB.Append("HM06002 = :HM06002, ");
        //        strB.Append("HM06003 = :HM06003, ");
        //        strB.Append("HM06004 = :HM06004, ");
        //        strB.Append("HM06005 = :HM06005, ");
        //        strB.Append("HM06006 = :HM06006, ");
        //        strB.Append("HM06007 = :HM06007, ");
        //        strB.Append("HM06008 = :HM06008, ");
        //        strB.Append("HM06009 = :HM06009, ");
        //        strB.Append("HM06012 = CURRENT_TIMESTAMP, ");
        //        strB.Append("HM06013 = :HM06013, ");
        //        strB.Append("HM06016 = :HM06016, ");
        //        strB.Append("HM06017 = :HM06017 ");
        //        strB.Append("WHERE HM06001 = :HM06001 ");
        //        strB.Append("AND HM06002 = :HM06002 ");
        //    }
        //    para = new NCPara(":HM06001", NpgsqlDbType.Char, 10, _HM06001);
        //    list.Add(para);
        //    para = new NCPara(":HM06002", NpgsqlDbType.Char, 4, _HM06002, " ");
        //    list.Add(para);
        //    para = new NCPara(":HM06003", NpgsqlDbType.Char, 20, _HM06003, " ");
        //    list.Add(para);
        //    para = new NCPara(":HM06004", NpgsqlDbType.Char, 5, _HM06004, " ");
        //    list.Add(para);
        //    para = new NCPara(":HM06005", NpgsqlDbType.Numeric, 0, _HM06005, 0);
        //    list.Add(para);
        //    para = new NCPara(":HM06006", NpgsqlDbType.Char, 4, _HM06006, " ");
        //    list.Add(para);
        //    para = new NCPara(":HM06007", NpgsqlDbType.Char, 4, _HM06007, " ");
        //    list.Add(para);
        //    para = new NCPara(":HM06008", NpgsqlDbType.Char, 4, _HM06008, " ");
        //    list.Add(para);
        //    para = new NCPara(":HM06009", NpgsqlDbType.Numeric, 0, _HM06009, 0);
        //    list.Add(para);
        //    para = new NCPara(":HM06011", NpgsqlDbType.Char, 20, _HM06011, " ");
        //    list.Add(para);
        //    para = new NCPara(":HM06013", NpgsqlDbType.Char, 20, _HM06013, " ");
        //    list.Add(para);
        //    para = new NCPara(":HM06016", NpgsqlDbType.Char, 20, _HM06016, " ");
        //    list.Add(para);
        //    para = new NCPara(":HM06017", NpgsqlDbType.Numeric, 0, _HM06017, 0);
        //    list.Add(para);
        //    return DataBase.ExecSql(strB.ToString(), list, connection, transaction);
        //}

        ///// <summary>
        ///// 指定した部位マスターテーブルを削除する
        ///// </summary>
        ///// <param name="connection"></param>
        ///// <param name="transaction"></param>
        ///// <returns></returns>
        //public override bool DeleteTable(NpgsqlConnection connection, NpgsqlTransaction transaction)
        //{
        //    StringBuilder strB = new StringBuilder();
        //    NCArrayList list = new NCArrayList();
        //    NCPara para = null;
        //    strB.Append("DELETE FROM HM06BUIM ");
        //    strB.Append("WHERE HM06001 = :HM06001 ");
        //    strB.Append("AND HM06002 = :HM06002 ");
        //    para = new NCPara(":HM06001", NpgsqlDbType.Char, 10, _HM06001);
        //    list.Add(para);
        //    para = new NCPara(":HM06002", NpgsqlDbType.Char, 4, _HM06002);
        //    list.Add(para);
        //    return DataBase.ExecSql(strB.ToString(), list, connection, transaction);
        //}

        ///// <summary>
        ///// 指定した部位マスターテーブルの存在をチェックする
        ///// </summary>
        ///// <param name="connection"></param>
        ///// <param name="transaction"></param>
        ///// <returns></returns>
        //public override bool IsExists(NpgsqlConnection connection, NpgsqlTransaction transaction)
        //{
        //    StringBuilder strB = new StringBuilder();
        //    NCArrayList list = new NCArrayList();
        //    NCPara para = null;
        //    strB.Append("SELECT COUNT(0) AS CNT ");
        //    strB.Append("FROM HM06BUIM ");
        //    strB.Append("WHERE HM06001 = :HM06001 ");
        //    strB.Append("AND HM06002 = :HM06002 ");
        //    para = new NCPara(":HM06001", NpgsqlDbType.Char, 10, _HM06001);
        //    list.Add(para);
        //    para = new NCPara(":HM06002", NpgsqlDbType.Char, 4, _HM06002);
        //    list.Add(para);
        //    object obj = new object();
        //    if (DataBase.ExecSql(strB.ToString(), list, ref obj, connection, transaction) &&
        //        obj != null && Int32.Parse(obj.ToString()) > 0)
        //    {
        //        return true;
        //    }
        //    return false;
        //}
    }
}

