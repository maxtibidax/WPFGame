using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.IO;
using System.Text.Json;

namespace WPFGame
{
    public partial class MainWindow : Window
    {
        private MainMenu mainMenu;
        private GameBoard gameBoard;
        private SettingsScreen settingsScreen;
        private int currentGridSize = 4; // По умолчанию 4x4

        public MainWindow()
        {
            InitializeComponent();

            // Создаем все экраны
            mainMenu = new MainMenu();
            gameBoard = new GameBoard();
            settingsScreen = new SettingsScreen();

            // Подписываемся на события
            mainMenu.StartGameClicked += StartNewGame;
            mainMenu.ExitClicked += () => Close();
            mainMenu.ShowSettingsClicked += ShowSettings;

            gameBoard.BackToMenuClicked += () => MainContent.Content = mainMenu;

            settingsScreen.BackToMenuClicked += () => MainContent.Content = mainMenu;
            settingsScreen.SettingsSaved += OnSettingsSaved;

            // Определяем, что показывать при старте
            if (File.Exists("game2048_save.json"))
            {
                MainContent.Content = gameBoard;
            }
            else
            {
                MainContent.Content = mainMenu;
            }

            // Обработчики событий
            Closing += (s, e) => gameBoard.SaveGame();
            KeyDown += MainWindow_KeyDown;

            // Загружаем текущие настройки
            LoadSettings();
        }

        private void LoadSettings()
        {
            const string SettingsFilePath = "game2048_settings.json";

            if (File.Exists(SettingsFilePath))
            {
                try
                {
                    string json = File.ReadAllText(SettingsFilePath);
                    var settings = JsonSerializer.Deserialize<GameSettings>(json);
                    currentGridSize = settings.GridSize;
                }
                catch (Exception)
                {
                    currentGridSize = 4; // По умолчанию 4x4 при ошибке
                }
            }
        }

        private void OnSettingsSaved(int gridSize)
        {
            currentGridSize = gridSize;
        }

        private void ShowSettings()
        {
            MainContent.Content = settingsScreen;
        }

        private void StartNewGame()
        {
            gameBoard.SetGridSize(currentGridSize);
            gameBoard.InitializeGame();
            MainContent.Content = gameBoard;
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (MainContent.Content == gameBoard)
            {
                gameBoard.HandleKeyPress(e);
                e.Handled = true;
            }
        }
    }
}