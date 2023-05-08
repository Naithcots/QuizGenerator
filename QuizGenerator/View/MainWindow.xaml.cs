using Microsoft.Win32;
using QuizGenerator.ViewModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace QuizGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenReadFileDialog_Button(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();

            if(dialog.ShowDialog() == true)
            {
                string destinationFilePath = dialog.FileName;
                (this.DataContext as MainViewModel)?.LoadQuizFromDatabase(destinationFilePath);
            }
        }

        private void OpenSaveFileDialog_Button(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog();

            if (dialog.ShowDialog() == true)
            {
                string destinationFilePath = dialog.FileName;
                (this.DataContext as MainViewModel)?.SaveQuizToDatabase(destinationFilePath);
            }
        }
    }
}
