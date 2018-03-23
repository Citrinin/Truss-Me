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
    /// Interaction logic for UpdateProjectDialog.xaml
    /// </summary>
    public partial class UpdateProjectDialog : Window
    {
        private Project _project;
        public UpdateProjectDialog(Project project)
        {
            InitializeComponent();
            DataContext = this;
            this._project = project;
            Description = this._project.Description;
            Code = this._project.Code;
        }

        private void BtnOK_OnClick(object sender, RoutedEventArgs e)
        {
            this._project.Description = Description;
            DialogResult = true;
        }

        private void BtnCancel_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }


        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
            "Description", typeof(string), typeof(UpdateProjectDialog), new PropertyMetadata(default(string)));

        public string Description
        {
            get => (string) GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }

        public static readonly DependencyProperty CodeProperty = DependencyProperty.Register(
            "Code", typeof(string), typeof(UpdateProjectDialog), new PropertyMetadata(default(string)));

        public string Code
        {
            get => (string) GetValue(CodeProperty);
            set => SetValue(CodeProperty, value);
        }
    }
}
