using DI.DiNetWinServiceObject;

namespace DI.DiNetWinServiceObject
{
    /// <summary>
    /// マップ管理
    /// </summary>
    public class HM04MAPMDC : CMObjectDC
    {
        // 工事コード
        private string _HM04001;
        // マップコード
        private string _HM04002;
        // マップ名
        private string _HM04003;
        // マップグループコード
        private string _HM04004;
        // ファイルコード
        private string _HM04005;
        // 並び順
        private string _HM04006;
        // 表示範囲X
        private int _HM04007;
        // 表示範囲Y
        private int _HM04008;
        // 表示範囲Width
        private int _HM04009;
        // 表示範囲Height
        private int _HM04010;
        // 制御点1X
        private int _HM04011;
        // 制御点1Y
        private int _HM04012;
        // 制御点2X
        private int _HM04013;
        // 制御点2Y
        private int _HM04014;
        // Xガイド起用
        private int _HM04015;
        // XガイドX
        private int _HM04016;
        // XガイドY
        private int _HM04017;
        //XガイドWidth
        private int _HM04018;
        //XガイドHeight
        private int _HM04019;
        //Yガイド起用
        private int _HM04020;
        //YガイドX
        private int _HM04021;
        //YガイドY
        private int _HM04022;
        //YガイドWidth
        private int _HM04023;
        //YガイドHeight
        private int _HM04024;
        //Z範囲From
        private int _HM04025;
        //Z範囲To
        private int _HM04026;
        //表示倍率
        private int _HM04027;
        //XY表示
        private int _HM04028;
        //作成日時
        private DateTime _HM04029;
        //作成オペレータ
        private string _HM04030;
        //更新日時
        private DateTime _HM04031;
        //更新オペレータ
        private string _HM04032;
        //制御点1論理座標X
        private int? _HM04035;
        //制御点1論理座標Y
        private int? _HM04036;
        //制御点2論理座標X
        private int? _HM04037;
        //制御点2論理座標Y
        private int? _HM04038;
        // 階コード
        private string _HM04039;

        // 落書きの表示範囲X
        private int? _HM04040;
        // 落書きの表示範囲Y
        private int? _HM04041;
        // 落書きのマップコード
        private string _HM04042;

        /// <summary>
        /// 工事コード
        /// </summary>
        public string HM04001
        {
            get { return _HM04001; }

            set { _HM04001 = value; }
        }

        /// <summary>
        /// マップコード
        /// </summary>
        public string HM04002
        {
            get { return _HM04002; }

            set { _HM04002 = value; }
        }

        /// <summary>
        /// マップ名
        /// </summary>
        public string HM04003
        {
            get { return _HM04003; }

            set { _HM04003 = value; }
        }

        /// <summary>
        /// マップグループコード
        /// </summary>
        public string HM04004
        {
            get { return _HM04004; }

            set { _HM04004 = value; }
        }

        /// <summary>
        /// ファイルコード
        /// </summary>
        public string HM04005
        {
            get { return _HM04005; }

            set { _HM04005 = value; }
        }

        /// <summary>
        /// 並び順
        /// </summary>
        public string HM04006
        {
            get { return _HM04006; }

            set { _HM04006 = value; }
        }

        /// <summary>
        /// 表示範囲X
        /// </summary>
        public int HM04007
        {
            get { return _HM04007; }

            set { _HM04007 = value; }
        }

        /// <summary>
        /// 表示範囲Y
        /// </summary>
        public int HM04008
        {
            get { return _HM04008; }

            set { _HM04008 = value; }
        }

        /// <summary>
        /// 表示範囲Width
        /// </summary>
        public int HM04009
        {
            get { return _HM04009; }

            set { _HM04009 = value; }
        }

        /// <summary>
        /// 表示範囲Height
        /// </summary>
        public int HM04010
        {
            get { return _HM04010; }

            set { _HM04010 = value; }
        }

        /// <summary>
        /// 制御点1X
        /// </summary>
        public int HM04011
        {
            get { return _HM04011; }

            set { _HM04011 = value; }
        }

