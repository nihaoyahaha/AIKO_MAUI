using DI.DiNetWinServiceObject;
using System.Collections;
using System.Data;

namespace DI.DiNetWinServiceObject
{
    /// <summary>
    /// 断面マスターリモートサービス
    /// </summary>
    public class HM10DANMLISTDC : CMObjectDC
    {
        // 断面マスター
        private DataTable _HM10DANM ;

        /// <summary>
        /// 断面マスター
        /// </summary>
        public DataTable HM10DANM
        {
            get { return _HM10DANM; }
            set { _HM10DANM = value; }
        }

        // 断面マスターリスト
        private ArrayList _HM10DANMLIST = new ArrayList();

        /// <summary>
        /// 断面マスターリスト
        /// </summary>
        public ArrayList HM10DANMLIST
        {
            get { return _HM10DANMLIST; }
            set { _HM10DANMLIST = value; }
        }
        private int _Num;

        public int Num
        {
            get { return _Num; }
            set { _Num = value; }
        }
    }
}

