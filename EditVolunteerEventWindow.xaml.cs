using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace VolunteeringWpfApp
{
    /// <summary>
    /// Логика взаимодействия для EditVolunteerEventWindow.xaml
    /// </summary>
    public partial class EditVolunteerEventWindow : Window
    {
        int? _volunteerEventId;
        public int SelectedEventId = -1;
        int OldVolunteerId;
        int OldEventId;
        int OldRoleId;
        bool DoneButtonStateCheck = false;

        public EditVolunteerEventWindow(int? volunteerEventId)
        {
            InitializeComponent();
            try
            {
                using (var context = new VolunteeringDbContext())
                {
                    _volunteerEventId = volunteerEventId;

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

                    var volunteerEventContext = context.VolunteerEvents.FirstOrDefault(ve => ve.Id == _volunteerEventId);
                    if (volunteerEventContext != null)
                    {
                        VolunteerComboBox.SelectedValue = volunteerEventContext.VolunteerId;
                        EventComboBox.SelectedValue = volunteerEventContext.EventId;
                        RoleComboBox.SelectedValue = volunteerEventContext.RoleId;

                        OldVolunteerId = volunteerEventContext.VolunteerId;
                        OldEventId = volunteerEventContext.EventId;
                        OldRoleId = volunteerEventContext.RoleId;

                        DoneButtonStateCheck = true;

                    }
                }
                UpdateRolesByVolunteer();
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

                    var volunteerEventContext = context.VolunteerEvents.FirstOrDefault(ve => ve.Id == _volunteerEventId);
                    if (volunteerEventContext == null)
                    {
                        MessageBox.Show("Волонтёрская активность не найдена", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (volunteerEventContext.VolunteerId != (int)VolunteerComboBox.SelectedValue)
                    {
                        volunteerEventContext.VolunteerId = (int)VolunteerComboBox.SelectedValue;
                    }

                    if (volunteerEventContext.EventId != (int)EventComboBox.SelectedValue)
                    {
                        volunteerEventContext.EventId = (int)EventComboBox.SelectedValue;
                    }

                    if (volunteerEventContext.RoleId != (int)RoleComboBox.SelectedValue)
                    {
                        volunteerEventContext.RoleId = (int)RoleComboBox.SelectedValue;
                    }

                    volunteerEventContext.LastModifiedDateTime = DateTime.Now;

                    context.SaveChanges();

                    SelectedEventId = volunteerEventContext.EventId;
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

            MessageBox.Show("Данные выбранной волонтерской активности были успешно обновлены", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
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

                        if (DoneButtonStateCheck == true)
                        {
                            bool allComboBoxesFilled = VolunteerComboBox.SelectedItem != null &&
                                EventComboBox.SelectedItem != null &&
                                RoleComboBox.SelectedItem != null;

                            bool anyValueChanged = (int)VolunteerComboBox.SelectedValue != OldVolunteerId ||
                                (int)EventComboBox.SelectedValue != OldEventId ||
                                (RoleComboBox.SelectedValue != null && (int)RoleComboBox.SelectedValue != OldRoleId);

                            bool isEligible = currentEvent.EndDateTime.Date >= eligibilityDate;

                            DoneButton.IsEnabled = allComboBoxesFilled && anyValueChanged && isEligible;
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

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }
    }
}