using DI.DiNetWinServiceObject;

namespace DI.DiNetWinServiceObject
{
	/// <summary>
	/// IP制限マスター HM26DEVRES
	/// </summary>
	public class HM26DEVRESDC : CMObjectDC
	{

		/// <summary>
		/// 会社コード
		/// </summary>
		public string HM26001 { get; set; } = string.Empty;

		/// <summary>
		/// オペレータコード
		/// </summary>
		public string HM26002 { get; set; } = string.Empty;

		/// <summary>
		/// 端末のUUID
		/// </summary>
		public string HM26003 { get; set; } = string.Empty;

		/// <summary>
		/// 端末名
		/// </summary>
		public string HM26004 { get; set; } = string.Empty;

		/// <summary>
		/// 備考
		/// </summary>
		public string HM26005 { get; set; } = string.Empty;

		/// <summary>
		/// 並び順
		/// </summary>
		public string HM26006 { get; set; } = string.Empty;

		/// <summary>
		/// 自動取得
		/// </summary>
		public int HM26007 { get; set; }

		/// <summary>
		/// 登録済
		/// </summary>
		public int HM26008 { get; set; }

		/// <summary>
		/// 作成日時
		/// </summary>
		public DateTime HM26009 { get; set; }

		/// <summary>
		/// 作成オペレータ
		/// </summary>
		public string HM26010 { get; set; } = string.Empty;

		/// <summary>
		/// 更新日時
		/// </summary>
		public DateTime HM26011 { get; set; }

		/// <summary>
		/// 更新オペレータ
		/// </summary>
		public string HM26012 { get; set; } = string.Empty;
	}
}
