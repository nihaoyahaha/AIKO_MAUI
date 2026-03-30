using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Aiko.UI.Behaviors;

/// <summary>
/// 制限数値のみ入力可能
/// </summary>
public class NumericOnlyBehavior : Behavior<Entry>
{
	public static readonly BindableProperty MaxLengthProperty =
		BindableProperty.Create(nameof(MaxLength), typeof(int), typeof(NumericOnlyBehavior), int.MaxValue);

	public int MaxLength
	{
		get => (int)GetValue(MaxLengthProperty);
		set => SetValue(MaxLengthProperty, value);
	}
	protected override void OnAttachedTo(Entry bindable)
	{
		bindable.TextChanged += OnEntryTextChanged;
		base.OnAttachedTo(bindable);
	}

	protected override void OnDetachingFrom(Entry bindable)
	{
		bindable.TextChanged -= OnEntryTextChanged;
		base.OnDetachingFrom(bindable);
	}

	private void OnEntryTextChanged(object sender, TextChangedEventArgs e)
	{
		if (string.IsNullOrEmpty(e.NewTextValue)) return;

		// 检查是否全部为数字 (0-9)
		// 如果你想允许小数点，可以将正则改为 @"^[0-9]*\.?[0-9]*$"
		bool isNumeric = Regex.IsMatch(e.NewTextValue, "^[0-9]*$");
		bool isLengthValid = e.NewTextValue.Length <= MaxLength;
		if (!isNumeric || !isLengthValid)
		{
			var entry = (Entry)sender;
			// 彻底拦截非法字符，回滚到上一次的合法状态
			entry.Text = e.OldTextValue;
		}
	}
}
