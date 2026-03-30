using DI.DiNetWinServiceObject;

namespace DI.DiNetWinServiceObject
{
	/// <summary>
	/// IP制限マスター HM26DEVRES
	/// </summary>
	public class HM26DEVRESDC : CMObjectDC
	{
		//会社コード
		private string _HM26001;

		//オペレータコード
		private string _HM26002;

		//端末のUUID
		private string _HM26003;

		//端末名
		private string _HM26004;

		//備考
		private string _HM26005;

		//並び順
		private string _HM26006;

		//自動取得
		private int _HM26007;

		//登録済
		private int _HM26008;

		//作成日時
		private DateTime _HM26009;

		//作成オペレータ
		private string _HM26010;

		//更新日時
		private DateTime _HM26011;

		//更新オペレータ
		private string _HM26012;

		/// <summary>
		/// 会社コード
		/// </summary>
		public string HM26001
		{
			get => _HM26001;
			set => _HM26001 = value;
		}

		/// <summary>
		/// オペレータコード
		/// </summary>
		public string HM26002
		{
			get => _HM26002;
			set => _HM26002 = value;
		}

		/// <summary>
		/// 端末のUUID
		/// </summary>
		public string HM26003
		{
			get => _HM26003;
			set => _HM26003 = value;
		}

		/// <summary>
		/// 端末名
		/// </summary>
		public string HM26004
		{
			get => _HM26004;
			set => _HM26004 = value;
		}

		/// <summary>
		/// 備考
		/// </summary>
		public string HM26005
		{
			get => _HM26005;
			set => _HM26005 = value;
		}

		/// <summary>
		/// 並び順
		/// </summary>
		public string HM26006
		{
			get => _HM26006;
			set => _HM26006 = value;
		}

		/// <summary>
		/// 自動取得
		/// </summary>
		public int HM26007
		{
			get => _HM26007;
			set => _HM26007 = value;
		}

		/// <summary>
		/// 登録済
		/// </summary>
		public int HM26008
		{
			get => _HM26008;
			set => _HM26008 = value;
		}

		/// <summary>
		/// 作成日時
		/// </summary>
		public DateTime HM26009
		{
			get => _HM26009;
			set => _HM26009 = value;
		}

		/// <summary>
		/// 作成オペレータ
		/// </summary>
		public string HM26010
		{
			get => _HM26010;
			set => _HM26010 = value;
		}

		/// <summary>
		/// 更新日時
		/// </summary>
		public DateTime HM26011
		{
			get => _HM26011;
			set => _HM26011 = value;
		}

		/// <summary>
		/// 更新オペレータ
		/// </summary>
		public string HM26012
		{
			get => _HM26012;
			set => _HM26012 = value;
		}
	}
}