        /// <summary>
        /// 制御点1Y
        /// </summary>
        public int HM04012
        {
            get { return _HM04012; }

            set { _HM04012 = value; }
        }

        /// <summary>
        /// 制御点2X
        /// </summary>
        public int HM04013
        {
            get { return _HM04013; }

            set { _HM04013 = value; }
        }

        /// <summary>
        /// 制御点2Y
        /// </summary>
        public int HM04014
        {
            get { return _HM04014; }

            set { _HM04014 = value; }
        }

        /// <summary>
        /// Xガイド起用
        /// </summary>
        public int HM04015
        {
            get { return _HM04015; }

            set { _HM04015 = value; }
        }

        /// <summary>
        /// XガイドX
        /// </summary>
        public int HM04016
        {
            get { return _HM04016; }

            set { _HM04016 = value; }
        }

        /// <summary>
        /// XガイドY
        /// </summary>
        public int HM04017
        {
            get { return _HM04017; }

            set { _HM04017 = value; }
        }

        /// <summary>
        /// XガイドWidth
        /// </summary>
        public int HM04018
        {
            get { return _HM04018; }

            set { _HM04018 = value; }
        }

        /// <summary>
        /// XガイドHeight
        /// </summary>
        public int HM04019
        {
            get { return _HM04019; }

            set { _HM04019 = value; }
        }

        /// <summary>
        /// Yガイド起用
        /// </summary>
        public int HM04020
        {
            get { return _HM04020; }

            set { _HM04020 = value; }
        }

        /// <summary>
        /// YガイドX
        /// </summary>
        public int HM04021
        {
            get { return _HM04021; }

            set { _HM04021 = value; }
        }

        /// <summary>
        /// YガイドY
        /// </summary>
        public int HM04022
        {
            get { return _HM04022; }

            set { _HM04022 = value; }
        }

        /// <summary>
        /// YガイドWidth
        /// </summary>
        public int HM04023
        {
            get { return _HM04023; }

            set { _HM04023 = value; }
        }

        /// <summary>
        /// YガイドHeight
        /// </summary>
        public int HM04024
        {
            get { return _HM04024; }

            set { _HM04024 = value; }
        }

        /// <summary>
        /// Z範囲From
        /// </summary>
        public int HM04025
        {
            get { return _HM04025; }

            set { _HM04025 = value; }
        }

        /// <summary>
        /// Z範囲To
        /// </summary>
        public int HM04026
        {
            get { return _HM04026; }

            set { _HM04026 = value; }
        }

        /// <summary>
        /// 表示倍率
        /// </summary>
        public int HM04027
        {
            get { return _HM04027; }

            set { _HM04027 = value; }
        }

        /// <summary>
        /// XY表示
        /// </summary>
        public int HM04028
        {
            get { return _HM04028; }

            set { _HM04028 = value; }
        }

        /// <summary>
        /// 作成日時
        /// </summary>
        public DateTime HM04029
        {
            get { return _HM04029; }

            set { _HM04029 = value; }
        }

        /// <summary>
        /// 作成オペレータ
        /// </summary>
        public string HM04030
        {
            get { return _HM04030; }

            set { _HM04030 = value; }
        }

        /// <summary>
        /// 更新日時
        /// </summary>
        public DateTime HM04031
        {
            get { return _HM04031; }

            set { _HM04031 = value; }
        }

        /// <summary>
        /// 更新オペレータ
        /// </summary>
        public string HM04032
        {
            get { return _HM04032; }

            set { _HM04032 = value; }
        }

        /// <summary>
        /// 制御点1論理座標X
        /// </summary>
        public int? HM04035
        {
            get { return _HM04035; }

            set { _HM04035 = value; }
        }

        /// <summary>
        /// 制御点1論理座標Y
        /// </summary>
        public int? HM04036
        {
            get { return _HM04036; }

            set { _HM04036 = value; }
        }

        /// <summary>
        /// 制御点2論理座標X
        /// </summary>
        public int? HM04037
        {
            get { return _HM04037; }

            set { _HM04037 = value; }
        }

