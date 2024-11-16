using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using VolunteeringWpfApp.Models;

namespace VolunteeringWpfApp
{
    /// <summary>
    /// Логика взаимодействия для AddNewVolunteerWindow.xaml
    /// </summary>
    public partial class AddNewVolunteerWindow : Window
    {
        string VolunteerPhotoPath = string.Empty;
        List<string> achievementsFilePaths = new List<string>();

        public AddNewVolunteerWindow()
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

                    var newVolunteer = new Volunteer
                    {
                        FirstName = FirstNameTextBox.Text,
                        LastName = LastNameTextBox.Text,
                        DateOfBirth = DateOfBirthDatePicker.SelectedDate.Value.Date,
                        Snils = SnilsTextBox.Text,
                        RegionId = (int)RegionComboBox.SelectedValue,
                        Email = EmailTextBox.Text,
                        Phone = PhoneTextBox.Text,
                        HasTransport = HasTransportCheckBox.IsChecked.Value,
                        LastModifiedDateTime = DateTime.Now
                    };

                    if (!string.IsNullOrWhiteSpace(MiddleNameTextBox.Text))
                    {
                        newVolunteer.MiddleName = MiddleNameTextBox.Text;
                    }

                    if (!string.IsNullOrWhiteSpace(AdditionalInfoTextBox.Text))
                    {
                        newVolunteer.AdditionalInfo = AdditionalInfoTextBox.Text;
                    }

                    if (achievementsFilePaths.Count != 0)
                    {
                        newVolunteer.AchievementsFileNames = new string[achievementsFilePaths.Count];

                        for (int i = 0; i < achievementsFilePaths.Count; i++)
                        {
                            newVolunteer.AchievementsFileNames[i] = Path.GetFileName(achievementsFilePaths[i]);
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


                    context.Volunteers.Add(newVolunteer);
                    context.SaveChanges();

                    if (!string.IsNullOrEmpty(VolunteerPhotoPath) && File.Exists(VolunteerPhotoPath))
                    {
                        var programDirectory = AppDomain.CurrentDomain.BaseDirectory;
                        var imagesDirectory = Path.Combine(programDirectory, "Images");

                        if (!Directory.Exists(imagesDirectory))
                        {
                            Directory.CreateDirectory(imagesDirectory);
                        }

                        string fileExtension = Path.GetExtension(VolunteerPhotoPath);
                        string newFilePath = Path.Combine(imagesDirectory, $"{newVolunteer.Id}{fileExtension}");

                        File.Copy(VolunteerPhotoPath, newFilePath, overwrite: true);

                        newVolunteer.PhotoFileName = Path.GetFileName(newFilePath);
                        context.SaveChanges();
                    }
                    else if (DeleteVolunteerImage.IsEnabled == true)
                    {
                        MessageBox.Show("Файл изображения не найден. Пожалуйста, проверьте путь и повторите попытку", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
            }
            catch (SqlException ex)
            {
                if ((ex.Message.Contains("A network-related or instance-specific error occurred while establishing a connection to SQL Server")) || (ex.Message.Contains("Служба SQL Server приостановлена. Новые соединения будут отклоняться.")))
                {
                    MessageBox.Show("Невозможно подключиться к серверу базы данных. Проверьте подключение к сети и убедитесь, что сервер базы данных доступен",
                                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show("Произошла ошибка при подключении к базе данных: " + ex.Message,
                                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                return;
            }

            MessageBox.Show("Волонтёр был успешно добавлен", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }

        private void DoneButtonState()
        {
            DateTime currentDate = DateTime.Now;

            DateTime minValidDate = currentDate.Date.AddYears(-14);

            bool isValidAge = DateOfBirthDatePicker.SelectedDate.HasValue &&
                              DateOfBirthDatePicker.SelectedDate.Value < minValidDate;

            DoneButton.IsEnabled = FirstNameTextBox.Text.Length > 0 &&
                LastNameTextBox.Text.Length > 0 &&
                SnilsTextBox.Text.Length > 0 && SnilsTextBox.Text.Length == 11 &&
                EmailTextBox.Text.Length > 0 && IsValidEmail(EmailTextBox.Text) &&
                PhoneTextBox.Text.Length > 0 && IsValidPhone(PhoneTextBox.Text) &&
                DateOfBirthDatePicker.SelectedDate.HasValue &&
                RegionComboBox.SelectedItem != null && isValidAge;
        }

        private void FirstNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            DoneButtonState();
        }

        private void LastNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
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

        private void PhoneTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            DoneButtonState();
        }

        private bool IsValidEmail(string email)
        {
            var emailRegex = new Regex(@"^[\w-]+(\.[\w-]+)*@([\w-]+\.)+[a-zA-Z]{2,7}$");
            return emailRegex.IsMatch(email);
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
                VolunteerPhotoPath = openFileDialog.FileName;

                if (!string.IsNullOrEmpty(VolunteerPhotoPath) && File.Exists(VolunteerPhotoPath))
                {
                    var fileInfo = new FileInfo(VolunteerPhotoPath);
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
                        bitmap.UriSource = new Uri(VolunteerPhotoPath);
                        bitmap.EndInit();
                        bitmap.Freeze();

                        VolunteerImage.Source = bitmap;
                        VolunteerImage.Visibility = Visibility.Visible;
                        VolunteerImageTextBlock.Visibility = Visibility.Hidden;
                        DeleteVolunteerImage.IsEnabled = true;
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
        }

        private void DeleteVolunteerImage_Click(object sender, RoutedEventArgs e)
        {
            VolunteerPhotoPath = string.Empty;
            VolunteerImage.Source = null;
            VolunteerImage.Visibility = Visibility.Hidden;
            VolunteerImageTextBlock.Visibility = Visibility.Visible;
            DeleteVolunteerImage.IsEnabled = false;
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
        }

        private void AchievementTextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(AchievementTextBox.Text))
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
