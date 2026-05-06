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

        /// <summary>
        /// 断面マスター
        /// </summary>
        public DataTable HM10DANM { get; set; } = new DataTable();

        /// <summary>
        /// 断面マスターリスト
        /// </summary>
        public ArrayList HM10DANMLIST { get; set; } = new ArrayList();

        public int Num { get; set; }
    }
}
