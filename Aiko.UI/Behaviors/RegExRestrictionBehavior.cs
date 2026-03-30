using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Aiko.UI.Behaviors;

/// <summary>
/// このBehaviorは、ユーザーが数字、英語、特殊文字のみを入力できるように制限します
/// </summary>
public class RegExRestrictionBehavior : Behavior<Entry>
{
	//文字、数字、および共通の句読点のみを許可する
	private const string FilterPattern = @"^[a-zA-Z0-9\s!@#$%^&*()_+={}\[\]:;""'<>,.?/\\|~`-]*$";

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

		if (!Regex.IsMatch(e.NewTextValue, FilterPattern))
		{
			var entry = (Entry)sender;
			entry.Text = e.OldTextValue;
		}
	}
}