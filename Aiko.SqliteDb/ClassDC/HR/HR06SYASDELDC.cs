using DI.DiNetWinServiceObject;

namespace DI.DiNetWinServiceObject
{
    public class HR06SYASDELDC : CMObjectDC
    {

        //工事コード
        public string HR06001 { get; set; } = string.Empty;
        //写真コード
        public string HR06002 { get; set; } = string.Empty;
        //アイテムコード
        public string HR06003 { get; set; } = string.Empty;
        //確認項目コード
        public string HR06004 { get; set; } = string.Empty;
        //作成日時
        public DateTime HR06005 { get; set; }
        //作成オペレータ
        public string HR06006 { get; set; } = string.Empty;
        //更新日時
        public DateTime HR06007 { get; set; }
        //更新オペレータ
        public string HR06008 { get; set; } = string.Empty;
        //同期日時
        public DateTime HR06009 { get; set; }
        //同期オペレータ
        public string HR06010 { get; set; } = string.Empty;
        // 変更フラグ
        public string HMCHANGE { get; set; } = string.Empty;
    }
}
