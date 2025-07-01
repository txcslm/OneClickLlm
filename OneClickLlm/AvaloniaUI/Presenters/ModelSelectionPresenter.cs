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
    private readonly ILanguageModelService _languageModelService;
    private readonly IFilePickerService _filePickerService;
    public event Action<bool?>? CloseRequested;

    [ObservableProperty]
    private string _selectedModelPath = string.Empty;

    [ObservableProperty]
    private string? _errorMessage;
    public ModelSelectionPresenter(ILanguageModelService languageModelService, IFilePickerService filePickerService)
    {
        _languageModelService = languageModelService;
        _filePickerService = filePickerService;
    }

    [RelayCommand]
    public async Task BrowseModelAsync(Window owner)
    {
        var path = await _filePickerService.OpenFileAsync(owner, "Выберите GGUF модель", "gguf");
        if (!string.IsNullOrWhiteSpace(path))
            SelectedModelPath = path;
    }

    [RelayCommand]
    private async Task ConfirmSelectionAsync()
    {
        if (string.IsNullOrWhiteSpace(SelectedModelPath)) return;
        try
        {
            await _languageModelService.LoadModelAsync(SelectedModelPath);
            CloseRequested?.Invoke(true);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }
}
