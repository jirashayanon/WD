using PhotoLibrary.Model;
using PhotoLibrary.ViewModel;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace PhotoLibrary.Views
{
    /// <summary>
    /// Description for Defects.
    /// </summary>
    public partial class DefectsView : Window
    {
        /// <summary>
        /// Initializes a new instance of the Defects class.
        /// </summary>
        public DefectsView()
        {
            InitializeComponent();

            //this.DataContext = new DefectsViewModel();
            this.PreviewKeyDown += new KeyEventHandler(HandleEsc);
        }

        private void HandleEsc(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.DialogResult = false;
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        public List<Defect> GetSelectedDefect()
        {
            return defectsViewModel.GetSelectedDefect();
        }

    }
}