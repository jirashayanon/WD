using PhotoLibrary.ViewModel;
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

namespace PhotoLibrary.Views
{
    /// <summary>
    /// Interaction logic for StartOQA.xaml
    /// </summary>
    public partial class StartOQA : Window
    {
        public StartOQA()
        {
            InitializeComponent();

            startOQAViewModel.FinishProcess += StartOQAViewModel_FinishProcess;
            startOQAViewModel.ErrorInputMissing += StartOQAViewModel_ErrorInputMissing;

            this.PreviewKeyDown += new KeyEventHandler(HandleEsc);
            TrayBox.Focus();
        }

        private void HandleEsc(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        private void StartOQAViewModel_ErrorInputMissing(object sender, EventArgs e)
        {
            MessageBox.Show("Invalid ID");
        }

        private void StartOQAViewModel_FinishProcess(object sender, EventArgs e)
        {
            this.DialogResult = true;
        }

        private void StackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            StackPanel sp = (StackPanel)sender;
            RadioButton rb = sp.Children[0] as RadioButton;
            rb.IsChecked = true;

            TextBox tb = sp.Children[1] as TextBox;
            if (tb != null)
            {
                tb.Focus();
            }
        }

        public StartOQAViewModel GetDataContext()
        {
            return startOQAViewModel;
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                StartOQAViewModel startOQAViewModel = this.DataContext as StartOQAViewModel;
                startOQAViewModel.ExecuteSubmitCommand();
            }
        }
    }
}
