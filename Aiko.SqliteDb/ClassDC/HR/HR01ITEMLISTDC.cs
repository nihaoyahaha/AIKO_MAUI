using DI.DiNetWinServiceObject;
using System.Collections;

namespace DI.DiNetWinServiceObject
{
    /// <summary>
    /// アイテムテーブルリモートサービス
    /// </summary>
    public class HR01ITEMLISTDC : CMObjectDC
    {
        // アイテムテーブルリスト
        private ArrayList _HR01ITEM = new ArrayList();
        // 検査結果テーブルリスト
        private ArrayList _HR02KSKK = new ArrayList();
        // 写真テーブルリスト
        private ArrayList _HR03ITEM = new ArrayList();
        // 検査履歴テーブルリスト
        private ArrayList _HR04KSHIS = new ArrayList();
        // 写真テーブル_並び順リスト
        private ArrayList _HR03005 = new ArrayList();
        // テーブル
        private ArrayList _TABLE = new ArrayList();

        /// <summary>
        /// アイテムテーブルリスト
        /// </summary>
        public ArrayList HR01ITEM
        {
            get { return _HR01ITEM; }
            set { _HR01ITEM = value; }
        }

        /// <summary>
        /// 検査結果テーブルリスト
        /// </summary>
        public ArrayList HR02KSKK
        {
            get { return _HR02KSKK; }
            set { _HR02KSKK = value; }
        }

        /// <summary>
        /// 写真テーブルタリスト
        /// </summary>
        public ArrayList HR03ITEM
        {
            get { return _HR03ITEM; }
            set { _HR03ITEM = value; }
        }

        /// <summary>
        /// 検査履歴テーブルリスト
        /// </summary>
        public ArrayList HR04KSHIS
        {
            get { return _HR04KSHIS; }
            set { _HR04KSHIS = value; }
        }

        /// <summary>
        /// 写真テーブル_並び順タリスト
        /// </summary>
        public ArrayList HR03005
        {
            get { return _HR03005; }
            set { _HR03005 = value; }
        }

        /// <summary>
        /// テーブル
        /// </summary>
        public ArrayList TABLE { get { return _TABLE; } set { _TABLE = value; } }

       
    }

}
