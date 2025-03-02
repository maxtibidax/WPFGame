using System;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Text.Json;

namespace WPFGame
{
    public partial class SettingsScreen : UserControl
    {
        public event Action BackToMenuClicked;
        public event Action<int> SettingsSaved;

        private const string SettingsFilePath = "game2048_settings.json";

        public SettingsScreen()
        {
            InitializeComponent();
            LoadSettings();
        }

        private void SaveSettings_Click(object sender, RoutedEventArgs e)
        {
            int gridSize = GetSelectedGridSize();
            SaveSettingsToFile(gridSize);
            SettingsSaved?.Invoke(gridSize);
            BackToMenuClicked?.Invoke();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            BackToMenuClicked?.Invoke();
        }

        private int GetSelectedGridSize()
        {
            if (Grid3x3.IsChecked == true) return 3;
            if (Grid5x5.IsChecked == true) return 5;
            return 4; // По умолчанию 4x4
        }

        private void SaveSettingsToFile(int gridSize)
        {
            try
            {
                var settings = new GameSettings
                {
                    GridSize = gridSize
                };

                string json = JsonSerializer.Serialize(settings);
                File.WriteAllText(SettingsFilePath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении настроек: {ex.Message}");
            }
        }

        private void LoadSettings()
        {
            if (File.Exists(SettingsFilePath))
            {
                try
                {
                    string json = File.ReadAllText(SettingsFilePath);
                    var settings = JsonSerializer.Deserialize<GameSettings>(json);

                    // Установка размера сетки
                    switch (settings.GridSize)
                    {
                        case 3:
                            Grid3x3.IsChecked = true;
                            break;
                        case 5:
                            Grid5x5.IsChecked = true;
                            break;
                        default:
                            Grid4x4.IsChecked = true;
                            break;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке настроек: {ex.Message}");
                    Grid4x4.IsChecked = true; // По умолчанию 4x4
                }
            }
            else
            {
                Grid4x4.IsChecked = true; // По умолчанию 4x4
            }
        }
    }

    public class GameSettings
    {
        public int GridSize { get; set; } = 4; // По умолчанию 4x4
    }
}