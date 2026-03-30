using Aiko.Common;
using Aiko.IServices.IServices;
using Aiko.SqliteDb;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace Aiko.Services.Services;

public class MapViewService : BaseService<MapViewService>, IMapViewService
{
	public MapViewService(ServiceContext<MapViewService> context) 
		: base(context) {}

    /// <summary>
    /// aikoApp実行中のアプリケーションコンテキスト
    /// </summary>
    public AikoAppContext AppContext => AikoAppContext;

    public async Task<ObservableCollection<ListItem>> GetHM05DataSource()
	{
		ObservableCollection<ListItem> listItems = new();
		HM05KAIM hm05DC = new HM05KAIM();
		hm05DC.HM05001 = AikoAppContext.WorkCD;

		var result = await HkksDb.GETHM05LISTAsync(hm05DC);
		listItems.Add(new ListItem("階マスタ", ""));
		foreach (var item in result)
		{
			ListItem listItem = new ListItem(item.HM05003, item.HM05002);
			listItems.Add(listItem);
		}
		HM04MAPM hm04DC = new HM04MAPM();
		hm04DC.HM04001 = AikoAppContext.WorkCD;
        List<HM04MAPM> hm04List = await HkksDb.GetHM04MAPListAAsync(hm04DC);

		if (hm04List.Count > 0)
		{
			ListItem listItem = new ListItem("未設定", "0");
			listItems.Add(listItem);
		}

		return listItems;
	}

	public async Task<(ObservableCollection<ListItem>, List<HM04MAPM> hm04List)> GetHM04DataSource(string mapCD)
	{
		ObservableCollection<ListItem> listItems = new();
		listItems.Add(new ListItem("マップ", ""));
		if (string.IsNullOrEmpty(mapCD)) return (listItems,null);

		HM04MAPM hm04DC = new HM04MAPM();
		hm04DC.HM04001 = AikoAppContext.WorkCD;
		hm04DC.HM04039 = mapCD;

		List<HM04MAPM> result = new();

		if (hm04DC.HM04039.Equals("0"))
		{
			result = await HkksDb.GetHM04MAPListAAsync(hm04DC);
		}
		else
		{
			result = await HkksDb.GetHM04MAPListAsync(hm04DC);
		}
		listItems = new();
		listItems.Add(new ListItem("マップ詳細", ""));
		foreach (var item in result)
		{
			ListItem listItem = new ListItem(item.HM04003, item.HM04002);
			listItems.Add(listItem);
		}
		return (listItems,result);
	}

	public async Task<ObservableCollection<ListItem>> GetHM07DataSource(string areaCD)
	{
		ObservableCollection<ListItem> listItems = new();
		listItems.Add(new ListItem("工区", " "));
		if (string.IsNullOrWhiteSpace(areaCD)) return listItems;

		HM07KOKU hm07DC = new HM07KOKU();
		hm07DC.HM07001 = AikoAppContext.WorkCD;
		hm07DC.HM07004 = areaCD;

		var result = await HkksDb.GetHM07KOKUcodeListAsync(hm07DC);
		listItems = new();
		listItems.Add(new ListItem("工区", " "));
		foreach (var item in result)
		{
			ListItem listItem = new ListItem(item.HM07003, item.HM07002);
			listItems.Add(listItem);
		}
		return listItems;
	}

	//部位取值
	public async Task<ObservableCollection<ListItem>> GetHM06DataSource(string mapCD, string areaCD)
	{
		ObservableCollection<ListItem> listItems = new();
		listItems.Add(new ListItem("部　位", ""));

		List<HM06BUIM> result = new();
		if (!string.IsNullOrWhiteSpace(areaCD))
		{
			HR01ITEM hr01DC = new HR01ITEM();
			hr01DC.HR01001 = AikoAppContext.WorkCD;
			hr01DC.HR01007 = areaCD;
			result = await HkksDb.GetHM06BUIMByHM07Async(hr01DC);
		}
		else
		{
			HM06BUIM hm06DC = new HM06BUIM();
			hm06DC.HM06001 = AikoAppContext.WorkCD;
			result = await HkksDb.GETHM06BUIMCODELISTAsync(hm06DC, mapCD);
		}

		foreach (var item in result)
		{
			ListItem listItem = new ListItem(item.HM06003, item.HM06002);
			listItems.Add(listItem);
		}
		return listItems;
	}

