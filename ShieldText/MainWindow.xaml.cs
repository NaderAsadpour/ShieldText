using System.Windows;
using System.Windows.Media;
using ShieldText.ViewModels;

namespace ShieldText;

public partial class MainWindow : Window
{
    private readonly MainViewModel _vm = new();

    public MainWindow()
    {
        InitializeComponent();
        DataContext = _vm;
    }

    // ── Buttons ─────────────────────────────────────────
    private void Encrypt_Click(object sender, RoutedEventArgs e)
    {
        _vm.Encrypt();
        UpdateStatus();
    }

    private void Decrypt_Click(object sender, RoutedEventArgs e)
    {
        _vm.Decrypt();
        UpdateStatus();
    }

    private void Copy_Click(object sender, RoutedEventArgs e)
    {
        _vm.CopyOutput();
        UpdateStatus();
    }

    private void Swap_Click(object sender, RoutedEventArgs e)
    {
        _vm.SwapTexts();
    }

    private void ClearAll_Click(object sender, RoutedEventArgs e)
    {
        _vm.Clear();
        PasswordBox.Clear();
        StatusText.Foreground = new SolidColorBrush(Color.FromRgb(107, 114, 128));
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(_vm.OutputText))
        {
            _vm.StatusMessage = "Nothing to save";
            _vm.IsSuccess = false;
            UpdateStatus();
            return;
        }

        var dialog = new Microsoft.Win32.SaveFileDialog
        {
            Title = "Save Output",
            Filter = "Text File (*.txt)|*.txt",
            FileName = $"shieldtext_{DateTime.Now:yyyyMMdd_HHmmss}"
        };

        if (dialog.ShowDialog() == true)
        {
            System.IO.File.WriteAllText(dialog.FileName, _vm.OutputText);
            _vm.StatusMessage = $"Saved to {System.IO.Path.GetFileName(dialog.FileName)}";
            _vm.IsSuccess = true;
            UpdateStatus();
        }
    }

    // ── Password Binding ─────────────────────────────────
    // PasswordBox به خاطر امنیت از Binding پشتیبانی نمیکنه
    // پس مقدارش رو دستی به ViewModel میدیم
    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        _vm.Password = PasswordBox.Password;
    }

    // ── Show/Hide Password ───────────────────────────────
    private bool _passwordVisible = false;

    private void ShowPassword_Click(object sender, RoutedEventArgs e)
    {
        _passwordVisible = !_passwordVisible;

        if (_passwordVisible)
        {
            PasswordVisible.Text = PasswordBox.Password;
            PasswordBox.Visibility = Visibility.Collapsed;
            PasswordVisible.Visibility = Visibility.Visible;
            ShowPasswordBtn.Content = "🙈 Hide";
        }
        else
        {
            PasswordBox.Password = PasswordVisible.Text;
            PasswordVisible.Visibility = Visibility.Collapsed;
            PasswordBox.Visibility = Visibility.Visible;
            ShowPasswordBtn.Content = "👁 Show";
        }
    }

    private void PasswordVisible_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
        _vm.Password = PasswordVisible.Text;
    }

    // ── Status Color ────────────────────────────────────
    private void UpdateStatus()
    {
        StatusText.Foreground = _vm.IsSuccess
            ? new SolidColorBrush(Color.FromRgb(61, 220, 151))   // سبز
            : new SolidColorBrush(Color.FromRgb(239, 68, 68));    // قرمز
    }
}