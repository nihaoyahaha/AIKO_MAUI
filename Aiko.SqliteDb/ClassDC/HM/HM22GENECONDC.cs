using DI.DiNetWinServiceObject;

namespace DI.DiNetWinServiceObject
{
	public class HM22GENECONDC : CMObjectDC
	{
		/// <summary>
		/// 会社コード
		/// </summary>
		public string HM22001 { get; set; } = string.Empty;

		/// <summary>
		/// ゼネコンコード
		/// </summary>
		public string HM22002 { get; set; } = string.Empty;

		/// <summary>
		/// ゼネコン名
		/// </summary>
		public string HM22003 { get; set; } = string.Empty;

		/// <summary>
		/// 並び順
		/// </summary>
		public string HM22004 { get; set; } = string.Empty;

		/// <summary>
		/// 作成日時
		/// </summary>
		public DateTime HM22005 { get; set; }

		/// <summary>
		/// 作成オペレータ
		/// </summary>
		public string HM22006 { get; set; } = string.Empty;

		/// <summary>
		/// 更新日時
		/// </summary>
		public DateTime HM22007 { get; set; }

		/// <summary>
		/// 更新オペレータ
		/// </summary>
		public string HM22008 { get; set; } = string.Empty;

		/// <summary>
		/// 同期日時
		/// </summary>
		public DateTime HM22009 { get; set; }

		/// <summary>
		/// 同期オペレータ
		/// </summary>
		public string HM22010 { get; set; } = string.Empty;

	}
}
