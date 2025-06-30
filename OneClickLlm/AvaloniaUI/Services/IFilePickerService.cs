using Avalonia.Controls;
using System.Threading.Tasks;

namespace OneClickLlm.AvaloniaUI.Services;

/// <summary>
/// Provides file selection dialogs.
/// </summary>
public interface IFilePickerService
{
    /// <summary>
    /// Shows a file open dialog and returns the chosen file path or null.
    /// </summary>
    Task<string?> OpenFileAsync(Window owner, string title, params string[] extensions);
}
