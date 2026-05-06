using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aiko.Common.Models;

/// <summary>
/// 建築現場の検査・確認における1つの確認項目を表します。
/// </summary>
public partial class InspectionItem : ObservableObject
{
	string _theme = Preferences.Default.Get("Theme", "Light");

	// 検査結果画像
	private string _checkResultImage = string.Empty;

	/// <summary>
	/// 検査結果画像
	/// </summary>
	public string CheckResultImage
	{
		get 
		{
			string themeSuffix = _theme == "Light" ? "light" : "dark";
			return $"{themeSuffix}{_checkResultImage}";
		}
		set
		{
			if (_checkResultImage != value)
			{
				_checkResultImage = value;

				OnPropertyChanged(nameof(CheckResultImage));
			}
		}
	}

	/// <summary>
	/// 結果コード
	/// </summary>
	[ObservableProperty]
	public partial int HR02005 { get; set; }

	/// <summary>
	/// 確認コード
	/// </summary>
	[ObservableProperty]
	public partial string HM13004 { get; set; }= string.Empty;

	/// <summary>
	/// 確認項目名
	/// </summary>
	[ObservableProperty]
	public partial string ArtistName { get; set; }= string.Empty;

	/// <summary>
	/// 工程コード
	/// </summary>
	[ObservableProperty]
	public partial string Composition { get; set; } = string.Empty;

	/// <summary>
	/// 工程名
	/// </summary>
	[ObservableProperty]
	public partial string CompositionName { get; set; } = string.Empty;

	[ObservableProperty]
	public partial DateTime ReleaseDateTime { get; set; }

	/// <summary>
	/// 確認日
	/// </summary>
	[ObservableProperty]
	public partial string HR02006 { get; set; } = string.Empty;

	/// <summary>
	/// 確認者
	/// </summary>
	[ObservableProperty]
	public partial string HR02007 { get; set; } = string.Empty;

	/// <summary>
	/// 値
	/// </summary>
	[ObservableProperty]
	public partial string HR02004 { get; set; } = string.Empty;

	/// <summary>
	/// 単位
	/// </summary>
	[ObservableProperty]
	public partial string HM13011 { get; set; } = string.Empty;

	/// <summary>
	/// 入力可能
	/// </summary>
	[ObservableProperty]
	public partial int HM13010 { get; set; }

	/// <summary>
	/// 指摘日
	/// </summary>
	[ObservableProperty]
	public partial string HR02008 { get; set; } = string.Empty;

	/// <summary>
	/// 指摘者
	/// </summary>
	[ObservableProperty]
	public partial string HR02009 { get; set; } = string.Empty;

	/// <summary>
	/// メモ
	/// </summary>
	[ObservableProperty]
	public partial string HR02010 { get; set; } = string.Empty;

	/// <summary>
	/// 説明
	/// </summary>
	[ObservableProperty]
	public partial string HM13012 { get; set; } = string.Empty;

	/// <summary>
	/// 方法
	/// </summary>
	[ObservableProperty]
	public partial string HR02011 { get; set; } = string.Empty;

	/// <summary>
	/// 確認方法
	/// </summary>
	[ObservableProperty]
	public partial int HR02019 { get; set; }

	/// <summary>
	/// 写真タイプ
	/// 0：不要　1：確認箇所ごと　2：工区・符号ごと　3：工区ごと
	/// </summary>
	[ObservableProperty]
	public partial int HM13009 { get; set; }

	[ObservableProperty]
	public partial bool HasSyas { get; set; } = false;

	/// <summary>
	/// 写真アイコンの背景色
	/// </summary>
	[ObservableProperty]
	public partial Color CameraColor { get; set; } = Colors.White;

	/// <summary>
	/// 枚数
	/// </summary>
	[ObservableProperty]
	public partial int Num { get; set; }

	/// <summary>
	/// ローカル画像数
	/// </summary>
	[ObservableProperty]
	public partial int LocalPicNum { get; set; }

	/// <summary>
	/// 写真枚数(写真フォルダー下にJPEG又はSVGの写真枚数)
	/// </summary>
	public string PicNumInfo
	{
		get { return $"{Num}({LocalPicNum})"; }
	}

	/// <summary>
	/// 枚数
	/// </summary>
	[ObservableProperty]
	public partial int UsedNum { get; set; }

	/// <summary>
	/// 作成日時
	/// </summary>
	[ObservableProperty]
	public partial string HR02013 { get; set; } = string.Empty;

	/// <summary>
	/// 作成オペレータ
	/// </summary>
	[ObservableProperty]
	public partial string HR02014 { get; set; } = string.Empty;

	/// <summary>
	/// 更新日時
	/// </summary>
	[ObservableProperty]
	public partial string HR02015 { get;set; } = string.Empty;

	/// <summary>
	/// 同期日時
	/// </summary>
	[ObservableProperty]
	public partial string HR02017 { get; set; } = string.Empty;

	/// <summary>
	/// 同期オペレータ
	/// </summary>
	[ObservableProperty]
	public partial string HR02018 { get; set; } = string.Empty;

	public void SetCameraColor()
	{
		if (HM13009 == 0)
		{
			//写真タイプ = 0 検査結果_写真枚数がゼロである場合_写真枚数が１以上である場合  白(現状のまま)
			CameraColor = Colors.White;
		}
		else
		{
			//写真タイプ(1,2,3)  
			//写真枚数がゼロである場合 黄
			//写真枚数が１以上である場合 緑(true)
			if (HasSyas)
			{
				CameraColor = Colors.Green;
			}
			else
			{
				CameraColor = Colors.Yellow;
			}
		}
	}

	partial void OnNumChanged(int value)
	{
		OnPropertyChanged(nameof(PicNumInfo));
	}

	partial void OnLocalPicNumChanged(int value)
	{
		OnPropertyChanged(nameof(PicNumInfo));
	}

	public InspectionItem Clone()
	{
		return new InspectionItem()
		{
			HR02005 = HR02005,
			HM13004 = HM13004,
			ArtistName = ArtistName,
			Composition = Composition,
			CompositionName = CompositionName,
			ReleaseDateTime = ReleaseDateTime,
			HR02006 = HR02006,
			HR02007 = HR02007,
			HR02004 = HR02004,
			HM13011 = HM13011,
			HM13010 = HM13010,
			HR02008 = HR02008,
			HR02009 = HR02009,
			HR02010 = HR02010,
			HM13012 = HM13012,
			HR02011 = HR02011,
			HR02019 = HR02019,
			HM13009 = HM13009,
			HasSyas = HasSyas,
			CameraColor = CameraColor,
			Num = Num ,
			LocalPicNum = LocalPicNum,
			UsedNum = UsedNum,
			HR02013 = HR02013,
			HR02014 = HR02014,
			HR02015 = HR02015,
			HR02017 = HR02017,
			HR02018 = HR02018
		};
	}
}
