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

namespace FindJob
{
    /// <summary>
    /// Логика взаимодействия для AnaliticsWindow.xaml
    /// </summary>
    public partial class AnaliticsWindow : Window
    {

        Database _db;
        

        public AnaliticsWindow(Database db)
        {
          
            InitializeComponent();
            _db = db;
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var minMaxZp = await _db.GetBiggestAndLowestSalaryAsync();
            var topSities = await _db.GetTopCitiesByVacanyCount();
            var avgZp = await _db.GetAvgSalary();
            ZpData.ItemsSource = new[] { minMaxZp.Biggest, minMaxZp.Lowest };
            VacanciData.ItemsSource = topSities;
            AvgZpText.Text = avgZp.ToString(); 
        }

    }
}
