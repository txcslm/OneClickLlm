using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OneClickLlm.Core.Services;

namespace OneClickLlm.AvaloniaUI.Presenters;

/// <summary>
/// Presenter для окна выбора моделей.
/// </summary>
public partial class ModelSelectionPresenter : PresenterBase
{
    private readonly IModelManager _modelManager;
    private readonly ILlmService _llmService;
    public event Action<bool?>? CloseRequested;
    
    [ObservableProperty]
    private ObservableCollection<ModelInfo> _localModels = new();

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(DeleteSelectedModelCommand))]
    [NotifyCanExecuteChangedFor(nameof(ConfirmSelectionCommand))]
    private ModelInfo? _selectedModel;

    [ObservableProperty]
    private string? _errorMessage;
    
    public ModelSelectionPresenter(IModelManager modelManager, ILlmService llmService)
    {
        _modelManager = modelManager;
        _llmService = llmService;
    }

    public async Task LoadModelsAsync()
    {
        try
        {
            var models = await _modelManager.GetLocalModelsAsync();
            LocalModels = new ObservableCollection<ModelInfo>(models);
        }
        catch(Exception ex)
        {
            // Обработка ошибки, если Ollama не запущен
            ErrorMessage = $"Error loading models: {ex.Message}";
        }
    }

    [RelayCommand(CanExecute = nameof(CanConfirmSelection))]
    private async Task ConfirmSelectionAsync()
    {
        if (SelectedModel == null) return;
        
        await _llmService.LoadModelAsync(SelectedModel.Id);
        CloseRequested?.Invoke(true); // true означает, что выбор подтвержден
    }
    private bool CanConfirmSelection() => SelectedModel != null;

    [RelayCommand(CanExecute = nameof(CanDeleteSelectedModel))]
    private async Task DeleteSelectedModelAsync()
    {
        if (SelectedModel == null) return;
        
        await _modelManager.DeleteModelAsync(SelectedModel.Id);
        LocalModels.Remove(SelectedModel);
        SelectedModel = null;
    }
    private bool CanDeleteSelectedModel() => SelectedModel != null;

    [RelayCommand]
    private void DownloadModel()
    {
        // Логика для скачивания новой модели (не реализована в MVP)
    }
}
