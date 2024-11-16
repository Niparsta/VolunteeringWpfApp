using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using VolunteeringWpfApp.Models;

namespace VolunteeringWpfApp
{
    /// <summary>
    /// Логика взаимодействия для AddNewVolunteerEventWindow.xaml
    /// </summary>
    public partial class AddNewVolunteerEventWindow : Window
    {
        public int SelectedEventId = -1;

        public AddNewVolunteerEventWindow()
        {

            InitializeComponent();
            try
            {

                using (var context = new VolunteeringDbContext())
                {
                    var volunteers = context.Volunteers.ToList();
                    if (volunteers.Any())
                    {
                        VolunteerComboBox.ItemsSource = volunteers;
                        VolunteerComboBox.IsEnabled = true;
                    }
                    else
                    {
                        VolunteerComboBox.IsEnabled = false;
                    }

                    var events = context.Events.ToList();
                    if (events.Any())
                    {
                        EventComboBox.ItemsSource = events;
                        EventComboBox.IsEnabled = true;
                    }
                    else
                    {
                        EventComboBox.IsEnabled = false;
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

        private void VolunteerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DoneButtonState();
            UpdateRolesByVolunteer();
        }

        private void EventComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DoneButtonState();
            UpdateRolesByVolunteer();
        }

        private void RoleComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DoneButtonState();
        }

        private void UpdateRolesByVolunteer()
        {
            if (VolunteerComboBox.SelectedValue != null && EventComboBox.SelectedValue != null)
            {
                try
                {
                    using (var context = new VolunteeringDbContext())
                    {
                        int selectedVolunteerId = (int)VolunteerComboBox.SelectedValue;
                        int selectedEventId = (int)EventComboBox.SelectedValue;
                        var volunteer = context.Volunteers.FirstOrDefault(v => v.Id == selectedVolunteerId);
                        if (volunteer != null)
                        {
                            var rolesQuery = context.Roles.AsQueryable();
                            var assignedRoleIds = context.VolunteerEvents.Where(ve => ve.VolunteerId == selectedVolunteerId && ve.EventId == selectedEventId).Select(ve => ve.RoleId).Distinct();
                            if (assignedRoleIds.Any())
                            {
                                rolesQuery = rolesQuery.Where(role => !assignedRoleIds.Contains(role.Id));
                            }
                            if (!volunteer.HasTransport)
                            {
                                rolesQuery = rolesQuery.Where(r => r.Id != 2);
                            }
                            if (rolesQuery.Any())
                            {
                                RoleComboBox.ItemsSource = rolesQuery.ToList();
                                RoleComboBox.SelectedIndex = -1;
                                RoleComboBox.IsEnabled = true;
                            }
                            else
                            {
                                RoleComboBox.SelectedIndex = -1;
                                RoleComboBox.IsEnabled = false;
                            }
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
            else
            {
                RoleComboBox.SelectedIndex = -1;
                RoleComboBox.IsEnabled = false;
            }
        }

        private void DoneButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var context = new VolunteeringDbContext())
                {
                    context.Database.Migrate();
                    var volunteerId = (int)VolunteerComboBox.SelectedValue;
                    var eventId = (int)EventComboBox.SelectedValue;
                    var newVolunteerEvent = new VolunteerEvent
                    {
                        VolunteerId = volunteerId,
                        EventId = eventId,
                        RoleId = (int)RoleComboBox.SelectedValue,
                        LastModifiedDateTime = DateTime.Now
                    };
                    context.VolunteerEvents.Add(newVolunteerEvent);
                    context.SaveChanges();

                    SelectedEventId = newVolunteerEvent.EventId;
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
            MessageBox.Show("Волонтёрская активность была успешно добавлена", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }

        private void DoneButtonState()
        {
            if (VolunteerComboBox.SelectedItem != null && EventComboBox.SelectedItem != null)
            {
                try
                {
                    using (var context = new VolunteeringDbContext())
                    {
                        context.Database.Migrate();
                        var volunteerId = (int)VolunteerComboBox.SelectedValue;
                        var eventId = (int)EventComboBox.SelectedValue;
                        var currentVolunteer = context.Volunteers.FirstOrDefault(v => v.Id == volunteerId);
                        var currentEvent = context.Events.FirstOrDefault(e => e.Id == eventId);
                        var eligibilityDate = currentVolunteer.DateOfBirth.Date.AddDays(1);

                        bool isEligible = currentEvent.EndDateTime.Date >= eligibilityDate;

                        DoneButton.IsEnabled = VolunteerComboBox.SelectedItem != null &&
                            EventComboBox.SelectedItem != null &&
                            RoleComboBox.SelectedItem != null &&
                            isEligible;
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