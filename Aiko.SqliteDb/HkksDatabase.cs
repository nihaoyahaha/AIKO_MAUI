using Microsoft.Extensions.Logging;
using Microsoft.Maui.Storage;
using SQLite;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
namespace Aiko.SqliteDb;

public class HkksDatabase
{
	private const string _databaseFileName = "hkks.db";
	private const SQLiteOpenFlags _flags =
		SQLiteOpenFlags.ReadWrite |
		SQLiteOpenFlags.Create |
		SQLiteOpenFlags.SharedCache;
	private readonly ILogger<HkksDatabase> _logger;
	private SQLiteAsyncConnection _db;
	public SQLiteAsyncConnection Db
	{
		get => _db;
	}

	public HkksDatabase(ILogger<HkksDatabase> logger)
	{
		_logger = logger;
	}

	public async Task InitAsync()
	{
		try
		{
			string dbPath = Path.Combine(FileSystem.AppDataDirectory, _databaseFileName);

			if (!File.Exists(dbPath))
			{
#if WINDOWS
                using var stream = Assembly.Load("Aiko.UI").GetManifestResourceStream(_databaseFileName);

                using var reader = new StreamReader(stream);

                using (MemoryStream memoryStream = new())
                {
                    stream.CopyTo(memoryStream);
                    File.WriteAllBytes(dbPath, memoryStream.ToArray());
                }
#else
				// 使用 MAUI 内置的文件系统接口打开 Resources/Raw 下的文件 // 此时路径就是你在 LogicalName 里定义的 "hkks.db" 
				using var stream = await FileSystem.OpenAppPackageFileAsync(_databaseFileName);
				using var newFile = File.Create(dbPath);
				await stream.CopyToAsync(newFile);
#endif

			}
			_db = new(dbPath, _flags);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
		}
	}

	#region 会社マスターを取得する
	/// <summary>
	/// 会社マスターを取得する
	/// </summary>
	/// <returns></returns>
	public async Task<List<HM01KAIS>> GetHM01KAIScodeListAsync()
	{
		try
		{
			return await _db.Table<HM01KAIS>()
				.OrderBy(x => x.HM01001)
				.ToListAsync();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return new List<HM01KAIS>();
		}

	}
	#endregion

	#region マップコードを取得する
	/// <summary>
	/// マップコードを取得する
	/// </summary>
	/// <returns></returns>
	public async Task<List<HM04MAPM>> GetHM04MAPMcodeListAsync(string hm04001)
	{
		try
		{
			return await _db.Table<HM04MAPM>()
				  .Where(hm04 => hm04.HM04001 == hm04001)
				  .OrderBy(hm04 => hm04.HM04006)
				  .ToListAsync();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return new List<HM04MAPM>();
		}
	}

	/// <summary>
	/// マップコードを取得する
	/// </summary>
	/// <returns></returns>
	public async Task<List<HM04MAPM>> GetHM04MAPListAAsync(HM04MAPM hm04DC)
	{
		try
		{
			StringBuilder query = new();
			List<string> parms = new();
			query.Append("SELECT A.HM04001, A.HM04002, A.HM04003, A.HM04004, A.HM04005, ");
			query.Append("A.HM04006, A.HM04007, A.HM04008, A.HM04009, A.HM04010, ");
			query.Append(" A.HM04011, A.HM04012 , A.HM04013 , A.HM04014, A.HM04015,  ");
			query.Append(" A.HM04016, A.HM04017 , A.HM04018 , A.HM04019, A.HM04020,  ");
			query.Append(" A.HM04021, A.HM04022 , A.HM04023 , A.HM04024, A.HM04025,  ");
			query.Append(" A.HM04026, A.HM04027 , A.HM04028 , A.HM04029, A.HM04030,  ");
			query.Append(" A.HM04031, A.HM04032 , A.HM04033 , A.HM04034, A.HM04035,  ");
			query.Append(" A.HM04036, A.HM04037 , A.HM04038 , A.HM04039, A.HM04040, A.HM04041,A.HM04042 ");
			query.Append("FROM HM04MAPM A  ");
			query.Append("WHERE A.HM04001= ? AND ( Trim(A.HM04039)='0' OR Trim(A.HM04039)='')");
			query.Append(" ORDER BY  A.HM04006 ");

			parms.Add(hm04DC.HM04001);
			return await _db.QueryAsync<HM04MAPM>(query.ToString(), parms.ToArray());
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return new List<HM04MAPM>();
		}
	}


	/// <summary>
	/// マップコードを取得する
	/// </summary>
	/// <returns></returns>
	public async Task<List<HM04MAPM>> GetHM04MAPListAsync(HM04MAPM hm04DC)
	{
		try
		{
			return await _db.Table<HM04MAPM>()
				.Where(hm04 => hm04.HM04001 == hm04DC.HM04001 && hm04.HM04039 == hm04DC.HM04039)
				.OrderBy(hm04 => hm04.HM04006)
				.ToListAsync();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return new List<HM04MAPM>();
		}

	}
	#endregion

	#region アイテムテーブルを取得する
	/// <summary>
	/// アイテムテーブルを取得する
	/// </summary>
	/// <returns></returns>
	public async Task<List<ITEMMETA>> GetHR01ITEMcodeListAsync(HR01ITEM hr01DC)
	{
		try
		{
			List<string> parms = new();
			string queryStr = $@"
               SELECT A.HR01001, A.HR01002, A.HR01003, A.HR01004, A.HR01005,A.HR01006, A.HR01007, 
               (case when G.hm04007 > 0 then A.HR01008 - G.hm04007 else  A.HR01008 end) as HR01008, 
               (case when G.hm04008 > 0 then A.HR01009 - G.hm04008 else A.HR01009 end) as HR01009, 
                A.HR01010, A.HR01011, A.HR01012 , A.HR01013 , A.HR01014, A.HR01015,  
                A.HR01016, A.HR01017 , A.HR01018 , A.HR01019, A.HR01020,  
                A.HR01021, A.HR01022, B.HM10004, C.HM07003, D.HM06016, 
               (case when G.hm04007 > 0 then C.HM07014 - G.hm04007 else C.HM07014 end) as HM07014, 
               (case when G.hm04008 > 0 then C.HM07015 - G.hm04008 else C.HM07015 end) as HM07015  
               FROM HR01ITEM A  
            
               LEFT OUTER JOIN HM10DANM B 
               ON A.HR01001 = B.HM10001 
               AND A.HR01020 = B.HM10003 
               LEFT OUTER JOIN HM07KOKU C 
               ON A.HR01001 = C.HM07001 
               AND A.HR01007 = C.HM07002 
               LEFT OUTER JOIN HM06BUIM D 
               ON A.HR01001 = D.HM06001 
               AND A.HR01019 = D.HM06002 
            
               LEFT JOIN hm04mapm G ON A.hr01001=G.hm04001 and A.hr01002=G.hm04002 
               WHERE A.HR01001= ?
               AND A.HR01002= ? ";

			parms.Add(hr01DC.HR01001);
			parms.Add(hr01DC.HR01002);

			if (!string.IsNullOrEmpty(hr01DC.HR01007))
			{
				queryStr += "AND A.HR01007= ? ";
				parms.Add(hr01DC.HR01007);
			}
			if (!string.IsNullOrEmpty(hr01DC.HR01019))
			{
				queryStr += "AND A.HR01019= ? ";
				parms.Add(hr01DC.HR01019);
			}
			queryStr += " ORDER BY  A.HR01003 ";
			return await _db.QueryAsync<ITEMMETA>(queryStr, parms.ToArray());

		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return new List<ITEMMETA>();
		}

	}
	#endregion

	#region 部位アイコンリソースファイルを取得する
	/// <summary>
	/// 部位アイコンリソースファイルを取得する
	/// </summary>
	/// <returns></returns>
	public async Task<List<HM06BUIM>> GetHM06016Async(HM06BUIM hm06DC)
	{
		try
		{
			return await _db.Table<HM06BUIM>()
				.Where(hm06 => hm06.HM06001 == hm06DC.HM06001 && hm06.HM06002 == hm06DC.HM06002)
				.ToListAsync();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return new List<HM06BUIM>();
		}
	}
	#endregion

	#region 判定を取得する 指定MAP下所有的确认点结果
	/// <summary>
	/// 判定を取得する 指定MAP下所有的确认点结果
	/// </summary>
	/// <param name="hr01"></param>
	/// <param name="hr09002">工程ID</param>
	/// <returns></returns>
	public async Task<List<ITEMKSKKRESULT>> GetHR02KSKKReultAsync(HR01ITEM hr01, string hr09002)
	{
		try
		{
			StringBuilder query = new();
			List<string> parms = new();

			query.Append("SELECT RE.HR01003, RE.HR01019, COUNT(RE.HR02005N) as \"COUNT\", SUM(RE.HR02005N) as \"SUM\", MIN(RE.HR02005N) as \"MIN\", MAX(RE.HR02005N) as \"MAX\" ");
			query.Append("FROM (SELECT R01.HR01001, R01.HR01003, R01.HR01019, COALESCE(R02.HR02005, 0) AS HR02005N FROM HR01ITEM R01 ");
			query.Append("INNER JOIN HM13KNKM M13 ON R01.HR01001 = M13.HM13001 AND R01.HR01019 = M13.HM13002 ");
			query.Append("INNER JOIN HR02KSKK R02 ON R01.HR01001 = R02.HR02001 AND R01.HR01003 = R02.HR02002 AND M13.HM13004 = R02.HR02003 ");
			query.Append("WHERE R01.HR01001 = ? AND R01.HR01002 = ? AND R01.HR01004 = 0 AND M13.HM13014 = 0 ");

			parms.Add(hr01.HR01001);
			parms.Add(hr01.HR01002);
			//工程
			if (!string.IsNullOrEmpty(hr09002))
			{
				query.Append("AND M13.HM13003 = ? ");
				parms.Add(hr09002);
			}
			query.Append("GROUP BY R01.HR01001, R01.HR01003, R01.HR01019, HR02005N) AS RE GROUP BY RE.HR01003,RE.HR01019 ORDER BY RE.HR01003 ");
			return await _db.QueryAsync<ITEMKSKKRESULT>(query.ToString(), parms.ToArray());

		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return new List<ITEMKSKKRESULT>();
		}
	}
	#endregion

