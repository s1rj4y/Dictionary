using Dictionary.ViewModel;
using System.Windows;

namespace Dictionary;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        this.DataContext = new MainViewModel();
    }
}