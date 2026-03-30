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
	/// <summary>
	/// 検査結果画像
	/// </summary>
	[ObservableProperty]	
	private string? _checkResultImage;

	/// <summary>
	/// 結果コード
	/// </summary>
	[ObservableProperty]
	private int _hR02005;

	/// <summary>
	/// 確認コード
	/// </summary>
	[ObservableProperty]
	private string? _hM13004;

	/// <summary>
	/// 確認項目名
	/// </summary>
	[ObservableProperty]
	private string? _artistName;

	/// <summary>
	/// 工程コード
	/// </summary>
	[ObservableProperty]
	private string? _composition;

	/// <summary>
	/// 工程名
	/// </summary>
	[ObservableProperty]
	private string? _compositionName;

	[ObservableProperty]
	private DateTime _releaseDateTime;

	/// <summary>
	/// 確認日
	/// </summary>
	[ObservableProperty]
	private string? _hR02006;

	/// <summary>
	/// 確認者
	/// </summary>
	[ObservableProperty]
	private string? _hR02007;

	/// <summary>
	/// 値
	/// </summary>
	[ObservableProperty]
	private string? _hR02004;

	/// <summary>
	/// 単位
	/// </summary>
	[ObservableProperty]
	private string _hM13011;

	/// <summary>
	/// 入力可能
	/// </summary>
	[ObservableProperty]
	private int _hM13010;

	/// <summary>
	/// 指摘日
	/// </summary>
	[ObservableProperty]
	private string? _hR02008;

	/// <summary>
	/// 指摘者
	/// </summary>
	[ObservableProperty]
	private string? _hR02009;

	/// <summary>
	/// メモ
	/// </summary>
	[ObservableProperty]
	private string? _hR02010;

	/// <summary>
	/// 説明
	/// </summary>
	[ObservableProperty]
	private string? _hM13012;

	/// <summary>
	/// 方法
	/// </summary>
	[ObservableProperty]
	private string? _hR02011;

	/// <summary>
	/// 確認方法
	/// </summary>
	[ObservableProperty]
	private int _hR02019;

	/// <summary>
	/// 写真タイプ
	/// 0：不要　1：確認箇所ごと　2：工区・符号ごと　3：工区ごと
	/// </summary>
	[ObservableProperty]
	private int _hM13009;

	[ObservableProperty]
	private bool _hasSyas = false;

	/// <summary>
	/// 写真アイコンの背景色
	/// </summary>
	[ObservableProperty]
	private Color _cameraColor = Colors.White;

	/// <summary>
	/// 枚数
	/// </summary>
	[ObservableProperty]
	private int _num;

	/// <summary>
	/// ローカル画像数
	/// </summary>
	[ObservableProperty]
	private int _localPicNum;

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
	private int _usedNum;

	/// <summary>
	/// 作成日時
	/// </summary>
	[ObservableProperty]
	private string? _hR02013;

	/// <summary>
	/// 作成オペレータ
	/// </summary>
	[ObservableProperty]
	private string? _hR02014;

	/// <summary>
	/// 更新日時
	/// </summary>
	[ObservableProperty]
	private string? _hR02015;

	/// <summary>
	/// 同期日時
	/// </summary>
	[ObservableProperty]
	private string? _hR02017;

	/// <summary>
	/// 同期オペレータ
	/// </summary>
	[ObservableProperty]
	private string? _hR02018;

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
			CheckResultImage = _checkResultImage,
			HR02005 = _hR02005,
			HM13004 = _hM13004,
			ArtistName = _artistName,
			Composition = _composition,
			CompositionName = _compositionName,
			ReleaseDateTime = _releaseDateTime,
			HR02006 = _hR02006,
			HR02007 = _hR02007,
			HR02004 = _hR02004,
			HM13011 = _hM13011,
			HM13010 = _hM13010,
			HR02008 = _hR02008,
			HR02009 = _hR02009,
			HR02010 = _hR02010,
			HM13012 = _hM13012,
			HR02011 = _hR02011,
			HR02019 = _hR02019,
			HM13009 = _hM13009,
			HasSyas = _hasSyas,
			CameraColor = _cameraColor,
			Num = _num ,
			LocalPicNum = _localPicNum,
			UsedNum = _usedNum,
			HR02013 = _hR02013,
			HR02014 = _hR02014,
			HR02015 = _hR02015,
			HR02017 = _hR02017,
			HR02018 = _hR02018
		};
	}
}
