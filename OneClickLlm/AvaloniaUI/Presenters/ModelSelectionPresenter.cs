using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OneClickLlm.Core.Services;
using OneClickLlm.AvaloniaUI.Services;

namespace OneClickLlm.AvaloniaUI.Presenters;

/// <summary>
/// Presenter for selecting a local GGUF model.
/// </summary>
public partial class ModelSelectionPresenter : PresenterBase
{
    private readonly ILlmService _llmService;
    private readonly IFilePickerService _filePicker;
    public event Action<bool?>? CloseRequested;

    [ObservableProperty]
    private string _modelPath = string.Empty;

    [ObservableProperty]
    private string? _errorMessage;
    public ModelSelectionPresenter(ILlmService llmService, IFilePickerService filePicker)
    {
        _llmService = llmService;
        _filePicker = filePicker;
    }

    public async Task BrowseForModelAsync(Window owner)
    {
        var path = await _filePicker.OpenFileAsync(owner, "Выберите GGUF модель", "gguf");
        if (!string.IsNullOrWhiteSpace(path))
            ModelPath = path;
    }

    [RelayCommand]
    private async Task ConfirmSelectionAsync()
    {
        if (string.IsNullOrWhiteSpace(ModelPath)) return;
        try
        {
            await _llmService.LoadModelAsync(ModelPath);
            CloseRequested?.Invoke(true);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }
}
