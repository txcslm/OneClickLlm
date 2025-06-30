using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OneClickLlm.Core.Services;

namespace OneClickLlm.AvaloniaUI.Presenters;

/// <summary>
/// Presenter для главного окна приложения (MainWindow).
/// </summary>
public partial class MainWindowPresenter : PresenterBase
{
    private readonly ILlmService _llmService;
    public event Action? OpenModelSelectionRequested;
    public event Action? OpenSettingsRequested;

    [ObservableProperty]
    private ObservableCollection<ChatMessage> _chatHistory = new();

    [ObservableProperty]
    private string _promptText = string.Empty;

    [ObservableProperty]
    private string _currentModelStatus = "Модель не загружена";

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SendMessageCommand))]
    private bool _isBusy;

    [ObservableProperty]
    private Color _messageTextColor = Colors.Black;

    [ObservableProperty]
    private Color _messageBackgroundColor = Colors.LightGray;

    public MainWindowPresenter(ILlmService llmService)
    {
        _llmService = llmService;
        UpdateModelStatus();
    }

    [RelayCommand(CanExecute = nameof(CanSendMessage))]
    private async Task SendMessageAsync()
    {
        if (string.IsNullOrWhiteSpace(PromptText) || _llmService.CurrentModel is null) return;

        IsBusy = true;
        var userPrompt = PromptText;
        PromptText = string.Empty;

        try
        {
            ChatHistory.Add(new ChatMessage(ChatMessageRole.User, userPrompt));

            var assistantMessage = new ChatMessage(ChatMessageRole.Assistant, "Генерация...");
            ChatHistory.Add(assistantMessage);

            var fullResponse = new StringBuilder();
            var history = ChatHistory.Take(ChatHistory.Count - 2);
            
            // Здесь мы используем дефолтные опции генерации. В реальном приложении они бы брались из настроек.
            var options = new GenerationOptions(Temperature: 0.7f, TopP: 0.9f);
            
            await foreach (var token in _llmService.GenerateResponseStreamAsync(userPrompt, history, options))
            {
                fullResponse.Append(token);
                // Обновляем последний элемент в коллекции, чтобы избежать мерцания UI
                ChatHistory[^1] = assistantMessage with { Content = fullResponse.ToString() };
            }
        }
        catch (Exception ex)
        {
            ChatHistory.Add(new ChatMessage(ChatMessageRole.Assistant, $"Произошла ошибка: {ex.Message}"));
        }
        finally
        {
            IsBusy = false;
        }
    }

    private bool CanSendMessage() => !IsBusy && !string.IsNullOrWhiteSpace(PromptText) && _llmService.CurrentModel != null;

    [RelayCommand]
    private void OpenModelSelection() => OpenModelSelectionRequested?.Invoke();

    [RelayCommand]
    private void OpenSettings() => OpenSettingsRequested?.Invoke();

    public void UpdateModelStatus()
    {
        CurrentModelStatus = _llmService.CurrentModel != null
            ? $"Модель: {_llmService.CurrentModel.DisplayName}"
            : "Модель не загружена";
        SendMessageCommand.NotifyCanExecuteChanged();
    }
}