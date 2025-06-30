using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace OneClickLlm.AvaloniaUI.Converters;

/// <summary>
/// Converts a boolean value to its inverse.
/// Useful when a control's enabled state is opposite of a view model flag.
/// </summary>
public class InverseBooleanConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is bool b ? !b : false;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is bool b ? !b : false;
}
