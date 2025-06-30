using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using OneClickLlm.Core.Services;

namespace OneClickLlm.AvaloniaUI.Converters;

public class RoleToColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is ChatMessageRole role)
        {
            return role == ChatMessageRole.User ? Brushes.LightBlue : Brushes.LightGray;
        }
        return Brushes.LightGray;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
