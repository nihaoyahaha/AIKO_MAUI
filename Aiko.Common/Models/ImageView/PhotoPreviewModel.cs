using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aiko.Common.Models.ImageView;

public partial class PhotoPreviewModel : ObservableObject
{
	/// <summary>
	/// ファイルパス
	/// </summary>
	public string ImageUrl { get; set; }=string.Empty;

	/// <summary>
	/// jpg写真データソース
	/// </summary>
	public ImageSource? JPgImageSource { get; set; }

	/// <summary>
	/// 画像の詳細
	/// </summary>
	[ObservableProperty]
	public partial PhotoLayer PhotoLayer { get; set; }

	/// <summary>
	/// 写真方式 0：JPG 1：SVG
	/// </summary>
	public int HR03017 { get; set; }

	/// <summary>
	/// 工事コード
	/// </summary>
	public string HR03001 { get; set; } = string.Empty;

	/// <summary>
	/// 写真コード
	/// </summary>
	public string HR03002 { get; set; } = string.Empty;

	/// <summary>
	/// アイテムコード
	/// </summary>
	public string HR03003 { get; set; } = string.Empty;

	/// <summary>
	/// 確認項目コード
	/// </summary>
	public string HR03004 { get; set; } = string.Empty;

	public void ChangePhotoSize(double screenWidth, double screenHeight)
	{
		if (screenWidth <= 0 || screenHeight <= 0) return;

		double photoWidth = double.Parse(PhotoLayer.PhotoWidth);
		double photoHeight = double.Parse(PhotoLayer.PhotoHeight);
		
		if (photoWidth <= screenWidth && photoHeight <= screenHeight) return;

		double scaleX = screenWidth / photoWidth;
		double scaleY = screenHeight / photoHeight;

		var fitScale = Math.Min(scaleX, scaleY);

		PhotoLayer.PhotoWidth = (photoWidth * fitScale).ToString();
		PhotoLayer.PhotoHeight = (photoHeight * fitScale).ToString();

		if (HR03017 == 1) ComputeGreenBackgroundSize(fitScale);
	}

	void ComputeGreenBackgroundSize(double scale) 
	{
		double greenWidth = double.Parse(PhotoLayer.GreenWidth);
		double greenHeight = double.Parse(PhotoLayer.GreenHeight);

		double greenLeft = PhotoLayer.Margin.Left;
		double greenTop = PhotoLayer.Margin.Top;

		PhotoLayer.GreenWidth = (greenWidth * scale).ToString();
		PhotoLayer.GreenHeight = (greenHeight * scale).ToString();
		PhotoLayer.Margin = new Thickness(greenLeft*scale,greenTop*scale,0,0);
	}

}