	#region 判定を取得する
	/// <summary>
	/// 判定を取得する
	/// </summary>
	/// <param name="hr02DC"></param>
	/// <returns></returns>
	public async Task<List<HR02KSKK>> GetHR02KSKKAsync(HR02KSKK hr02DC, string hr01019)
	{
		try
		{
			StringBuilder query = new();

			query.Append("SELECT B.HM13004, A.HR02005 FROM HR02KSKK A ");
			query.Append("INNER JOIN HM13KNKM B ON A.HR02001 = B.HM13001 AND A.HR02003 = B.HM13004 ");
			query.Append("WHERE 1 = 1 AND A.HR02001 = '" + hr02DC.HR02001 + "' AND B.HM13014 = 0 AND B.HM13002 = '" + hr01019 + "' AND A.HR02002 = '" + hr02DC.HR02002 + "' ");
			query.Append("UNION ");
			query.Append("SELECT DISTINCT D.HM13004, 0 AS HR02005 FROM HM13KNKM D ");
			query.Append("LEFT JOIN HR02KSKK E ON E.HR02001 = D.HM13001 AND E.HR02003 = D.HM13004 ");
			query.Append("WHERE 1 = 1 AND D.HM13001 = '" + hr02DC.HR02001 + "' AND D.HM13014 = 0 AND D.HM13002 = '" + hr01019 + "' ");
			query.Append("AND D.HM13004 NOT IN ( ");
			query.Append("SELECT D.HM13004 FROM HR02KSKK E ");
			query.Append("INNER JOIN HM13KNKM D ON E.HR02001 = D.HM13001 AND E.HR02003 = D.HM13004 ");
			query.Append("WHERE 1 = 1 AND E.HR02001 = '" + hr02DC.HR02001 + "' AND D.HM13014 = 0 AND D.HM13002 = '" + hr01019 + "' AND E.HR02002 = '" + hr02DC.HR02002 + "') ");

			return await _db.QueryAsync<HR02KSKK>(query.ToString());
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return new List<HR02KSKK>();
		}
	}
	#endregion

	#region 断面名を取得する
	/// <summary>
	/// 断面名の取得
	/// </summary>
	/// <param name="hm10DC">断面マスター</param>
	/// <returns></returns>
	public async Task<List<HM10DANM>> GetHM10004Async(HM10DANM hm10DC)
	{
		try
		{
			StringBuilder query = new();

			query.Append("SELECT A.HM10004 ");
			query.Append("FROM HM10DANM A ");
			query.Append(" WHERE 1 = 1 ");
			query.Append(" AND A.HM10001 = '" + hm10DC.HM10001 + "'");
			query.Append(" AND A.HM10003 = '" + hm10DC.HM10003 + "'");
			return await _db.QueryAsync<HM10DANM>(query.ToString());
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return new List<HM10DANM>();
		}
	}
	#endregion

	#region ファイルを取得する
	/// <summary>
	/// ファイルを取得する
	/// </summary>
	/// <returns></returns>
	public async Task<List<HM12FILE>> GetHM12FILEcodeListAsync(HM12FILE hm12DC)
	{
		try
		{
			return await _db.Table<HM12FILE>()
				.Where(hm12 => hm12.HM12001 == hm12DC.HM12001 && hm12.HM12002 == hm12DC.HM12002)
				.OrderBy(hm12 => hm12.HM12001)
				.ThenBy(hm12 => hm12.HM12002)
				.ToListAsync();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return new List<HM12FILE>();
		}
	}
	#endregion

	#region 工区部位联动
	/// <summary>
	/// ファイルを取得する
	/// </summary>
	/// <returns></returns>
	public async Task<List<HM06BUIM>> GetHM06BUIMByHM07Async(HR01ITEM hr01DC)
	{
		try
		{
			StringBuilder query = new();
			List<string> parms = new();

			query.Append(" SELECT DISTINCT  A.HM06002, A.HM06003 ");
			query.Append(" FROM HM06BUIM A  ");
			query.Append(" LEFT JOIN HR01ITEM B ON A.HM06001 = B.HR01001 AND A.HM06002 = B.HR01019  ");
			query.Append(" WHERE 1=1 ");
			query.Append(" AND B.HR01001= ? ");
			query.Append(" AND B.HR01007= ? ");
			query.Append(" ORDER BY A.HM06002 ");
			parms.Add(hr01DC.HR01001);
			parms.Add(hr01DC.HR01007);

			return await _db.QueryAsync<HM06BUIM>(query.ToString(), parms.ToArray());
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return new List<HM06BUIM>();
		}
	}
	#endregion

	#region 部位マスターを取得する
	/// <summary>
	/// 部位マスターを取得する
	/// </summary>
	/// <returns></returns>
	public async Task<List<HM06BUIM>> GETHM06BUIMCODELISTAsync(HM06BUIM hm06DC, string hm04002)
	{
		try
		{
			StringBuilder query = new();
			List<string> parms = new();
			query.Append(" SELECT DISTINCT  A.HM06002, A.HM06003 ");
			query.Append(" FROM HM06BUIM A  ");
			query.Append(" LEFT JOIN HR01ITEM B ON A.HM06001=B.HR01001 and A.HM06002 = B.HR01019 ");

			query.Append(" WHERE A.HM06001= ? ");
			query.Append(" AND   B.HR01002= ? ");
			query.Append(" ORDER BY  A.HM06002 ");

			parms.Add(hm06DC.HM06001);
			parms.Add(hm04002);

			return await _db.QueryAsync<HM06BUIM>(query.ToString(), parms.ToArray());
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return new List<HM06BUIM>();
		}

	}
	#endregion

	#region 階マスターを取得する
	/// <summary>
	/// 階マスターを取得する
	/// </summary>
	/// <returns></returns>
	public async Task<List<HM05KAIMCOUNT>> GETHM05LISTAsync(HM05KAIM hm05DC)
	{
		try
		{
			StringBuilder query = new();
			List<string> parms = new();

			query.Append("SELECT A.HM05001, A.HM05002, A.HM05003, A.HM05004, A.HM05005,(SELECT COUNT(*) FROM hm04mapm B where B.hm04001 = A.HM05001 AND B.hm04039=A.HM05002) AS MapCount ");
			query.Append("FROM HM05KAIM A  ");
			query.Append("WHERE A.HM05001= ? AND A.HM05005=0");
			query.Append(" ORDER BY  A.HM05004 ");

			parms.Add(hm05DC.HM05001);
			return await _db.QueryAsync<HM05KAIMCOUNT>(query.ToString(), parms.ToArray());
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return new List<HM05KAIMCOUNT>();
		}
	}
	#endregion

	#region 工区コードを取得する
	/// <summary>
	/// 工区コードを取得する
	/// </summary>
	/// <returns></returns>
	public async Task<List<HM07KOKU>> GetHM07KOKUcodeListAsync(HM07KOKU hm07DC)
	{
		try
		{
			return await _db.Table<HM07KOKU>()
				.Where(hm07 => hm07.HM07001 == hm07DC.HM07001 && hm07.HM07004 == hm07DC.HM07004)
				.OrderBy(hm07 => hm07.HM07005)
				.ToListAsync();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return new List<HM07KOKU>();
		}
	}
	#endregion

	#region 部位コードを取得する
	/// <summary>
	/// 部位コードを取得する
	/// </summary>
	/// <returns></returns>
	public async Task<List<HM06BUIM>> GetHM06ListAsync(HM06BUIM hm06DC)
	{
		try
		{
			return await _db.Table<HM06BUIM>()
				.Where(hm06 => hm06.HM06001 == hm06DC.HM06001 && hm06.HM06009 == 0)
				.OrderBy(hm06 => hm06.HM06006)
				.ToListAsync();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return new List<HM06BUIM>();
		}
	}
	#endregion

	#region 工程抽出
	/// <summary>
	/// 工程抽出
	/// </summary>
	/// <returns></returns>
	public async Task<List<HM09PROC>> GetHM09PROCAsync(HM09PROC hm09DC)
	{
		try
		{
			return await _db.Table<HM09PROC>()
				.Where(hm09 => hm09.HM09005 == 0 && hm09.HM09001 == hm09DC.HM09001)
				.OrderBy(hm09 => hm09.HM09004)
				.ToListAsync();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return new List<HM09PROC>();
		}
	}
	#endregion

	#region 工区名を取得する
	/// <summary>
	/// 工区名を取得する
	/// </summary>
	/// <returns></returns>
	public async Task<List<HM07KOKU>> GETHM0707003Async(HM07KOKU hm07DC)
	{
		try
		{
			StringBuilder query = new StringBuilder();
			query.Append("SELECT A.HM07001, A.HM07002, A.HM07003, A.HM07004 ");
			query.Append("FROM HM07KOKU A  ");
			query.Append("WHERE A.HM07001 = '" + hm07DC.HM07001 + "'");
			query.Append(" AND A.HM07002 = '" + hm07DC.HM07002 + "'");
			query.Append(" ORDER BY  A.HM07005 ");
			return await _db.QueryAsync<HM07KOKU>(query.ToString());
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return new List<HM07KOKU>();
		}
	}
	#endregion

	#region 工区マスターを取得する
	/// <summary>
	/// 工区マスターを取得する
	/// </summary>
	/// <returns></returns>
	public async Task<List<HM07KOKU>> GETHM07KOKUCODELISTAsync(HM07KOKU hm07DC)
	{
		try
		{
			return await _db.Table<HM07KOKU>()
				.Where(hm07 => hm07.HM07001 == hm07DC.HM07001 && hm07.HM07004 == hm07DC.HM07004)
				.OrderBy(hm07 => hm07.HM07005)
				.ToListAsync();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return new List<HM07KOKU>();
		}
	}
	#endregion

	#region マップガイドマスターを取得する
	/// <summary>
	/// マップガイドマスターを取得する
	/// </summary>
	/// <returns></returns>
	public async Task<List<HM14GUIDANDHM20>> GetHM14GUIDCODEListAsync(HM14GUID hm14DC)
	{
		try
		{
			StringBuilder query = new StringBuilder();
			List<object> parms = new();

			query.Append("SELECT ");
			query.Append("A.HM14001,A.HM14002, A.HM14003,A.HM14004,A.HM14005,A.HM14006,A.HM14007,A.HM14008,A.HM14015, ");
			query.Append("COALESCE(B.HM20005, 0) HM20005, ");
			query.Append("COALESCE(B.HM20006, 0) HM20006, ");
			query.Append("COALESCE(B.HM20007, 0) HM20007, ");
			query.Append("COALESCE(B.HM20008, 0) HM20008, ");
			query.Append("COALESCE(B.HM20009, 0) HM20009 ");
			query.Append(" FROM hm14guid A ");
			query.Append("left join hm20guidhead B on A.HM14001 = B.HM20001 AND A.HM14002 = B.HM20002 AND A.HM14015 = B.HM20003 ");
			query.Append(" WHERE A.HM14001 = ? ");
			query.Append(" AND A.HM14002 = ? ");
			query.Append(" AND A.HM14004 = ? ");
			query.Append(" AND A.HM14015 = ? ");
			query.Append("ORDER BY A.HM14005 ");

			parms.Add(hm14DC.HM14001);
			parms.Add(hm14DC.HM14002);
			parms.Add(hm14DC.HM14004);
			parms.Add(hm14DC.HM14015);

			return await _db.QueryAsync<HM14GUIDANDHM20>(query.ToString(), parms.ToArray());
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return new List<HM14GUIDANDHM20>();
		}
	}

