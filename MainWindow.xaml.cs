using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using VolunteeringWpfApp.Models;
using Bold = DocumentFormat.OpenXml.Wordprocessing.Bold;
using FontSize = DocumentFormat.OpenXml.Wordprocessing.FontSize;
using Path = System.IO.Path;
using Run = DocumentFormat.OpenXml.Wordprocessing.Run;
using RunProperties = DocumentFormat.OpenXml.Wordprocessing.RunProperties;
using Text = DocumentFormat.OpenXml.Wordprocessing.Text;


namespace VolunteeringWpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int? volunteersDataGridSelectedRowId;
        private int? eventsDataGridSelectedRowId;
        private int? volunteersEventsDataGridSelectedRowId;
        State AppState = new State();

        public MainWindow()
        {
            InitializeComponent();
            AppState = State.LoadFromFile();
            MyWindow.Height = AppState.MainWindowHeight;
            MyWindow.Width = AppState.MainWindowWidth;
            MyWindow.WindowState = AppState.MainWindowState;

            try
            {
                using (var context = new VolunteeringDbContext())
                {
                    context.Database.Migrate();

                    if (!context.Roles.Any())
                    {
                        var roles = new List<Role>
                    {
                        new Role { Name = "Медик" },
                        new Role { Name = "Помощник по транспортировке" },
                        new Role { Name = "Куратор социального медиа-контента" },
                        new Role { Name = "Переводчик" },
                        new Role { Name = "Технический ассистент" },
                        new Role { Name = "Волонтер по безопасности" },
                        new Role { Name = "Помощник по питанию" },
                        new Role { Name = "Промоутер мероприятий" },
                        new Role { Name = "Специалист по анкетированию" },
                        new Role { Name = "Ассистент по сбору пожертвований" },
                        new Role { Name = "Экологический волонтер" },
                        new Role { Name = "Координатор зоны адаптации" },
                        new Role { Name = "Поддержка маломобильных гостей" },
                        new Role { Name = "Ответственный за проведение" }
                    };

                        context.Roles.AddRange(roles);
                    };

                    if (!context.Regions.Any())
                    {
                        var regions = new List<Region>
                    {
                        new Region { Name = "Республика Адыгея" },
                        new Region { Name = "Республика Алтай" },
                        new Region { Name = "Республика Башкортостан" },
                        new Region { Name = "Республика Бурятия" },
                        new Region { Name = "Республика Дагестан" },
                        new Region { Name = "Донецкая Народная Республика" },
                        new Region { Name = "Республика Ингушетия" },
                        new Region { Name = "Кабардино-Балкарская Республика" },
                        new Region { Name = "Республика Калмыкия" },
                        new Region { Name = "Карачаево-Черкесская Республика" },
                        new Region { Name = "Республика Карелия" },
                        new Region { Name = "Республика Коми" },
                        new Region { Name = "Республика Крым" },
                        new Region { Name = "Луганская Народная Республика" },
                        new Region { Name = "Республика Марий Эл" },
                        new Region { Name = "Республика Мордовия" },
                        new Region { Name = "Республика Саха (Якутия)" },
                        new Region { Name = "Республика Северная Осетия — Алания" },
                        new Region { Name = "Республика Татарстан" },
                        new Region { Name = "Республика Тыва" },
                        new Region { Name = "Удмуртская Республика" },
                        new Region { Name = "Республика Хакасия" },
                        new Region { Name = "Чеченская Республика" },
                        new Region { Name = "Чувашская Республика" },
                        new Region { Name = "Алтайский край" },
                        new Region { Name = "Забайкальский край" },
                        new Region { Name = "Камчатский край" },
                        new Region { Name = "Краснодарский край" },
                        new Region { Name = "Красноярский край" },
                        new Region { Name = "Пермский край" },
                        new Region { Name = "Приморский край" },
                        new Region { Name = "Ставропольский край" },
                        new Region { Name = "Хабаровский край" },
                        new Region { Name = "Амурская область" },
                        new Region { Name = "Архангельская область" },
                        new Region { Name = "Астраханская область" },
                        new Region { Name = "Белгородская область" },
                        new Region { Name = "Брянская область" },
                        new Region { Name = "Владимирская область" },
                        new Region { Name = "Волгоградская область" },
                        new Region { Name = "Вологодская область" },
                        new Region { Name = "Воронежская область" },
                        new Region { Name = "Запорожская область" },
                        new Region { Name = "Ивановская область" },
                        new Region { Name = "Иркутская область" },
                        new Region { Name = "Калининградская область" },
                        new Region { Name = "Калужская область" },
                        new Region { Name = "Кемеровская область" },
                        new Region { Name = "Кировская область" },
                        new Region { Name = "Костромская область" },
                        new Region { Name = "Курганская область" },
                        new Region { Name = "Курская область" },
                        new Region { Name = "Ленинградская область" },
                        new Region { Name = "Липецкая область" },
                        new Region { Name = "Магаданская область" },
                        new Region { Name = "Московская область" },
                        new Region { Name = "Мурманская область" },
                        new Region { Name = "Нижегородская область" },
                        new Region { Name = "Новгородская область" },
                        new Region { Name = "Новосибирская область" },
                        new Region { Name = "Омская область" },
                        new Region { Name = "Оренбургская область" },
                        new Region { Name = "Орловская область" },
                        new Region { Name = "Пензенская область" },
                        new Region { Name = "Псковская область" },
                        new Region { Name = "Ростовская область" },
                        new Region { Name = "Рязанская область" },
                        new Region { Name = "Самарская область" },
                        new Region { Name = "Саратовская область" },
                        new Region { Name = "Сахалинская область" },
                        new Region { Name = "Свердловская область" },
                        new Region { Name = "Смоленская область" },
                        new Region { Name = "Тамбовская область" },
                        new Region { Name = "Тверская область" },
                        new Region { Name = "Томская область" },
                        new Region { Name = "Тульская область" },
                        new Region { Name = "Тюменская область" },
                        new Region { Name = "Ульяновская область" },
                        new Region { Name = "Херсонская область" },
                        new Region { Name = "Челябинская область" },
                        new Region { Name = "Ярославская область" },
                        new Region { Name = "Москва" },
                        new Region { Name = "Санкт-Петербург" },
                        new Region { Name = "Севастополь" },
                        new Region { Name = "Еврейская автономная область" },
                        new Region { Name = "Ненецкий автономный округ" },
                        new Region { Name = "Ханты-Мансийский автономный округ — Югра" },
                        new Region { Name = "Чукотский автономный округ" },
                        new Region { Name = "Ямало-Ненецкий автономный округ" }
                    };
                        context.Regions.AddRange(regions);
                    };

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
        }

        private void AddNewVolunteer()
        {
            var newWindow = new AddNewVolunteerWindow();
            newWindow.ShowDialog();

            LoadVolunteersDataGrid();
        }

        private void AddNewEvent()
        {
            var newWindow = new AddNewEventWindow();
            newWindow.ShowDialog();

            LoadEventsDataGrid();
            if (TabControl.SelectedIndex == 2)
            {
                LoadCurrentEventComboBox();
            }
        }

        private void AddNewVolunteerEvent()
        {
            var newWindow = new AddNewVolunteerEventWindow();
            newWindow.ShowDialog();

            if (newWindow.SelectedEventId == -1)
            {
                if (CurrentEventComboBox.IsEnabled && CurrentEventComboBox.SelectedValue != null)
                {
                    LoadVolunteerEventsDataGrid((int)CurrentEventComboBox.SelectedValue);
                }
            }
            else
            {
                LoadVolunteerEventsDataGrid(newWindow.SelectedEventId);
                CurrentEventComboBox.SelectedValue = newWindow.SelectedEventId;
            }
        }

        private void EditVolunteer()
        {
            if (volunteersDataGridSelectedRowId.HasValue)
            {
                var newWindow = new EditVolunteerWindow(volunteersDataGridSelectedRowId);
                newWindow.ShowDialog();
                LoadVolunteersDataGrid();
            }
            else
            {
                MessageBox.Show("Ни одна строка не была выбрана в таблице", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditEvent()
        {
            if (eventsDataGridSelectedRowId.HasValue)
            {
                var newWindow = new EditEventWindow(eventsDataGridSelectedRowId);
                newWindow.ShowDialog();

                LoadEventsDataGrid();
            }
            else
            {
                MessageBox.Show("Ни одна строка не была выбрана в таблице", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditVolunteerEvent()
        {
            if (volunteersEventsDataGridSelectedRowId.HasValue)
            {
                var newWindow = new EditVolunteerEventWindow(volunteersEventsDataGridSelectedRowId);
                newWindow.ShowDialog();

                if (newWindow.SelectedEventId == -1)
                {
                    if (CurrentEventComboBox.IsEnabled && CurrentEventComboBox.SelectedValue != null)
                    {
                        LoadVolunteerEventsDataGrid((int)CurrentEventComboBox.SelectedValue);
                    }
                }
                else
                {
                    LoadVolunteerEventsDataGrid(newWindow.SelectedEventId);
                    CurrentEventComboBox.SelectedValue = newWindow.SelectedEventId;
                }
            }
            else
            {
                MessageBox.Show("Ни одна строка не была выбрана в таблице", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task DeleteVolunteer()
        {
            MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить выбранного волонтёра?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                if (volunteersDataGridSelectedRowId.HasValue)
                {
                    try
                    {
                        using (var context = new VolunteeringDbContext())
                        {
                            var volunteers = await context.Volunteers
                                .FindAsync(volunteersDataGridSelectedRowId.Value);

                            if (volunteers != null)
                            {
                                context.Volunteers.Remove(volunteers);
                                await context.SaveChangesAsync();
                                MessageBox.Show("Волонтёр успешно удален", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            else
                            {
                                MessageBox.Show("Волонтёр с указанным идентификатором не найден", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    }
                }
            }
            LoadVolunteersDataGrid();
        }

        private async Task DeleteEvent()
        {
            MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить выбранное мероприятие?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                if (eventsDataGridSelectedRowId.HasValue)
                {
                    try
                    {
                        using (var context = new VolunteeringDbContext())
                        {
                            var events = await context.Events
                                .FindAsync(eventsDataGridSelectedRowId.Value);

                            if (events != null)
                            {
                                context.Events.Remove(events);
                                await context.SaveChangesAsync();
                                MessageBox.Show("Мероприятие успешно удалено", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            else
                            {
                                MessageBox.Show("Мероприятие с указанным идентификатором не найдено", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    }
                }
            }
            LoadEventsDataGrid();
        }

        private async Task DeleteVolunteerEvent()
        {
            MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить выбранную волонтёрскую активность?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                if (volunteersEventsDataGridSelectedRowId.HasValue)
                {
                    try
                    {
                        using (var context = new VolunteeringDbContext())
                        {
                            var volunteerEvent = await context.VolunteerEvents
                                .FindAsync(volunteersEventsDataGridSelectedRowId.Value);

                            if (volunteerEvent != null)
                            {
                                context.VolunteerEvents.Remove(volunteerEvent);
                                await context.SaveChangesAsync();
                                MessageBox.Show("Волонтёрская активная успешно удалена", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            else
                            {
                                MessageBox.Show("Волонтёрская активность с указанным идентификатором не найдена", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    }
                }
                else
                {
                    MessageBox.Show("Ни одна строка не была выбрана в таблице", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            LoadVolunteerEventsDataGrid((int)CurrentEventComboBox.SelectedValue);
        }

        private async void LoadVolunteersDataGrid()
        {
            EditVolunteerButton.IsEnabled = false;
            DeleteVolunteerButton.IsEnabled = false;
            EditVolunteerMenuItem.IsEnabled = false;
            DeleteVolunteerMenuItem.IsEnabled = false;
            ExportToWordTextDocumentButton.IsEnabled = false;
            ExportToWordTextDocumentMenuItem.IsEnabled = false;
            VolunteerImage.Source = null;
            VolunteerImage.Visibility = Visibility.Hidden;
            VolunteerImageTextBlock.Visibility = Visibility.Visible;
            VolunteerImageTextBlock.Text = "Выберите волонтера в таблице, чтобы показать его изображение";

            try
            {
                using (var context = new VolunteeringDbContext())
                {
                    var volunteers = await context.Volunteers.Include(r => r.Region).ToListAsync();

                    var programDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    var imagesDirectory = Path.Combine(programDirectory, "Images");
                    var achievementsDirectory = Path.Combine(programDirectory, "Achievements");

                    var validPhotoFileNames = volunteers
                        .Where(v => !string.IsNullOrEmpty(v.PhotoFileName))
                        .Select(v => v.PhotoFileName)
                        .ToHashSet(StringComparer.OrdinalIgnoreCase);

                    var validAchievementFileNames = volunteers
                            .SelectMany(v => v.AchievementsFileNames ?? Array.Empty<string>())
                            .ToHashSet(StringComparer.OrdinalIgnoreCase);

                    foreach (var volunteer in volunteers)
                    {
                        if (!string.IsNullOrEmpty(volunteer.PhotoFileName))
                        {
                            var photoPath = Path.Combine(imagesDirectory, volunteer.PhotoFileName);
                            if (!File.Exists(photoPath))
                            {
                                volunteer.PhotoFileName = null;
                            }
                        }

                        if (volunteer.AchievementsFileNames != null && volunteer.AchievementsFileNames.Length > 0)
                        {
                            var validAchievements = new List<string>(volunteer.AchievementsFileNames);

                            foreach (var achievementFileName in volunteer.AchievementsFileNames)
                            {
                                var achievementPath = Path.Combine(achievementsDirectory, achievementFileName);
                                if (!File.Exists(achievementPath))
                                {
                                    validAchievements.Remove(achievementFileName);
                                }
                            }

                            volunteer.AchievementsFileNames = validAchievements.Count > 0 ? validAchievements.ToArray() : null;
                        }
                    }

                    if (Directory.Exists(imagesDirectory))
                    {
                        var imageFiles = Directory.GetFiles(imagesDirectory);
                        foreach (var imageFile in imageFiles)
                        {
                            var fileName = Path.GetFileName(imageFile);
                            if (!validPhotoFileNames.Contains(fileName))
                            {
                                File.Delete(imageFile);
                            }
                        }
                    }

                    if (Directory.Exists(achievementsDirectory))
                    {
                        var achievementFiles = Directory.GetFiles(achievementsDirectory);

                        foreach (var achievementFile in achievementFiles)
                        {
                            var fileName = Path.GetFileName(achievementFile);
                            if (!validAchievementFileNames.Contains(fileName))
                            {
                                File.Delete(achievementFile);
                            }
                        }
                    }

                    await context.SaveChangesAsync();

                    VolunteersDataGrid.ItemsSource = null;
                    VolunteersDataGrid.ItemsSource = volunteers;

                    ExportToExcelSpreadsheetButton.IsEnabled = volunteers.Any();
                    ExportToExcelSpreadsheetMenuItem.IsEnabled = volunteers.Any();
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
            }
        }

        private async void LoadEventsDataGrid()
        {
            EditEventButton.IsEnabled = false;
            DeleteEventButton.IsEnabled = false;
            EditEventMenuItem.IsEnabled = false;
            DeleteEventMenuItem.IsEnabled = false;
            ExportToWordTextDocumentButton.IsEnabled = false;
            VolunteerImageTextBlock.Text = "Выберите волонтера в таблице, чтобы показать его изображение";
            ExportToWordTextDocumentMenuItem.IsEnabled = false;
            try
            {
                using (var context = new VolunteeringDbContext())
                {
                    var events = await context.Events.Include(r => r.Region).ToListAsync();
                    EventsDataGrid.ItemsSource = events;
                    if (events.Any())
                    {
                        ExportToExcelSpreadsheetButton.IsEnabled = true;
                        ExportToExcelSpreadsheetMenuItem.IsEnabled = true;
                    }
                    else
                    {
                        ExportToExcelSpreadsheetButton.IsEnabled = false;
                        ExportToExcelSpreadsheetMenuItem.IsEnabled = false;
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

        private async void LoadVolunteerEventsDataGrid(int eventId)
        {
            VolunteerImageTextBlock.Text = "Выберите волонтера в таблице, чтобы показать его изображение";
            EditVolunteerEventButton.IsEnabled = false;
            DeleteVolunteerEventButton.IsEnabled = false;
            EditVolunteerEventMenuItem.IsEnabled = false;
            DeleteVolunteerEventMenuItem.IsEnabled = false;
            ExportToWordTextDocumentButton.IsEnabled = false;
            ExportToWordTextDocumentMenuItem.IsEnabled = false;
            try
            {
                using (var context = new VolunteeringDbContext())
                {
                    var volunteerEvent = await context.VolunteerEvents.Where(e => e.EventId == eventId).Include(v => v.Volunteer).Include(r => r.Role).ToListAsync();

                    VolunteerEventsDataGrid.ItemsSource = volunteerEvent;
                    if (CurrentEventComboBox.SelectedIndex != -1 && CurrentEventComboBox.IsEnabled == true && volunteerEvent.Any())
                    {
                        ExportToExcelSpreadsheetButton.IsEnabled = true;
                        ExportToExcelSpreadsheetMenuItem.IsEnabled = true;
                    }
                    else
                    {
                        ExportToExcelSpreadsheetButton.IsEnabled = false;
                        ExportToExcelSpreadsheetMenuItem.IsEnabled = false;
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

        private async void LoadCurrentEventComboBox()
        {
            try
            {
                var selectedValueBeforeUpdate = CurrentEventComboBox.SelectedValue;

                using (var context = new VolunteeringDbContext())
                {
                    var events = await context.Events.ToListAsync();
                    if (events.Any())
                    {
                        CurrentEventComboBox.IsEnabled = true;
                        CurrentEventComboBox.ItemsSource = events;

                        if (selectedValueBeforeUpdate != null)
                        {
                            var selectedEvent = events.FirstOrDefault(e => e.Id == (int)selectedValueBeforeUpdate);
                            if (selectedEvent != null)
                            {
                                CurrentEventComboBox.SelectedValue = selectedValueBeforeUpdate;
                            }
                            else
                            {
                                CurrentEventComboBox.SelectedValue = events.First().Id;
                            }
                        }

                    }
                    else
                    {
                        CurrentEventComboBox.IsEnabled = false;
                        CurrentEventComboBox.ItemsSource = null;
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

        public void ImportVolunteersFromExcelSpreadsheet(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    MessageBox.Show("Файл не найден. Проверьте путь к файлу и повторите попытку", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                using (var spreadsheetDocument = SpreadsheetDocument.Open(filePath, false))
                {
                    var workbookPart = spreadsheetDocument.WorkbookPart;
                    var sheet = workbookPart.Workbook.Sheets.Elements<Sheet>().FirstOrDefault(s => s.Name == "Волонтёры");
                    if (sheet == null)
                    {
                        MessageBox.Show("Лист с данными не найден", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var worksheetPart = (WorksheetPart)workbookPart.GetPartById(sheet.Id);
                    var sheetData = worksheetPart.Worksheet.Elements<SheetData>().FirstOrDefault();

                    if (sheetData == null)
                    {
                        MessageBox.Show("Нет данных для импорта", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    using (var context = new VolunteeringDbContext())
                    {
                        context.Volunteers.RemoveRange(context.Volunteers);
                        foreach (var row in sheetData.Elements<Row>().Skip(1))
                        {
                            var cells = row.Elements<Cell>().ToList();

                            string regionName = GetCellValue(workbookPart, cells[5]);
                            var region = context.Regions.FirstOrDefault(r => r.Name == regionName);

                            if (region == null)
                            {
                                MessageBox.Show($"Регион '{regionName}' не найден в базе данных. Проверьте данные и повторите попытку", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }

                            var volunteer = new Volunteer
                            {
                                LastName = GetCellValue(workbookPart, cells[0]),
                                FirstName = GetCellValue(workbookPart, cells[1]),
                                MiddleName = GetCellValue(workbookPart, cells[2]),
                                DateOfBirth = DateTime.ParseExact(GetCellValue(workbookPart, cells[3]), "dd.MM.yyyy", null),
                                Snils = GetCellValue(workbookPart, cells[4]),
                                Email = GetCellValue(workbookPart, cells[6]),
                                Phone = GetCellValue(workbookPart, cells[7]),
                                HasTransport = GetCellValue(workbookPart, cells[8]) == "Да",
                                AdditionalInfo = GetCellValue(workbookPart, cells[9]),
                                RegionId = region.Id,
                                LastModifiedDateTime = DateTime.Now
                            };

                            context.Volunteers.Add(volunteer);
                        }

                        context.SaveChanges();
                    }
                    LoadVolunteersDataGrid();
                }

                MessageBox.Show("Данные успешно импортированы из файла Excel", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (IOException ex)
            {
                MessageBox.Show("Ошибка при чтении файла: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (FormatException ex)
            {
                MessageBox.Show("Ошибка при обработке данных файла: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Ошибка при работе с базой данных: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void ImportEventsFromExcelSpreadsheet(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    MessageBox.Show("Файл не найден. Проверьте путь к файлу и повторите попытку", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                using (var spreadsheetDocument = SpreadsheetDocument.Open(filePath, false))
                {
                    var workbookPart = spreadsheetDocument.WorkbookPart;
                    var sheet = workbookPart.Workbook.Sheets.Elements<Sheet>().FirstOrDefault(s => s.Name == "Мероприятия");
                    if (sheet == null)
                    {
                        MessageBox.Show("Лист с данными 'Мероприятия' не найден", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var worksheetPart = (WorksheetPart)workbookPart.GetPartById(sheet.Id);
                    var sheetData = worksheetPart.Worksheet.Elements<SheetData>().FirstOrDefault();

                    if (sheetData == null)
                    {
                        MessageBox.Show("Нет данных для импорта", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    using (var context = new VolunteeringDbContext())
                    {
                        context.Events.RemoveRange(context.Events);
                        context.SaveChanges();

                        foreach (var row in sheetData.Elements<Row>().Skip(1))
                        {
                            var cells = row.Elements<Cell>().ToList();

                            string regionName = GetCellValue(workbookPart, cells[3]);
                            var region = context.Regions.FirstOrDefault(r => r.Name == regionName);

                            if (region == null)
                            {
                                MessageBox.Show($"Регион '{regionName}' не найден в базе данных. Проверьте данные и повторите попытку", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }

                            var eventEntry = new Event
                            {
                                Name = GetCellValue(workbookPart, cells[0]),
                                StartDateTime = DateTime.ParseExact(GetCellValue(workbookPart, cells[1]), "dd.MM.yyyy HH:mm", null),
                                EndDateTime = DateTime.ParseExact(GetCellValue(workbookPart, cells[2]), "dd.MM.yyyy HH:mm", null),
                                RegionId = region.Id,
                                Location = GetCellValue(workbookPart, cells[4]),
                                AdditionalInfo = GetCellValue(workbookPart, cells[5]),
                                LastModifiedDateTime = DateTime.Now
                            };

                            context.Events.Add(eventEntry);
                        }

                        context.SaveChanges();
                    }
                    LoadEventsDataGrid();
                }

                MessageBox.Show("Данные успешно импортированы из файла Excel", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (IOException ex)
            {
                MessageBox.Show("Ошибка при чтении файла: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (FormatException ex)
            {
                MessageBox.Show("Ошибка при обработке данных файла: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Ошибка при работе с базой данных: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GetCellValue(WorkbookPart workbookPart, Cell cell)
        {
            if (cell == null) return string.Empty;

            var value = cell.InnerText;
            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            {
                var sharedStringTablePart = workbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
                if (sharedStringTablePart != null)
                {
                    value = sharedStringTablePart.SharedStringTable.ElementAt(int.Parse(value)).InnerText;
                }
            }
            return value;
        }

        private void ImportFromExcelSpreedsheetSelection()
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Книга Excel|*.xlsx",
                Title = "Открыть"
            };
            if (TabControl.SelectedIndex == 0)
            {
                if (openFileDialog.ShowDialog() == true)
                {
                    ImportVolunteersFromExcelSpreadsheet(openFileDialog.FileName);
                }
            }

            else if (TabControl.SelectedIndex == 1)
            {
                if (openFileDialog.ShowDialog() == true)
                {
                    ImportEventsFromExcelSpreadsheet(openFileDialog.FileName);
                }
            }
            else if (TabControl.SelectedIndex == 2)
            {
                return;
            }
        }

        public void ImportVolunteersFromXMLFile(string filePath)
        {
            try
            {
                using (var context = new VolunteeringDbContext())
                {
                    var xDocument = XDocument.Load(filePath);

                    if (xDocument.Root?.Name != "Volunteers")
                    {
                        MessageBox.Show("Неверный формат файла: отсутствует ожидаемый тег 'Volunteers'", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var volunteersElements = xDocument.Element("Volunteers").Elements("Volunteer");

                    foreach (var volunteerElement in volunteersElements)
                    {
                        var volunteer = new Volunteer
                        {
                            FirstName = volunteerElement.Element("FirstName").Value,
                            LastName = volunteerElement.Element("LastName").Value,
                            MiddleName = string.IsNullOrWhiteSpace(volunteerElement.Element("MiddleName")?.Value) ? null : volunteerElement.Element("MiddleName").Value,
                            DateOfBirth = DateTime.Parse(volunteerElement.Element("DateOfBirth").Value),
                            Snils = volunteerElement.Element("Snils").Value,
                            RegionId = int.Parse(volunteerElement.Element("RegionId").Value),
                            Email = volunteerElement.Element("Email")?.Value,
                            Phone = volunteerElement.Element("Phone")?.Value,
                            HasTransport = bool.Parse(volunteerElement.Element("HasTransport").Value),
                            AdditionalInfo = string.IsNullOrWhiteSpace(volunteerElement.Element("AdditionalInfo")?.Value) ? null : volunteerElement.Element("AdditionalInfo").Value,
                            PhotoFileName = string.IsNullOrWhiteSpace(volunteerElement.Element("PhotoFileName")?.Value) ? null : volunteerElement.Element("PhotoFileName").Value,
                            LastModifiedDateTime = DateTime.Parse(volunteerElement.Element("LastModifiedDateTime").Value)
                        };


                        var achievementsElement = volunteerElement.Element("AchievementsFileNames");
                        if (achievementsElement != null && achievementsElement.Elements("FileName").Any())
                        {
                            volunteer.AchievementsFileNames = achievementsElement
                                .Elements("FileName")
                                .Select(e => e.Value)
                                .ToArray();
                        }
                        else
                        {
                            volunteer.AchievementsFileNames = null;
                        }
                        var existingVolunteer = context.Volunteers.FirstOrDefault(v => v.Snils == volunteer.Snils);
                        if (existingVolunteer != null)
                        {
                            existingVolunteer.FirstName = volunteer.FirstName;
                            existingVolunteer.LastName = volunteer.LastName;
                            existingVolunteer.MiddleName = volunteer.MiddleName;
                            existingVolunteer.DateOfBirth = volunteer.DateOfBirth;
                            existingVolunteer.Snils = volunteer.Snils;
                            existingVolunteer.RegionId = volunteer.RegionId;
                            existingVolunteer.Email = volunteer.Email;
                            existingVolunteer.Phone = volunteer.Phone;
                            existingVolunteer.HasTransport = volunteer.HasTransport;
                            existingVolunteer.AdditionalInfo = volunteer.AdditionalInfo;
                            existingVolunteer.PhotoFileName = volunteer.PhotoFileName;
                            existingVolunteer.LastModifiedDateTime = volunteer.LastModifiedDateTime;

                            if (volunteer.AchievementsFileNames != null)
                            {
                                existingVolunteer.AchievementsFileNames = volunteer.AchievementsFileNames;
                            }
                        }
                        else
                        {
                            context.Volunteers.Add(volunteer);
                        }
                    }

                    context.SaveChanges();
                    MessageBox.Show("Данные успешно импортированы из файла", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при импорте данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            LoadVolunteersDataGrid();
        }

        public void ImportEventsFromXMLFile(string filePath)
        {
            try
            {
                using (var context = new VolunteeringDbContext())
                {
                    var xDocument = XDocument.Load(filePath);

                    if (xDocument.Root?.Name != "Events")
                    {
                        MessageBox.Show("Неверный формат файла: отсутствует ожидаемый тег 'Events'", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var eventsElements = xDocument.Element("Events").Elements("Event");

                    foreach (var eventElement in eventsElements)
                    {
                        var eventItem = new Event
                        {
                            Name = eventElement.Element("Name").Value,
                            StartDateTime = DateTime.Parse(eventElement.Element("StartDateTime").Value),
                            EndDateTime = DateTime.Parse(eventElement.Element("EndDateTime").Value),
                            RegionId = int.Parse(eventElement.Element("RegionId").Value),
                            Location = eventElement.Element("Location").Value,
                            AdditionalInfo = string.IsNullOrWhiteSpace(eventElement.Element("AdditionalInfo")?.Value) ? null : eventElement.Element("AdditionalInfo").Value,
                            LastModifiedDateTime = DateTime.Parse(eventElement.Element("LastModifiedDateTime").Value)
                        };

                        var existingEvent = context.Events.FirstOrDefault(e =>
                            e.Name == eventItem.Name &&
                            e.StartDateTime == eventItem.StartDateTime);

                        if (existingEvent != null)
                        {
                            existingEvent.Name = eventItem.Name;
                            existingEvent.StartDateTime = eventItem.StartDateTime;
                            existingEvent.EndDateTime = eventItem.EndDateTime;
                            existingEvent.RegionId = eventItem.RegionId;
                            existingEvent.Location = eventItem.Location;
                            existingEvent.AdditionalInfo = eventItem.AdditionalInfo;
                            existingEvent.LastModifiedDateTime = eventItem.LastModifiedDateTime;
                        }
                        else
                        {
                            context.Events.Add(eventItem);
                        }
                    }

                    context.SaveChanges();
                    MessageBox.Show("Данные успешно импортированы из файла", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при импорте данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            LoadEventsDataGrid();
        }



        private void ImportFromXMLFileSelection()
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Данные XML|*.xml",
                Title = "Открыть"
            };
            if (TabControl.SelectedIndex == 0)
            {
                if (openFileDialog.ShowDialog() == true)
                {
                    ImportVolunteersFromXMLFile(openFileDialog.FileName);
                }
            }

            else if (TabControl.SelectedIndex == 1)
            {
                if (openFileDialog.ShowDialog() == true)
                {
                    ImportEventsFromXMLFile(openFileDialog.FileName);
                }
            }
            else if (TabControl.SelectedIndex == 2)
            {
                return;
            }
        }

        public void ExportVolunteersToExcelSpreadsheet(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    try
                    {
                        using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                        {
                            fileStream.Close();
                        }
                    }
                    catch (IOException)
                    {
                        MessageBox.Show("Файл уже используется другим процессом. Пожалуйста, закройте его и попробуйте снова", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                using (var context = new VolunteeringDbContext())
                {
                    var volunteers = context.Volunteers.Include(r => r.Region).ToList();

                    if (volunteers.Count == 0)
                    {
                        MessageBox.Show("Нет данных для экспорта", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    using (var spreadsheetDocument = SpreadsheetDocument.Create(filePath, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook))
                    {
                        WorkbookPart workbookPart = spreadsheetDocument.AddWorkbookPart();
                        workbookPart.Workbook = new Workbook();

                        WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                        worksheetPart.Worksheet = new Worksheet(new SheetData());

                        Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.AppendChild(new Sheets());
                        Sheet sheet = new Sheet()
                        {
                            Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart),
                            SheetId = 1,
                            Name = "Волонтёры"
                        };
                        sheets.Append(sheet);

                        SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                        Row headerRow = new Row();
                        headerRow.Append(
                            new Cell() { DataType = CellValues.String, CellValue = new CellValue("Фамилия") },
                            new Cell() { DataType = CellValues.String, CellValue = new CellValue("Имя") },
                            new Cell() { DataType = CellValues.String, CellValue = new CellValue("Отчество") },
                            new Cell() { DataType = CellValues.String, CellValue = new CellValue("Дата рождения") },
                            new Cell() { DataType = CellValues.String, CellValue = new CellValue("СНИЛС") },
                            new Cell() { DataType = CellValues.String, CellValue = new CellValue("Регион проживания") },
                            new Cell() { DataType = CellValues.String, CellValue = new CellValue("Адрес электронной почты") },
                            new Cell() { DataType = CellValues.String, CellValue = new CellValue("Номер телефона") },
                            new Cell() { DataType = CellValues.String, CellValue = new CellValue("Наличие транспортного средства") },
                            new Cell() { DataType = CellValues.String, CellValue = new CellValue("Дополнительная информация") }
                        );

                        sheetData.Append(headerRow);

                        foreach (var currentVolunteer in volunteers)
                        {
                            Row row = new Row();
                            row.Append(
                                new Cell() { DataType = CellValues.String, CellValue = new CellValue(currentVolunteer.LastName) },
                                new Cell() { DataType = CellValues.String, CellValue = new CellValue(currentVolunteer.FirstName) },
                                new Cell() { DataType = CellValues.String, CellValue = new CellValue(currentVolunteer.MiddleName ?? string.Empty) },
                                new Cell() { DataType = CellValues.String, CellValue = new CellValue(currentVolunteer.DateOfBirth.ToString("dd.MM.yyyy")) },
                                new Cell() { DataType = CellValues.String, CellValue = new CellValue(currentVolunteer.Snils) },
                                new Cell() { DataType = CellValues.String, CellValue = new CellValue(currentVolunteer.Region.Name) },
                                new Cell() { DataType = CellValues.String, CellValue = new CellValue(currentVolunteer.Email) },
                                new Cell() { DataType = CellValues.String, CellValue = new CellValue(currentVolunteer.Phone) },
                                new Cell() { DataType = CellValues.String, CellValue = new CellValue(currentVolunteer.HasTransport ? "Да" : "Нет") },
                                new Cell() { DataType = CellValues.String, CellValue = new CellValue(currentVolunteer.AdditionalInfo ?? string.Empty) }
                                );
                            sheetData.Append(row);
                        }

                        workbookPart.Workbook.Save();
                    }

                    MessageBox.Show($"Данные успешно экспортированы в файл: {filePath}", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
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

        public void ExportEventsToExcelSpreadsheet(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    try
                    {
                        using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                        {
                            fileStream.Close();
                        }
                    }
                    catch (IOException)
                    {
                        MessageBox.Show("Файл уже используется другим процессом. Пожалуйста, закройте его и попробуйте снова", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                using (var context = new VolunteeringDbContext())
                {
                    var events = context.Events.Include(e => e.Region).ToList();

                    if (events.Count == 0)
                    {
                        MessageBox.Show("Нет данных для экспорта", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    using (var spreadsheetDocument = SpreadsheetDocument.Create(filePath, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook))
                    {
                        WorkbookPart workbookPart = spreadsheetDocument.AddWorkbookPart();
                        workbookPart.Workbook = new Workbook();

                        WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                        worksheetPart.Worksheet = new Worksheet(new SheetData());

                        Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.AppendChild(new Sheets());
                        Sheet sheet = new Sheet()
                        {
                            Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart),
                            SheetId = 1,
                            Name = "Мероприятия"
                        };
                        sheets.Append(sheet);

                        SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                        Row headerRow = new Row();
                        headerRow.Append(
                            new Cell() { DataType = CellValues.String, CellValue = new CellValue("Название") },
                            new Cell() { DataType = CellValues.String, CellValue = new CellValue("Дата и время начала") },
                            new Cell() { DataType = CellValues.String, CellValue = new CellValue("Дата и время окончания") },
                            new Cell() { DataType = CellValues.String, CellValue = new CellValue("Регион") },
                            new Cell() { DataType = CellValues.String, CellValue = new CellValue("Место проведения") },
                            new Cell() { DataType = CellValues.String, CellValue = new CellValue("Дополнительная информация") }
                        );

                        sheetData.Append(headerRow);

                        foreach (var currentEvent in events)
                        {
                            Row row = new Row();
                            row.Append(
                                new Cell() { DataType = CellValues.String, CellValue = new CellValue(currentEvent.Name) },
                                new Cell() { DataType = CellValues.String, CellValue = new CellValue(currentEvent.StartDateTime.ToString("dd.MM.yyyy HH:mm")) },
                                new Cell() { DataType = CellValues.String, CellValue = new CellValue(currentEvent.EndDateTime.ToString("dd.MM.yyyy HH:mm")) },
                                new Cell() { DataType = CellValues.String, CellValue = new CellValue(currentEvent.Region.Name) },
                                new Cell() { DataType = CellValues.String, CellValue = new CellValue(currentEvent.Location) },
                                new Cell() { DataType = CellValues.String, CellValue = new CellValue(currentEvent.AdditionalInfo ?? string.Empty) }
                                );
                            sheetData.Append(row);
                        }

                        workbookPart.Workbook.Save();
                    }

                    MessageBox.Show($"Данные успешно экспортированы в файл: {filePath}", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
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

        public void ExportVolunteerEventsToExcelSpreadsheet(string filePath, int eventId)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    try
                    {
                        using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                        {
                            fileStream.Close();
                        }
                    }
                    catch (IOException)
                    {
                        MessageBox.Show("Файл уже используется другим процессом. Пожалуйста, закройте его и попробуйте снова", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                using (var context = new VolunteeringDbContext())
                {
                    var volunteerEvents = context.VolunteerEvents.Where(e => e.EventId == eventId).Include(v => v.Volunteer).Include(r => r.Role).ToList();

                    if (volunteerEvents.Count == 0)
                    {
                        MessageBox.Show("Нет данных для экспорта", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    using (var spreadsheetDocument = SpreadsheetDocument.Create(filePath, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook))
                    {
                        WorkbookPart workbookPart = spreadsheetDocument.AddWorkbookPart();
                        workbookPart.Workbook = new Workbook();

                        WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                        worksheetPart.Worksheet = new Worksheet(new SheetData());

                        Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.AppendChild(new Sheets());
                        Sheet sheet = new Sheet()
                        {
                            Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart),
                            SheetId = 1,
                            Name = "Волонтёрская активность"
                        };
                        sheets.Append(sheet);

                        SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                        Row headerRow = new Row();
                        headerRow.Append(
                            new Cell() { DataType = CellValues.String, CellValue = new CellValue("Фамилия") },
                            new Cell() { DataType = CellValues.String, CellValue = new CellValue("Имя") },
                            new Cell() { DataType = CellValues.String, CellValue = new CellValue("Отчество") },
                            new Cell() { DataType = CellValues.String, CellValue = new CellValue("СНИЛС") },
                            new Cell() { DataType = CellValues.String, CellValue = new CellValue("Роль") }
                        );

                        sheetData.Append(headerRow);

                        foreach (var currentVolunteerEvent in volunteerEvents)
                        {
                            Row row = new Row();
                            row.Append(
                                new Cell() { DataType = CellValues.String, CellValue = new CellValue(currentVolunteerEvent.Volunteer.LastName) },
                                new Cell() { DataType = CellValues.String, CellValue = new CellValue(currentVolunteerEvent.Volunteer.FirstName) },
                                new Cell() { DataType = CellValues.String, CellValue = new CellValue(currentVolunteerEvent.Volunteer.MiddleName ?? string.Empty) },
                                new Cell() { DataType = CellValues.String, CellValue = new CellValue(currentVolunteerEvent.Volunteer.Snils) },
                                new Cell() { DataType = CellValues.String, CellValue = new CellValue(currentVolunteerEvent.Role.Name) }
                                );
                            sheetData.Append(row);
                        }

                        workbookPart.Workbook.Save();
                    }

                    MessageBox.Show($"Данные успешно экспортированы в файл: {filePath}", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
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

        private void ExportToExcelSpreedsheetSelection()
        {
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Книга Excel|*.xlsx",
                Title = "Сохранить как"
            };
            if (TabControl.SelectedIndex == 0)
            {
                if (saveFileDialog.ShowDialog() == true)
                {
                    ExportVolunteersToExcelSpreadsheet(saveFileDialog.FileName);
                }
            }

            else if (TabControl.SelectedIndex == 1)
            {
                if (saveFileDialog.ShowDialog() == true)
                {
                    ExportEventsToExcelSpreadsheet(saveFileDialog.FileName);
                }
            }
            else if (TabControl.SelectedIndex == 2)
            {
                if (saveFileDialog.ShowDialog() == true)
                {
                    ExportVolunteerEventsToExcelSpreadsheet(saveFileDialog.FileName, (int)CurrentEventComboBox.SelectedValue);
                }
            }
        }

        public void ExportVolunteerToWordTextDocument(string filePath, int? volunteerId)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    try
                    {
                        using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                        {
                            fileStream.Close();
                        }
                    }
                    catch (IOException)
                    {
                        MessageBox.Show("Файл уже используется другим процессом. Пожалуйста, закройте его и попробуйте снова", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                using (var context = new VolunteeringDbContext())
                {
                    var currentVolunteer = context.Volunteers.Where(v => v.Id == volunteerId).Include(r => r.Region).FirstOrDefault();

                    if (currentVolunteer == null)
                    {
                        MessageBox.Show("Нет данных для экспорта", "Информация", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document))
                    {
                        MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                        mainPart.Document = new Document(new Body());

                        Body body = mainPart.Document.Body;

                        Paragraph titleParagraph = new Paragraph(new Run(new Text("Информация о волонтёре")));
                        titleParagraph.ParagraphProperties = new ParagraphProperties
                        {
                            Justification = new Justification { Val = JustificationValues.Center }
                        };
                        titleParagraph.Descendants<Run>().First().RunProperties = new RunProperties
                        {
                            Bold = new Bold(),
                            FontSize = new FontSize { Val = "32" },
                            RunFonts = new RunFonts { Ascii = "Calibri Light", HighAnsi = "Calibri Light" },
                        };
                        body.AppendChild(titleParagraph);


                        AppendInfo(body, "Фамилия", currentVolunteer.LastName);
                        AppendInfo(body, "Имя", currentVolunteer.FirstName);
                        if (currentVolunteer.MiddleName != null)
                        {
                            AppendInfo(body, "Отчество", currentVolunteer.MiddleName);
                        }
                        AppendInfo(body, "Дата рождения", currentVolunteer.DateOfBirth.ToString("dd.MM.yyyy"));
                        AppendInfo(body, "СНИЛС", currentVolunteer.Snils);
                        AppendInfo(body, "Регион проживания", currentVolunteer.Region.Name);
                        AppendInfo(body, "Адрес электронной почты", currentVolunteer.Email);
                        AppendInfo(body, "Номер телефона", currentVolunteer.Phone);
                        AppendInfo(body, "Наличие транспортного средства", currentVolunteer.HasTransport ? "Да" : "Нет");
                        if (currentVolunteer.AdditionalInfo != null)
                        {
                            AppendInfo(body, "Дополнительная информация", currentVolunteer.AdditionalInfo);
                        }

                        var volunteerEvents = context.VolunteerEvents.Where(v => v.VolunteerId == volunteerId).Include(e => e.Event).Include(r => r.Role).Select(ve => $"{ve.Event.Name} ({ve.Event.StartDateTime.ToString("dd.MM.yyyy HH:mm")} - {ve.Event.EndDateTime.ToString("dd.MM.yyyy HH:mm")}, {ve.Role.Name})").ToList();

                        if (volunteerEvents.IsNullOrEmpty())
                        {
                            AppendInfo(body, "Участие в мероприятиях", "Отсутствует");
                        }
                        else
                        {
                            AppendInfo(body, "Участие в мероприятиях", string.Empty);
                            foreach (var eventInfo in volunteerEvents)
                            {
                                AppendBulletPoint(body, eventInfo);
                            }
                        }
                        mainPart.Document.Save();
                    }

                    MessageBox.Show($"Данные успешно экспортированы в файл: {filePath}", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
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

        public void ExportEventToWordTextDocument(string filePath, int? eventId)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    try
                    {
                        using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                        {
                            fileStream.Close();
                        }
                    }
                    catch (IOException)
                    {
                        MessageBox.Show("Файл уже используется другим процессом. Пожалуйста, закройте его и попробуйте снова", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                using (var context = new VolunteeringDbContext())
                {
                    var currentEvent = context.Events.Where(v => v.Id == eventId).Include(r => r.Region).FirstOrDefault();

                    if (currentEvent == null)
                    {
                        MessageBox.Show("Нет данных для экспорта", "Информация", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document))
                    {
                        MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                        mainPart.Document = new Document(new Body());

                        Body body = mainPart.Document.Body;

                        Paragraph titleParagraph = new Paragraph(new Run(new Text("Информация о мероприятии")));
                        titleParagraph.ParagraphProperties = new ParagraphProperties
                        {
                            Justification = new Justification { Val = JustificationValues.Center }
                        };
                        titleParagraph.Descendants<Run>().First().RunProperties = new RunProperties
                        {
                            Bold = new Bold(),
                            FontSize = new FontSize { Val = "32" },
                            RunFonts = new RunFonts { Ascii = "Calibri Light", HighAnsi = "Calibri Light" },
                        };
                        body.AppendChild(titleParagraph);


                        AppendInfo(body, "Название", currentEvent.Name);
                        AppendInfo(body, "Дата и время начала", currentEvent.StartDateTime.ToString("dd.MM.yyyy HH:mm"));
                        AppendInfo(body, "Дата и время окончания", currentEvent.EndDateTime.ToString("dd.MM.yyyy HH:mm"));
                        AppendInfo(body, "Регион", currentEvent.Region.Name);
                        AppendInfo(body, "Место проведения", currentEvent.Location);
                        if (currentEvent.AdditionalInfo != null)
                        {
                            AppendInfo(body, "Дополнительная информация", currentEvent.AdditionalInfo);
                        }

                        var volunteerEvents = context.VolunteerEvents
                            .Where(ve => ve.EventId == eventId)
                            .Include(v => v.Volunteer)
                            .Include(r => r.Role)
                            .GroupBy(v => v.Volunteer)
                            .Select(g => $"{g.Key.LastName} {g.Key.FirstName} {(string.IsNullOrEmpty(g.Key.MiddleName) ? string.Empty : g.Key.MiddleName)} ({string.Join(", ", g.Select(ve => ve.Role.Name))})")
                            .ToList();

                        if (!volunteerEvents.Any())
                        {
                            AppendInfo(body, "Участие в мероприятиях", "Отсутствует");
                        }
                        else
                        {
                            AppendInfo(body, "Участие в мероприятиях", string.Empty);
                            foreach (var eventInfo in volunteerEvents)
                            {
                                AppendBulletPoint(body, eventInfo);
                            }
                        }

                        mainPart.Document.Save();
                    }

                    MessageBox.Show($"Данные успешно экспортированы в файл: {filePath}", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
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

        private static void AppendInfo(Body body, string label, string value)
        {
            Paragraph paragraph = new Paragraph();

            Run labelRun = new Run();
            labelRun.AppendChild(new Text($"{label}: ") { Space = SpaceProcessingModeValues.Preserve });
            labelRun.RunProperties = new RunProperties { Bold = new Bold() };

            Run valueRun = new Run();
            valueRun.AppendChild(new Text(value));

            paragraph.Append(labelRun);
            paragraph.Append(valueRun);

            body.AppendChild(paragraph);
        }

        private static void AppendBulletPoint(Body body, string value)
        {
            Paragraph paragraph = new Paragraph();

            paragraph.ParagraphProperties = new ParagraphProperties
            {
                NumberingProperties = new NumberingProperties
                {
                    NumberingId = new NumberingId { Val = 1 },
                    NumberingLevelReference = new NumberingLevelReference { Val = 0 }
                }
            };

            Run valueRun = new Run(new Text(value));
            paragraph.Append(valueRun);

            body.AppendChild(paragraph);
        }

        private void ExportToWordTextDocumentSelection()
        {
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Документ Word|*.docx",
                Title = "Сохранить как"
            };
            if (TabControl.SelectedIndex == 0)
            {
                if (saveFileDialog.ShowDialog() == true)
                {
                    ExportVolunteerToWordTextDocument(saveFileDialog.FileName, volunteersDataGridSelectedRowId);
                }
            }

            else if (TabControl.SelectedIndex == 1)
            {
                if (saveFileDialog.ShowDialog() == true)
                {
                    ExportEventToWordTextDocument(saveFileDialog.FileName, eventsDataGridSelectedRowId);
                }
            }
            else if (TabControl.SelectedIndex == 2)
            {
                return;
            }
        }

        public void ExportVolunteersToXMLFile(string filePath)
        {
            using (var context = new VolunteeringDbContext())
            {
                var volunteersQuery = context.Volunteers.AsQueryable();

                var volunteers = volunteersQuery
                    .Include(v => v.Region)
                    .ToList();

                var xDocument = new XDocument(
                    new XElement("Volunteers",
                        volunteers.Select(v => new XElement("Volunteer",
                            new XElement("Id", v.Id),
                            new XElement("FirstName", v.FirstName),
                            new XElement("LastName", v.LastName),
                            new XElement("MiddleName", v.MiddleName),
                            new XElement("DateOfBirth", v.DateOfBirth),
                            new XElement("Snils", v.Snils),
                            new XElement("RegionId", v.RegionId),
                            new XElement("Email", v.Email),
                            new XElement("Phone", v.Phone),
                            new XElement("HasTransport", v.HasTransport),
                            new XElement("AdditionalInfo", v.AdditionalInfo),
                            new XElement("PhotoFileName", v.PhotoFileName),
                            new XElement("AchievementsFileNames", v.AchievementsFileNames?.Select(fileName => new XElement("FileName", fileName))),
                            new XElement("LastModifiedDateTime", v.LastModifiedDateTime)
                        ))
                    )
                );
                MessageBox.Show($"Данные успешно экспортированы в файл: {filePath}", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                xDocument.Save(filePath);
            }
        }

        public void ExportEventsToXMLFile(string filePath)
        {
            using (var context = new VolunteeringDbContext())
            {
                var eventsQuery = context.Events.AsQueryable();

                var events = eventsQuery
                    .Include(e => e.Region)
                    .ToList();

                var xDocument = new XDocument(
                    new XElement("Events",
                        events.Select(e => new XElement("Event",
                            new XElement("Id", e.Id),
                            new XElement("Name", e.Name),
                            new XElement("StartDateTime", e.StartDateTime),
                            new XElement("EndDateTime", e.EndDateTime),
                            new XElement("RegionId", e.RegionId),
                            new XElement("Location", e.Location),
                            new XElement("AdditionalInfo", e.AdditionalInfo),
                            new XElement("LastModifiedDateTime", e.LastModifiedDateTime)
                        ))
                    )
                );

                MessageBox.Show($"Данные успешно экспортированы в файл: {filePath}", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                xDocument.Save(filePath);
            }
        }

        private void ExportToXMLFileSelection()
        {
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Данные XML|*.xml",
                Title = "Сохранить как"
            };
            if (TabControl.SelectedIndex == 0)
            {
                if (saveFileDialog.ShowDialog() == true)
                {
                    ExportVolunteersToXMLFile(saveFileDialog.FileName);
                }
            }

            else if (TabControl.SelectedIndex == 1)
            {
                if (saveFileDialog.ShowDialog() == true)
                {
                    ExportEventsToXMLFile(saveFileDialog.FileName);
                }
            }
            else if (TabControl.SelectedIndex == 2)
            {
                return;
            }
        }

        private void AddNewVolunteerButton_Click(object sender, RoutedEventArgs e)
        {
            AddNewVolunteer();
        }

        private void AddNewEventButton_Click(object sender, RoutedEventArgs e)
        {
            AddNewEvent();
        }

        private void AddNewVolunteerEventButton_Click(object sender, RoutedEventArgs e)
        {
            AddNewVolunteerEvent();
        }

        private void EditVolunteerButton_Click(object sender, RoutedEventArgs e)
        {
            EditVolunteer();
        }

        private void EditEventButton_Click(object sender, RoutedEventArgs e)
        {
            EditEvent();
        }

        private void EditVolunteerEventButton_Click(object sender, RoutedEventArgs e)
        {
            EditVolunteerEvent();
        }

        private async void DeleteVolunteerButton_Click(object sender, RoutedEventArgs e)
        {
            await DeleteVolunteer();
        }

        private async void DeleteEventButton_Click(object sender, RoutedEventArgs e)
        {
            await DeleteEvent();
        }

        private async void DeleteVolunteerEventButton_Click(object sender, RoutedEventArgs e)
        {
            await DeleteVolunteerEvent();
        }

        private void ExportToWordTextDocumentButton_Click(object sender, RoutedEventArgs e)
        {
            ExportToWordTextDocumentSelection();
        }

        private void ExportToExcelSpreadsheetButton_Click(object sender, RoutedEventArgs e)
        {
            ExportToExcelSpreedsheetSelection();
        }

        private void ImportFromExcelSpreadsheetButton_Click(object sender, RoutedEventArgs e)
        {
            ImportFromExcelSpreedsheetSelection();
        }

        private void ImportFromXMLFileButton_Click(object sender, RoutedEventArgs e)
        {
            ImportFromXMLFileSelection();
        }

        private void ExportToXMLFileButton_Click(object sender, RoutedEventArgs e)
        {
            ExportToXMLFileSelection();
        }

        private void CurrentEventComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CurrentEventComboBox.SelectedValue != null)
            {
                LoadVolunteerEventsDataGrid((int)CurrentEventComboBox.SelectedValue);
            }
        }

        private void VolunteersDataGrid_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is FrameworkElement element && element.DataContext is Volunteer selectedVolunteer)
            {
                volunteersDataGridSelectedRowId = selectedVolunteer.Id;
                VolunteerImage.Source = null;
                if (selectedVolunteer.PhotoFileName != null)
                {
                    var programDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    var imagesDirectory = Path.Combine(programDirectory, "Images");
                    string imagePath = Path.Combine(imagesDirectory, selectedVolunteer.PhotoFileName);

                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.UriSource = new Uri(imagePath, UriKind.Absolute);
                    bitmap.EndInit();
                    bitmap.Freeze();

                    VolunteerImage.Source = bitmap;

                    VolunteerImage.Visibility = Visibility.Visible;
                    VolunteerImageTextBlock.Visibility = Visibility.Hidden;
                }
                else
                {
                    VolunteerImage.Visibility = Visibility.Hidden;
                    VolunteerImageTextBlock.Visibility = Visibility.Visible;
                    VolunteerImageTextBlock.Text = "Нет фото";
                }

                EditVolunteerButton.IsEnabled = true;
                DeleteVolunteerButton.IsEnabled = true;
                EditVolunteerMenuItem.IsEnabled = true;
                DeleteVolunteerMenuItem.IsEnabled = true;
                ExportToWordTextDocumentButton.IsEnabled = true;
                ExportToWordTextDocumentMenuItem.IsEnabled = true;
            }
            else
            {
                VolunteerImageTextBlock.Text = "Выберите волонтера в таблице, чтобы показать его изображение";
                VolunteerImage.Source = null;
                VolunteerImage.Visibility = Visibility.Hidden;
                VolunteerImageTextBlock.Visibility = Visibility.Visible;

                EditVolunteerButton.IsEnabled = false;
                DeleteVolunteerButton.IsEnabled = false;
                EditVolunteerMenuItem.IsEnabled = false;
                DeleteVolunteerMenuItem.IsEnabled = false;
                ExportToWordTextDocumentButton.IsEnabled = false;
                ExportToWordTextDocumentMenuItem.IsEnabled = false;
            }
        }

        private void EventsDataGrid_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is FrameworkElement element && element.DataContext is Event selectedEvent)
            {
                eventsDataGridSelectedRowId = selectedEvent.Id;
                EditEventButton.IsEnabled = true;
                DeleteEventButton.IsEnabled = true;
                EditEventMenuItem.IsEnabled = true;
                DeleteEventMenuItem.IsEnabled = true;
                ExportToWordTextDocumentButton.IsEnabled = true;
                ExportToWordTextDocumentMenuItem.IsEnabled = true;
            }
            else
            {
                EditEventButton.IsEnabled = false;
                DeleteEventButton.IsEnabled = false;
                EditEventMenuItem.IsEnabled = false;
                DeleteEventMenuItem.IsEnabled = false;
                ExportToWordTextDocumentButton.IsEnabled = false;
                ExportToWordTextDocumentMenuItem.IsEnabled = false;
            }
        }

        private void VolunteerEventsDataGrid_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is FrameworkElement element && element.DataContext is VolunteerEvent selectedVolunteerEvent)
            {
                volunteersEventsDataGridSelectedRowId = selectedVolunteerEvent.Id;
                EditVolunteerEventButton.IsEnabled = true;
                DeleteVolunteerEventButton.IsEnabled = true;
                EditVolunteerEventMenuItem.IsEnabled = true;
                DeleteVolunteerEventMenuItem.IsEnabled = true;
            }
            else
            {
                EditVolunteerEventButton.IsEnabled = false;
                DeleteVolunteerEventButton.IsEnabled = false;
                EditVolunteerEventMenuItem.IsEnabled = false;
                DeleteVolunteerEventMenuItem.IsEnabled = false;
            }
        }

        private async void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.OriginalSource != TabControl)
                return;

            if (TabControl.SelectedIndex == 0)
            {
                LoadVolunteersDataGrid();
                EditEventButton.IsEnabled = false;
                DeleteEventButton.IsEnabled = false;
                EditEventMenuItem.IsEnabled = false;
                DeleteEventMenuItem.IsEnabled = false;
                EditVolunteerEventButton.IsEnabled = false;
                DeleteVolunteerEventButton.IsEnabled = false;
                EditVolunteerEventMenuItem.IsEnabled = false;
                DeleteVolunteerEventMenuItem.IsEnabled = false;
                ExportToWordTextDocumentButton.IsEnabled = false;
                ExportToWordTextDocumentMenuItem.IsEnabled = false;
                ImportFromExcelSpreadsheetButton.IsEnabled = true;
                ImportFromExcelSpreadsheetMenuItem.IsEnabled = true;
                ImportFromXMLFileButton.IsEnabled = true;
                ImportFromXMLFileMenuItem.IsEnabled = true;
                if (VolunteersDataGrid.Columns.Count != 0)
                {
                    ExportToXMLFileButton.IsEnabled = true;
                    ExportToXMLFileMenuItem.IsEnabled = true;
                }
                else
                {
                    ExportToXMLFileButton.IsEnabled = false;
                    ExportToXMLFileMenuItem.IsEnabled = false;
                }

            }
            else if (TabControl.SelectedIndex == 1)
            {
                LoadEventsDataGrid();
                VolunteerImage.Visibility = Visibility.Hidden;
                VolunteerImageTextBlock.Visibility = Visibility.Visible;
                EditVolunteerButton.IsEnabled = false;
                DeleteVolunteerButton.IsEnabled = false;
                EditVolunteerMenuItem.IsEnabled = false;
                DeleteVolunteerMenuItem.IsEnabled = false;
                EditVolunteerEventButton.IsEnabled = false;
                DeleteVolunteerEventButton.IsEnabled = false;
                EditVolunteerEventMenuItem.IsEnabled = false;
                DeleteVolunteerEventMenuItem.IsEnabled = false;
                ExportToWordTextDocumentButton.IsEnabled = false;
                ExportToWordTextDocumentMenuItem.IsEnabled = false;
                ImportFromExcelSpreadsheetButton.IsEnabled = true;
                ImportFromExcelSpreadsheetMenuItem.IsEnabled = true;
                ImportFromXMLFileButton.IsEnabled = true;
                ImportFromXMLFileMenuItem.IsEnabled = true;
                if (EventsDataGrid.Columns.Count != 0)
                {
                    ExportToXMLFileButton.IsEnabled = true;
                    ExportToXMLFileMenuItem.IsEnabled = true;
                }
                else
                {
                    ExportToXMLFileButton.IsEnabled = false;
                    ExportToXMLFileMenuItem.IsEnabled = false;
                }
            }
            else if (TabControl.SelectedIndex == 2)
            {
                LoadCurrentEventComboBox();
                VolunteerImage.Visibility = Visibility.Hidden;
                VolunteerImageTextBlock.Visibility = Visibility.Visible;
                EditVolunteerButton.IsEnabled = false;
                DeleteVolunteerButton.IsEnabled = false;
                EditVolunteerMenuItem.IsEnabled = false;
                DeleteVolunteerMenuItem.IsEnabled = false;
                EditEventButton.IsEnabled = false;
                DeleteEventButton.IsEnabled = false;
                EditEventMenuItem.IsEnabled = false;
                DeleteEventMenuItem.IsEnabled = false;
                ExportToWordTextDocumentButton.IsEnabled = false;
                ExportToWordTextDocumentMenuItem.IsEnabled = false;
                ImportFromExcelSpreadsheetButton.IsEnabled = false;
                ImportFromExcelSpreadsheetMenuItem.IsEnabled = false;
                ImportFromXMLFileButton.IsEnabled = false;
                ImportFromXMLFileMenuItem.IsEnabled = false;
                ExportToXMLFileButton.IsEnabled = false;
                ExportToXMLFileMenuItem.IsEnabled = false;
                if (CurrentEventComboBox.IsEnabled)
                {
                    if (CurrentEventComboBox.Items.Count == 0)
                    {
                        CurrentEventComboBox.SelectedIndex = 0;
                    }

                    if (CurrentEventComboBox.SelectedValue != null)
                    {
                        LoadVolunteerEventsDataGrid((int)CurrentEventComboBox.SelectedValue);
                    }
                }
                try
                {
                    using (var context = new VolunteeringDbContext())
                    {
                        var volunteers = await context.Volunteers.ToListAsync();
                        var events = await context.Events.ToListAsync();
                        if (volunteers.Any() && events.Any())
                        {
                            AddNewVolunteerEventButton.IsEnabled = true;
                        }
                        else
                        {
                            AddNewVolunteerEventButton.IsEnabled = false;
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

        private void AddNewVolunteerMenuItem_Click(object sender, RoutedEventArgs e)
        {
            AddNewVolunteer();
        }

        private void AddNewEventMenuItem_Click(object sender, RoutedEventArgs e)
        {
            AddNewEvent();
        }

        private void AddNewVolunteerEventMenuItem_Click(object sender, RoutedEventArgs e)
        {
            AddNewVolunteerEvent();
        }

        private void EditVolunteerMenuItem_Click(object sender, RoutedEventArgs e)
        {
            EditVolunteer();
        }

        private void EditEventMenuItem_Click(object sender, RoutedEventArgs e)
        {
            EditEvent();
        }

        private void EditVolunteerEventMenuItem_Click(object sender, RoutedEventArgs e)
        {
            EditVolunteerEvent();
        }

        private async void DeleteVolunteerMenuItem_Click(object sender, RoutedEventArgs e)
        {
            await DeleteVolunteer();
        }

        private async void DeleteEventMenuItem_Click(object sender, RoutedEventArgs e)
        {
            await DeleteEvent();
        }

        private async void DeleteVolunteerEventMenuItem_Click(object sender, RoutedEventArgs e)
        {
            await DeleteVolunteerEvent();
        }

        private void ExportToWordTextDocumentMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ExportToWordTextDocumentSelection();
        }

        private void ExportToExcelSpreadsheetMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ExportToExcelSpreedsheetSelection();
        }

        private void ImportFromExcelSpreadsheetMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ImportFromExcelSpreedsheetSelection();
        }

        private void ImportFromXMLFileMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ImportFromXMLFileSelection();
        }

        private void ExportToXMLFileMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ExportToXMLFileSelection();
        }

        private void AboutProgramMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var newWindow = new AboutProgramWindow();
            newWindow.ShowDialog();
        }

        private void MyWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            AppState.MainWindowHeight = MyWindow.Height;
            AppState.MainWindowWidth = MyWindow.Width;
            AppState.SaveToFile();
        }

        private void MyWindow_StateChanged(object sender, EventArgs e)
        {
            AppState.MainWindowState = MyWindow.WindowState;
            AppState.SaveToFile();
        }
    }
}