using System.Windows;
using System.Windows.Input;

namespace PhotoLibrary.Views
{
    /// <summary>
    /// Description for StartWindow.
    /// </summary>
    public partial class StartView : Window
    {
        /// <summary>
        /// Initializes a new instance of the StartWindow class.
        /// </summary>
        public StartView()
        {
            InitializeComponent();

            this.NameTextBox.Focus();

            this.PreviewKeyDown += new KeyEventHandler(HandleEsc);
        }

        private void HandleEsc(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Submit();
        }

        private void Submit()
        {
            int round;
            if (NameTextBox.Text.Length == 0)
            {
                MessageBox.Show("Please enter your name");
            }
            else if (!int.TryParse(RoundTextBox.Text, out round))
            {
                MessageBox.Show("Please enter round (integer)");
            }
            else
            {
                this.DialogResult = true;
            }
        }

        public string GetName()
        {
            return NameTextBox.Text;
        }

        public int GetRound()
        {
            int round = 1;
            if (int.TryParse(RoundTextBox.Text, out round))
            {
                return round;
            }
            else
            {
                return 1;
            }
        }

        private void TextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                Submit();
            }
        }
    }
}