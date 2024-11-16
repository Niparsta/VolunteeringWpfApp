using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace VolunteeringWpfApp
{
    /// <summary>
    /// Логика взаимодействия для EditEventWindow.xaml
    /// </summary>

    public partial class EditEventWindow : Window
    {
        string OldName;
        DateTime OldStartDateTime;
        DateTime OldEndDateTime;
        int OldRegionId;
        string OldLocation;
        string? OldAdditionInfo = string.Empty;
        int? _eventId;
        bool DoneButtonStateCheck = false;

        public EditEventWindow(int? eventId)
        {
            _eventId = eventId;
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

                    var eventContext = context.Events.FirstOrDefault(e => e.Id == _eventId);
                    if (eventContext != null)
                    {
                        NameTextBox.Text = eventContext.Name;
                        StartDateTimePicker.Value = eventContext.StartDateTime;
                        EndDateTimePicker.Value = eventContext.EndDateTime;
                        RegionComboBox.SelectedValue = eventContext.RegionId;
                        LocationTextBox.Text = eventContext.Location;
                        AdditionalInfoTextBox.Text = eventContext.AdditionalInfo;

                        OldName = eventContext.Name;
                        OldStartDateTime = eventContext.StartDateTime;
                        OldEndDateTime = eventContext.EndDateTime;
                        OldRegionId = eventContext.RegionId;
                        OldLocation = eventContext.Location;
                        OldAdditionInfo = eventContext.AdditionalInfo ?? string.Empty;

                        DoneButtonStateCheck = true;
                    }

                    var volunteerEvent = context.VolunteerEvents.FirstOrDefault(ve => ve.EventId == _eventId);
                    if (volunteerEvent != null)
                    {
                        StartDateTimePicker.IsEnabled = false;
                        EndDateTimePicker.IsEnabled = false;
                    }
                    else
                    {
                        StartDateTimePicker.IsEnabled = true;
                        EndDateTimePicker.IsEnabled = true;
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


                    var eventContext = context.Events.FirstOrDefault(v => v.Id == _eventId);
                    if (eventContext == null)
                    {
                        MessageBox.Show("Выбранное мероприятие не найдено", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (eventContext.Name != NameTextBox.Text)
                    {
                        eventContext.Name = NameTextBox.Text;
                    }

                    if (eventContext.StartDateTime != StartDateTimePicker.Value)
                    {
                        eventContext.StartDateTime = StartDateTimePicker.Value.Value.AddSeconds(-StartDateTimePicker.Value.Value.Second);
                    }

                    if (eventContext.EndDateTime != EndDateTimePicker.Value)
                    {
                        eventContext.EndDateTime = EndDateTimePicker.Value.Value.AddSeconds(-EndDateTimePicker.Value.Value.Second);
                    }

                    if (eventContext.RegionId != (int)RegionComboBox.SelectedValue)
                    {
                        eventContext.RegionId = (int)RegionComboBox.SelectedValue;
                    }

                    if (eventContext.Location != LocationTextBox.Text)
                    {
                        eventContext.Location = LocationTextBox.Text;
                    }

                    if (eventContext.AdditionalInfo != AdditionalInfoTextBox.Text)
                    {
                        eventContext.AdditionalInfo = string.IsNullOrWhiteSpace(AdditionalInfoTextBox.Text) ? null : AdditionalInfoTextBox.Text;
                    }

                    eventContext.LastModifiedDateTime = DateTime.Now;

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
            MessageBox.Show("Данные выбранного мероприятия были успешно обновлены", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }

        private void DoneButtonState()
        {
            if (DoneButtonStateCheck == true)
            {

                DoneButton.IsEnabled =
                    (NameTextBox.Text != OldName ||
                    StartDateTimePicker.Value != OldStartDateTime ||
                    EndDateTimePicker.Value != OldEndDateTime ||
                    (int)RegionComboBox.SelectedValue != OldRegionId ||
                    LocationTextBox.Text != OldLocation ||
                    AdditionalInfoTextBox.Text != OldAdditionInfo) && StartDateTimePicker.Value < EndDateTimePicker.Value && NameTextBox.Text.Length > 0 &&
                StartDateTimePicker.Value.HasValue &&
                EndDateTimePicker.Value.HasValue &&
                RegionComboBox.SelectedItem != null &&
                LocationTextBox.Text.Length > 0;
            }
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

        private void AdditionalInfoTextBox_TextChanged(object sender, TextChangedEventArgs e)
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
