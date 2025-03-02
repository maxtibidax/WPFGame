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
        private MainMenu mainMenu; // Одно объявление
        private GameBoard gameBoard; // Одно объявление

        public MainWindow() // Один конструктор
        {
            InitializeComponent();
            mainMenu = new MainMenu();
            gameBoard = new GameBoard();

            mainMenu.StartGameClicked += StartNewGame;
            mainMenu.ExitClicked += () => Close();
            gameBoard.BackToMenuClicked += () => MainContent.Content = mainMenu;

            if (File.Exists("game2048_save.json"))
            {
                MainContent.Content = gameBoard;
            }
            else
            {
                MainContent.Content = mainMenu;
            }

            Closing += (s, e) => gameBoard.SaveGame();  
            KeyDown += MainWindow_KeyDown;
        }

        private void StartNewGame()
        {
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