using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Aiko.UI.Behaviors;

/// <summary>
/// このBehaviorは、ユーザー入力時に非数字と非英字をブロックし、リアルタイムで小文字を大文字に変換します
/// </summary>
public class UpperAlphaNumericBehavior : Behavior<Entry>
{
	// 正規表現：数字（0-9）と英字（a-z，A-Z）のみを一致させる
	private static readonly Regex AlphaNumericRegex = new Regex("^[a-zA-Z0-9]*$");

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
		var entry = sender as Entry;
		if (entry == null || string.IsNullOrEmpty(e.NewTextValue))
			return;

		//不正な文字（数字、文字以外）が含まれているかどうかをチェック
		if (!AlphaNumericRegex.IsMatch(e.NewTextValue))
		{
			// 不正な文字が含まれている場合は、古い値にロールバックします
			entry.Text = e.OldTextValue;
			return;
		}

		// 小文字があるかどうかをチェックし、大文字に変換します
		string uppercased = e.NewTextValue.ToUpper();

		if (e.NewTextValue != uppercased)
		{
			// 注意：修改 entry.Text 会再次触发 TextChanged 事件
			// 但由于下次进入时 e.NewTextValue == uppercased，所以不会形成死循环
			entry.Text = uppercased;
		}
	}
}
