using DI.DiNetWinServiceObject;
using System.ServiceModel;

namespace Aiko.SqliteDb
{
    [ServiceContract(Namespace = "http://DI.DiNetWinServiceObject")]
    public interface IUWPServiceAPI
    {
        [OperationContract]
        void SetConnectionString(string conn);
        [OperationContract]
        bool UpdateMasterTable(CMObjectDC objDC);
        [OperationContract]
        string GetMaxHR03002(string hr03001);
        [OperationContract]
        IList<HC01CONTDC> GetHC01CONTList();
        [OperationContract]
        IList<HM01KAISDC> GetHM01KAIScodeList();
        [OperationContract]
        IList<HM02OPERDC> GetHM02OPERcodeList();
        [OperationContract]
        IList<HM03PROJDC> GetHM03PROJcodeList(List<string> codeList);
        [OperationContract]
        IList<HM04MAPMDC> GetHM04MAPMListByCode(string hm04001);
        [OperationContract]
        IList<HM05KAIMDC> GetHM05KAIMListByCode(string hm05001);
        [OperationContract]
        IList<HM06BUIMDC> GetHM06BUIMListByCode(string hm06001);
        [OperationContract]
        IList<HM07KOKUDC> GetHM07KOKUListByCode(string hm07001);
        [OperationContract]
        IList<HM08GRPMDC> GetHM08GRPMListByCode(string hm08001);
        [OperationContract]
        IList<HM09PROCDC> GetHM09PROCListByCode(string hm09001);
        [OperationContract]
        IList<HM10DANMDC> GetHM10DANMListByCode(string hm10001);
        [OperationContract]

        //*****  12？    sunjiawei  20171010  start
        //*****
        IList<HM11MEMODC> GetHM11MEMOListByCode();
        [OperationContract]
        //*****  12？    sunjiawei  20171010  start

        IList<HM12FILEDC> GetHM12FILEListByCode(string hm12001, string downLoadTime);
        [OperationContract]
        IList<HM12FILEDC> GetHM12FILEKeyListByCode(string hm12001);

        [OperationContract]
        IList<HM13KNKMDC> GetHM13KNKMListByCode(string hm13001);
        [OperationContract]
        IList<HM14GUIDDC> GetHM14GUListByCode(string hm14001);

        //**sjw 20180502 start
        //**1462 タブレット版　　配筋確認画面修正
        [OperationContract]
        IList<HM16SHDIRDC> GetHM16SHDIRListByCode(string hm16001);
        //**sjw 20180502 end

        [OperationContract]
        IList<HM17VERSIONDC> GetHM17VERSIONList();

        [OperationContract]
        IList<HM22GENECONDC> GetHM22GENECONList();
        [OperationContract]
        IList<HM23BUNRUIDC> GetHM23BUNRUIListByCode(string hm23001);

        [OperationContract]
        IList<HM15OSUBDC> GetHM15OSUBListByCode(string hm15002);
        [OperationContract]
        int GetHR01ITEMListCount(string hr01001, string downLoadTime);
        [OperationContract]
        IList<HR01ITEMDC> GetHR01ITEMListByCode(string hr01001, string downLoadTime);
        [OperationContract]
        IList<HR01ITEMDC> GetHR01ITEMListByPage(string hr01001, string downLoadTime, int pageSize, int pageStart);
        [OperationContract]
        IList<HR01ITEMDC> GetHR01ITEMKeyListByCode(string hr01001);
        [OperationContract]
        IList<HR02KSKKDC> GetHR02KSKKListByCode(string hr02Arr);
        [OperationContract]
        int GetHR02KSKKListCount(string hr02Arr, string downLoadTime);
        [OperationContract]
        IList<HR02KSKKDC> GetHR02KSKKListByPage(string hr02Arr, string downLoadTime, int pageSize, int pageStart);
        [OperationContract]
        IList<HR02KSKKDC> GetHR02KSKKListByCode2(string hr02Arr, string downLoadTime);
        [OperationContract]
        IList<HR03SYASDC> GetHR03SYASKeyListByCode(string hr03001);

        [OperationContract]
        int GetHR03SYASListCount(string hr03Arr, string downLoadTime);
        [OperationContract]
        IList<HR03SYASDC> GetHR03SYASListByPage(string hr03Arr, string downLoadTime, int pageSize, int pageStart);
        [OperationContract]
        IList<HR03SYASDC> GetHR03SYASListByCode(string hr03Arr, string downLoadTime);
        [OperationContract]
        IList<HR04KSHISDC> GetHR04KSHISListByCode(string hr04Arr);
        [OperationContract]
        IList<HR05KOKUMINFODC> GetHR05KOKUMINFOListByCode(string hr05001);
        [OperationContract]
        IList<HR06SYASDELDC> GetHR06SYASDELListByCode(string hr06001);
        [OperationContract]
        string GetMaxHR04004ByCode(string hr04Arr);
        [OperationContract]
        bool UpdateHr02(List<HR02KSKKDC> hr02List);

        //**sjw 20180228 start 
        //**1365 IP設定するとき、IPとポートに対してチェックを行う
        [OperationContract]
        int IpPortIsTrue();
        //**sjw 20180228 end

        //**** #1505 xiezhi 20180614 start
        [OperationContract]
        IList<HM10DANMDC> GetHM10DANMKeyListByCode(string hm10001);

        [OperationContract]
        IList<HM10DANMDC> GetHM10DANMByCode(string hm10001, string hm10003);

        [OperationContract]
        string GetSystemDateTime();

        [OperationContract]
        IList<HM19GUIDCOLORDC> GetHM19GUIDCOLORListByCode(string hm19001);

        [OperationContract]
        IList<HM20GUIDHEADDC> GetHM20GUIDHEADListByCode(string hm20001);
        //**** #1505 xiezhi 20180614 end

        [OperationContract]
        bool CheckIP(string hm01001, string hm02001, string hm03001, string ip);

        [OperationContract]
        string GetIpAddress();

        [OperationContract]
        bool CheckUUID(HM26DEVRESDC hm26DC);
    }
}
