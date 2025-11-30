using System.Windows.Input;

namespace ErpCrm.Desktop.Core;

/// <summary>
/// ICommand implementasyonu
/// MVVM pattern için komut bağlama sağlar
/// </summary>
public class RelayCommand : ICommand
{
    private readonly Action<object?> _execute;
    private readonly Func<object?, bool>? _canExecute;

    /// <summary>
    /// Yeni RelayCommand oluşturur
    /// </summary>
    /// <param name="execute">Çalıştırılacak aksiyon</param>
    /// <param name="canExecute">Çalıştırılabilirlik kontrolü (opsiyonel)</param>
    public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    /// <summary>
    /// Parametre olmadan çalışan komut için constructor
    /// </summary>
    /// <param name="execute">Çalıştırılacak aksiyon</param>
    public RelayCommand(Action execute) : this(_ => execute())
    {
    }

    /// <summary>
    /// CanExecute değişiklik olayı
    /// </summary>
    public event EventHandler? CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }

    /// <summary>
    /// Komutun çalıştırılabilir olup olmadığını kontrol eder
    /// </summary>
    /// <param name="parameter">Komut parametresi</param>
    /// <returns>Çalıştırılabilirse true</returns>
    public bool CanExecute(object? parameter)
    {
        return _canExecute?.Invoke(parameter) ?? true;
    }

    /// <summary>
    /// Komutu çalıştırır
    /// </summary>
    /// <param name="parameter">Komut parametresi</param>
    public void Execute(object? parameter)
    {
        _execute(parameter);
    }

    /// <summary>
    /// CanExecute durumunu yeniden değerlendirir
    /// </summary>
    public void RaiseCanExecuteChanged()
    {
        CommandManager.InvalidateRequerySuggested();
    }
}

/// <summary>
/// Async komutlar için RelayCommand
/// </summary>
public class AsyncRelayCommand : ICommand
{
    private readonly Func<object?, Task> _execute;
    private readonly Func<object?, bool>? _canExecute;
    private bool _isExecuting;

    /// <summary>
    /// Yeni AsyncRelayCommand oluşturur
    /// </summary>
    /// <param name="execute">Çalıştırılacak async aksiyon</param>
    /// <param name="canExecute">Çalıştırılabilirlik kontrolü (opsiyonel)</param>
    public AsyncRelayCommand(Func<object?, Task> execute, Func<object?, bool>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    /// <summary>
    /// Parametre olmadan çalışan async komut için constructor
    /// </summary>
    /// <param name="execute">Çalıştırılacak async aksiyon</param>
    public AsyncRelayCommand(Func<Task> execute) : this(_ => execute())
    {
    }

    /// <summary>
    /// CanExecute değişiklik olayı
    /// </summary>
    public event EventHandler? CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }

    /// <summary>
    /// Komutun çalıştırılabilir olup olmadığını kontrol eder
    /// </summary>
    /// <param name="parameter">Komut parametresi</param>
    /// <returns>Çalıştırılabilirse true</returns>
    public bool CanExecute(object? parameter)
    {
        return !_isExecuting && (_canExecute?.Invoke(parameter) ?? true);
    }

    /// <summary>
    /// Komutu çalıştırır
    /// </summary>
    /// <param name="parameter">Komut parametresi</param>
    public async void Execute(object? parameter)
    {
        if (_isExecuting)
        {
            return;
        }

        _isExecuting = true;
        RaiseCanExecuteChanged();

        try
        {
            await _execute(parameter);
        }
        finally
        {
            _isExecuting = false;
            RaiseCanExecuteChanged();
        }
    }

    /// <summary>
    /// CanExecute durumunu yeniden değerlendirir
    /// </summary>
    public void RaiseCanExecuteChanged()
    {
        CommandManager.InvalidateRequerySuggested();
    }
}