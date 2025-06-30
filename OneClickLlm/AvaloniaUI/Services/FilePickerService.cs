using Avalonia.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneClickLlm.AvaloniaUI.Services;

/// <summary>
/// Default implementation of <see cref="IFilePickerService"/> using Avalonia dialogs.
/// </summary>
public class FilePickerService : IFilePickerService
{
    public async Task<string?> OpenFileAsync(Window owner, string title, params string[] extensions)
    {
        var dialog = new OpenFileDialog
        {
            Title = title,
            AllowMultiple = false,
            Filters = new List<FileDialogFilter>
            {
                new FileDialogFilter
                {
                    Name = string.Join(", ", extensions.Select(e => $"*.{e}")),
                    Extensions = extensions.ToList()
                }
            }
        };
        var result = await dialog.ShowAsync(owner);
        return result?.FirstOrDefault();
    }
}
