using DI.DiNetWinServiceObject;

namespace DI.DiNetWinServiceObject
{
    /// <summary>
    /// コントロールファイル
    /// </summary>
    public class HC01CONTDC : CMObjectDC
    {
        /// <summary>
        /// コード
        /// </summary>
        private string _HC01001;
        /// <summary>
        /// 会社名
        /// </summary>
        private string _HC01002;
        /// <summary>
        /// 郵便番号
        /// </summary>
        private string _HC01003;
        /// <summary>
        /// 都道府県
        /// </summary>
        private string _HC01004;
        /// <summary>
        /// 市区町村
        /// </summary>
        private string _HC01005;
        /// <summary>
        /// 番地
        /// </summary>
        private string _HC01006;
        /// <summary>
        /// 電話番号
        /// </summary>
        private string _HC01007;
        /// <summary>
        /// FAX番号
        /// </summary>
        private string _HC01008;
        /// <summary>
        /// ファイルサーバタイプ
        /// </summary>
        private int _HC01009;
        /// <summary>
        /// ファイルサーバアドレス
        /// </summary>
        private string _HC01010;
        /// <summary>
        /// ファイルサーバユーザID
        /// </summary>
        private string _HC01011;
        /// <summary>
        /// ファイルサーバパスワード
        /// </summary>
        private string _HC01012;
        /// <summary>
        /// 図面ファイルフォル
        /// </summary>
        private string _HC01013;
        /// <summary>
        /// 作成日時
        /// </summary>
        private DateTime _HC01014;
        /// <summary>
        /// 作成オペレータ
        /// </summary>
        private string _HC01015;
        /// <summary>
        /// 更新日時
        /// </summary>
        private DateTime _HC01016;
        /// <summary>
        /// 更新オペレータ
        /// </summary>
        private string _HC01017;

		/// <summary>
		/// ファイルサーバポート
		/// </summary>
		private int _HC01020;

		/// <summary>
		/// ファイルサーバの暗号化
		/// </summary>
		private int _HC01021;

		/// <summary>
		/// コード
		/// </summary>
		public string HC01001 {
            get { return _HC01001; }
            set { _HC01001 = value; }
        }

        /// <summary>
        /// 会社名
        /// </summary>
        public string HC01002 {
            get { return _HC01002; }
            set { _HC01002 = value; }
        }

        /// <summary>
        /// 郵便番号
        /// </summary>
        public string HC01003 {
            get { return _HC01003; }
            set { _HC01003 = value; }
        }

        /// <summary>
        /// 都道府県
        /// </summary>
        public string HC01004 {
            get { return _HC01004; }
            set { _HC01004 = value; }
        }

        /// <summary>
        /// 市区町村
        /// </summary>
        public string HC01005 {
            get { return _HC01005; }
            set { _HC01005 = value; }
        }

        /// <summary>
        /// 番地
        /// </summary>
        public string HC01006 {
            get { return _HC01006; }
            set { _HC01006 = value; }
        }

        /// <summary>
        /// 電話番号
        /// </summary>
        public string HC01007 {
            get { return _HC01007; }
            set { _HC01007 = value; }
        }

        /// <summary>
        /// FAX番号
        /// </summary>
        public string HC01008 {
            get { return _HC01008; }
            set { _HC01008 = value; }
        }

        /// <summary>
        /// ファイルサーバタイプ
        /// </summary>
        public int HC01009 {
            get { return _HC01009; }
            set { _HC01009 = value; }
        }

        /// <summary>
        /// ファイルサーバアドレス
        /// </summary>
        public string HC01010 {
            get { return _HC01010; }
            set { _HC01010 = value; }
        }

        /// <summary>
        /// ファイルサーバユーザID
        /// </summary>
        public string HC01011 {
            get { return _HC01011; }
            set { _HC01011 = value; }
        }

        /// <summary>
        /// ファイルサーバパスワード
        /// </summary>
        public string HC01012 {
            get { return _HC01012; }
            set { _HC01012 = value; }
        }

        /// <summary>
        /// 図面ファイルフォル
        /// </summary>
        public string HC01013 {
            get { return _HC01013; }
            set { _HC01013 = value; }
        }
        /// <summary>
        /// 作成日時
        /// </summary>
        public DateTime  HC01014 {
            get { return _HC01014; }
            set { _HC01014 = value; }
        }
        /// <summary>
        /// 作成オペレータ
        /// </summary>
        public string HC01015 {
            get { return _HC01015; }
            set { _HC01015 = value; }
        }
        /// <summary>
        /// 更新日時
        /// </summary>
        public DateTime HC01016 {
            get { return _HC01016; }
            set { _HC01016 = value; }
        }
        /// <summary>
        /// 更新オペレータ
        /// </summary>
        public string HC01017 {
            get { return _HC01017; }
            set { _HC01017 = value; }
        }

		/// <summary>
		/// ファイルサーバポート
		/// </summary>
		public int HC01020
        {
            get { return _HC01020; }
            set { _HC01020 = value; }
        }

		/// <summary>
		/// ファイルサーバの暗号化
		/// </summary>
		public int HC01021
		{
			get { return _HC01021; }
			set { _HC01021 = value; }
		}
		/// <summary>
		///// コントロールファイルを更新する
		///// </summary>
		///// <param name="connection"></param>
		///// <param name="transaction"></param>
		///// <returns></returns>
		//public override bool UpdateTable(NpgsqlConnection connection, NpgsqlTransaction transaction) 
		//    {
		//    StringBuilder strB = new StringBuilder();
		//    NCArrayList list = new NCArrayList();
		//    NCPara para = null;