	public async Task<List<HM19GUIDCOLOR>> GetHM19GUIDCOLORListAsync(HM19GUIDCOLOR hm19Dc)
	{
		try
		{
			return await _db.Table<HM19GUIDCOLOR>()
				.Where(hm19 => hm19.HM19001 == hm19Dc.HM19001 && hm19.HM19002 == hm19Dc.HM19002)
				.Where(hm19 => hm19.HM19003 == hm19Dc.HM19003 && hm19.HM19004 == hm19Dc.HM19004)
				.ToListAsync();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return new List<HM19GUIDCOLOR>();
		}
	}

	public async Task<List<HM20GUIDHEAD>> GetHM20GUIDHEADNUMListAsync(HM20GUIDHEAD hm20DC)
	{
		try
		{
			return await _db.Table<HM20GUIDHEAD>()
				.Where(hm20 => hm20.HM20001 == hm20DC.HM20001 && hm20.HM20002 == hm20DC.HM20002)
				.OrderBy(hm20 => hm20.HM20003)
				.ToListAsync();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return new List<HM20GUIDHEAD>();
		}
	}
	#endregion

	#region 工区コードを取得する
	/// <summary>
	/// 工区コードを取得する
	/// </summary>
	/// <returns></returns>
	public async Task<List<HM02OPER>> GetHM02OPERcodeListAsync(HM02OPER hm02)
	{
		try
		{
			StringBuilder query = new();
			List<string> parms = new();
			query.Append("SELECT  A.HM02002,A.HM02004,A.HM02005,A.HM02015 ");
			query.Append("FROM HM02OPER A  ");
			query.Append("WHERE A.HM02001=? ");
			query.Append(" AND Trim(A.HM02003)=? ");
			query.Append(" AND Trim(A.HM02004)=? ");
			query.Append(" ORDER BY  A.HM02001 ");

			parms.Add(hm02.HM02001);
			parms.Add(hm02.HM02003);
			parms.Add(hm02.HM02004);
			return await _db.QueryAsync<HM02OPER>(query.ToString(), parms.ToArray());
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return null;
		}
	}
	#endregion

	#region 工区コードを取得する
	/// <summary>
	/// 工区コードを取得する
	/// </summary>
	/// <returns></returns>
	public async Task<List<HM02OPER>> GetHM02OPERcodeListAAsync(HM02OPER hm02DC)
	{
		try
		{
			StringBuilder query = new();
			List<string> parms = new();

			query.Append("SELECT  A.HM02001,A.HM02002,A.HM02005,A.HM02007 ");
			query.Append("FROM HM02OPER A  ");
			query.Append("WHERE A.HM02004= ? AND (A.HM02005=1 OR A.HM02005=2) AND A.HM02006=0 AND Trim(A.HM02015)= ? ");
			query.Append(" ORDER BY  A.HM02001 ");

			parms.Add(hm02DC.HM02004);
			parms.Add(hm02DC.HM02015);
			return await _db.QueryAsync<HM02OPER>(query.ToString(), parms.ToArray());

		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return new List<HM02OPER>();
		}

	}
	#endregion

	#region オペレータサブテーブルを取得する
	/// <summary>
	/// オペレータサブテーブルを取得する
	/// </summary>
	/// <returns></returns>
	public async Task<List<HM15OSUB>> GetHM15OSUBcodeListAAsync(HM15OSUB hm15DC)
	{
		try
		{
			return await _db.Table<HM15OSUB>()
				.Where(hm15 => hm15.HM15001 == hm15DC.HM15001 && hm15.HM15009 == hm15DC.HM15009)
				.OrderBy(hm15 => hm15.HM15002)
				.ToListAsync();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return new List<HM15OSUB>();
		}
	}
	#endregion

	#region 工事マスターを取得する
	/// <summary>
	/// 工事マスターを取得する
	/// </summary>
	/// <returns></returns>
	public async Task<List<HM03PROJ>> GetHM03PROJcodeListAsync(HM03PROJ hm03DC)
	{
		try
		{
			return await _db.Table<HM03PROJ>()
				.Where(hm03 => hm03.HM03001 == hm03DC.HM03001 && hm03.HM03005 == 0)
				.OrderBy(hm03 => hm03.HM03001)
				.ToListAsync();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return new List<HM03PROJ>();
		}
	}
	#endregion

	#region 工事マスターのリストを取得する
	/// <summary>
	/// 工事マスターのリストを取得する
	/// </summary>
	/// <returns></returns>
	public async Task<List<HM03PROJ>> GetHM03PROJListAsync()
	{
		try
		{
			return await _db.Table<HM03PROJ>()
				.Where(hm03 => hm03.HM03005 == 0)
				.OrderBy(hm03 => hm03.HM03001)
				.ToListAsync();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return new List<HM03PROJ>();
		}
	}

	/// <summary>
	/// 工事マスターのリストを取得する
	/// </summary>
	/// <param name="hm03004">会社コード</param>
	/// <returns></returns>
	public async Task<List<HM03PROJ>> GetHM03PROJListByHM03004Async(string hm03004)
	{
		try
		{
			return await _db.Table<HM03PROJ>()
				.Where(hm03 => hm03.HM03004 == hm03004 && hm03.HM03005 == 0)
				.OrderBy(hm03 => hm03.HM03001)
				.ToListAsync();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return new List<HM03PROJ>();
		}
	}
	#endregion

	#region 対象プロジェクトのリストを取得する(検査結果テーブル)
	/// <summary>
	/// 対象プロジェクトのリストを取得する(検査結果テーブル)
	/// </summary>
	/// <param name="strCode">工事コード</param>
	/// <returns></returns>
	public async Task<List<HR02KSKK>> GetHR02KSKKListAsync(List<string> strCode)
	{
		try
		{
			strCode.Add("0");
			StringBuilder query = new();
			var newCodes = strCode.Select(x => $"'{x}'").ToList();
			string str = string.Join(",", newCodes);

			query.Append("SELECT A.HR02001, A.HR02002, A.HR02003,A.HR02004,");
			query.Append("A.HR02005, A.HR02006, A.HR02007, A.HR02008, A.HR02009, ");
			query.Append("A.HR02010, A.HR02011, A.HR02012, A.HR02013, A.HR02014, ");
			query.Append("A.HR02015, A.HR02016, A.HR02017, A.HR02018, A.HR02019 ");
			query.Append("FROM HR02KSKK A  ");
			query.Append($"WHERE A.HR02001 IN ({str})");
			query.Append(" ORDER BY  A.HR02001 ");
			return await _db.QueryAsync<HR02KSKK>(query.ToString());
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return new List<HR02KSKK>();
		}
	}
	#endregion

	#region 対象プロジェクトのリストを取得する(検査履歴テーブル)
	/// <summary>
	/// 対象プロジェクトのリストを取得する(検査履歴テーブル)
	/// </summary>
	/// <param name="strCode">工事コード</param>
	/// <returns></returns>
	public async Task<List<HR04KSHIS>> GetHR04KSHISListAsync(List<string> strCode)
	{
		try
		{
			strCode.Add("0");
			var newCodes = strCode.Select(x => $"'{x}'").ToList();
			string str = string.Join(",", newCodes);

			StringBuilder query = new StringBuilder();
			query.Append("SELECT A.HR04001, A.HR04002, A.HR04003,A.HR04004,");
			query.Append("A.HR04005, A.HR04006, A.HR04007, A.HR04008, A.HR04009, ");
			query.Append("A.HR04010, A.HR04011, A.HR04012, A.HR04013, A.HR04014, ");
			query.Append("A.HR04015, A.HR04016 ");
			query.Append("FROM HR04KSHIS A  ");
			query.Append($"WHERE A.HR04001 IN ({str})");
			query.Append(" ORDER BY  A.HR04001 ");

			return await _db.QueryAsync<HR04KSHIS>(query.ToString());
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return new List<HR04KSHIS>();
		}
	}
	#endregion

	#region 対象プロジェクトのリストを取得する(写真テーブル)
	/// <summary>
	/// 対象プロジェクトのリストを取得する(写真テーブル)
	/// </summary>
	/// <param name="strCode">工事コード</param>
	/// <returns></returns>
	public async Task<List<HR03SYAS>> GetHR03SYASListAsync(List<string> strCode)
	{
		try
		{
			var newCodes = strCode.Select(x => $"'{x}'").ToList();
			string str = string.Join(",", newCodes);

			StringBuilder query = new StringBuilder();
			query.Append("SELECT A.HR03001, trim(A.HR03002) as HR03002, A.HR03003,A.HR03004,");
			query.Append("A.HR03005, A.HR03006, A.HR03007, A.HR03008, A.HR03009, ");
			query.Append("A.HR03010, A.HR03011, A.HR03012, A.HR03013, A.HR03014, ");
			query.Append("A.HR03015, A.HR03016 ");
			query.Append("FROM HR03SYAS A  ");
			query.Append($"WHERE  A.HR03001 IN ({str})");
			query.Append(" ORDER BY  A.HR03001 ");

			return await _db.QueryAsync<HR03SYAS>(query.ToString());
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return new List<HR03SYAS>();
		}
	}
	#endregion

	#region 対象プロジェクトのリストを取得する(写真テーブル)
	/// <summary>
	/// 対象プロジェクトのリストを取得する(写真テーブル)
	/// </summary>
	/// <param name="strCode">工事コード</param>
	/// <returns></returns>
	public async Task<List<HR03SYAS>> GetHR03SYASListUpAsync(List<string> strCode)
	{
		try
		{
			strCode.Add("0");
			var newCodes = strCode.Select(x => $"'{x}'").ToList();
			string str = string.Join(",", newCodes);

			StringBuilder query = new StringBuilder();
			query.Append("SELECT A.HR03001, trim(A.HR03002) as HR03002, A.HR03003,A.HR03004,");
			query.Append("A.HR03005, A.HR03006, A.HR03007, A.HR03008, A.HR03009, ");
			query.Append("A.HR03010, A.HR03011, A.HR03012, A.HR03013, A.HR03014, ");
			query.Append("A.HR03015, A.HR03016 ");
			query.Append("FROM HR03SYAS A  ");
			query.Append("WHERE ( HR03013 > HR03015 ");
			query.Append("OR HR03011 = HR03015 )");
			query.Append($" AND  A.HR03001 IN ({str})");
			query.Append(" ORDER BY  A.HR03001 ");
			return await _db.QueryAsync<HR03SYAS>(query.ToString());
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return new List<HR03SYAS>();
		}
	}
	#endregion

