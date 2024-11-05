using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI.UI.Controls;
using PokemonTypeLibrary.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PkmnTypeCalcWinUi.ViewModels
{
    public class MainWindowViewModel : ObservableObject
    {
        public static string EmptyTypeName { get; } = PkmnTypeFactory.CreateEmptyPkmnType().TypeName;
        private ObservableCollection<IPkmnType> _pkmnTypeList = new(PkmnTypeFactory.GeneratePkmnTypeList().Where(x => x.TypeName != EmptyTypeName).ToList());
        private IPkmnType _selectedPrimaryType = PkmnTypeFactory.CreateEmptyPkmnType(), _selectedSecondaryType = PkmnTypeFactory.CreateEmptyPkmnType();
        private bool calculatedTableVisibility;
        private ObservableCollection<IPkmnType> primaryPkmnTypeList = new(PkmnTypeFactory.GeneratePkmnTypeList());
        private ObservableCollection<IPkmnType> secondaryPkmnTypeList = new(PkmnTypeFactory.GeneratePkmnTypeList());
        public IRelayCommand<DataGridColumnEventArgs> SortCommand { get; }

        public MainWindowViewModel()
        {
            SortCommand = new RelayCommand<DataGridColumnEventArgs>(OnSortCommandExecuted);
        }
        private void OnSortCommandExecuted(DataGridColumnEventArgs e)
        {
            //Use the Tag property to pass the bound column name for the sorting implementation
            if (e.Column.Tag.ToString() == nameof(PkmnType.TypeName))
            {
                if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
                {
                    PkmnTypeList = new ObservableCollection<IPkmnType>(PkmnTypeList.OrderBy(x => x.TypeName));
                    e.Column.SortDirection = DataGridSortDirection.Ascending;
                }
                else
                {
                    PkmnTypeList = new ObservableCollection<IPkmnType>(PkmnTypeList.OrderByDescending(x => x.TypeName));
                    e.Column.SortDirection = DataGridSortDirection.Descending;
                }
            }
            else if (e.Column.Tag.ToString() == nameof(PkmnType.DmgMultiplier))
            {
                if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
                {
                    PkmnTypeList = new ObservableCollection<IPkmnType>(PkmnTypeList.OrderBy(x => x.DmgMultiplier));
                    e.Column.SortDirection = DataGridSortDirection.Ascending;
                }
                else
                {
                    PkmnTypeList = new ObservableCollection<IPkmnType>(PkmnTypeList.OrderByDescending(x => x.DmgMultiplier));
                    e.Column.SortDirection = DataGridSortDirection.Descending;
                }
            }
        }

        public ObservableCollection<IPkmnType> PrimaryPkmnTypeList
        {
            get => primaryPkmnTypeList;
            set
            {
                primaryPkmnTypeList = value;
                SetProperty(ref primaryPkmnTypeList, value);
            }
        }

        public ObservableCollection<IPkmnType> SecondaryPkmnTypeList 
        {
            get => secondaryPkmnTypeList; 
            set 
            { 
                secondaryPkmnTypeList = value; 
                SetProperty(ref secondaryPkmnTypeList, value); 
            } 
        }
        public bool CalculatedTableVisibility
        {
            get => calculatedTableVisibility;
            set { SetProperty(ref calculatedTableVisibility, value); }
        }
        public ObservableCollection<IPkmnType> PkmnTypeList
        {
            get => _pkmnTypeList;
            set 
            {
                SetProperty(ref _pkmnTypeList, value); 
            }
        }

        public IPkmnType SelectedPrimaryType
        {
            get => PrimaryPkmnTypeList.Where(type => type.TypeName == _selectedPrimaryType.TypeName).Single();
            set
            {
                if (value is not null && value.TypeName != _selectedPrimaryType.TypeName)
                {
                    SetProperty(ref _selectedPrimaryType, value);
                    _selectedPrimaryType = value;
                    Calculate(nameof(SelectedPrimaryType));
                }
            }
        }
        public IPkmnType SelectedSecondaryType
        {
            get => SecondaryPkmnTypeList.Where(type => type.TypeName == _selectedSecondaryType.TypeName).Single();
            set
            {
                if (value is not null && value.TypeName != _selectedSecondaryType.TypeName)
                {
                    SetProperty(ref _selectedSecondaryType, value);
                    _selectedSecondaryType = value;
                    Calculate(nameof(SelectedSecondaryType));
                }
            }
        }

        private IPkmnType? lastRemovedPrimaryType = null;
        private IPkmnType? lastRemovedSecondaryType = null;
        private List<IPkmnType> fullTypeList = PkmnTypeFactory.GeneratePkmnTypeList();
        public void Calculate(string setType)
        {
            // reset list state
            if (lastRemovedPrimaryType != null)
            {
                PrimaryPkmnTypeList.Insert(fullTypeList.IndexOf(fullTypeList.Where(x => x.TypeName == lastRemovedPrimaryType.TypeName).Single()), lastRemovedPrimaryType);
                lastRemovedPrimaryType = null;
            }
            if (lastRemovedSecondaryType != null)
            {
                SecondaryPkmnTypeList.Insert(fullTypeList.IndexOf(fullTypeList.Where(x => x.TypeName == lastRemovedSecondaryType.TypeName).Single()), lastRemovedSecondaryType);
                lastRemovedSecondaryType = null;
            }

            // hide datagrid when both types are set to empty type
            if (_selectedPrimaryType.TypeName == EmptyTypeName && _selectedSecondaryType.TypeName == EmptyTypeName)
            {
                CalculatedTableVisibility = false;
                return;
            }

            // remove already selected type from the other combobox
            if (setType == nameof(SelectedPrimaryType) && _selectedPrimaryType.TypeName != EmptyTypeName)
            {
                lastRemovedSecondaryType = SecondaryPkmnTypeList.Where(type => type.TypeName == _selectedPrimaryType.TypeName).Single();
                SecondaryPkmnTypeList.Remove(lastRemovedSecondaryType);
            }
            else if (setType == nameof(SelectedSecondaryType) && _selectedSecondaryType.TypeName != EmptyTypeName)
            {
                lastRemovedPrimaryType = PrimaryPkmnTypeList.Where(type => type.TypeName == _selectedSecondaryType.TypeName).Single();
                PrimaryPkmnTypeList.Remove(lastRemovedPrimaryType);
            }

            CalculatedTableVisibility = true;

            // calculate damage multiplier for each pkmn type in the list
            foreach (var pkmnType in PkmnTypeList)
            {
                pkmnType.DmgMultiplier = pkmnType.CalculateDmgMultiplier(SelectedPrimaryType, SelectedSecondaryType);
            }

            // sort by damage multiplier from highest to lowest
            PkmnTypeList = new(PkmnTypeList.OrderByDescending(x => x.DmgMultiplier));
        }
    }
}
