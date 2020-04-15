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
    public partial class DefectChooseOne : Window
    {
        public Defect CurrentDefect { get; private set; }

        /// <summary>
        /// Initializes a new instance of the Defects class.
        /// </summary>
        public DefectChooseOne()
        {
            InitializeComponent();

            defectsViewModel.Suspension[0].IsSelected = true;
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
            List<Defect> list_defect = defectsViewModel.GetSelectedDefect();
            if (list_defect.Count == 1)
            {
                CurrentDefect = list_defect[0];
                this.DialogResult = true;
            }
            else
            {
                MessageBox.Show("Please select defect");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        public List<Defect> GetSelectedDefect()
        {
            return defectsViewModel.GetSelectedDefect();
        }

        public void SetDefect(string defectName)
        {
            defectsViewModel.SetDefectSelected(defectName);
        }
    }
}