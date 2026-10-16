using System.Windows.Controls;
using ezRclone.ViewModels;

namespace ezRclone;

public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void DriveLetter_DropDownOpened(object? sender, EventArgs e)
    {
        if (sender is ComboBox { DataContext: RemoteViewModel remote })
            remote.RefreshDriveLetters();
    }
}
