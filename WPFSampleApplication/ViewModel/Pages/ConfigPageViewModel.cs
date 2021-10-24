using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFSampleApplication.Model;
using WPFSampleApplication.ViewModel;
using WPFSampleApplication.View;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows;
using System.ComponentModel;
using HelperLibrary;

namespace WPFSampleApplication.ViewModel {
    public class ConfigPageViewModel :BaseViewModel {

        public string EmailUsername { get; set; }
        public string TMSServiceUsername { get; set; }

        #region Commands
        public ICommand SaveSettings { get; private set; }
        public ICommand ResetSettingsToDefault { get; private set; }
        #endregion

        private IPagesNavigator _pagesNavigator;
        private bool _wereSettingsChanged;
        private bool _prevAutosaveSetting;
        private (PasswordBox Email, PasswordBox TMSService)? _passBoxes;

        
        public ConfigPageViewModel(ApplicationViewModel applicationViewModel, IPagesNavigator pagesNavigator) : base(applicationViewModel) {
            _pagesNavigator = pagesNavigator;

            _pagesNavigator.NavigatedTo += _pagesNavigator_NavigatedTo;
            _pagesNavigator.NavigateFrom += _pagesNavigator_NavigateFrom;

            AppViewModel.Settings.AnySettingsChanged += Settings_AnySettingsChanged;

            SaveSettings = new Command((obj) => {
                AppViewModel.Settings.SaveToFiles();
                if (_passBoxes != null) {
                    CredentialManager.WriteCreds(AppViewModel.Settings.Process.EmailCredName, EmailUsername, _passBoxes.Value.Email.SecurePassword);
                    CredentialManager.WriteCreds(AppViewModel.Settings.Process.TMSServiceCredName, TMSServiceUsername, _passBoxes.Value.TMSService.SecurePassword);
                }
                _wereSettingsChanged = false;
                AppViewModel.BackgroundProcessModel.UpdateProcessSettings();
            });

            ResetSettingsToDefault = new Command((obj) => {
                AppViewModel.Settings.ResetToDefault();
                _wereSettingsChanged = false;
            });
        }

        private void _pagesNavigator_NavigateFrom(object sender, Page e) {
            if (e.DataContext == this && (_wereSettingsChanged || WereCredentialsChanged())) {
                var result = MessageBox.Show(AppViewModel.AppSubtitles.GetText("msgbox_save_settings_text"), AppViewModel.AppSubtitles.GetText("msgbox_save_settings_caption"),
                    MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.None, MessageBoxOptions.None);

                if (result == MessageBoxResult.Yes) {
                    SaveSettings.Execute(null);
                } else {
                    AppViewModel.Settings.LoadFromFiles();
                }

                AppViewModel.Settings.AutosaveAnyChanges = _prevAutosaveSetting;
            }
        }

        private void _pagesNavigator_NavigatedTo(object sender, Page e) {
            if (e.DataContext == this) {
                //Try to get password boxes. Breaks MVVM, but you can't really bind passwordboxes
                var cfgPage = e as ConfigPage;
                if (_passBoxes == null && cfgPage != null) {
                    _passBoxes = (cfgPage.EmailPassword, cfgPage.TMSServicePassword);
                }

                _wereSettingsChanged = false;
                _prevAutosaveSetting = AppViewModel.Settings.AutosaveAnyChanges;
                AppViewModel.Settings.AutosaveAnyChanges = false;

                if (_passBoxes != null) {
                    var cred = CredentialManager.ReadCredsString(AppViewModel.Settings.Process.EmailCredName);
                    _passBoxes.Value.Email.Password = cred.password;
                    EmailUsername = cred.username;

                    cred = CredentialManager.ReadCredsString(AppViewModel.Settings.Process.TMSServiceCredName);
                    _passBoxes.Value.TMSService.Password = cred.password;
                    TMSServiceUsername = cred.username;
                }
            }
        }

        private bool WereCredentialsChanged() {
            if (_passBoxes != null) {
                var cred = CredentialManager.ReadCredsString(AppViewModel.Settings.Process.EmailCredName);
                if ((cred.username != EmailUsername) || (cred.password != _passBoxes.Value.Email.Password))
                    return true;

                cred = CredentialManager.ReadCredsString(AppViewModel.Settings.Process.TMSServiceCredName);
                if ((cred.username != TMSServiceUsername )|| (cred.password != _passBoxes.Value.TMSService.Password))
                    return true;

                return false;
            } else {
                return false;
            }
        }


        private void Settings_AnySettingsChanged(object sender, EventArgs e) {
            _wereSettingsChanged = true;
        }

        ~ConfigPageViewModel() {
            _pagesNavigator.NavigatedTo -= _pagesNavigator_NavigatedTo;
            _pagesNavigator.NavigateFrom -= _pagesNavigator_NavigateFrom;
        }
    }
}