		//    // 新規の場合
		//    if (IsNew) 
		//     {
		//        // 指定したデータが存在しない
		//        if (IsExists(connection, transaction) == false)
		//            {
		//            strB.Append("Insert into HC01CONT ");
		//            strB.Append("(HC01001, HC01002, ");
		//            strB.Append("HC01003, HC01004, HC01005, HC01006, ");
		//            strB.Append("HC01007, HC01008, HC01009, HC01010, HC01011, HC01012, HC01013, ");
		//            strB.Append("HC01014, HC01015, HC01016, HC01017 ");
		//            strB.Append(") Values ( ");
		//            strB.Append(":HC01001, :HC01002,");
		//            strB.Append(":HC01003, :HC01004, :HC01005, :HC01006, ");
		//            strB.Append(":HC01007, :HC01008, :HC01009, :HC01010, :HC01011, :HC01012, :HC01013, ");
		//            strB.Append("CURRENT_TIMESTAMP, :HC01015, CURRENT_TIMESTAMP, :HC01017 ");
		//            strB.Append(") ");

		//        }
		//        else
		//        {
		//            return false;
		//        }
		//    }
		//    // 修正の場合
		//    else {
		//        strB.Append("Update HC01CONT Set ");
		//        strB.Append("HC01002 = :HC01002, ");
		//        strB.Append("HC01003 = :HC01003, ");
		//        strB.Append("HC01004 = :HC01004, ");
		//        strB.Append("HC01005 = :HC01005, ");
		//        strB.Append("HC01006 = :HC01006, ");
		//        strB.Append("HC01007 = :HC01007, ");
		//        strB.Append("HC01008 = :HC01008, ");
		//        strB.Append("HC01009 = :HC01009, ");
		//        strB.Append("HC01010 = :HC01010, ");
		//        strB.Append("HC01011 = :HC01011, ");
		//        strB.Append("HC01012 = :HC01012, ");
		//        strB.Append("HC01013 = :HC01013, ");
		//        strB.Append("HC01016 = CURRENT_TIMESTAMP, ");
		//        strB.Append("HC01017 = :HC01017 ");
		//        strB.Append("Where HC01001 = :HC01001 ");
		//    }

		//    para = new NCPara(":HC01001", NpgsqlDbType.Char, 3, _HC01001, " ");
		//    list.Add(para);
		//    para = new NCPara(":HC01002", NpgsqlDbType.Char, 32, _HC01002, " ");
		//    list.Add(para);
		//    para = new NCPara(":HC01003", NpgsqlDbType.Char, 10, _HC01003, " ");
		//    list.Add(para);
		//    para = new NCPara(":HC01004", NpgsqlDbType.Char, 32, _HC01004, " ");
		//    list.Add(para);
		//    para = new NCPara(":HC01005", NpgsqlDbType.Char, 32, _HC01005, " ");
		//    list.Add(para);
		//    para = new NCPara(":HC01006", NpgsqlDbType.Char, 32, _HC01006, " ");
		//    list.Add(para);
		//    para = new NCPara(":HC01007", NpgsqlDbType.Char, 13, _HC01007, " ");
		//    list.Add(para);
		//    para = new NCPara(":HC01008", NpgsqlDbType.Char, 13, _HC01008, " ");
		//    list.Add(para);
		//    para = new NCPara(":HC01009", NpgsqlDbType.Numeric, 0, _HC01009, 0);
		//    list.Add(para);
		//    para = new NCPara(":HC01010", NpgsqlDbType.Char, 64, _HC01010, " ");
		//    list.Add(para);
		//    para = new NCPara(":HC01011", NpgsqlDbType.Char, 24, _HC01011, " ");
		//    list.Add(para);
		//    para = new NCPara(":HC01012", NpgsqlDbType.Char, 24, _HC01012, " ");
		//    list.Add(para);
		//    para = new NCPara(":HC01013", NpgsqlDbType.Char, 64, _HC01013, " ");
		//    list.Add(para);
		//    para = new NCPara(":HC01015", NpgsqlDbType.Char, 20, _HC01015, " ");
		//    list.Add(para);
		//    para = new NCPara(":HC01017", NpgsqlDbType.Char, 20, _HC01017, " ");
		//    list.Add(para);

		//    return DataBase.ExecSql(strB.ToString(), list, connection, transaction);
		//}

		///// <summary>
		///// 指定したレコードの存在をチェックする
		///// </summary>
		///// <param name="connection"></param>
		///// <param name="transaction"></param>
		///// <returns></returns>
		//public override bool IsExists(NpgsqlConnection connection, NpgsqlTransaction transaction) {
		//    StringBuilder strB = new StringBuilder();
		//    NCArrayList list = new NCArrayList();
		//    NCPara para = null;
		//    strB.Append("Select Count(0) As Cnt ");
		//    strB.Append("From HC01CONT ");
		//    strB.Append("Where HC01001 = :HC01001 ");
		//    para = new NCPara(":HC01001", NpgsqlDbType.Char, 3, _HC01001);
		//    list.Add(para);
		//    object obj = new object();
		//    if (DataBase.ExecSql(strB.ToString(), list, ref obj, connection, transaction) &&
		//        obj != null && PFunc.ObjectToInt(obj) > 0) {
		//        return true;
		//    }

		//    return false;
		//}
	}
}