	/// <summary>
	///  工程リストの初期化
	/// </summary>
	/// <param name="workCD"></param>
	/// <returns></returns>
	public async Task<ObservableCollection<ListItem>> GetHM09DataSource()
	{
		ObservableCollection<ListItem> listItems = new();
		listItems.Add(new ListItem("工程(すべて)", " "));

		HM09PROC hm09DC = new HM09PROC();
		hm09DC.HM09001 = AikoAppContext.WorkCD;

		var result = await HkksDb.GetHM09ListAsync(hm09DC, false);
		for (int iHm09 = 0; iHm09 < result.Count; iHm09++)
		{
			ListItem listItem = new ListItem(result[iHm09].HM09003, result[iHm09].HM09002);
			listItems.Add(listItem);
		}
		return listItems;
	}

	public async Task<ObservableCollection<ListItem>> GetHM12DataSource()
	{
		ObservableCollection<ListItem> listItems = new();
		HM23BUNRUI hm23DC = new HM23BUNRUI();
		hm23DC.HM23001 = AikoAppContext.WorkCD;

		var result = await HkksDb.GetHM12FILEClassAsync(hm23DC);
		listItems.Add(new ListItem("構造図", ""));
		foreach (var item in result)
		{
			ListItem listItem = new ListItem(item.HM23003, item.HM23002);
			listItems.Add(listItem);
		}
		return listItems;
	}


	public async Task<HM12FILE> GetHM12FILEcodeList(HM12FILE hm12) 
	{
        var result = await HkksDb.GetHM12FILEcodeListAsync(hm12);

        return result.FirstOrDefault();

    }

    /// <summary>
    /// 获取每个确认点的图标颜色状态
    /// </summary>
    /// <param name="hr01"></param>
    /// <param name="hm09002"></param>
    /// <returns></returns>
    public async Task<Dictionary<string, int>> GetHR02005(HR01ITEM hr01, string hm09002 = "")
    {
        Dictionary<string, int> m_dicHR02005 = new Dictionary<string, int>();
        List<ITEMKSKKRESULT> m_dicHR02005List = await HkksDb.GetHR02KSKKReultAsync(hr01, hm09002);
        if (m_dicHR02005List == null) return m_dicHR02005;

        foreach (var row in m_dicHR02005List)
        {
            // HR02005 取值
            // 0：未確認 1：不合格 2：対象外 3：合格 4：是正済

            string key = row.HR01003.Trim() + "-" + row.HR01019.Trim();

            int count = row.COUNT;
            int sum = row.SUM;
            int min = row.MIN;
            int max = row.MAX;

            int status = 2;
            int[] values = { 0, 1, 2, 3, 4 };

            for (int mask = 1; mask < (1 << values.Length); mask++)
            {
                int c = 0;
                int s = 0;
                int mn = int.MaxValue;
                int mx = int.MinValue;

                bool has0 = false;
                bool has1 = false;
                bool has2 = false;
                bool has3 = false;
                bool has4 = false;

                for (int i = 0; i < values.Length; i++)
                {
                    if ((mask & (1 << i)) == 0)
                        continue;

                    int v = values[i];
                    c++;
                    s += v;
                    mn = Math.Min(mn, v);
                    mx = Math.Max(mx, v);

                    switch (v)
                    {
                        case 0: has0 = true; break;
                        case 1: has1 = true; break;
                        case 2: has2 = true; break;
                        case 3: has3 = true; break;
                        case 4: has4 = true; break;
                    }
                }

                if (c != count || s != sum || mn != min || mx != max)
                    continue;

                //0:绿色; 1:红色; 2:白色; 3:黄色; 4:蓝色

                if (has1)
                {
                    // 不合格あり
                    status = 1;
                }
                else if (has0 && !has2 && !has3 && !has4)
                {
                    // 全て未確認
                    status = 2;
                }
                else if (has4)
                {
                    // 不合格なし、是正済あり
                    status = 4;
                }
                else if (!has0 && !has4 && (has2 || has3))
                {
                    // 合格と対象外以外なし
                    status = 0;
                }
                else
                {
                    // 不合格なし、合格や対象外や未確認あり
                    status = 3;
                }

                break;
            }

            m_dicHR02005[key] = status;
        }

        return m_dicHR02005;
    }

    /// <summary>
    /// アイテムテーブルを取得する
    /// </summary>
    /// <param name="hr01"></param>
    /// <returns></returns>
    public async Task<List<ITEMMETA>> GetHR01ITEMcodeList(HR01ITEM hr01)
	{
		return await HkksDb.GetHR01ITEMcodeListAsync(hr01);
    }

