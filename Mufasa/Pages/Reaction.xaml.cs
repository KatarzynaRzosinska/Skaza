using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using Mufasa.BackEnd.Designer;
using System.Globalization;
using System.ComponentModel;
using System.Collections.Specialized;

namespace Mufasa.Pages
{

    /// <summary>
    /// Interaction logic for Reaction.xaml
    public partial class Reaction : UserControl
    {
        public Reaction()
        {
            InitializeComponent();
            if (Design.Designer.ConstructionList.Count() != 0)
            {
                InitializeFragmentsListBox();
            }
        }

        private double dNTP;
        private double poly;
        private double water;
        private double maxWater;
        private double buffer;


        /// <summary>
        /// Fragment list.
        /// </summary>
        private ObservableCollection<Fragment> fragmentList;


        /// <summary>
        /// Reaction list box initialization.
        /// </summary>
        private void InitializeFragmentsListBox()
        {
            
            dNTP = (double)Design.Designer.Settings.ReactionVolume / 100.0;
            poly = (double)Design.Designer.Settings.ReactionVolume / 50.0;
            buffer = (double)Design.Designer.Settings.ReactionVolume / 5.0;
            dNTPTextBlock.Text = dNTP.ToString();
            polyTextBlock.Text = poly.ToString();
            bufferTextBlock.Text = buffer.ToString();
            maxWater = (double)Design.Designer.Settings.ReactionVolume - dNTP - poly - buffer;
            water = maxWater;
            waterTextBlock.Text = water.ToString();

            fragmentList = new ObservableCollection<Fragment>();
            for (int i = 0; i < Design.Designer.ConstructionList.Count(); i++)
            {
                Fragment f = Design.Designer.FragmentDict[Design.Designer.ConstructionList[i]];
                
                if(i==0)
                {
                    f.IsVector = true;
                }
                else
                {
                    f.IsVector = false;
                }
                fragmentList.Add(f);
            }

            this.Items = new ObservableCollection<FragmentViewModel>(fragmentList.Select(m => new FragmentViewModel(m)));
            this.Fragments = (ListCollectionView)CollectionViewSource.GetDefaultView(this.Items);

            this.Items.CollectionChanged += new NotifyCollectionChangedEventHandler(Items_CollectionChanged);
            foreach (var m in this.Items)
                m.PropertyChanged += new PropertyChangedEventHandler(Item_PropertyChanged);

            concentrationsDataGrid.ItemsSource = Fragments;
        }

        //Concentrations were changed
        void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //Update the collection view if refresh isn't possible
            if (this.Fragments.IsEditingItem)
                this.Fragments.CommitEdit();
            if (this.Fragments.IsAddingNew)
                this.Fragments.CommitNew();

            this.Fragments.Refresh();
            double volume = 0.0;
            for (int i = 0; i < Fragments.Count; i++ )
            {
                concentrationsDataGrid.UpdateLayout();
                concentrationsDataGrid.ScrollIntoView(concentrationsDataGrid.Items[i]);
                DataGridRow row = (DataGridRow)concentrationsDataGrid.ItemContainerGenerator.ContainerFromIndex(i);
                DataGridCell RowColumn = concentrationsDataGrid.Columns[2].GetCellContent(row).Parent as DataGridCell;
                string cellValue = ((TextBlock)RowColumn.Content).Text;
                double vol = Double.Parse(cellValue);
                if(!Double.IsInfinity(vol))
                    volume += vol;
            }

            water = maxWater - volume;
            waterTextBlock.Text = water.ToString();
            waterTextBlock.UpdateLayout();

        }

        //Items were added or removed
        void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //Attach the observer for the properties
            if (e.NewItems != null)
                foreach (var vm in e.NewItems.OfType<FragmentViewModel>())
                    vm.PropertyChanged += Item_PropertyChanged;

            //Refresh when it is possible
            if (!this.Fragments.IsAddingNew && !this.Fragments.IsEditingItem)            
                this.Fragments.Refresh();
        }

        private ObservableCollection<FragmentViewModel> Items { get; set; }

        public ListCollectionView Fragments { get; set; }

        /// <summary>
        /// IsVisible click event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.InitializeFragmentsListBox();
        }

        private void concentrationsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }           
    }
}
