using Aiko.Common;
using Aiko.SqliteDb;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aiko.IServices.IServices;

public interface IMapViewService:IServiceBase
{
	public Task<ObservableCollection<ListItem>> GetHM05DataSource();
	public Task<(ObservableCollection<ListItem>, List<HM04MAPM> hm04List)> GetHM04DataSource(string mapCD);
	public Task<ObservableCollection<ListItem>> GetHM07DataSource(string areaCD);
	public Task<ObservableCollection<ListItem>> GetHM06DataSource(string mapCD, string areaCD);
	public Task<ObservableCollection<ListItem>> GetHM09DataSource();
	public Task<ObservableCollection<ListItem>> GetHM12DataSource();


    public Task<HM12FILE> GetHM12FILEcodeList(HM12FILE hm12);

	public Task<Dictionary<string, int>> GetHR02005(HR01ITEM hr01, string hm09002 = "");

    /// <summary>
    /// アイテムテーブルを取得する
    /// </summary>
    /// <param name="hr01"></param>
    /// <returns></returns>
    public Task<List<ITEMMETA>> GetHR01ITEMcodeList(HR01ITEM hr01);

    /// <summary>
    /// 採用写真枚数/写真枚数
    /// </summary>
    /// <param name="hm09002">工程code</param>
    /// <returns></returns>
    public Task<Dictionary<string, string>> GetPicCount(string hm09002);

    /// <summary>
    /// 工区多辺形情報を取得する
    /// </summary>
    /// <param name="hr05"></param>
    /// <returns></returns>
    public Task<List<HR05KOKUMINFO>> GetHR05KOKUMINFOList(HR05KOKUMINFO hr05);

    /// <summary>
    /// マップガイドマスターを取得する
    /// </summary>
    /// <param name="hm14"></param>
    /// <returns></returns>
    public Task<List<HM14GUIDANDHM20>> GetHM14GUIDCODEList(HM14GUID hm14);

    /// <summary>
    /// マップガイドヘッダマスター取得する
    /// </summary>
    /// <param name="mapCode"></param>
    /// <returns></returns>

    public Task<ObservableCollection<ListItem>> GetHM20GUIDHEADNUMList(string mapCode);

    /// <summary>
    /// マップガイドマスターを取得する
    /// </summary>
    /// <param name="mapCode"></param>
    /// <param name="hm14004"></param>
    /// <param name="hm14015"></param>
    /// <returns></returns>

    public Task<List<HM14GUIDANDHM20>> GetHM14GUIDCODEList(string mapCode, int hm14004, int hm14015);

    /// <summary>
    /// マップガイド色マスターを取得する
    /// </summary>
    /// <param name="mapCode"></param>
    /// <param name="hm19003"></param>
    /// <param name="hm19004"></param>
    /// <returns></returns>
    public Task<List<HM19GUIDCOLOR>> GetHM19GUIDCOLORList(string mapCode, int hm19003, int hm19004);
}
