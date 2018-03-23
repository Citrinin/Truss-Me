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
using System.Windows.Shapes;
using TrussMe.Model.Entities;

namespace TrussMe.WPF.Dialogs
{
    /// <summary>
    /// Interaction logic for AddProjectDialog.xaml
    /// </summary>
    public partial class AddProjectDialog : Window
    {
        public AddProjectDialog()
        {
            InitializeComponent();
            DataContext = this;
        }
        public Project NewProject { get; set; }

        public string Code
        {
            get => (string)GetValue(CodeProperty);
            set => SetValue(CodeProperty, value);
        }

        // Using a DependencyProperty as the backing store for Code.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CodeProperty =
            DependencyProperty.Register("Code", typeof(string), typeof(AddProjectDialog), new PropertyMetadata(""));

        private void BtnCancel_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void BtnOK_OnClick(object sender, RoutedEventArgs e)
        {
            NewProject = new Project
            {
                ProjectId = 0,
                Code = this.Code,
                Description = this.TextBoxDescription.Text
            };
            DialogResult = true;
        }
    }
}
