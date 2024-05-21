using BankDataTransformation.ViewModels;
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

namespace BankDataTransformation.Windows
{
    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IMainViewModel _windowVM;
        public MainWindow(IMainViewModel windowVM)
        {
            _windowVM = windowVM;
            InitializeComponent();
            this.DataContext = _windowVM;
           

        }

       
        private void LoadFile_Click(object sender, RoutedEventArgs e)
        {
           _windowVM.LoadFileFromPath();
        }

      

        private void AddNewRule_BtnClick(object sender, RoutedEventArgs e)
        {

        }
        private void EditRule_BtnClick(object sender, RoutedEventArgs e)
        {

        }

        private void DeleteRule_BtnClick(object sender, RoutedEventArgs e)
        {
            _windowVM.DeleteRule();
        }

        private void ApplySingleRule_BtnClick(object sender, RoutedEventArgs e)
        {
            _windowVM.ApplyRule();
        }
        private void ApplyAllRules_BtnClick(object sender, RoutedEventArgs e)
        {
            _windowVM.ApplyAllRules();
        }

        private void ResetRules_BtnClick(object sender, RoutedEventArgs e)
        {
            _windowVM.ResetRules();
        }
    }
}
