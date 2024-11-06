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
            this.InitializeComponent();
            AppWindow.SetIcon(
                Path.Combine(AppContext.BaseDirectory, "Assets/pokeball.ico"));
        }

        private void OnRootSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var elm = (FrameworkElement)sender;
            elm.SizeChanged -= OnRootSizeChanged;

            // Edit from original: account for scaling
            var scale = elm.XamlRoot.RasterizationScale;
            var height = (int)Math.Ceiling(elm.DesiredSize.Height * scale);
            var width = (int)Math.Ceiling(elm.DesiredSize.Width * scale);

            AppWindow.ResizeClient(new(width, height));
            //Activate();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ClearDataGridSortDirections();
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
