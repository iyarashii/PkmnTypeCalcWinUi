using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using PkmnTypeCalcWinUi.ViewModels;
using System;
using System.IO;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PkmnTypeCalcWinUi
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // set title bar color to black
            AppWindow.TitleBar.BackgroundColor = Windows.UI.Color.FromArgb(255, 0, 0, 0);
            AppWindow.SetIcon(
                Path.Combine(AppContext.BaseDirectory, "Assets/pokeball.ico"));
            AppWindow.Resize(new Windows.Graphics.SizeInt32(450, 130));
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ClearDataGridSortDirections();
            if ((rootGrid.DataContext as MainWindowViewModel).CalculatedTableVisibility)
            {
                AppWindow.Resize(new Windows.Graphics.SizeInt32(450, 740));
            }
            else
            {
                AppWindow.Resize(new Windows.Graphics.SizeInt32(450, 130));
            }
        }

        private void ClearDataGridSortDirections()
        {
            foreach (var column in dmgCalcDataGrid.Columns)
            {
                column.SortDirection = null;
            }
        }

        private EventHandler<DataGridColumnEventArgs> GetSortHandler()
        {
            return (sender, args) => 
            {
                var dataGrid = (sender as DataGrid);
                // Remove sorting indicators from other columns
                foreach (var dgColumn in dataGrid.Columns)
                {
                    if (dgColumn.Tag.ToString() != args.Column.Tag.ToString())
                    {
                        dgColumn.SortDirection = null;
                    }
                }

                (rootGrid.DataContext as MainWindowViewModel).SortCommand.Execute(args);
            };
        }

      
    }
}
