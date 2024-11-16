using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using VolunteeringWpfApp.Models;

namespace VolunteeringWpfApp
{
    /// <summary>
    /// Логика взаимодействия для AddNewEventWindow.xaml
    /// </summary>

    public partial class AddNewEventWindow : Window
    {
        public AddNewEventWindow()
        {
            InitializeComponent();

            try
            {
                using (var context = new VolunteeringDbContext())
                {
                    var regions = context.Regions.ToList();
                    if (regions.Any())
                    {
                        RegionComboBox.ItemsSource = regions;
                        RegionComboBox.IsEnabled = true;
                    }
                    else
                    {
                        RegionComboBox.IsEnabled = false;
                    }
                }
            }
            catch (SqlException ex)
            {
                if ((ex.Message.Contains("A network-related or instance-specific error occurred while establishing a connection to SQL Server")) || (ex.Message.Contains("Служба SQL Server приостановлена. Новые соединения будут отклоняться.")))
                {
                    MessageBox.Show("Невозможно подключиться к серверу базы данных. Проверьте подключение к сети и убедитесь, что сервер базы данных доступен",
                                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    Application.Current.Shutdown();
                }
                else
                {
                    MessageBox.Show("Произошла ошибка при подключении к базе данных: " + ex.Message,
                                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    Application.Current.Shutdown();
                }
                return;
            }
        }

        private void DoneButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var context = new VolunteeringDbContext())
                {
                    context.Database.Migrate();

                    var newEvent = new Event
                    {
                        Name = NameTextBox.Text,
                        StartDateTime = StartDateTimePicker.Value.Value.AddSeconds(-StartDateTimePicker.Value.Value.Second),
                        EndDateTime = EndDateTimePicker.Value.Value.AddSeconds(-EndDateTimePicker.Value.Value.Second),
                        RegionId = (int)RegionComboBox.SelectedValue,
                        Location = LocationTextBox.Text,
                        LastModifiedDateTime = DateTime.Now
                    };

                    if (!string.IsNullOrWhiteSpace(AdditionalInfoTextBox.Text))
                    {
                        newEvent.AdditionalInfo = AdditionalInfoTextBox.Text;
                    }

                    context.Events.Add(newEvent);

                    context.SaveChanges();
                }
            }
            catch (SqlException ex)
            {
                if ((ex.Message.Contains("A network-related or instance-specific error occurred while establishing a connection to SQL Server")) || (ex.Message.Contains("Служба SQL Server приостановлена. Новые соединения будут отклоняться.")))
                {
                    MessageBox.Show("Невозможно подключиться к серверу базы данных. Проверьте подключение к сети и убедитесь, что сервер базы данных доступен",
                                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    Application.Current.Shutdown();
                }
                else
                {
                    MessageBox.Show("Произошла ошибка при подключении к базе данных: " + ex.Message,
                                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    Application.Current.Shutdown();
                }
                return;
            }

            MessageBox.Show("Мероприятие было успешно добавлено", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }

        private void DoneButtonState()
        {
            DoneButton.IsEnabled = NameTextBox.Text.Length > 0 &&
                StartDateTimePicker.Value.HasValue &&
                EndDateTimePicker.Value.HasValue &&
                RegionComboBox.SelectedItem != null &&
                LocationTextBox.Text.Length > 0 &&
                StartDateTimePicker.Value < EndDateTimePicker.Value;
        }

        private void NameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            DoneButtonState();
        }

        private void StartDateTimePicker_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            DoneButtonState();
        }

        private void EndDateTimePicker_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            DoneButtonState();
        }

        private void RegionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DoneButtonState();
        }

        private void LocationTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            DoneButtonState();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }
    }
}
