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
    /// Логика взаимодействия для VacanciesRequestWindow.xaml
    /// </summary>
    public partial class VacanciesRequestWindow : Window
    {
        HhApiClient _client;
        Database _db;

        public VacanciesRequestWindow(HhApiClient client, Database db) 
        {
            _db = db;
            _client = client;
            InitializeComponent();
            ExperienceComboBox.ItemsSource = GetValues<Experience>();
            EmploymentComboBox.ItemsSource = GetValues<Employment>();
            ScheduleComboBox.ItemsSource = GetValues<Schedule>();
        }

        private List<string> GetValues<TEnum>() where TEnum : struct, Enum
        {
            return Enum.GetNames<TEnum>().Append(string.Empty).ToList();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {          

            try
            {
                var dd = await _db.GetTopCitiesByVacanyCount();

                var request = new VacanciesRequest()
                {
                    Page = !string.IsNullOrWhiteSpace(PageTexBox.Text) ? int.Parse(PageTexBox.Text) : 0,
                    PageSize = !string.IsNullOrWhiteSpace(PageSizeTextBox.Text) ? int.Parse(PageSizeTextBox.Text) : 0,
                    Salary = !string.IsNullOrWhiteSpace(SalaryTexBox.Text) ? long.Parse(SalaryTexBox.Text) : null,
                    OnlyWithSalary = OnlyWithSalaryTextBox.IsChecked == true,
                    Experience = !string.IsNullOrWhiteSpace(ExperienceComboBox.Text) ? Enum.Parse<Experience>(ExperienceComboBox.Text) : null,
                    Employment = !string.IsNullOrWhiteSpace(EmploymentComboBox.Text) ? Enum.Parse<Employment>(EmploymentComboBox.Text) : null,
                    Schedule = !string.IsNullOrWhiteSpace(ScheduleComboBox.Text) ? Enum.Parse<Schedule>(ScheduleComboBox.Text) : null,
                    DateFrom = DateFromPicer.SelectedDate?.Date == null? null: DateOnly.FromDateTime(DateFromPicer.SelectedDate.Value.Date),
                    DateTo = DateToPicer.SelectedDate?.Date == null ? null : DateOnly.FromDateTime(DateToPicer.SelectedDate.Value.Date),
                };

                var response = await _client.GetVacancies(request);
                
                if(response is null)
                {
                    MessageBox.Show("Запрос завешился с ошибкой");
                    return;
                }

                MessageBox.Show($"Запрос успешно прошел. Доступно странийц: {response.Pages}", "", MessageBoxButton.OK, MessageBoxImage.Information);

                try
                {
                    await _db.MergeVacancies(response.Items);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                MessageBox.Show("Данные успешно сохранены");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
