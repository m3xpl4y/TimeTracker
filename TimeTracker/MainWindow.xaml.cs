using System.Windows;

namespace TimeTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
            Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            if (DataContext is MainWindowViewModel vm && !vm.OnClose)
            {
                var stopCommand = vm.StopCommand;
                var result = MessageBox.Show("Do you really want to close?", "Close TimeTracker",
                    MessageBoxButton.OKCancel, MessageBoxImage.Question);
                if (result == MessageBoxResult.OK)
                {
                    if (!vm.IsStopButtonEnabled)
                    {
                        e.Cancel = true;
                    }
                    stopCommand!.Execute(e);
                    e.Cancel = false;
                }

            }
        }
    }
}
