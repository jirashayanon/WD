using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Interaction logic for AddDefect.xaml
    /// </summary>
    public partial class AddDefect : Window
    {
        public string DefectName { get; set; }
        public AddDefect()
        {
            InitializeComponent();

            this.PreviewKeyDown += new KeyEventHandler(HandleEsc);

            DefectTextbox.Focus();
        }

        private void HandleEsc(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Finish();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                Finish();
            }
        }

        public void Finish()
        {
            DefectName = DefectTextbox.Text;
            if (DefectName == "")
            {
                MessageBox.Show("Please add defect name", "Warning!");
            }
            else
            {
                this.DialogResult = true;
            }
        }
    }
}
