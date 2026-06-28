using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using ShieldText.Services;

namespace ShieldText.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    private readonly CryptoService _cryptoService = new();

    // ── Fields ──────────────────────────────────────────
    private string _inputText = string.Empty;
    private string _outputText = string.Empty;
    private string _password = string.Empty;
    private string _statusMessage = string.Empty;
    private bool _isSuccess;

    // ── Properties ──────────────────────────────────────
    public string InputText
    {
        get => _inputText;
        set { _inputText = value; OnPropertyChanged(); }
    }

    public string OutputText
    {
        get => _outputText;
        set { _outputText = value; OnPropertyChanged(); }
    }

    public string Password
    {
        get => _password;
        set { _password = value; OnPropertyChanged(); }
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set { _statusMessage = value; OnPropertyChanged(); }
    }

    public bool IsSuccess
    {
        get => _isSuccess;
        set { _isSuccess = value; OnPropertyChanged(); }
    }

    // ── Actions ─────────────────────────────────────────
    public void Encrypt()
    {
        if (!Validate()) return;

        try
        {
            OutputText = _cryptoService.Encrypt(InputText, Password);
            SetStatus("Encrypted successfully", true);
        }
        catch
        {
            SetStatus("Encryption failed", false);
        }
    }

    public void Decrypt()
    {
        if (!Validate()) return;

        try
        {
            OutputText = _cryptoService.Decrypt(InputText, Password);
            SetStatus("Decrypted successfully", true);
        }
        catch
        {
            SetStatus("Wrong password or invalid input", false);
        }
    }

    public void CopyOutput()
    {
        if (string.IsNullOrEmpty(OutputText)) return;
        Clipboard.SetText(OutputText);
        SetStatus("Copied to clipboard", true);
    }

    public void SwapTexts()
    {
        (InputText, OutputText) = (OutputText, InputText);
    }

    public void Clear()
    {
        InputText = string.Empty;
        OutputText = string.Empty;
        Password = string.Empty;
        StatusMessage = string.Empty;
    }

    // ── Helpers ─────────────────────────────────────────
    private bool Validate()
    {
        if (string.IsNullOrWhiteSpace(InputText))
        {
            SetStatus("Please enter text", false);
            return false;
        }
        if (string.IsNullOrWhiteSpace(Password))
        {
            SetStatus("Please enter password", false);
            return false;
        }
        return true;
    }

    private void SetStatus(string message, bool success)
    {
        StatusMessage = message;
        IsSuccess = success;
    }

    // ── INotifyPropertyChanged ───────────────────────────
    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}