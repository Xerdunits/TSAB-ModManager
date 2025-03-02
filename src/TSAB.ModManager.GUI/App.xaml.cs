using System;
using System.Windows;
using TSAB.ModManager.GUI.Views;

namespace TSAB.ModManager.GUI
{
    public partial class App : Application
    {
        [STAThread] // WPF benötigt STAThread für die UI
        public static void Main()
        {
            App app = new App();
            app.InitializeComponent();
            app.Run();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            MainWindow mainWindow = new MainWindow();
            mainWindow.DataContext = new MainWindowViewModel(); // Set the ViewModel as DataContext

            mainWindow.Show();
        }
    }
}
