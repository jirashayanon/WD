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
    /// Interaction logic for UpdateSerialNumber.xaml
    /// </summary>
    public partial class UpdateSerialNumber : Window
    {
        public string SerialNumber { get; set; }
        public UpdateSerialNumber(string serial)
        {
            InitializeComponent();

            SerialNumberTextbox.Text = serial;

            this.PreviewKeyDown += new KeyEventHandler(HandleEsc);
            SerialNumberTextbox.Focus();
        }

        private void HandleEsc(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                Finish();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Finish();
        }

        public void Finish()
        {
            SerialNumber = SerialNumberTextbox.Text.ToUpper();
            if (SerialNumber.Contains("?"))
            {
                MessageBox.Show("Please write correct serial number.");
            }
            else
            {
                this.DialogResult = true;
            }
        }
    }
}
