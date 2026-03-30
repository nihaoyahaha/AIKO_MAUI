using DI.DiNetWinServiceObject;

namespace DI.DiNetWinServiceObject
{
    /// <summary>
    /// 工区マスターリモートサービス
    /// </summary>
    public class HM07KOKUDC : CMObjectDC
    {
        // 工事コード
        private string _HM07001;
        // 工区コード
        private string _HM07002;
        // 工区名
        private string _HM07003;
        // マップコード
        private string _HM07004;
        // 並び順
        private string _HM07005;
        // 打設予定日
        private int _HM07006;
        // 非表示フラグ
        private int _HM07007;
        // 作成日時
        private DateTime _HM07008;
        // 作成オペレータ
        private string _HM07009;
        // 更新日時
        private DateTime _HM07010;
        // 更新オペレータ
        private string _HM07011;
        // 同期日時
        private DateTime _HM07012;
        // 同期オペレータ
        private string _HM07013;
        // マップに表示位置X
        private int _HM07014;
        // マップに表示位置Y
        private int _HM07015;
        // 変更フラグ
        private string _HMCHANGE;

        /// <summary>
        /// 工事コード
        /// </summary>
        public string HM07001
        {
            get { return _HM07001; }

            set { _HM07001 = value; }
        }

        /// <summary>
        /// 工区コード
        /// </summary>
        public string HM07002
        {
            get { return _HM07002; }

            set { _HM07002 = value; }
        }

        /// <summary>
        /// 工区名
        /// </summary>
        public string HM07003
        {
            get { return _HM07003; }

            set { _HM07003 = value; }
        }

        /// <summary>
        /// マップコード
        /// </summary>
        public string HM07004
        {
            get { return _HM07004; }

            set { _HM07004 = value; }
        }

        /// <summary>
        /// 並び順
        /// </summary>
        public string HM07005
        {
            get { return _HM07005; }

            set { _HM07005 = value; }
        }

        /// <summary>
        /// 打設予定日
        /// </summary>
        public int HM07006
        {
            get { return _HM07006; }

            set { _HM07006 = value; }
        }

        /// <summary>
        /// 非表示フラグ
        /// </summary>
        public int HM07007
        {
            get { return _HM07007; }

            set { _HM07007 = value; }
        }

        //作成日時
        public DateTime HM07008
        {
            get { return _HM07008; }

            set { _HM07008 = value; }
        }

        /// <summary>
        /// 作成オペレータ
        /// </summary>
        public string HM07009
        {
            get { return _HM07009; }

            set { _HM07009 = value; }
        }

        /// <summary>
        /// 更新日時
        /// </summary>
        public DateTime HM07010
        {
            get { return _HM07010; }

            set { _HM07010 = value; }
        }

        /// <summary>
        /// 更新オペレータ
        /// </summary>
        public string HM07011
        {
            get { return _HM07011; }

            set { _HM07011 = value; }
        }

        /// <summary>
        /// 同期日時
        /// </summary>
        public DateTime HM07012
        {
            get { return _HM07012; }

            set { _HM07012 = value; }
        }

        /// <summary>
        /// 同期オペレータ
        /// </summary>
        public string HM07013
        {
            get { return _HM07013; }

            set { _HM07013 = value; }
        }

        /// <summary>
        /// マップに表示位置X
        /// </summary>
        public int HM07014
        {
            get { return _HM07014; }

            set { _HM07014 = value; }
        }

        /// <summary>
        /// マップに表示位置Y
        /// </summary>
        public int HM07015
        {
            get { return _HM07015; }

            set { _HM07015= value; }
        }

        /// <summary>
        /// 変更フラグ
        /// </summary>
        public string HMCHANGE {
            get { return _HMCHANGE; }

            set { _HMCHANGE = value; }
        }



        ///// <summary>
        ///// 確認項目を更新する
        ///// </summary>
        ///// <param name="connection"></param>
        ///// <param name="transaction"></param>
        ///// <returns></returns>
        //public override bool UpdateTable(NpgsqlConnection connection, NpgsqlTransaction transaction) {
        //    StringBuilder strB = new StringBuilder();
        //    NCArrayList list = new NCArrayList();
        //    NCPara para = null;