	#region 断面名を取得する
	/// <summary>
	/// 断面名の取得
	/// </summary>
	/// <param name="hm10DC">断面マスター</param>
	/// <returns></returns>
	public async Task<HC01CONT> GetHC01ListAsync(HC01CONT hc01DC)
	{
		try
		{
			var tableQuery = _db.Table<HC01CONT>();
			if (hc01DC != null && !string.IsNullOrEmpty(hc01DC.HC01001))
			{
				tableQuery = tableQuery.Where(hc01 => hc01.HC01001 == hc01DC.HC01001);
			}
			var list = await tableQuery
				 .OrderBy(hc01 => hc01.HC01001)
				 .ToListAsync();
			return list.Count > 0 ? list[0] : new HC01CONT();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return new HC01CONT();
		}
	}
	#endregion

	#region 同期・オペレータサブテーブルのリストを取得する
	/// <summary>
	/// 同期・オペレータサブテーブルのリストを取得する
	/// </summary>
	/// <returns></returns>
	public async Task<List<HM15OSUB>> GetHM15ListAsync(string strCode, string strCompanyCode)
	{
		try
		{
			var tableQuery = _db.Table<HM15OSUB>();
			if (!string.IsNullOrEmpty(strCode))
			{
				tableQuery = tableQuery.Where(hm15 => hm15.HM15001 == strCode && hm15.HM15009 == strCompanyCode);
			}
			return await tableQuery
				.OrderBy(hm15 => hm15.HM15001)
				.ToListAsync();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return new List<HM15OSUB>();
		}
	}
	#endregion

	#region 同期・対象プロジェクトのリストを取得する(検査結果テーブル)
	/// <summary>
	/// 同期・対象プロジェクトのリストを取得する(検査結果テーブル)
	/// </summary>
	/// <param name="strCode">工事コード</param>
	/// <returns></returns>
	public async Task<List<HR02KSKK>> GetHR02ListAsync(List<string> strCode)
	{
		try
		{
			strCode.Add("0");
			var newCodes = strCode.Select(x => $"'{x}'").ToList();
			string str = string.Join(",", newCodes);

			StringBuilder query = new StringBuilder();
			query.Append("SELECT A.HR02001, A.HR02002, A.HR02003,A.HR02004,");
			query.Append("A.HR02005, A.HR02006, A.HR02007, A.HR02008, A.HR02009, ");
			query.Append("A.HR02010, A.HR02011, A.HR02012, A.HR02013, A.HR02014, ");
			query.Append("A.HR02015, A.HR02016, A.HR02017, A.HR02018, A.HR02019 ");
			query.Append("FROM HR02KSKK A  ");
			query.Append("WHERE ( A.HR02015 > A.HR02017 or A.HR02013 = A.HR02017 )");
			query.Append($"AND A.HR02001 IN ({str})");
			query.Append(" ORDER BY  A.HR02001 , A.HR02002 , A.HR02003 ");

			return await _db.QueryAsync<HR02KSKK>(query.ToString());
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return new List<HR02KSKK>();
		}
	}

	/// <summary>
	/// 同期・対象プロジェクトのリストを取得する(検査結果テーブル)
	/// </summary>
	/// <returns></returns>
	public async Task<List<HR02KSKK>> GetHR02List_1Async(List<string> strCode)
	{
		try
		{
			strCode.Add("0");
			var newCodes = strCode.Select(x => $"'{x}'").ToList();
			string str = string.Join(",", newCodes);

			StringBuilder query = new StringBuilder();
			query.Append("SELECT A.HR02001, A.HR02002, A.HR02003,A.HR02004,");
			query.Append("A.HR02005, A.HR02006, A.HR02007, A.HR02008, A.HR02009, ");
			query.Append("A.HR02010, A.HR02011, A.HR02012, A.HR02013, A.HR02014, ");
			query.Append("A.HR02015, A.HR02016, A.HR02017, A.HR02018, A.HR02019 ");
			query.Append("FROM HR02KSKK A  ");
			query.Append($"WHERE A.HR02001 IN ({str})");
			query.Append(" ORDER BY  A.HR02001 , A.HR02002 , A.HR02003 ");
			return await _db.QueryAsync<HR02KSKK>(query.ToString());
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return new List<HR02KSKK>();
		}
	}



	#endregion

	#region 同期・対象プロジェクトのリストを取得する(検査履歴テーブル)
	/// <summary>
	/// 同期・対象プロジェクトのリストを取得する(検査履歴テーブル)
	/// </summary>
	/// <param name="strCode">工事コード</param>
	/// <returns></returns>
	public async Task<List<HR04KSHIS>> GetHR04ListAsync(HR02KSKK HR02)
	{
		try
		{
			return await _db.Table<HR04KSHIS>()
				.Where(hr04 => hr04.HR04001 == HR02.HR02001 && hr04.HR04002 == HR02.HR02002)
				.Where(hr04 => hr04.HR04003 == HR02.HR02003)
				.OrderBy(hr04 => hr04.HR04001)
				.ThenBy(hr04 => hr04.HR04002)
				.ThenBy(hr04 => hr04.HR04003)
				.ThenBy(hr04 => hr04.HR04004)
				.ToListAsync();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return new List<HR04KSHIS>();
		}
	}
	#endregion

	#region 同期・対象プロジェクトのリストを取得する(写真テーブル)
	/// <summary>
	/// 同期・対象プロジェクトのリストを取得する(写真テーブル)
	/// </summary>
	/// <param name="strCode">工事コード</param>
	/// <returns></returns>
	public async Task<List<HR03SYAS>> GetHR03ListAsync(string code, string downLoadTime, int hr03009)
	{
		try
		{
			StringBuilder query = new StringBuilder();
			query.Append("SELECT A.HR03001, TRIM(A.HR03002) as HR03002, A.HR03003,A.HR03004,");
			query.Append("A.HR03005, A.HR03006, A.HR03007, A.HR03008, A.HR03009, ");
			query.Append("A.HR03010, A.HR03011, A.HR03012, A.HR03013, A.HR03014, ");
			query.Append("A.HR03015, A.HR03016, A.HR03017, A.HR03018, A.HR03019, A.HR03020 ");
			query.Append("FROM HR03SYAS A  ");
			query.Append("WHERE  A.HR03001 = '" + code + "'");
			if (downLoadTime != "")
			{
				query.Append("AND A.HR03013> '" + downLoadTime + "'");
			}
			query.Append(" OR (A.HR03001 = '" + code + "' and A.HR03009>= " + hr03009 + ")");
			query.Append(" ORDER BY  A.HR03001 , A.HR03002 ");

			return await _db.QueryAsync<HR03SYAS>(query.ToString());
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return new List<HR03SYAS>();
		}
	}
	#endregion

	#region 同期・対象プロジェクトのリストを取得する(写真テーブル)
	/// <summary>
	/// 同期・対象プロジェクトのリストを取得する(写真テーブル)
	/// </summary>
	/// <param name="strCode">工事コード</param>
	/// <returns></returns>
	public async Task<List<HM12FILE>> GetHM12ListAsync(List<string> strCode)
	{
		try
		{
			strCode.Add("0");
			var newCodes = strCode.Select(x => $"'{x}'").ToList();
			string str = string.Join(",", newCodes);

			StringBuilder query = new StringBuilder();
			query.Append("SELECT A.HM12001, TRIM(A.HM12002) as HM12002, TRIM(A.HM12003) as HM12003,TRIM(A.HM12004) as HM12004,");
			query.Append("A.HM12005, A.HM12006, A.HM12007, A.HM12008, A.HM12009, ");
			query.Append("A.HM12010, A.HM12011 ");
			query.Append("FROM HM12FILE A  ");
			query.Append($"WHERE  A.HM12001 IN ({str})");
			query.Append(" ORDER BY  A.HM12001 , A.HM12002 ");

			return await _db.QueryAsync<HM12FILE>(query.ToString());
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return new List<HM12FILE>();
		}
	}
	#endregion

	#region 工程リストを取得する（非表示フラグ＝０）
	/// <summary>
	/// 工程リストを取得する（非表示フラグ＝０）
	/// </summary>
	/// <param name="hm09DC">検索条件データ</param>
	/// <returns>検索結果</returns>
	public async Task<List<HM09PROC>> GetHM09ListAsync(HM09PROC hm09DC, bool blFlag)
	{
		try
		{
			var tableQuery = _db.Table<HM09PROC>()
				.Where(hm09 => hm09.HM09001 == hm09DC.HM09001);
			if (blFlag)
			{
				tableQuery = tableQuery.Where(hm09 => hm09.HM09002 == hm09DC.HM09002);
			}
			return await tableQuery.Where(hm09 => hm09.HM09005 == 0)
				.OrderBy(hm09 => hm09.HM09004)
				.ToListAsync();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return new List<HM09PROC>();
		}
	}

	/// <summary>
	/// 工程リストを取得する（非表示フラグ＝０）
	/// </summary>
	/// <param name="hm09DC">検索条件データ</param>
	/// <returns>検索結果</returns>
	public async Task<List<HM09PROC>> GetHM09List_HM13002Async(HM09PROC hm09DC, string hm13002)
	{
		try
		{
			StringBuilder query = new StringBuilder();
			query.Append("SELECT A.HM09001, A.HM09002, A.HM09003, A.HM09004,A.HM09005 ");
			query.Append("FROM HM09PROC A ");
			query.Append("INNER JOIN(SELECT hm13001, HM13003 FROM hm13knkm WHERE HM13002 = '" + hm13002 + "' AND hm13014=0 GROUP BY hm13001,HM13003) B on A.hm09001 = B.hm13001 and A.HM09002 = B.HM13003 ");
			query.Append("WHERE 1 = 1 ");
			query.Append("AND A.HM09001 = '" + hm09DC.HM09001 + "'");
			query.Append("AND A.HM09005 = 0 ");
			query.Append("ORDER BY  A.HM09004");
			return await _db.QueryAsync<HM09PROC>(query.ToString());
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return new List<HM09PROC>();
		}
	}
	#endregion

