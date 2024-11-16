using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Win32;

namespace VolunteeringWpfApp
{
    /// <summary>
    /// Логика взаимодействия для EditVolunteerWindow.xaml
    /// </summary>

    public partial class EditVolunteerWindow : Window
    {
        string OldFirstName;
        string OldLastName;
        string? OldMiddleName = string.Empty;
        DateTime OldDateOfBirth;
        string OldSnils;
        int OldRegionId;
        string OldEmail;
        string OldPhone;
        bool OldHasTransport;
        string? OldAdditionalInfo = string.Empty;
        string OldVolunteerPhotoPath = string.Empty;
        bool PhotoChanged = false;
        List<string> achievementsFilePaths = new List<string>();
        bool AchievementChanged = false;
        bool DoneButtonStateCheck = false;
        int? _volunteerId;

        public EditVolunteerWindow(int? volunteerId)
        {
            _volunteerId = volunteerId;
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

                    var volunteerContext = context.Volunteers.FirstOrDefault(v => v.Id == _volunteerId);
                    if (volunteerContext != null)
                    {
                        FirstNameTextBox.Text = volunteerContext.FirstName;
                        LastNameTextBox.Text = volunteerContext.LastName;
                        MiddleNameTextBox.Text = volunteerContext.MiddleName;
                        DateOfBirthDatePicker.SelectedDate = volunteerContext.DateOfBirth;
                        SnilsTextBox.Text = volunteerContext.Snils;
                        RegionComboBox.SelectedValue = volunteerContext.RegionId;
                        EmailTextBox.Text = volunteerContext.Email;
                        PhoneTextBox.Text = volunteerContext.Phone;
                        HasTransportCheckBox.IsChecked = volunteerContext.HasTransport;
                        AdditionalInfoTextBox.Text = volunteerContext.AdditionalInfo;

                        OldFirstName = volunteerContext.FirstName;
                        OldLastName = volunteerContext.LastName;
                        OldMiddleName = volunteerContext.MiddleName ?? string.Empty;
                        OldDateOfBirth = volunteerContext.DateOfBirth;
                        OldSnils = volunteerContext.Snils;
                        OldRegionId = volunteerContext.RegionId;
                        OldEmail = volunteerContext.Email;
                        OldPhone = volunteerContext.Phone;
                        OldHasTransport = volunteerContext.HasTransport;
                        OldAdditionalInfo = volunteerContext.AdditionalInfo ?? string.Empty;
                        if (volunteerContext.PhotoFileName != null)
                        {
                            var programDirectory = AppDomain.CurrentDomain.BaseDirectory;
                            var imagesDirectory = Path.Combine(programDirectory, "Images");
                            OldVolunteerPhotoPath = Path.Combine(imagesDirectory, volunteerContext.PhotoFileName);
                            var bitmap = new BitmapImage();
                            bitmap.BeginInit();
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap.UriSource = new Uri(OldVolunteerPhotoPath);
                            bitmap.EndInit();
                            bitmap.Freeze();
                            VolunteerImage.Source = bitmap;

                            VolunteerImage.Visibility = Visibility.Visible;
                            VolunteerImageTextBlock.Visibility = Visibility.Hidden;
                            DeleteVolunteerImage.IsEnabled = true;
                        }

                        if (volunteerContext.AchievementsFileNames != null)
                        {
                            AchievementTextBox.Text = string.Join(", ", volunteerContext.AchievementsFileNames);
                            AchievementTextBox.TextAlignment = TextAlignment.Left;
                            AchievementButton.Content = "Удалить";
                        }

                        DoneButtonStateCheck = true;

                        var volunteerEventHasTransport = context.VolunteerEvents.FirstOrDefault(ve => ve.VolunteerId == _volunteerId && ve.RoleId == 2);
                        if (volunteerEventHasTransport != null)
                        {
                            HasTransportCheckBox.IsEnabled = false;
                        }
                        else
                        {
                            HasTransportCheckBox.IsEnabled = true;
                        }

                        var volunteerEventCheckDate = context.VolunteerEvents.FirstOrDefault(ve => ve.VolunteerId == _volunteerId);
                        if (volunteerEventCheckDate != null)
                        {
                            DateOfBirthDatePicker.IsEnabled = false;
                        }
                        else
                        {
                            DateOfBirthDatePicker.IsEnabled = true;
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

        private void DoneButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var context = new VolunteeringDbContext())
                {
                    context.Database.Migrate();

                    var volunteerContext = context.Volunteers.FirstOrDefault(v => v.Id == _volunteerId);
                    if (volunteerContext == null)
                    {
                        MessageBox.Show("Выбранный волонтёр не найден", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (volunteerContext.FirstName != FirstNameTextBox.Text)
                    {
                        volunteerContext.FirstName = FirstNameTextBox.Text;
                    }

                    if (volunteerContext.LastName != LastNameTextBox.Text)
                    {
                        volunteerContext.LastName = LastNameTextBox.Text;
                    }

                    if (volunteerContext.DateOfBirth != DateOfBirthDatePicker.SelectedDate)
                    {
                        volunteerContext.DateOfBirth = DateOfBirthDatePicker.SelectedDate.Value.Date;
                    }

                    if (volunteerContext.Snils != SnilsTextBox.Text)
                    {
                        volunteerContext.Snils = SnilsTextBox.Text;
                    }

                    if (volunteerContext.RegionId != (int)RegionComboBox.SelectedValue)
                    {
                        volunteerContext.RegionId = (int)RegionComboBox.SelectedValue;
                    }

                    if (volunteerContext.Email != EmailTextBox.Text)
                    {
                        volunteerContext.Email = EmailTextBox.Text;
                    }

                    if (volunteerContext.Phone != PhoneTextBox.Text)
                    {
                        volunteerContext.Phone = PhoneTextBox.Text;
                    }

                    if (volunteerContext.HasTransport != HasTransportCheckBox.IsChecked)
                    {
                        volunteerContext.HasTransport = HasTransportCheckBox.IsChecked.Value;
                    }

                    if (volunteerContext.MiddleName != MiddleNameTextBox.Text)
                    {
                        volunteerContext.MiddleName = string.IsNullOrWhiteSpace(MiddleNameTextBox.Text) ? null : MiddleNameTextBox.Text;
                    }

                    if (volunteerContext.AdditionalInfo != AdditionalInfoTextBox.Text)
                    {
                        volunteerContext.AdditionalInfo = string.IsNullOrWhiteSpace(AdditionalInfoTextBox.Text) ? null : AdditionalInfoTextBox.Text;
                    }

                    if (AchievementChanged)
                    {
                        if (achievementsFilePaths.Count != 0)
                        {
                            volunteerContext.AchievementsFileNames = new string[achievementsFilePaths.Count];

                            for (int i = 0; i < achievementsFilePaths.Count; i++)
                            {
                                var achievementsFilePath = achievementsFilePaths[i];

                                if (string.IsNullOrEmpty(achievementsFilePath))
                                {
                                    MessageBox.Show("Путь к файлу не может быть пустым", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                    volunteerContext.AchievementsFileNames = null;
                                    continue;
                                }

                                if (!File.Exists(achievementsFilePath))
                                {
                                    MessageBox.Show($"Файл не найден: {achievementsFilePath}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                    volunteerContext.AchievementsFileNames = null;
                                    continue;
                                }

                                volunteerContext.AchievementsFileNames[i] = Path.GetFileName(achievementsFilePath);
                            }

                            var programDirectory = AppDomain.CurrentDomain.BaseDirectory;
                            var achievementsDirectory = Path.Combine(programDirectory, "Achievements");

                            if (!Directory.Exists(achievementsDirectory))
                            {
                                Directory.CreateDirectory(achievementsDirectory);
                            }

                            foreach (var achievementFilePath in achievementsFilePaths)
                            {
                                if (File.Exists(achievementFilePath))
                                {
                                    string fileName = Path.GetFileName(achievementFilePath);
                                    string newFilePath = Path.Combine(achievementsDirectory, fileName);

                                    File.Copy(achievementFilePath, newFilePath, overwrite: true);
                                }
                                else
                                {
                                    MessageBox.Show($"Файл достижения '{achievementFilePath}' не найден. Пожалуйста, проверьте путь и повторите попытку", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }
                        }
                        else
                        {
                            volunteerContext.AchievementsFileNames = null;
                        }
                    }


                    volunteerContext.LastModifiedDateTime = DateTime.Now;
                    context.SaveChanges();

                    if (PhotoChanged)
                    {
                        if (File.Exists(OldVolunteerPhotoPath) && !OldVolunteerPhotoPath.IsNullOrEmpty() && DeleteVolunteerImage.IsEnabled == true)
                        {
                            var programDirectory = AppDomain.CurrentDomain.BaseDirectory;
                            var imagesDirectory = Path.Combine(programDirectory, "Images");

                            if (!Directory.Exists(imagesDirectory))
                            {
                                Directory.CreateDirectory(imagesDirectory);
                            }

                            string fileExtension = Path.GetExtension(OldVolunteerPhotoPath);
                            string newFilePath = Path.Combine(imagesDirectory, $"{volunteerContext.Id}{fileExtension}");

                            File.Copy(OldVolunteerPhotoPath, newFilePath, overwrite: true);

                            volunteerContext.PhotoFileName = Path.GetFileName(newFilePath);
                            context.SaveChanges();
                        }
                        else if (DeleteVolunteerImage.IsEnabled == true)
                        {
                            MessageBox.Show("Файл изображения не найден. Пожалуйста, проверьте путь и повторите попытку", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        else if (DeleteVolunteerImage.IsEnabled == false)
                        {
                            File.Delete(OldVolunteerPhotoPath);
                            volunteerContext.PhotoFileName = null;
                            context.SaveChanges();
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

            MessageBox.Show("Данные выбранного волонтёра были успешно обновлены", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }


        private void DoneButtonState()
        {
            DateTime currentDate = DateTime.Now;

            DateTime minValidDate = currentDate.Date.AddYears(-14);

            bool isValidAge = DateOfBirthDatePicker.SelectedDate.HasValue &&
                              DateOfBirthDatePicker.SelectedDate.Value < minValidDate;

            if (DoneButtonStateCheck == true)
            {
                DoneButton.IsEnabled =
                    (FirstNameTextBox.Text != OldFirstName ||
                    LastNameTextBox.Text != OldLastName ||
                    MiddleNameTextBox.Text != OldMiddleName ||
                    DateOfBirthDatePicker.SelectedDate != OldDateOfBirth ||
                    SnilsTextBox.Text != OldSnils ||
                    (int)RegionComboBox.SelectedValue != OldRegionId ||
                    EmailTextBox.Text != OldEmail ||
                    PhoneTextBox.Text != OldPhone ||
                    HasTransportCheckBox.IsChecked != OldHasTransport ||
                    AdditionalInfoTextBox.Text != OldAdditionalInfo || PhotoChanged || AchievementChanged) &&
                    IsValidPhone(PhoneTextBox.Text) &&
                    IsValidEmail(EmailTextBox.Text) &&
                    FirstNameTextBox.Text.Length > 0 &&
                    LastNameTextBox.Text.Length > 0 &&
                    SnilsTextBox.Text.Length > 0 &&
                    SnilsTextBox.Text.Length == 11 &&
                    PhoneTextBox.Text.Length > 0 &&
                    DateOfBirthDatePicker.SelectedDate.HasValue &&
                    RegionComboBox.SelectedItem != null && isValidAge;
            }
        }

        private void FirstNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            DoneButtonState();
        }

        private void LastNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            DoneButtonState();
        }

        private void AdditionalInfoTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            DoneButtonState();
        }

        private void MiddleNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            DoneButtonState();
        }

        private void HasTransportCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            DoneButtonState();
        }

        private void DateOfBirthDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DoneButtonState();
        }

        private void SnilsTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            DoneButtonState();
        }

        private void NumericTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void RegionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DoneButtonState();
        }

        private void EmailTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            DoneButtonState();
        }

        private bool IsValidEmail(string email)
        {
            var emailRegex = new Regex(@"^[\w-]+(\.[\w-]+)*@([\w-]+\.)+[a-zA-Z]{2,7}$");
            return emailRegex.IsMatch(email);
        }

        private void PhoneTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            DoneButtonState();
        }

        private bool IsValidPhone(string phone)
        {
            var phoneRegex = new Regex(@"^\d{11}$");
            return phoneRegex.IsMatch(phone);
        }

        private void ImageGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Изображение|*.jpg;*.jpeg;*.png;*.bmp;*.gif",
                Title = "Выбрать изображение"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                OldVolunteerPhotoPath = openFileDialog.FileName;

                if (!string.IsNullOrEmpty(OldVolunteerPhotoPath) && File.Exists(OldVolunteerPhotoPath))
                {
                    var fileInfo = new FileInfo(OldVolunteerPhotoPath);
                    if (fileInfo.Length > 5 * 1024 * 1024)
                    {
                        MessageBox.Show("Файл слишком большой. Максимальный размер: 5 МБ", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    try
                    {
                        var bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.UriSource = new Uri(OldVolunteerPhotoPath);
                        bitmap.EndInit();
                        bitmap.Freeze();

                        VolunteerImage.Source = bitmap;
                        VolunteerImage.Visibility = Visibility.Visible;
                        VolunteerImageTextBlock.Visibility = Visibility.Hidden;
                        DeleteVolunteerImage.IsEnabled = true;
                        PhotoChanged = true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при открытии файла: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Этот формат файла не поддерживается", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            DoneButtonState();
        }

        private void DeleteVolunteerImage_Click(object sender, RoutedEventArgs e)
        {
            VolunteerImage.Source = null;
            VolunteerImage.Visibility = Visibility.Hidden;
            VolunteerImageTextBlock.Visibility = Visibility.Visible;
            DeleteVolunteerImage.IsEnabled = false;
            PhotoChanged = true;
            DoneButtonState();
        }

        private void AchievementButton_Click(object sender, RoutedEventArgs e)
        {
            if (AchievementButton.Content.ToString() == "Добавить")
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Multiselect = true,
                    Filter = "Все файлы (*.*)|*.*"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    achievementsFilePaths.AddRange(openFileDialog.FileNames);
                    AchievementButton.Content = "Удалить";

                    var fileNames = openFileDialog.FileNames.Select(Path.GetFileName);
                    AchievementTextBox.Text = string.Join(", ", fileNames);
                    AchievementTextBox.TextAlignment = TextAlignment.Left;
                }
            }
            else
            {
                achievementsFilePaths.Clear();
                AchievementButton.Content = "Добавить";
                AchievementTextBox.Text = string.Empty;
            }
            AchievementChanged = true;
            DoneButtonState();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }

        private void AchievementTextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(AchievementTextBox.Text))
            {
                if (AchievementChanged)
                {
                    foreach (string filePath in achievementsFilePaths)
                    {

                        if (File.Exists(filePath))
                        {
                            try
                            {
                                Process.Start(new ProcessStartInfo
                                {
                                    FileName = filePath,
                                    UseShellExecute = true
                                });
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Не удалось открыть файл {Path.GetFileName(filePath)}: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                        else
                        {
                            MessageBox.Show($"Файл не найден: {filePath}", "Файл не найден", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                }
                else
                {
                    string baseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Achievements");

                    string[] fileNames = AchievementTextBox.Text.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string fileName in fileNames)
                    {
                        string trimmedFileName = fileName.Trim();

                        string filePath = Path.Combine(baseDirectory, trimmedFileName);

                        if (File.Exists(filePath))
                        {
                            try
                            {
                                Process.Start(new ProcessStartInfo
                                {
                                    FileName = filePath,
                                    UseShellExecute = true
                                });
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Не удалось открыть файл {trimmedFileName}: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                        else
                        {
                            MessageBox.Show($"Файл {trimmedFileName} не найден в директории {baseDirectory}", "Файл не найден", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                }      
            }
        }
    }
}
