using System;
using System.Windows.Input;

namespace ErpCrm.Desktop.Core {
    public class RelayCommand : ICommand {
        public event EventHandler CanExecuteChanged;
        public bool CanExecute(object parameter) => true;
        public void Execute(object parameter) {
            // Execute logic
        }
    }
}