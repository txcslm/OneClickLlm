using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OneClickLlm.Core.Services;

namespace OneClickLlm.AvaloniaUI.Presenters;

/// <summary>
/// Presenter for selecting a local GGUF model.
/// </summary>
public partial class ModelSelectionPresenter : PresenterBase
{
    private readonly ILlmService _llmService;
    public event Action<bool?>? CloseRequested;

    [ObservableProperty]
    private string _modelPath = string.Empty;

    [ObservableProperty]
    private string? _errorMessage;

    public ModelSelectionPresenter(ILlmService llmService)
    {
        _llmService = llmService;
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