	#region 階・グループリストを取得する。

	/// <summary>
	/// 階・グループリストの取得
	/// </summary>
	/// <param name="hm08DC">グループマスター</param>
	/// <returns></returns>
	public async Task<List<HM08GRPM>> GetHM08GRPMListAsync(HM08GRPM hm08DC)
	{
		try
		{
			StringBuilder query = new();
			List<string> parms = new();

			query.AppendLine(" SELECT HM08002, HM08003 ");
			query.AppendLine(" FROM HM08GRPM ");
			query.AppendLine("WHERE 1 = 1 ");
			query.AppendLine("AND HM08001 = ? ");
			query.AppendLine("AND HM08005 = ? ");
			query.AppendLine("AND HM08008 = 0 ");
			query.AppendLine("AND HM08004 NOT IN('1001','1002','1003','1004') ");
			query.AppendLine(" ORDER BY HM08006");

			parms.Add(hm08DC.HM08001);
			parms.Add(hm08DC.HM08005);
			return await _db.QueryAsync<HM08GRPM>(query.ToString(), parms.ToArray());
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return new List<HM08GRPM>();
		}
	}
	#endregion

	#region 配筋表断面コードの取得

	/// <summary>
	/// 配筋表コードの取得
	/// </summary>
	/// <param name="hm10DC">断面マスター</param>
	/// <returns></returns>
	public async Task<List<HM10DANM>> GetHM10ListAsync(String strCode, String strCodeMast)
	{
		try
		{
			StringBuilder query = new();
			query.Append("SELECT A.HM10003, A.HM10004 ");
			query.Append("FROM HM10DANM  A ");
			query.Append("INNER JOIN HM08GRPM B ");
			query.Append("ON A.HM10001 = B.HM08001 ");
			query.Append("AND A.HM10002 = B.HM08002 ");
			query.Append("INNER JOIN HM06BUIM  C ");
			query.Append("ON B.HM08001 = C.HM06001 ");
			query.Append("AND B.HM08005 = C.HM06002 ");
			query.Append("WHERE 1 = 1 ");
			query.Append("AND B.HM08004 = '1004' ");
			query.Append("AND A.HM10011 = 0 ");
			query.Append("AND A.HM10001 = '" + strCode + "' ");
			query.Append("AND C.HM06002 = '" + strCodeMast + "' ");

			return await _db.QueryAsync<HM10DANM>(query.ToString());
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return new List<HM10DANM>();
		}
	}
	#endregion

	#region 工区データを取得する
	/// <summary>
	/// 工区データの取得
	/// </summary>
	/// <returns></returns>
	public async Task<List<HM07KOKU>> GetHM07codeListAsync(HM07KOKU hm07DC)
	{
		try
		{
			return await _db.Table<HM07KOKU>()
				.Where(hm07 => hm07.HM07001 == hm07DC.HM07001)
				.Where(hm07 => hm07.HM07004 == hm07DC.HM07004)
				.Where(hm07 => hm07.HM07007 == 0)
				.OrderBy(hm07 => hm07.HM07005)
				.ToListAsync();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return new List<HM07KOKU>();
		}
	}
	#endregion

	#region 工区多辺形情報を取得する
	/// <summary>
	/// 工区多辺形情報を取得する
	/// </summary>
	/// <returns></returns>
	public async Task<List<HR05KOKUMINFO>> GetHR05KOKUMINFOListAsync(HR05KOKUMINFO hr05DC)
	{
		try
		{
			StringBuilder query = new StringBuilder();

			query.Append("SELECT HR05001, HR05002, HR05003, ");
			query.Append("(case when C.hm04007>0 then HR05004 - C.hm04007 else HR05004 end) as HR05004,");
			query.Append("(case when C.hm04008>0 then HR05005 - C.hm04008 else HR05005 end) as HR05005,");
			query.Append("HR05006, HR05007,HR05008,HR05009 FROM hr05kokuminfo A ");
			query.Append("LEFT JOIN hr01item B on A.HR05001 = B.HR01001 and A.HR05002 = B.HR01003 ");
			query.Append("LEFT JOIN hm04mapm C on B.HR01001 = C.HM04001 and B.HR01002 = C.HM04002 ");
			query.Append("WHERE 1 = 1 ");
			query.Append("AND HR05001 = '" + hr05DC.HR05001 + "' ");
			query.Append("AND HR05002 = '" + hr05DC.HR05002 + "' ");
			if (hr05DC.HR05003 > 0)
			{
				query.Append("AND HR05003 = '" + hr05DC.HR05003 + "' ");
			}
			return await _db.QueryAsync<HR05KOKUMINFO>(query.ToString());
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return new List<HR05KOKUMINFO>();
		}
	}
	#endregion

	/// <summary>
	/// 工区多辺形情報を更新する
	/// </summary>
	/// <param name="objDC"></param>
	/// <returns></returns>
	public async Task<bool> UpdateHR05KOKUMINFOListAsync(List<HR05KOKUMINFO> hr05List)
	{
		try
		{
			await _db.RunInTransactionAsync(tran =>
			{
				if (hr05List.Count > 0)
				{
					foreach (var item in hr05List)
					{
						tran.InsertOrReplace(item);
					}
				}
			});
			return true;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return false;
		}
	}

	/// <summary>
	/// 指定した工区多辺形情報を削除する
	/// </summary>
	/// <param name="objDC"></param>
	/// <returns></returns>
	public async Task<bool> DeleteHR05KOKUMINFOListAsync(List<HR05KOKUMINFO> hr05ListDL)
	{
		try
		{
			await _db.RunInTransactionAsync(tran =>
			{
				if (hr05ListDL.Count > 0)
				{
					foreach (var item in hr05ListDL)
					{
						tran.Delete(item);
					}
				}
			});
			return true;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return false;
		}
	}

	#region 断面コードを取得する
	/// <summary>
	/// 断面コードの取得
	/// </summary>
	/// <param name="hm10DC">断面マスター</param>
	/// <param name="intType">null：断面リストを取得する；null以外：断面コードを取得する</param>
	/// <returns></returns>
	public async Task<List<HM10DANM>> GetHM10DANMListAsync(HM10DANM hm10DC, string strDMCode)
	{
		try
		{
			var tableQuery = _db.Table<HM10DANM>()
				.Where(hm10 => hm10.HM10001 == hm10DC.HM10001);

			if (string.IsNullOrEmpty(strDMCode))
			{
				tableQuery = tableQuery.Where(hm10 => hm10.HM10002 == hm10DC.HM10002)
					.Where(hm10 => hm10.HM10011 == 0)
					.OrderBy(hm10 => hm10.HM10005);
			}
			else
			{
				tableQuery = tableQuery.Where(hm10 => hm10.HM10011 == 0)
					.Where(hm10 => hm10.HM10003 == strDMCode);
			}

			return await tableQuery.ToListAsync();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return new List<HM10DANM>();
		}
	}
	#endregion

	#region タブ_共通を取得する
	/// <summary>
	/// タブ_共通の取得
	/// </summary>
	/// <param name="hm06DC">部位マスター</param>
	/// <returns></returns>
	public async Task<List<HM06BUIM>> GetTabHm06ListAsync(HM06BUIM hm06DC)
	{
		try
		{
			StringBuilder query = new StringBuilder();
			query.Append("SELECT HM06001, HM06007, HM06008 ");
			query.Append("FROM HM06BUIM ");
			query.Append("WHERE HM06002 = '" + hm06DC.HM06002 + "'  ");
			return await _db.QueryAsync<HM06BUIM>(query.ToString());
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return new List<HM06BUIM>();
		}
	}
	#endregion

	#region タブ_特記を取得する
	/// <summary>
	/// タブ_特記の取得
	/// </summary>
	/// <param name="hm13DC">確認項目マスター</param>
	/// <returns></returns>
	public async Task<List<HM13KNKM>> GetTabHm13ListAsync(HM13KNKM hm13DC)
	{
		try
		{
			StringBuilder query = new StringBuilder();
			query.Append("SELECT HM13001, HM13013 ");
			query.Append("FROM HM13KNKM ");
			query.Append("WHERE HM13004 = '" + hm13DC.HM13004 + "' ");

			return await _db.QueryAsync<HM13KNKM>(query.ToString());
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return new List<HM13KNKM>();
		}
	}
	#endregion

	#region ファイルデータを取得する
	/// <summary>
	/// ファイルデータを取得する
	/// </summary>
	/// <returns></returns>
	public async Task<List<HM12FILE>> GetHM12FILEAsync(HM12FILE hm12DC)
	{
		try
		{
			return await _db.Table<HM12FILE>()
				.Where(hm12 => hm12.HM12001 == hm12DC.HM12001)
				.Where(hm12 => hm12.HM12002 == hm12DC.HM12002)
				.OrderBy(hm12 => hm12.HM12001)
				.ThenBy(hm12 => hm12.HM12002)
				.ToListAsync();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return new List<HM12FILE>();
		}
	}
	#endregion

	#region 部位マスターの命名規則を取得する
	/// <summary>
	/// 部位マスターの命名規則の取得
	/// </summary>
	/// <returns></returns>
	public async Task<List<HM06BUIM>> GetHM06TypeAsync(HM06BUIM hm06DC)
	{
		try
		{
			StringBuilder query = new StringBuilder();
			query.Append("SELECT HM06018 ");
			query.Append("FROM HM06BUIM ");
			query.Append("WHERE 1 = 1 ");
			query.Append("AND HM06001 = '" + hm06DC.HM06001 + "' ");
			query.Append("AND HM06002 = '" + hm06DC.HM06002 + "' ");
			query.Append("AND HM06009 = 0 ");

			return await _db.QueryAsync<HM06BUIM>(query.ToString());
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return new List<HM06BUIM>();
		}
	}
	#endregion

	#region 確認項目リストを取得する

