using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TrussMe.Model.Entities;

namespace TrussMe.WPF.Dialogs
{
    /// <summary>
    /// Interaction logic for AddTrussDialog.xaml
    /// </summary>
    public partial class AddTrussDialog : Window
    {

        public AddTrussDialog(IEnumerable<TypeOfLoad> loadList)
        {
            InitializeComponent();
            DataContext = this;
            LoadList = new List<TypeOfLoad>(loadList);
        }

        public AddTrussDialog(ProjectTruss projectTrussToEdit, IEnumerable<TypeOfLoad> loadList):this(loadList)
        {
            TextBoxName.IsEnabled = false;
            FavTruss.Span = projectTrussToEdit.Truss.Span;
            FavTruss.Slope = projectTrussToEdit.Truss.Slope;
            FavTruss.PanelAmount = projectTrussToEdit.Truss.PanelAmount;
            FavTruss.SupportHeight = projectTrussToEdit.Truss.SupportDepth;
            LoadListCB.SelectedItem = LoadList.First(x => x.LoadId == projectTrussToEdit.Truss.LoadId);
            UnitForceCheckBox.IsChecked = projectTrussToEdit.Truss.UnitForce;
            NewProjectTruss = projectTrussToEdit;
        }

        public Truss NewTruss { get; set; }

        public ProjectTruss NewProjectTruss { get; set; } = new ProjectTruss();

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            FavTruss.DrawTruss();
        }

        private void AddTrussBtn_Click(object sender, RoutedEventArgs e)
        {
            NewProjectTruss.Truss = new Truss()
            {
                Span = FavTruss.Span,
                Slope = (float)FavTruss.Slope,
                PanelAmount = FavTruss.PanelAmount,
                SupportDepth = FavTruss.SupportHeight,
                LoadId = (LoadListCB.SelectedItem as TypeOfLoad).LoadId,
                LoadType = (LoadListCB.SelectedItem as TypeOfLoad).LoadType,
                UnitForce = UnitForceCheckBox.IsChecked == true

            };

            DialogResult = true;
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        public List<TypeOfLoad> LoadList
        {
            get => (List<TypeOfLoad>)GetValue(LoadListProperty);
            set => SetValue(LoadListProperty, value);
        }

        // Using a DependencyProperty as the backing store for LoadList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LoadListProperty =
            DependencyProperty.Register("LoadList", typeof(List<TypeOfLoad>), typeof(AddTrussDialog), new PropertyMetadata(null));

        private void TextBox_OnLostFocus(object sender, RoutedEventArgs e)
        {
            FavTruss.DrawTruss();
        }
    }
}