        //    // 新規の場合
        //    if (IsNew) {
        //        // 指定したデータが存在しない
        //        if (IsExists(connection, transaction) == false) {
        //            strB.Append("INSERT INTO HM07KOKU( ");
        //            strB.Append("HM07001 ,HM07002 ,HM07003 ,HM07004 ,HM07005 ,HM07006 ,HM07007 ,HM07008, ");
        //            strB.Append("HM07009 ,HM07010 ,HM07011 )");
        //            strB.Append("VALUES( ");
        //            strB.Append(":HM07001 ,:HM07002 ,:HM07003 ,:HM07004 ,:HM07005 ,:HM07006 ,:HM07007 ,CURRENT_TIMESTAMP, ");
        //            strB.Append(":HM07009 ,CURRENT_TIMESTAMP ,:HM07011 )");
        //        } else {
        //            return false;
        //        }
        //    }
        //    // 修正の場合
        //    else {
        //        strB.Append("UPDATE HM07KOKU SET ");
        //        strB.Append("HM07001 = :HM07001, ");
        //        strB.Append("HM07002 = :HM07002, ");
        //        strB.Append("HM07003 = :HM07003, ");
        //        strB.Append("HM07004 = :HM07004, ");
        //        strB.Append("HM07005 = :HM07005, ");
        //        strB.Append("HM07006 = :HM07006, ");
        //        strB.Append("HM07007 = :HM07007, ");
        //        strB.Append("HM07010 = CURRENT_TIMESTAMP, ");
        //        strB.Append("HM07011 = :HM07011, ");
        //        strB.Append("WHERE HM07001 = :HM07001 ");
        //        strB.Append("AND HM07002 = :HM07002 ");
        //    }
        //    para = new NCPara(":HM07001", NpgsqlDbType.Char, 10, _HM07001);
        //    list.Add(para);
        //    para = new NCPara(":HM07002", NpgsqlDbType.Char, 4, _HM07002, " ");
        //    list.Add(para);
        //    para = new NCPara(":HM07003", NpgsqlDbType.Char, 24, _HM07003, " ");
        //    list.Add(para);
        //    para = new NCPara(":HM07004", NpgsqlDbType.Char, 4, _HM07004, " ");
        //    list.Add(para);
        //    para = new NCPara(":HM07005", NpgsqlDbType.Char, 4, _HM07005, " ");
        //    list.Add(para);
        //    para = new NCPara(":HM07006", NpgsqlDbType.Numeric, 0, _HM07006, 0);
        //    list.Add(para);
        //    para = new NCPara(":HM07007", NpgsqlDbType.Numeric, 0, _HM07007, 0);
        //    list.Add(para);
        //    para = new NCPara(":HM07009", NpgsqlDbType.Char, 20, _HM07009, " ");
        //    list.Add(para);
        //    para = new NCPara(":HM07011", NpgsqlDbType.Char, 20, _HM07011, " ");
        //    list.Add(para);

        //    return DataBase.ExecSql(strB.ToString(), list, connection, transaction);
        //}

        ///// <summary>
        ///// 指定した部位マスターテーブルを削除する
        ///// </summary>
        ///// <param name="connection"></param>
        ///// <param name="transaction"></param>
        ///// <returns></returns>
        //public override bool DeleteTable(NpgsqlConnection connection, NpgsqlTransaction transaction) {
        //    StringBuilder strB = new StringBuilder();
        //    NCArrayList list = new NCArrayList();
        //    NCPara para = null;
        //    strB.Append("DELETE FROM HM07KOKU ");
        //    strB.Append("WHERE HM07001 = :HM07001 ");
        //    strB.Append("AND HM07002 = :HM07002 ");
        //    para = new NCPara(":HM07001", NpgsqlDbType.Char, 10, _HM07001);
        //    list.Add(para);
        //    para = new NCPara(":HM07002", NpgsqlDbType.Char, 4, _HM07002);
        //    list.Add(para);
        //    return DataBase.ExecSql(strB.ToString(), list, connection, transaction);
        //}

        ///// <summary>
        ///// 指定した部位マスターテーブルの存在をチェックする
        ///// </summary>
        ///// <param name="connection"></param>
        ///// <param name="transaction"></param>
        ///// <returns></returns>
        //public override bool IsExists(NpgsqlConnection connection, NpgsqlTransaction transaction) {
        //    StringBuilder strB = new StringBuilder();
        //    NCArrayList list = new NCArrayList();
        //    NCPara para = null;
        //    strB.Append("SELECT COUNT(0) AS CNT ");
        //    strB.Append("FROM HM07KOKU ");
        //    strB.Append("WHERE HM07001 = :HM07001 ");
        //    strB.Append("AND HM07002 = :HM07002 ");
        //    para = new NCPara(":HM07001", NpgsqlDbType.Char, 10, _HM07001);
        //    list.Add(para);
        //    para = new NCPara(":HM07002", NpgsqlDbType.Char, 4, _HM07002);
        //    list.Add(para);
        //    object obj = new object();
        //    if (DataBase.ExecSql(strB.ToString(), list, ref obj, connection, transaction) &&
        //        obj != null && Int32.Parse(obj.ToString()) > 0) {
        //        return true;
        //    }
        //    return false;
        //}
    }
}

