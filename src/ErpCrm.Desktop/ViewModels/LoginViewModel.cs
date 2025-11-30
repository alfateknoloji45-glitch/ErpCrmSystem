namespace ErpCrm.Desktop.ViewModels {
    public class LoginViewModel : ViewModelBase {
        private LoginRequest _loginRequest = new LoginRequest();
        public LoginRequest LoginRequest {
            get => _loginRequest;
            set {
                _loginRequest = value;
                OnPropertyChanged();
            }
        }
    }
}