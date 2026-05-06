using DI.DiNetWinServiceObject;
using System.Collections;

namespace DI.DiNetWinServiceObject
{
    /// <summary>
    /// アイテムテーブルリモートサービス
    /// </summary>
    public class HR01ITEMLISTDC : CMObjectDC
    {

        /// <summary>
        /// アイテムテーブルリスト
        /// </summary>
        public ArrayList HR01ITEM { get; set; } = new ArrayList();

        /// <summary>
        /// 検査結果テーブルリスト
        /// </summary>
        public ArrayList HR02KSKK { get; set; } = new ArrayList();

        /// <summary>
        /// 写真テーブルタリスト
        /// </summary>
        public ArrayList HR03ITEM { get; set; } = new ArrayList();

        /// <summary>
        /// 検査履歴テーブルリスト
        /// </summary>
        public ArrayList HR04KSHIS { get; set; } = new ArrayList();

        /// <summary>
        /// 写真テーブル_並び順タリスト
        /// </summary>
        public ArrayList HR03005 { get; set; } = new ArrayList();

        /// <summary>
        /// テーブル
        /// </summary>
        public ArrayList TABLE { get; set; } = new ArrayList();

    }

}
