using DI.DiNetWinServiceObject;
using System.Collections;

namespace DI.DiNetWinServiceObject
{
    /// <summary>
    /// マップガイドマスターリモートサービス
    /// </summary>
    public class HM14GUIDDCLISTDC : CMObjectDC
    {
        // 部位マスタリスト
        private ArrayList _HM14GUID = new ArrayList();

        /// <summary>
        /// 部位マスタリスト
        /// </summary>
        public ArrayList HM14GUID
        {
            get { return _HM14GUID; }
            set { _HM14GUID = value; }
        }

      
    }
}

