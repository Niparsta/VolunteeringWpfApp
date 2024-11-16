using System.IO;
using System.Windows;
using Newtonsoft.Json;

namespace VolunteeringWpfApp.Models
{
    internal class State
    {
        public string? DatabaseConnectionString { get; set; }
        public string? DatabaseName { get; set; }
        public WindowState MainWindowState { get; set; }
        public double MainWindowHeight { get; set; }
        public double MainWindowWidth { get; set; }

        private static readonly string ConfigFilePath = "config.json";

        public State()
        {
            DatabaseConnectionString = ".\\SQLEXPRESS";
            DatabaseName = "VolunteeringDb";
            MainWindowState = WindowState.Normal;
            MainWindowHeight = 460.0;
            MainWindowWidth = 840.0;
        }

        public static State LoadFromFile()
        {
            if (File.Exists(ConfigFilePath))
            {
                try
                {
                    string json = File.ReadAllText(ConfigFilePath);
                    return JsonConvert.DeserializeObject<State>(json) ?? new State();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при чтении файла конфигурации: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            var defaultState = new State();
            defaultState.SaveToFile();
            return defaultState;
        }

        public void SaveToFile()
        {
            try
            {
                string json = JsonConvert.SerializeObject(this, Formatting.Indented);
                File.WriteAllText(ConfigFilePath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении файла конфигурации: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

