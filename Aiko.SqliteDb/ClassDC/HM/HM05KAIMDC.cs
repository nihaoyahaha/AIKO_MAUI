using DI.DiNetWinServiceObject;

namespace DI.DiNetWinServiceObject
{
    /// <summary>
    /// 階マスターリモートサービス
    /// </summary>
    public class HM05KAIMDC : CMObjectDC
    {
        // 工事コード
        private string _HM05001;
        // 階コード
        private string _HM05002;
        // 階名
        private string _HM05003;
        // 並び順
        private string _HM05004;
        // 非表示フラグ
        private int _HM05005;
        // 作成日時
        private DateTime _HM05006;
        // 作成オペレータ
        private string _HM05007;
        // 更新日時
        private DateTime _HM05008;
        // 更新オペレータ
        private string _HM05009;
        // 変更フラグ
        private string _HMCHANGE;

        /// <summary>
        /// 工事コード
        /// </summary>
        public string HM05001 {
            get { return _HM05001; }
            set { _HM05001 = value; }
        }

        /// <summary>
        /// 階コード
        /// </summary>
        public string HM05002 {
            get { return _HM05002; }
            set { _HM05002 = value; }
        }

        /// <summary>
        /// 階名
        /// </summary>
        public string HM05003 {
            get { return _HM05003; }
            set { _HM05003 = value; }
        }

        /// <summary>
        /// 並び順
        /// </summary>
        public string HM05004 {
            get { return _HM05004; }
            set { _HM05004 = value; }
        }

        /// <summary>
        /// 非表示フラグ
        /// </summary>
        public int HM05005 {
            get { return _HM05005; }
            set { _HM05005 = value; }
        }

        /// <summary>
        /// 作成日時
        /// </summary>
        public DateTime HM05006 {
            get { return _HM05006; }
            set { _HM05006 = value; }
        }

        /// <summary>
        /// 作成オペレータ
        /// </summary>
        public string HM05007 {
            get { return _HM05007; }
            set { _HM05007 = value; }
        }

        /// <summary>
        /// 更新日時
        /// </summary>
        public DateTime HM05008 {
            get { return _HM05008; }
            set { _HM05008 = value; }
        }

        /// <summary>
        /// 更新オペレータ
        /// </summary>
        public string HM05009 {
            get { return _HM05009; }
            set { _HM05009 = value; }
        }

        /// <summary>
        /// 変更フラグ
        /// </summary>
        public string HMCHANGE {
            get { return _HMCHANGE; }
            set { _HMCHANGE = value; }
        }

        ///// <summary>
        ///// 階マスターを更新する
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
        //            strB.Append("INSERT INTO HM05KAIM( ");
        //            strB.Append("HM05001, HM05002, HM05003, HM05004, HM05005, HM05006, HM05007, HM05008, HM05009 )");
        //            strB.Append("VALUES( ");
        //            strB.Append(":HM05001, :HM05002, :HM05003, :HM05004, :HM05005, CURRENT_TIMESTAMP, :HM05007, CURRENT_TIMESTAMP, :HM05009 )");
        //        } else {
        //            return false;
        //        }
        //    }
        //    // 修正の場合
        //    else {
        //        strB.Append("UPDATE HM05KAIM SET ");
        //        strB.Append("HM05001 = :HM05001,  ");
        //        strB.Append("HM05002 = :HM05002,  ");
        //        strB.Append("HM05003 = :HM05003,  ");
        //        strB.Append("HM05004 = :HM05004,  ");
        //        strB.Append("HM05005 = :HM05005,  ");
        //        strB.Append("HM05008 = CURRENT_TIMESTAMP,  ");
        //        strB.Append("HM05009 = :HM05009   ");
        //        strB.Append("WHERE HM05001 = :HM05001 ");
        //        strB.Append("AND HM05002 = :HM05002 ");
        //    }
        //    para = new NCPara(":HM05001", NpgsqlDbType.Char, 10, _HM05001);
        //    list.Add(para);
        //    para = new NCPara(":HM05002", NpgsqlDbType.Char, 4, _HM05002, " ");
        //    list.Add(para);
        //    para = new NCPara(":HM05003", NpgsqlDbType.Char, 24, _HM05003, " ");
        //    list.Add(para);
        //    para = new NCPara(":HM05004", NpgsqlDbType.Char, 4, _HM05004, " ");
        //    list.Add(para);
        //    para = new NCPara(":HM05005", NpgsqlDbType.Numeric, 0, _HM05005, 0);
        //    list.Add(para);
        //    para = new NCPara(":HM05007", NpgsqlDbType.Char, 20, _HM05007, " ");
        //    list.Add(para);
        //    para = new NCPara(":HM05009", NpgsqlDbType.Char, 20, _HM05009, " ");
        //    list.Add(para);
        //    return DataBase.ExecSql(strB.ToString(), list, connection, transaction);
        //}

        ///// <summary>
        ///// 指定した階マスターを削除する
        ///// </summary>
        ///// <param name="connection"></param>
        ///// <param name="transaction"></param>
        ///// <returns></returns>
        //public override bool DeleteTable(NpgsqlConnection connection, NpgsqlTransaction transaction) {
        //    StringBuilder strB = new StringBuilder();
        //    NCArrayList list = new NCArrayList();
        //    NCPara para = null;
        //    strB.Append("DELETE FROM HM05KAIM ");
        //    strB.Append("WHERE HM05001 = :HM05001 ");
        //    strB.Append("AND HM05002 = :HM05002 ");
        //    para = new NCPara(":HM05001", NpgsqlDbType.Char, 10, _HM05001);
        //    list.Add(para);
        //    para = new NCPara(":HM05002", NpgsqlDbType.Char, 4, _HM05002);
        //    list.Add(para);
        //    return DataBase.ExecSql(strB.ToString(), list, connection, transaction);
        //}

        ///// <summary>
        ///// 指定した階マスターの存在をチェックする
        ///// </summary>
        ///// <param name="connection"></param>
        ///// <param name="transaction"></param>
        ///// <returns></returns>
        //public override bool IsExists(NpgsqlConnection connection, NpgsqlTransaction transaction) {
        //    StringBuilder strB = new StringBuilder();
        //    NCArrayList list = new NCArrayList();
        //    NCPara para = null;
        //    strB.Append("SELECT COUNT(0) AS CNT ");
        //    strB.Append("FROM HM05KAIM ");
        //    strB.Append("WHERE HM05001 = :HM05001 ");
        //    strB.Append("AND HM05002 = :HM05002 ");
        //    para = new NCPara(":HM05001", NpgsqlDbType.Char, 10, _HM05001);
        //    list.Add(para);
        //    para = new NCPara(":HM05002", NpgsqlDbType.Char, 4, _HM05002);
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
