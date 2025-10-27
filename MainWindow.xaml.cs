using System.Windows;
using Dictionary.ViewModel;

namespace Dictionary;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        this.DataContext = new MainViewModel();
    }
}