using System;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace OneClickLlm.AvaloniaUI.Presenters;

/// <summary>
/// Presenter for settings window allowing to change chat colors.
/// </summary>
public partial class SettingsPresenter : PresenterBase
{
    private readonly MainWindowPresenter _mainPresenter;

    public event Action? CloseRequested;

    [ObservableProperty]
    private Color _messageTextColor;

    [ObservableProperty]
    private Color _messageBackgroundColor;

    public SettingsPresenter(MainWindowPresenter mainPresenter)
    {
        _mainPresenter = mainPresenter;
        _messageTextColor = mainPresenter.MessageTextColor;
        _messageBackgroundColor = mainPresenter.MessageBackgroundColor;
    }

    [RelayCommand]
    private void Confirm()
    {
        _mainPresenter.MessageTextColor = MessageTextColor;
        _mainPresenter.MessageBackgroundColor = MessageBackgroundColor;
        CloseRequested?.Invoke();
    }

    [RelayCommand]
    private void Cancel() => CloseRequested?.Invoke();
}
