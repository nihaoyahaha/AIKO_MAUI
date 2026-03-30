using System.Runtime.Serialization;

namespace DI.DiNetWinServiceObject
{
	[KnownType(typeof(HC01CONTDC))]
	[KnownType(typeof(HM01KAISDC))]
	[KnownType(typeof(HM02OPERDC))]
	[KnownType(typeof(HM03PROJDC))]
	[KnownType(typeof(HM04MAPMDC))]
	[KnownType(typeof(HM04MAPMLISTDC))]
	[KnownType(typeof(HM05KAIMDC))]
	[KnownType(typeof(HM05KAIMLISTDC))]
	[KnownType(typeof(HM06BUIMDC))]
	[KnownType(typeof(HM06BUIMLISTDC))]
	[KnownType(typeof(HM07KOKUDC))]
	[KnownType(typeof(HM07KOKULISTDC))]
	[KnownType(typeof(HM08GRPMDC))]
	[KnownType(typeof(HM08GRPMLISTDC))]
	[KnownType(typeof(HM09PROCDC))]
	[KnownType(typeof(HM09PROCLISTDC))]
	[KnownType(typeof(HM10DANMDC))]
	[KnownType(typeof(HM10DANMLISTDC))]
	[KnownType(typeof(HM11MEMODC))]
	[KnownType(typeof(HM11MEMOLISTDC))]
	[KnownType(typeof(HM12FILEDC))]
	[KnownType(typeof(HM13KNKMDC))]
	[KnownType(typeof(HM13KNKMLISTDC))]
	[KnownType(typeof(HM14GUIDDC))]
	[KnownType(typeof(HM14GUIDDCLISTDC))]
	[KnownType(typeof(HM15OSUBDC))]
	[KnownType(typeof(HM16SHDIRDC))]
	[KnownType(typeof(HM16SHDIRLISTDC))]
	[KnownType(typeof(HM17VERSIONDC))]
	[KnownType(typeof(HM17VERSIONLISTDC))]
	[KnownType(typeof(HM19GUIDCOLORDC))]
	[KnownType(typeof(HM20GUIDHEADDC))]
	[KnownType(typeof(HR01ITEMDC))]
	[KnownType(typeof(HR01ITEMLISTDC))]
	[KnownType(typeof(HR02KSKKDC))]
	[KnownType(typeof(HR02KSKKLISTDC))]
	[KnownType(typeof(HR03SYASDC))]
	[KnownType(typeof(HR03SYASLISTDC))]
	[KnownType(typeof(HR04KSHISDC))]
	[KnownType(typeof(HR04KSHISLISTDC))]
	[KnownType(typeof(HR05KOKUMINFODC))]
	[KnownType(typeof(HR06SYASDELDC))]
	[KnownType(typeof(HR06SYASDELLISTDC))]
	[KnownType(typeof(HM22GENECONDC))]
	[KnownType(typeof(HM23BUNRUIDC))]
	[KnownType(typeof(HM26DEVRESDC))]

	public class CMObjectDC
    {
      
        // お知らせ
        private string _strNotice = "";
        // お知らせ備考
        private string _strComment = "";
        // お知らせの種類
        private int _nNoticeKind = 0;
        // 新規または変更
        private bool _bIsNew = true;

        /// <summary>
        /// 新規または変更
        /// </summary>
        public bool IsNew
        {
            get { return _bIsNew; }
            set { _bIsNew = value; }
        }

        /// <summary>
        /// お知らせ
        /// </summary>
        public string Notice
        {
            get { return _strNotice; }
            set { _strNotice = value; }
        }

        /// <summary>
        /// お知らせ備考
        /// </summary>
        public string NoticeComment
        {
            get { return _strComment; }
            set { _strComment = value; }
        }

        /// <summary>
        /// お知らせの種類
        /// </summary>
        public int NoticeKind
        {
            get { return _nNoticeKind; }
            set { _nNoticeKind = value; }
        }        

    }
}
