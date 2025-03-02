using System.Windows.Input;
using TSAB.ModManager.GUI.ViewModels.Helpers;
using TSAB.ModManager.GUI.ViewModels;
using TSAB.ModManager.GUI.Views;

public class MainWindowViewModel : ViewModelBase
{
    private object _currentView;

    // Property to track the currently active view
    public object CurrentView
    {
        get => _currentView;
        set
        {
            _currentView = value;
            OnPropertyChanged(nameof(CurrentView));
        }
    }

    // Commands to switch between views
    public ICommand ShowModsViewCommand { get; }
    public ICommand ShowSettingsViewCommand { get; }
    public ICommand ShowGameFilesViewCommand { get; }

    public MainWindowViewModel()
    {
        ShowModsViewCommand = new RelayCommand(_ => CurrentView = new ModsView());
        ShowSettingsViewCommand = new RelayCommand(_ => CurrentView = new SettingsView());
        ShowGameFilesViewCommand = new RelayCommand(_ => CurrentView = new GameFilesView());

        // Set the default view to ModsView
        CurrentView = new ModsView();
    }
}
