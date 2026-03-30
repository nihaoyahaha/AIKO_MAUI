using DI.DiNetWinServiceObject;

namespace DI.DiNetWinServiceObject
{
    public class HR06SYASDELDC : CMObjectDC
    {
        //工事コード
        private string _HR06001;
        //写真コード
        private string _HR06002;
        //アイテムコード
        private string _HR06003;
        //確認項目コード
        private string _HR06004;
        //作成日時
        private DateTime _HR06005;
        //作成オペレータ
        private string _HR06006;
        //更新日時
        private DateTime _HR06007;
        //更新オペレータ
        private string _HR06008;
        //同期日時
        private DateTime _HR06009;
        //同期オペレータ
        private string _HR06010;
        // 変更フラグ
        private string _HMCHANGE;

        //工事コード
        public string HR06001 { get => _HR06001; set => _HR06001 = value; }
        //写真コード
        public string HR06002 { get => _HR06002; set => _HR06002 = value; }
        //アイテムコード
        public string HR06003 { get => _HR06003; set => _HR06003 = value; }
        //確認項目コード
        public string HR06004 { get => _HR06004; set => _HR06004 = value; }
        //作成日時
        public DateTime HR06005 { get => _HR06005; set => _HR06005 = value; }
        //作成オペレータ
        public string HR06006 { get => _HR06006; set => _HR06006 = value; }
        //更新日時
        public DateTime HR06007 { get => _HR06007; set => _HR06007 = value; }
        //更新オペレータ
        public string HR06008 { get => _HR06008; set => _HR06008 = value; }
        //同期日時
        public DateTime HR06009 { get => _HR06009; set => _HR06009 = value; }
        //同期オペレータ
        public string HR06010 { get => _HR06010; set => _HR06010 = value; }
        // 変更フラグ
        public string HMCHANGE { get => _HMCHANGE; set => _HMCHANGE = value; }
    }
}