        /// <summary>
        /// 制御点2論理座標Y
        /// </summary>
        public int? HM04038
        {
            get { return _HM04038; }

            set { _HM04038 = value; }
        }

        /// <summary>
        /// 階コード
        /// </summary>
        public string HM04039
        {
            get { return _HM04039; }

            set { _HM04039 = value; }
        }

        /// <summary>
        /// 落書きの表示範囲X
        /// </summary>
        public int? HM04040
        {
            get { return _HM04040; }

            set { _HM04040 = value; }
        }

        /// <summary>
        /// 落書きの表示範囲Y
        /// </summary>
        public int? HM04041
        {
            get { return _HM04041; }

            set { _HM04041 = value; }
        }

        /// <summary>
        /// 落書きのマップコード
        /// </summary>
        public string HM04042
        {
            get { return _HM04042; }

            set { _HM04042 = value; }
        }

        // 変更フラグ
        private string _HMCHANGE;

        /// <summary>
        /// 変更フラグ
        /// </summary>
        public string HMCHANGE
        {
            get { return _HMCHANGE; }

            set { _HMCHANGE = value; }
        }

        ///// <summary>
        ///// 確認項目を更新する
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
        //            strB.Append("INSERT INTO HM04MAPM( ");
        //            strB.Append("HM04001 ,HM04002 ,HM04003 ,HM04004 ,HM04005 ,HM04006 ,HM04007 ,HM04008, ");
        //            strB.Append("HM04009 ,HM04010 ,HM04011 ,HM04012 ,HM04013,HM04014,HM04015 ,HM04016 ,HM04017,HM04018, ");
        //            strB.Append("HM04019 ,HM04020 ,HM04021 ,HM04022 ,HM04023,HM04024,HM04025 ,HM04026 ,HM04027,HM04028, ");
        //            strB.Append("HM04029 ,HM04030 ,HM04031 ,HM04032 ,HM04035,HM04036,HM04037 ,HM04038  ) ");
        //            strB.Append("VALUES( ");
        //            strB.Append(":HM04001 ,:HM04002 ,:HM04003 ,:HM04004 ,:HM04005 ,:HM04006 ,:HM04007 ,:HM04008, ");
        //            strB.Append(":HM04009 ,:HM04010 ,:HM04011 ,:HM04012 ,:HM04013 ,:HM04014 ,:HM04015 ,:HM04016, ");
        //            strB.Append(":HM04017 ,:HM04018 ,:HM04019 ,:HM04020 ,:HM04021 ,:HM04022 ,:HM04023 ,:HM04024, ");
        //            strB.Append(":HM04025 ,:HM04026 ,:HM04027 ,:HM04028 ,CURRENT_TIMESTAMP ,:HM04030 ,CURRENT_TIMESTAMP ,:HM04032, ");
        //            strB.Append(":HM04035 ,:HM04036 ,:HM04037 ,:HM04038 ) ");
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //    // 修正の場合
        //    else
        //    {
        //        if (HMCHANGE == "HM04003")
        //        {
        //            strB.Append("UPDATE HM04MAPM SET ");
        //            strB.Append("HM04003 = :HM04003, ");
        //            strB.Append("HM04031 = CURRENT_TIMESTAMP, ");
        //            strB.Append("HM04032 = :HM04032 ");
        //            strB.Append("WHERE HM04001 = :HM04001 ");
        //            strB.Append("AND HM04002 = :HM04002 ");
        //        }
        //        else if (HMCHANGE == "HM04005")
        //        {
        //            strB.Append("UPDATE HM04MAPM SET ");
        //            strB.Append("HM04005 = :HM04005, ");
        //            strB.Append("HM04007 = :HM04007, ");
        //            strB.Append("HM04008 = :HM04008, ");
        //            strB.Append("HM04009 = :HM04009, ");
        //            strB.Append("HM04010 = :HM04010, ");
        //            strB.Append("HM04011 = :HM04011, ");
        //            strB.Append("HM04012 = :HM04012, ");
        //            strB.Append("HM04013 = :HM04013, ");
        //            strB.Append("HM04014 = :HM04014, ");
        //            strB.Append("HM04031 = CURRENT_TIMESTAMP, ");
        //            strB.Append("HM04032 = :HM04032, ");
        //            strB.Append("HM04035 = :HM04035, ");
        //            strB.Append("HM04036 = :HM04036, ");
        //            strB.Append("HM04037 = :HM04037, ");
        //            strB.Append("HM04038 = :HM04038 ");
        //            strB.Append("WHERE HM04001 = :HM04001 ");
        //            strB.Append("AND HM04002 = :HM04002 ");
        //        }
        //        else if (HMCHANGE == "HM04MAPM")
        //        {
        //            strB.Append("UPDATE HM04MAPM SET ");
        //            strB.Append("HM04007 = :HM04007, ");
        //            strB.Append("HM04008 = :HM04008, ");
        //            strB.Append("HM04009 = :HM04009, ");
        //            strB.Append("HM04010 = :HM04010, ");
        //            strB.Append("HM04011 = :HM04011, ");
        //            strB.Append("HM04012 = :HM04012, ");
        //            strB.Append("HM04013 = :HM04013, ");
        //            strB.Append("HM04014 = :HM04014, ");
        //            strB.Append("HM04015 = :HM04015, ");
        //            strB.Append("HM04020 = :HM04020, ");
        //            strB.Append("HM04027 = :HM04027, ");
        //            strB.Append("HM04028 = :HM04028, ");
        //            strB.Append("HM04031 = CURRENT_TIMESTAMP, ");
        //            strB.Append("HM04032 = :HM04032, ");
        //            strB.Append("HM04035 = :HM04035, ");
        //            strB.Append("HM04036 = :HM04036, ");
        //            strB.Append("HM04037 = :HM04037, ");
        //            strB.Append("HM04038 = :HM04038 ");
        //            strB.Append("WHERE HM04001 = :HM04001 ");
        //            strB.Append("AND HM04002 = :HM04002 ");
        //        }
        //    }
        //    para = new NCPara(":HM04001", NpgsqlDbType.Char, 10, HM04001);
        //    list.Add(para);
        //    para = new NCPara(":HM04002", NpgsqlDbType.Char, 5, HM04002, " ");
        //    list.Add(para);
        //    para = new NCPara(":HM04003", NpgsqlDbType.Char, 20, HM04003, " ");
        //    list.Add(para);
        //    para = new NCPara(":HM04004", NpgsqlDbType.Char, 4, HM04004, " ");
        //    list.Add(para);
        //    para = new NCPara(":HM04005", NpgsqlDbType.Char, 4, HM04005, " ");
        //    list.Add(para);
        //    para = new NCPara(":HM04006", NpgsqlDbType.Char, 5, HM04006, " ");
        //    list.Add(para);
        //    para = new NCPara(":HM04007", NpgsqlDbType.Numeric, 6, HM04007, 0);
        //    list.Add(para);
        //    para = new NCPara(":HM04008", NpgsqlDbType.Numeric, 6, HM04008, 0);
        //    list.Add(para);
        //    para = new NCPara(":HM04009", NpgsqlDbType.Numeric, 6, HM04009, 0);
        //    list.Add(para);
        //    para = new NCPara(":HM04010", NpgsqlDbType.Numeric, 6, HM04010, 0);
        //    list.Add(para);
        //    para = new NCPara(":HM04011", NpgsqlDbType.Numeric, 6, HM04011, 0);
        //    list.Add(para);
        //    para = new NCPara(":HM04012", NpgsqlDbType.Numeric, 6, HM04012, 0);
        //    list.Add(para);
        //    para = new NCPara(":HM04013", NpgsqlDbType.Numeric, 6, HM04013, 0);
        //    list.Add(para);
        //    para = new NCPara(":HM04014", NpgsqlDbType.Numeric, 6, HM04014, 0);
        //    list.Add(para);
        //    para = new NCPara(":HM04015", NpgsqlDbType.Numeric, 0, HM04015, 0);
        //    list.Add(para);
        //    para = new NCPara(":HM04016", NpgsqlDbType.Numeric, 6, HM04016, 0);
        //    list.Add(para);
        //    para = new NCPara(":HM04017", NpgsqlDbType.Numeric, 6, HM04017, 0);
        //    list.Add(para);
        //    para = new NCPara(":HM04018", NpgsqlDbType.Numeric, 6, HM04018, 0);
        //    list.Add(para);
        //    para = new NCPara(":HM04019", NpgsqlDbType.Numeric, 6, HM04019, 0);
        //    list.Add(para);
        //    para = new NCPara(":HM04020", NpgsqlDbType.Numeric, 0, HM04020, 0);
        //    list.Add(para);
        //    para = new NCPara(":HM04021", NpgsqlDbType.Numeric, 6, HM04021, 0);
        //    list.Add(para);
        //    para = new NCPara(":HM04022", NpgsqlDbType.Numeric, 6, HM04022, 0);
        //    list.Add(para);
        //    para = new NCPara(":HM04023", NpgsqlDbType.Numeric, 6, HM04023, 0);
        //    list.Add(para);
        //    para = new NCPara(":HM04024", NpgsqlDbType.Numeric, 6, HM04024, 0);
        //    list.Add(para);
        //    para = new NCPara(":HM04025", NpgsqlDbType.Numeric, 6, HM04025, 0);
        //    list.Add(para);
        //    para = new NCPara(":HM04026", NpgsqlDbType.Numeric, 6, HM04026, 0);
        //    list.Add(para);
        //    para = new NCPara(":HM04027", NpgsqlDbType.Numeric, 6, HM04027, 0);
        //    list.Add(para);
        //    para = new NCPara(":HM04028", NpgsqlDbType.Numeric, 0, HM04028, 0);
        //    list.Add(para);
        //    para = new NCPara(":HM04030", NpgsqlDbType.Char, 20, HM04030, " ");
        //    list.Add(para);
        //    para = new NCPara(":HM04032", NpgsqlDbType.Char, 20, HM04032, " ");
        //    list.Add(para);
        //    para = new NCPara(":HM04035", NpgsqlDbType.Numeric, 6, HM04035);
        //    list.Add(para);
        //    para = new NCPara(":HM04036", NpgsqlDbType.Numeric, 6, HM04036);
        //    list.Add(para);
        //    para = new NCPara(":HM04037", NpgsqlDbType.Numeric, 6, HM04037);
        //    list.Add(para);
        //    para = new NCPara(":HM04038", NpgsqlDbType.Numeric, 6, HM04038);
        //    list.Add(para);
        //    para = new NCPara(":HM04039", NpgsqlDbType.Char, 4, HM04039, " ");
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
        //    StringBuilder strB1 = new StringBuilder();
        //    NCArrayList list = new NCArrayList();
        //    NCPara para = null;
        //    strB.Append("DELETE FROM HM04MAPM ");
        //    strB.Append("WHERE HM04001 = :HM04001 "); 
        //    strB.Append("AND HM04002 = :HM04002 ");
        //    strB1.Append("DELETE FROM HM14GUID ");
        //    strB1.Append("WHERE HM14001 = :HM04001 "); 
        //    strB1.Append("AND HM14002 = :HM04002 ");
        //    para = new NCPara(":HM04001", NpgsqlDbType.Char, 10, HM04001);
        //    list.Add(para);
        //    para = new NCPara(":HM04002", NpgsqlDbType.Char, 5, HM04002);
        //    list.Add(para);
        //     if (DataBase.ExecSql(strB.ToString(), list, connection, transaction))
        //    {
        //        return DataBase.ExecSql(strB1.ToString(), list, connection, transaction);
        //    }
        //    else
        //    {
        //        return false;
        //    }
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
        //    strB.Append("FROM HM04MAPM ");
        //    strB.Append("WHERE HM04001 = :HM04001 ");
        //    strB.Append("AND HM04002 = :HM04002 ");
        //    para = new NCPara(":HM04001", NpgsqlDbType.Char, 10, HM04001);
        //    list.Add(para);
        //    para = new NCPara(":HM04002", NpgsqlDbType.Char, 5, HM04002);
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