	/// <summary>
	/// 確認項目リストの取得(HM13KNKM)
	/// </summary>
	/// <returns></returns>
	public async Task<List<HM13KNKM>> GetHM13ListAsync(HM13KNKM hm13DC, HR01ITEM hR01ITEM, bool blAdd)
	{
		try
		{
			StringBuilder query = new StringBuilder();

			query.Append(" SELECT a.HM13005 as HM13005, a.HM13009 as HM13009, a.HM13006 as HM13006, a.HM13003 as HM13003, ");
			query.Append(" a.HM13010 as HM13010, a.HM13011 as HM13011, a.HM13004 as HM13004, a.HM13012 as HM13012, a.HM13013 as HM13013,'ADD' AS CHANGE");
			query.Append(" FROM HM13KNKM a ");
			query.Append(" inner JOIN( SELECT * FROM HR02KSKK WHERE HR02002 = '" + hR01ITEM.HR01003 + "') b ");
			query.Append(" on a.hm13001 = b.HR02001 and a.hm13004 = b.hr02003");
			query.Append(" WHERE 1 = 1  ");
			query.Append(" AND a.HM13001='" + hm13DC.HM13001 + "'   ");
			query.Append(" AND a.HM13014 = 0 ");
			query.Append(" AND a.HM13002='" + hm13DC.HM13002 + "' ");
			if (!blAdd)
			{
				query.Append("AND a.HM13003='" + hm13DC.HM13003 + "' ");
			}
			query.Append("ORDER BY a.HM13006  ");

			return await _db.QueryAsync<HM13KNKM>(query.ToString());
		}

		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return new List<HM13KNKM>();
		}
	}

	/// <summary>
	/// 確認項目リストの取得
	/// </summary>
	/// <returns></returns>
	public async Task<List<HR02KSKK>> GetHR02ListAsync(HR02KSKK hr02DC)
	{
		try
		{
			return await _db.Table<HR02KSKK>()
				.Where(hr02 => hr02.HR02001 == hr02DC.HR02001)
				.Where(hr02 => hr02.HR02002 == hr02DC.HR02002)
				.ToListAsync();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return new List<HR02KSKK>();
		}
	}

	/// <summary>
	/// 確認項目リストの取得
	/// </summary>
	/// <returns></returns>
	public async Task<List<HM09PROC>> GetHM03ListAsync(HM09PROC hm09DC)
	{
		try
		{
			StringBuilder query = new StringBuilder();
			query.Append("SELECT A.HM09001, A.HM09002, A.HM09003, A.HM09004,A.HM09005 ");
			query.Append("FROM HM09PROC A ");
			query.Append("WHERE 1 = 1 ");
			query.Append("AND A.HM09001 = '" + hm09DC.HM09001 + "' ");
			query.Append("AND A.HM09002 = '" + hm09DC.HM09002 + "' ");
			query.Append("AND A.HM09005 = 0 ");
			query.Append("ORDER BY  A.HM09004");

			return await _db.QueryAsync<HM09PROC>(query.ToString());
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return new List<HM09PROC>();
		}
	}

	#endregion

	#region 検査項目で写真撮影が必要な場合項目の写真アイコンの色を変更したい

	public async Task<List<HR03TYPECOUNT>> GetHR03TYPECOUNTAsync(HR01ITEM hr01DC)
	{
		try
		{
			StringBuilder query = new StringBuilder();

			query.AppendLine(" select* from( ");
			query.AppendLine(" select A.HM13004, coalesce(B.IDCOUNT,0) IDCOUNT, 2 AS STYPE from HM13KNKM A ");
			query.AppendLine(" LEFT JOIN( ");
			query.AppendLine(" SELECT HR03004, COUNT(HR03002) IDCOUNT FROM HR01ITEM A ");
			query.AppendLine(" INNER JOIN HR03SYAS B ON A.HR01001 = B.HR03001 AND A.HR01003 = B.HR03003 ");
			query.AppendLine(" WHERE A.HR01001 = '" + hr01DC.HR01001 + "' AND A.HR01002 = '" + hr01DC.HR01002 + "' AND A.HR01003 <> '" + hr01DC.HR01003 + "' AND A.HR01019 = '" + hr01DC.HR01019 + "'  AND A.HR01007 = '" + hr01DC.HR01007 + "'  AND A.HR01020 = '" + hr01DC.HR01020 + "'  AND B.HR03006 = 2 ");
			query.AppendLine(" GROUP BY HR03004 ");
			query.AppendLine(" ) B on A.HM13004 = B.HR03004 ");
			query.AppendLine(" where hm13001 = '" + hr01DC.HR01001 + "' AND HM13002 = '" + hr01DC.HR01019 + "' AND HM13009 = 2 ");

			query.AppendLine(" UNION ");

			query.AppendLine(" select A.HM13004,coalesce(B.IDCOUNT, 0) IDCOUNT, 3 AS STYPE from HM13KNKM A ");
			query.AppendLine(" LEFT JOIN( ");
			query.AppendLine(" SELECT HR03004, COUNT(HR03002) IDCOUNT FROM HR01ITEM A ");
			query.AppendLine(" INNER JOIN HR03SYAS B ON A.HR01001 = B.HR03001 AND A.HR01003 = B.HR03003 ");
			query.AppendLine(" WHERE A.HR01001 = '" + hr01DC.HR01001 + "' AND A.HR01002 = '" + hr01DC.HR01002 + "' AND A.HR01003 <> '" + hr01DC.HR01003 + "' AND A.HR01007 = '" + hr01DC.HR01007 + "' AND B.HR03006 = 2 ");
			query.AppendLine(" GROUP BY HR03004 ");
			query.AppendLine(" ) B on A.HM13004 = B.HR03004 ");
			query.AppendLine(" where hm13001 = '" + hr01DC.HR01001 + "' AND HM13002 = '" + hr01DC.HR01019 + "' AND HM13009 = 3  ");

			query.AppendLine(" UNION ");

			query.AppendLine(" select A.HM13004,coalesce(B.IDCOUNT, 0) IDCOUNT, 4 AS STYPE from HM13KNKM A ");
			query.AppendLine(" LEFT JOIN( ");
			query.AppendLine(" SELECT HR03004, COUNT(HR03002) IDCOUNT FROM HR01ITEM A ");
			query.AppendLine(" INNER JOIN HR03SYAS B ON A.HR01001 = B.HR03001 AND A.HR01003 = B.HR03003 ");
			query.AppendLine(" WHERE A.HR01001 = '" + hr01DC.HR01001 + "' AND A.HR01002 = '" + hr01DC.HR01002 + "' AND A.HR01003 <> '" + hr01DC.HR01003 + "' AND B.HR03006 = 2 ");
			query.AppendLine(" GROUP BY HR03004 ");
			query.AppendLine(" ) B on A.HM13004 = B.HR03004 ");
			query.AppendLine(" where hm13001 = '" + hr01DC.HR01001 + "' AND HM13002 = '" + hr01DC.HR01019 + "' AND HM13009 = 4  ) A ");
			query.AppendLine(" order by A.STYPE,A.HM13004 ");

			return await _db.QueryAsync<HR03TYPECOUNT>(query.ToString());
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return new List<HR03TYPECOUNT>();
		}
	}
	#endregion

	#region 確認項目_写真を取得する

	/// <summary>
	/// 確認項目_写真の取得
	/// </summary>
	/// <param name="HR03DC">写真テーブル</param>
	/// <returns></returns>
	public async Task<List<HR03SYAS>> GetHR03PicAsync(HR03SYAS HR03DC)
	{
		try
		{
			StringBuilder query = new StringBuilder();
			query.Append("SELECT TRIM(HR03002) as HR03002, HR03004, HR03005, HR03006, HR03007, ");
			query.Append(" HR03008, HR03009, HR03010, HR03011, HR03012, HR03013, ");
			query.Append(" HR03014, HR03015, HR03016, HR03017, HR03018, HR03019, HR03020 ");
			query.Append("FROM HR03SYAS ");
			query.Append("WHERE 1 = 1 ");
			query.Append("AND HR03001 = '" + HR03DC.HR03001 + "' ");
			query.Append("AND HR03003 = '" + HR03DC.HR03003 + "' ");
			query.Append("AND HR03004 = '" + HR03DC.HR03004 + "' ");
			query.Append(" ORDER BY HR03005 ");

			return await _db.QueryAsync<HR03SYAS>(query.ToString());
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return new List<HR03SYAS>();
		}
	}
	#endregion

	#region 自動採番

	/// <summary>
	/// 写真コード自動採番
	/// </summary>
	/// <returns></returns>
	public async Task<List<HR03SYAS>> GetMaxHR03002Async(HR03SYAS hr03DC)
	{
		try
		{
			StringBuilder query = new StringBuilder();
			query.Append("SELECT * FROM( ");
			query.Append("SELECT substr ( A.HR03002 , 15 , 4 ) AS HR03002 ");
			query.Append("FROM HR03SYAS A ");
			query.Append("WHERE 1 = 1 ");
			query.Append("AND A.HR03001 = '" + hr03DC.HR03001 + "' AND A.HR03003 = '" + hr03DC.HR03003 + "'");
			query.Append(" UNION ");
			query.Append("SELECT substr ( A.HR06002 , 15 , 4 ) AS HR03002 ");
			query.Append("FROM HR06SYASDEL A ");
			query.Append("WHERE 1 = 1 ");
			query.Append("AND A.HR06001 = '" + hr03DC.HR03001 + "' AND A.HR06003 = '" + hr03DC.HR03003 + "') A ORDER BY A.HR03002 DESC ");
			return await _db.QueryAsync<HR03SYAS>(query.ToString());
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return new List<HR03SYAS>();
		}
	}

	/// <summary>
	/// 検査履歴テーブル自動採番
	/// </summary>
	/// <returns></returns>
	public async Task<int> GetMaxHR04004Async(HR04KSHIS hr04DC)
	{
		try
		{
			var result = await _db.Table<HR04KSHIS>()
				.Where(hr04 => hr04.HR04001 == hr04DC.HR04001)
				.Where(hr04 => hr04.HR04002 == hr04DC.HR04002)
				.Where(hr04 => hr04.HR04003 == hr04DC.HR04003)
				.ToListAsync();

            return result.Any() ? result.Max(hr04 => hr04.HR04004) : 0;

        }
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return 0;
		}
	}

	#endregion

	/// <summary>
	/// テーブルを更新する
	/// </summary>
	/// <param name="objDC"></param>
	/// <returns></returns>
	public async Task<bool> UpdateTableAsync(List<HR02KSKK> hr02List, List<HR03SYAS> hr03List, List<HR03SYAS> hr03ListDL, List<HR04KSHIS> hr04List, List<HR06SYASDEL> hr06List)
	{
		bool bReturn = true;
		try
		{
			await _db.RunInTransactionAsync(tran =>
			{
				if (hr02List != null && hr02List.Count > 0)
				{
					Updatehr02kskk(tran, hr02List);
				}
				if (hr03List != null && hr03List.Count > 0)
				{
					foreach (var item in hr03List)
					{
						tran.InsertOrReplace(item);
					}

				}
				if (hr04List!=null && hr04List.Count > 0)
				{
					foreach (var item in hr04List)
					{
						tran.InsertOrReplace(item);
					}

				}
				if (hr06List != null && hr06List.Count > 0)
				{
					foreach (var item in hr06List)
					{
						tran.InsertOrReplace(item);
					}

				}
				if (hr03ListDL != null)
				{
					for (int intCnt = 0; intCnt < hr03ListDL.Count; intCnt++)
					{
						tran.Delete(hr03ListDL[intCnt]);
					}
				}
			});
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			bReturn = false;
		}
		return bReturn;
	}

	public void Updatehr02kskk(SQLiteConnection tran, List<HR02KSKK> hr02List)
	{
		try
		{
			StringBuilder strBUpdate = new StringBuilder();

			if (hr02List != null)
			{
				foreach (HR02KSKK hr02DC in hr02List)
				{
					strBUpdate = new StringBuilder();
					List<object> ps = new List<object>();
					// 更新SQL文を作成する。
					strBUpdate.AppendLine("UPDATE HR02KSKK SET ");
					strBUpdate.AppendLine("HR02005 =?,");
					ps.Add(hr02DC.HR02005);
					// 0：未確認   確認日 確認者 メモ
					if (hr02DC.HR02005 == 0)
					{
						strBUpdate.AppendLine("HR02004 =?,");
						strBUpdate.AppendLine("HR02006 = ?,");
						strBUpdate.AppendLine("HR02007 = ?,");
						strBUpdate.AppendLine("HR02008 = ?,");
						strBUpdate.AppendLine("HR02009 = ?,");
						strBUpdate.AppendLine("HR02010 = ?,");
						strBUpdate.AppendLine("HR02011 = ?,");
						ps.Add(hr02DC.HR02004);
						ps.Add(hr02DC.HR02006);
						ps.Add(hr02DC.HR02007);
						ps.Add(hr02DC.HR02008);
						ps.Add(hr02DC.HR02009);
						ps.Add(hr02DC.HR02010);
						ps.Add(hr02DC.HR02011);
					}
					// 1：不合格　 指摘日 指摘者 メモ
					else if (hr02DC.HR02005 == 1)
					{
						strBUpdate.AppendLine("HR02006 = ?,");
						strBUpdate.AppendLine("HR02007 = ?,");
						strBUpdate.AppendLine("HR02008 = ?,");
						strBUpdate.AppendLine("HR02009 = ?,");
						strBUpdate.AppendLine("HR02010 = ?,");

						ps.Add(hr02DC.HR02006);
						ps.Add(hr02DC.HR02007);
						ps.Add(hr02DC.HR02008);
						ps.Add(hr02DC.HR02009);
						ps.Add(hr02DC.HR02010);
					}
					// 2：該当しない　 確認日 確認者 メモ
					else if (hr02DC.HR02005 == 2)
					{
						strBUpdate.AppendLine("HR02006 = ?,");
						strBUpdate.AppendLine("HR02007 = ?,");
						strBUpdate.AppendLine("HR02010 = ?,");

						ps.Add(hr02DC.HR02006);
						ps.Add(hr02DC.HR02007);
						ps.Add(hr02DC.HR02010);
					}
					// 3：合格　      確認日 確認者 
					else if (hr02DC.HR02005 == 3)
					{
						strBUpdate.AppendLine("HR02004 =?,");
						strBUpdate.AppendLine("HR02006 =?,");
						strBUpdate.AppendLine("HR02007 =?,");
						ps.Add(hr02DC.HR02004);
						ps.Add(hr02DC.HR02006);
						ps.Add(hr02DC.HR02007);
					}
					// 4：是正済      確認日 確認者 処理
					else if (hr02DC.HR02005 == 4)
					{
						strBUpdate.AppendLine("HR02004 =?,");
						strBUpdate.AppendLine("HR02006 = ?,");
						strBUpdate.AppendLine("HR02007 = ?,");
						strBUpdate.AppendLine("HR02011 = ?,");
						ps.Add(hr02DC.HR02004);
						ps.Add(hr02DC.HR02006);
						ps.Add(hr02DC.HR02007);
						ps.Add(hr02DC.HR02011);
					}

					strBUpdate.AppendLine("HR02012 = 0,");
					strBUpdate.AppendLine("HR02015 = datetime(CURRENT_TIMESTAMP,'localtime'),");
					strBUpdate.AppendLine("HR02016 = ?, ");
					strBUpdate.AppendLine("HR02019 = ? ");
					strBUpdate.AppendLine("WHERE HR02001 = ? ");
					strBUpdate.AppendLine("AND HR02002 = ? ");
					strBUpdate.AppendLine("AND HR02003 = ? ");

					ps.Add(hr02DC.HR02016);
					ps.Add(hr02DC.HR02019);
					ps.Add(hr02DC.HR02001);
					ps.Add(hr02DC.HR02002);
					ps.Add(hr02DC.HR02003);
					tran.Execute(strBUpdate.ToString(), ps.ToArray());
				}
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
		}
	}

	#region 工事名工事事務所
	/// <summary>
	/// 判定工事事務所
	/// </summary>
	/// <param name="hm03Proj"></param>
	/// <returns></returns>
	public async Task<List<HM03PROJ>> GetHM03PROJAsync(HM03PROJ hm03Proj)
	{
		try
		{
			StringBuilder query = new StringBuilder();
			query.Append("SELECT A.hm03002 ");
			query.Append(" ,B.hm22003 AS HM03003 ");
			query.Append("FROM HM03PROJ A ");
			query.Append("INNER JOIN hm22GENECON B ON A.HM03004 = B.HM22001 AND A.HM03013 = B.HM22002 ");
			query.Append("WHERE 1 = 1 ");
			query.Append("AND A.hm03001 = '" + hm03Proj.HM03001 + "'");
			return await _db.QueryAsync<HM03PROJ>(query.ToString());
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return new List<HM03PROJ>();
		}
	}
	#endregion

	/// <summary>
	/// menoコードを取得する
	/// </summary>
	/// <returns></returns>
	public async Task<List<HM11MEMOCHECK>> GetHM11ListAsync(HM11MEMOCHECK hm11Dc)
	{
		try
		{
			StringBuilder query = new StringBuilder();
			query.Append("SELECT  A.HM11001, A.HM11002, A.HM11003,A.HM11004 ");
			query.Append("FROM HM11MEMO A ");
			query.Append("WHERE 1 = 1 ");
			query.Append("AND A.HM11001 = '" + hm11Dc.HM11001 + "'");
			query.Append("ORDER BY  A.HM11004");
			return await _db.QueryAsync<HM11MEMOCHECK>(query.ToString());
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return new List<HM11MEMOCHECK>();
		}
	}

	//分類名称初始化
	public async Task<List<HM23BUNRUI>> GetHM12FILEClassAsync(HM23BUNRUI hm23DC)
	{
		try
		{
			StringBuilder query = new StringBuilder();
			query.Append("SELECT B.HM23002,B.HM23003 FROM HM12FILE A  ");
			query.Append("INNER JOIN HM23BUNRUI B ON A.HM12001=B.HM23001 AND A.HM12015=B.HM23002 ");
			query.Append("WHERE 1 = 1 ");
			query.Append("AND A.HM12001 = '" + hm23DC.HM23001 + "' ");
			query.Append("GROUP BY B.HM23002,B.HM23003 ORDER BY B.HM23004 ");
			return await _db.QueryAsync<HM23BUNRUI>(query.ToString());
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return new List<HM23BUNRUI>();
		}
	}

	public async Task<List<HM12FILE>> GetHM12FILEIDAsync(HM12FILE hm12DC)
	{
		try
		{
			return await _db.Table<HM12FILE>()
				.Where(hm12 => hm12.HM12015 == hm12DC.HM12015)
				.Where(hm12 => hm12.HM12001 == hm12DC.HM12001)
				.OrderBy(hm12 => hm12.HM12002)
				.ToListAsync();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return new List<HM12FILE>();
		}
	}

	public async Task<List<HM16SHDIR>> GetHM16ListAsync(HM16SHDIR hm16DC)
	{
		try
		{
			return await _db.Table<HM16SHDIR>()
				.Where(hm16 => hm16.HM16001 == hm16DC.HM16001)
				.OrderBy(hm16 => hm16.HM16002)
				.ToListAsync();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return new List<HM16SHDIR>();
		}
	}

	/// <summary>
	/// 利用バージョン登録、更新
	/// </summary>
	/// <param name="objDC"></param>
	/// <returns></returns>
	public async Task UpdateHM17VERSIONAsync(HM17VERSION hm17)
	{
		try
		{
			if (hm17.HM17017 == 1)
			{
				var result = await _db.Table<HM17VERSION>()
					.Where(x => x.HM17002 == hm17.HM17002)
					.Where(x => x.HM17012 == hm17.HM17012)
					.Where(x => x.HM17014 == hm17.HM17014)
					.Where(x => x.HM17015 == hm17.HM17015)
					.ToListAsync();

				if (result.Count == 1)
				{
					HM17VERSION hm17Old = result[0];
					hm17Old.HM17002 = hm17.HM17002;
					hm17Old.HM17003 = hm17.HM17003;
					hm17Old.HM17005 = hm17.HM17005;
					hm17Old.HM17008 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
					hm17Old.HM17009 = hm17.HM17009;
					hm17Old.HM17012 = hm17.HM17012;
					hm17Old.HM17013 = hm17.HM17013;
					hm17Old.HM17016 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
					hm17Old.HM17017 = hm17.HM17017;
					hm17Old.HM17018 = hm17.HM17018;

					await _db.InsertOrReplaceAsync(hm17Old);
				}
				else
				{
					hm17.HM17006 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
					hm17.HM17008 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
					hm17.HM17010 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
					hm17.HM17016 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

					await _db.InsertOrReplaceAsync(hm17);
				}
			}
			else
			{
				await _db.RunInTransactionAsync(tran =>
				{
					StringBuilder query = new StringBuilder();
					List<object> parms = new();

					query.Append("UPDATE hm17version SET ");
					query.Append("HM17008 = datetime(CURRENT_TIMESTAMP,'localtime'), ");
					query.Append("HM17009 = ?, ");
					query.Append("HM17016 = datetime(CURRENT_TIMESTAMP,'localtime'), ");
					query.Append("HM17017 = ? ");
					query.Append("WHERE HM17002 = ? ");
					query.Append("AND HM17012 = ? ");
					query.Append("AND HM17015 = ? ");

					parms.Add(hm17.HM17009);
					parms.Add(hm17.HM17017);
					parms.Add(hm17.HM17002);
					parms.Add(hm17.HM17012);
					parms.Add(hm17.HM17015);
					tran.Execute(query.ToString(), parms.ToArray());
				});
			}

		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
		}
	}

	/// <summary>
	/// 写真を撮ってから直接データを保存します。
	/// </summary>
	/// <param name="HR02"></param>
	/// <param name="HR03"></param>
	/// <param name="HR03">type 0:add 1:delete</param>
	public async Task UpdateHR02HR03Async(List<HR03SYAS> HR03List, int type)
	{
		try
		{
			await _db.RunInTransactionAsync(tran =>
			{
				if (type == 0)
				{
					foreach (HR03SYAS hr03 in HR03List)
					{
						tran.InsertOrReplace(hr03);
					}
				}
				else if (type == 1)
				{
					foreach (HR03SYAS hr03 in HR03List)
					{
						//画像を削除
						tran.Delete(hr03);
					}
				}
			});
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
		}
	}

	/// <summary>
	/// 採用写真枚数/写真枚数
	/// </summary>
	/// <param name="hr01001"></param>
	/// <returns></returns>
	public async Task<List<HR01ITEMPICCOUNT>> GetPicCountAsync(string hr01001, string hm09002)
	{
		try
		{
			StringBuilder query = new StringBuilder();
			if (hm09002 == "")
			{
				query.Append("SELECT HR01001, HR01003, COALESCE(b.userPiceCount, 0) UserPiceCount,COALESCE(c.allPicCount, 0) AllPicCount FROM hr01item A ");
				query.Append("left join(SELECT hr03001, hr03003, COUNT(1) userPiceCount FROM hr03syas WHERE hr03006 = 2 GROUP BY hr03001,hr03003) B on a.hr01001 = b.hr03001 and a.hr01003 = b.hr03003 ");
				query.Append("left join(SELECT hr03001, hr03003, COUNT(1) allPicCount FROM hr03syas GROUP BY hr03001, hr03003) c on a.hr01001 = c.hr03001 and a.hr01003 = c.hr03003 ");
				query.Append("where a.hr01001 = '" + hr01001 + "'");
			}
			else
			{
				query.Append("SELECT HR01001, HR01003, COALESCE(b.userPiceCount, 0) UserPiceCount,COALESCE(c.allPicCount, 0) AllPicCount FROM hr01item A ");
				query.Append("left join( ");
				query.Append("SELECT hr03001, hr03003, COUNT(1) userPiceCount FROM hr03syas A ");
				query.Append("inner join( ");
				query.Append("SELECT A.* FROM hr02kskk A ");
				query.Append("INNER JOIN(SELECT* FROM hm13knkm where hm13003= '" + hm09002 + "' and hm13014 = 0) B on A.hr02001 = B.hm13001 AND A.hr02003 = B.hm13004) B ");
				query.Append("on A.hr03001 = b.hr02001 AND A.hr03003 = B.hr02002   And A.hr03004 = B.hr02003 ");
				query.Append("WHERE hr03006 = 2 GROUP BY hr03001,hr03003) B on a.hr01001 = b.hr03001 and a.hr01003 = b.hr03003 ");
				query.Append("left join( ");
				query.Append("SELECT hr03001, hr03003, COUNT(1) allPicCount FROM hr03syas A ");
				query.Append("inner join( ");
				query.Append("SELECT A.* FROM hr02kskk A ");
				query.Append("INNER JOIN(SELECT* FROM hm13knkm where hm13003= '" + hm09002 + "'  and hm13014 = 0) B on A.hr02001 = B.hm13001 AND A.hr02003 = B.hm13004) B ");
				query.Append("on A.hr03001 = b.hr02001 AND A.hr03003 = B.hr02002   And A.hr03004 = B.hr02003 ");
				query.Append("GROUP BY hr03001,hr03003 ");
				query.Append(") c on a.hr01001 = c.hr03001 and a.hr01003 = c.hr03003 ");
				query.Append("where a.hr01001 = '" + hr01001 + "'");
			}
			return await _db.QueryAsync<HR01ITEMPICCOUNT>(query.ToString());
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return new List<HR01ITEMPICCOUNT>();
		}
	}

	/// <summary>
	/// 断面コードの取得
	/// </summary>
	/// <param name="hm10DC"></param>
	/// <param name="hm08005"></param>
	/// <returns></returns>
	public async Task<List<HM10DANM>> GetHM10DANMList3Async(HM10DANM hm10, string hm08005)
	{
		try
		{
			StringBuilder query = new StringBuilder();

			if (hm10.HM10003 == "")
			{
				query.AppendLine("  SELECT '' HM10003,'' HM10004,'' HM10006,-10 HM10007,-10 HM10008,0 HM10009,0 HM10010, ");
				query.AppendLine("  -10 HM10019, -10 HM10020, 0 HM10021, 0 HM10022,'' HM10023, -10 HM10024,'' HM10025 ");
			}
			else
			{
				query.AppendLine("SELECT A.HM10003,'' HM10004,A.HM10006,A.HM10007,A.HM10008,A.HM10009,A.HM10010,");
				query.AppendLine(" A.HM10019,HM10020,A.HM10021,A.HM10022,A.HM10023,A.HM10024,A.HM10025 ");
				query.AppendLine(" FROM HM10DANM A ");
				query.AppendLine(" WHERE A.HM10001 = '" + hm10.HM10001 + "' and A.HM10003 = '" + hm10.HM10003 + "' ");
			}
			query.AppendLine(" UNION ALL");
			query.AppendLine(" SELECT A.* FROM(SELECT A.HM10003,A.HM10004,A.HM10006,A.HM10007,A.HM10008,A.HM10009,A.HM10010,");
			query.AppendLine(" A.HM10019,HM10020,A.HM10021,A.HM10022,A.HM10023,A.HM10024,A.HM10025 ");
			query.AppendLine(" FROM HM10DANM A INNER JOIN HM08GRPM B ON A.HM10002=B.HM08002 AND A.HM10001=B.HM08001 ");
			query.AppendLine(" WHERE A.HM10001 = '" + hm10.HM10001 + "' ");
			query.AppendLine(" AND B.HM08004 = '1001' ");
			query.AppendLine(" AND B.HM08005 = '" + hm08005 + "' ");
			query.AppendLine(" AND A.HM10011 = 0 ");
			query.AppendLine(" ORDER BY HM10005) A ");

			return await _db.QueryAsync<HM10DANM>(query.ToString());
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return new List<HM10DANM>();
		}
	}

	/// <summary>
	/// 保存CheckPointPageDetail页面左侧数据
	/// </summary>
	/// <param name="hr03"></param>
	/// <returns></returns>
	public async Task<bool> UpdateHR03SYAS_LeftAsync(HR03SYAS hr03)
	{
		try
		{
			StringBuilder query = new StringBuilder();
			List<object> ps = new List<object>();
			query.AppendLine("UPDATE HR03SYAS SET ");
			query.AppendLine("HR03007 = ?,");
			query.AppendLine("HR03008 =?,");
			query.AppendLine("HR03013 = datetime(CURRENT_TIMESTAMP,'localtime'),");
			query.AppendLine("HR03014 = ?,");
			query.AppendLine("HR03018 =?,");
			query.AppendLine("HR03019 =?,");
			query.AppendLine("HR03020 =?");

			ps.Add(hr03.HR03007);
			ps.Add(hr03.HR03008);
			ps.Add(hr03.HR03014);
			ps.Add(hr03.HR03018);
			ps.Add(hr03.HR03019);
			ps.Add(hr03.HR03020);

			query.AppendLine(" WHERE HR03001 = ? ");
			query.AppendLine("AND HR03002 = ? ");
			ps.Add(hr03.HR03001);
			ps.Add(hr03.HR03002);
			await _db.ExecuteAsync(query.ToString(), ps.ToArray());
			return true;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return false;
		}
	}

	/// <summary>
	/// 更新写真テーブル
	/// </summary>
	/// <param name="list"></param>
	/// <returns></returns>
	public async Task<bool> UpdateHR03SYASAsync(List<HR03SYAS> list)
	{
		try
		{
			await _db.RunInTransactionAsync(tran =>
			{
				foreach (var item in list)
				{
					StringBuilder query = new StringBuilder();
					List<object> ps = new List<object>();
					if (item.CHANGE == "DELETE")
					{
						query.AppendLine(" DELETE FROM HR03SYAS WHERE HR03001 = ? AND HR03002 = ? ");
						ps.Add(item.HR03001);
						ps.Add(item.HR03002);
						tran.Execute(query.ToString(), ps.ToArray());
					}
					if (item.CHANGE == "UPDATE")
					{
						query.AppendLine("UPDATE HR03SYAS SET ");
						query.AppendLine("HR03006 = ?,");
						query.AppendLine("HR03007 = ?,");
						query.AppendLine("HR03008 = ?,");
						query.AppendLine("HR03013 = datetime(CURRENT_TIMESTAMP,'localtime'),");
						query.AppendLine("HR03014 = ?,");
						query.AppendLine("HR03018 = ?,");
						query.AppendLine("HR03019 = ? ");
						query.AppendLine(" WHERE HR03001 = ? ");
						query.AppendLine("AND HR03002 = ? ");
						ps.Add(item.HR03006);
						ps.Add(item.HR03007);
						ps.Add(item.HR03008);
						ps.Add(item.HR03014);
						ps.Add(item.HR03018);
						ps.Add(item.HR03019);
						ps.Add(item.HR03001);
						ps.Add(item.HR03002);
						tran.Execute(query.ToString(), ps.ToArray());
					}
				}
			});
			return true;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return false;
		}
	}

	/// <summary>
	/// 获取配筋点下的所有图片，根据图片计算有效的工程和确认项目数据
	/// </summary>
	/// <param name="hr01"></param>
	/// <returns></returns>
	public async Task<List<HR01ITEMPINFO>> GetHR01ITEMPINFOAsync(HR01ITEM hr01)
	{
		try
		{
			StringBuilder query = new StringBuilder();
			query.AppendLine("SELECT HR03003, HM13003, HM09003, HR03004, HM13005, HR03002, HR03017,HM13012 FROM hr03syas A");
			query.AppendLine("INNER JOIN hm13knkm B ON A.hr03001=B.hm13001 and A.hr03004=B.hm13004");
			query.AppendLine("INNER JOIN hm09proc C ON B.hm13001=C.hm09001 and B.hm13003=C.hm09002");
			query.AppendLine("WHERE hr03001='" + hr01.HR01001 + "' AND hr03003 = '" + hr01.HR01003 + "'");

			return await _db.QueryAsync<HR01ITEMPINFO>(query.ToString());
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return new List<HR01ITEMPINFO>();
		}
	}
}

