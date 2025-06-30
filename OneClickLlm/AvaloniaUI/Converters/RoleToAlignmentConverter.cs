using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Layout;
using OneClickLlm.Core.Services;

namespace OneClickLlm.AvaloniaUI.Converters;

public class RoleToAlignmentConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is ChatMessageRole role)
        {
            return role == ChatMessageRole.User ? HorizontalAlignment.Right : HorizontalAlignment.Left;
        }
        return HorizontalAlignment.Left;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