    /// <summary>
    /// 採用写真枚数/写真枚数
    /// </summary>
    /// <param name="hm09002">工程code</param>
    /// <returns></returns>
    public async Task<Dictionary<string, string>> GetPicCount(string hm09002)
    {
        Dictionary<string, string> dictionaryPicCount = new Dictionary<string, string>();
        //
        List<HR01ITEMPICCOUNT> picCount =await HkksDb.GetPicCountAsync(AikoAppContext.WorkCD, hm09002);

        if (picCount != null && picCount.Count > 0)
        {
            foreach (HR01ITEMPICCOUNT item in picCount)
            {
                int userPicCount = Convert.ToInt32(item.UserPiceCount);
                int allPicCount = Convert.ToInt32(item.AllPicCount);

                if (userPicCount + allPicCount == 0)
                {
                    dictionaryPicCount.Add(item.HR01003, "");
                }
                else
                {
                    dictionaryPicCount.Add(item.HR01003, "[" + userPicCount + "/" + allPicCount + "]");
                }

            }
        }

        return dictionaryPicCount;
    }

    /// <summary>
    /// 工区多辺形情報を取得する
    /// </summary>
    /// <param name="hr05"></param>
    /// <returns></returns>
    public async Task<List<HR05KOKUMINFO>> GetHR05KOKUMINFOList(HR05KOKUMINFO hr05) 
    {
        return await HkksDb.GetHR05KOKUMINFOListAsync(hr05);
    }

    /// <summary>
    /// マップガイドマスターを取得する
    /// </summary>
    /// <param name="hm14"></param>
    /// <returns></returns>
    public async Task<List<HM14GUIDANDHM20>> GetHM14GUIDCODEList(HM14GUID hm14) 
    {
        return await HkksDb.GetHM14GUIDCODEListAsync(hm14);
    }

    /// <summary>
    /// マップガイドヘッダマスター取得する
    /// </summary>
    /// <param name="mapCode"></param>
    /// <returns></returns>

    public async Task<ObservableCollection<ListItem>> GetHM20GUIDHEADNUMList(string mapCode) 
    {
        ObservableCollection<ListItem> listItems = new();

        HM20GUIDHEAD hm20 = new HM20GUIDHEAD();
        // 工事コード
        hm20.HM20001 = AikoAppContext.WorkCD;
        // マップコード
        hm20.HM20002 = mapCode;

        var result = await HkksDb.GetHM20GUIDHEADNUMListAsync(hm20);

        if (result != null && result.Count > 0)
        {
            foreach (var item in result)
            {
                ListItem listItem = new ListItem(item.HM20004, item.HM20003.ToString());
                listItems.Add(listItem);
            }
        }
        else
        {
            ListItem listItem = new ListItem("1", "1");
            listItems.Add(listItem);
        }

        return listItems;
 
    }

    /// <summary>
    /// マップガイドマスターを取得する
    /// </summary>
    /// <param name="mapCode"></param>
    /// <param name="hm14004"></param>
    /// <param name="hm14015"></param>
    /// <returns></returns>

    public async Task<List<HM14GUIDANDHM20>> GetHM14GUIDCODEList(string mapCode,int hm14004, int hm14015) 
    {
        // マップガイドマスターリモートサービス
        HM14GUID hm14DC = new HM14GUID();
        // 工事コード
        hm14DC.HM14001 = AikoAppContext.WorkCD;
        // マップコード
        hm14DC.HM14002 = mapCode;
        // ガイドタイプ
        hm14DC.HM14004 = hm14004;

        hm14DC.HM14015 = hm14015;

        return await HkksDb.GetHM14GUIDCODEListAsync(hm14DC);
    }

    /// <summary>
    /// マップガイド色マスターを取得する
    /// </summary>
    /// <param name="mapCode"></param>
    /// <param name="hm19003"></param>
    /// <param name="hm19004"></param>
    /// <returns></returns>
    public async Task<List<HM19GUIDCOLOR>> GetHM19GUIDCOLORList(string mapCode, int hm19003, int hm19004) 
    {
        HM19GUIDCOLOR hm19 = new HM19GUIDCOLOR();
        hm19.HM19001 = AikoAppContext.WorkCD;
        hm19.HM19002 = mapCode;
        hm19.HM19003 = hm19003;
        hm19.HM19004 = hm19004;

        return await HkksDb.GetHM19GUIDCOLORListAsync(hm19);
    }


}
