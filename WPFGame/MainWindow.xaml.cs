﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.IO;
using System.Text.Json;

namespace Game2048
{
    public partial class MainWindow : Window
    {
        private const int GridSize = 4;
        private int[,] board;
        private Border[,] tiles;
        private Random random;
        private int score;
        private const string SaveFilePath = "game2048_save.json";

        public MainWindow()
        {
            InitializeComponent();
            Closing += MainWindow_Closing;
            LoadGameOrStartNew();
        }

        private void LoadGameOrStartNew()
        {
            if (File.Exists(SaveFilePath))
            {
                var result = MessageBox.Show(
                    "Найдено сохранение предыдущей игры. Загрузить его?",
                    "Загрузка игры",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    LoadGame();
                }
                else
                {
                    InitializeGame();
                }
            }
            else
            {
                InitializeGame();
            }
        }

        private void InitializeGame()
        {
            board = new int[GridSize, GridSize];
            tiles = new Border[GridSize, GridSize];
            random = new Random();
            score = 0;

            AddRandomTile();
            AddRandomTile();
            UpdateUI();
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveGame();
        }

        private void SaveGame()
        {
            try
            {
                // Конвертируем 2D массив в 1D для сериализации
                int[] flatBoard = new int[GridSize * GridSize];
                for (int i = 0; i < GridSize; i++)
                {
                    for (int j = 0; j < GridSize; j++)
                    {
                        flatBoard[i * GridSize + j] = board[i, j];
                    }
                }

                var saveData = new GameSaveData
                {
                    Board = flatBoard,
                    Score = score
                };
                string json = JsonSerializer.Serialize(saveData);
                File.WriteAllText(SaveFilePath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении игры: {ex.Message}");
            }
        }

        private void LoadGame()
        {
            try
            {
                string json = File.ReadAllText(SaveFilePath);
                var saveData = JsonSerializer.Deserialize<GameSaveData>(json);

                // Конвертируем 1D массив обратно в 2D
                board = new int[GridSize, GridSize];
                for (int i = 0; i < GridSize; i++)
                {
                    for (int j = 0; j < GridSize; j++)
                    {
                        board[i, j] = saveData.Board[i * GridSize + j];
                    }
                }

                tiles = new Border[GridSize, GridSize];
                random = new Random();
                score = saveData.Score;

                UpdateUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке игры: {ex.Message}\nНовая игра будет начата.");
                InitializeGame();
            }
        }

        // Класс для сохранения данных игры с одномерным массивом
        private class GameSaveData
        {
            public int[] Board { get; set; }  // Изменено на одномерный массив
            public int Score { get; set; }
        }

        // Остальные методы остаются без изменений
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            bool moved = false;
            switch (e.Key)
            {
                case Key.Up:
                    moved = MoveUp();
                    break;
                case Key.Down:
                    moved = MoveDown();
                    break;
                case Key.Left:
                    moved = MoveLeft();
                    break;
                case Key.Right:
                    moved = MoveRight();
                    break;
            }

            if (moved)
            {
                AddRandomTile();
                UpdateUI();
                if (IsGameOver())
                {
                    MessageBox.Show($"Игра окончена! Ваш счет: {score}");
                    InitializeGame();
                }
            }
        }

        private void AddTile(int row, int col, int value)
        {
            var tile = new Border
            {
                Style = (Style)FindResource("TileStyle"),
                RenderTransform = new ScaleTransform(),
                RenderTransformOrigin = new Point(0.5, 0.5),
                Background = GetTileColor(value),
                Child = new TextBlock
                {
                    Text = value.ToString(),
                    FontSize = 24,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Foreground = value <= 4 ? Brushes.Black : Brushes.White
                }
            };

            Grid.SetRow(tile, row);
            Grid.SetColumn(tile, col);
            GameGrid.Children.Add(tile);
            tiles[row, col] = tile;

            var storyboard = (Storyboard)FindResource("TileAppearAnimation");
            Storyboard.SetTarget(storyboard, tile);
            storyboard.Begin();
        }

        private void UpdateUI()
        {
            GameGrid.Children.Clear();
            for (int i = 0; i < GridSize; i++)
            {
                for (int j = 0; j < GridSize; j++)
                {
                    if (board[i, j] != 0)
                    {
                        AddTile(i, j, board[i, j]);
                    }
                }
            }
            ScoreText.Text = $"Score: {score}";
        }

        private void AddRandomTile()
        {
            var emptyCells = new System.Collections.Generic.List<(int, int)>();
            for (int i = 0; i < GridSize; i++)
            {
                for (int j = 0; j < GridSize; j++)
                {
                    if (board[i, j] == 0)
                        emptyCells.Add((i, j));
                }
            }

            if (emptyCells.Count > 0)
            {
                var cell = emptyCells[random.Next(emptyCells.Count)];
                board[cell.Item1, cell.Item2] = random.NextDouble() < 0.9 ? 2 : 4;
            }
        }

        private Brush GetTileColor(int value)
        {
            return value switch
            {
                0 => Brushes.Transparent,
                2 => new SolidColorBrush(Color.FromRgb(238, 228, 218)),
                4 => new SolidColorBrush(Color.FromRgb(237, 224, 200)),
                8 => new SolidColorBrush(Color.FromRgb(242, 177, 121)),
                16 => new SolidColorBrush(Color.FromRgb(245, 149, 99)),
                32 => new SolidColorBrush(Color.FromRgb(246, 124, 95)),
                64 => new SolidColorBrush(Color.FromRgb(246, 94, 59)),
                128 => new SolidColorBrush(Color.FromRgb(237, 207, 114)),
                256 => new SolidColorBrush(Color.FromRgb(237, 204, 97)),
                512 => new SolidColorBrush(Color.FromRgb(237, 200, 80)),
                1024 => new SolidColorBrush(Color.FromRgb(237, 197, 63)),
                2048 => new SolidColorBrush(Color.FromRgb(237, 194, 46)),
                _ => new SolidColorBrush(Color.FromRgb(60, 58, 50))
            };
        }

        private bool MoveUp()
        {
            bool moved = false;
            for (int j = 0; j < GridSize; j++)
            {
                for (int i = 1; i < GridSize; i++)
                {
                    if (board[i, j] != 0)
                    {
                        int k = i;
                        while (k > 0 && (board[k - 1, j] == 0 || board[k - 1, j] == board[k, j]))
                        {
                            if (board[k - 1, j] == 0)
                            {
                                board[k - 1, j] = board[k, j];
                                board[k, j] = 0;
                                moved = true;
                            }
                            else if (board[k - 1, j] == board[k, j])
                            {
                                board[k - 1, j] *= 2;
                                score += board[k - 1, j];
                                board[k, j] = 0;
                                moved = true;
                                break;
                            }
                            k--;
                        }
                    }
                }
            }
            return moved;
        }

        private bool MoveDown()
        {
            RotateBoard(2);
            bool moved = MoveUp();
            RotateBoard(2);
            return moved;
        }

        private bool MoveLeft()
        {
            RotateBoard(1);
            bool moved = MoveUp();
            RotateBoard(3);
            return moved;
        }

        private bool MoveRight()
        {
            RotateBoard(3);
            bool moved = MoveUp();
            RotateBoard(1);
            return moved;
        }

        private void RotateBoard(int times)
        {
            for (int t = 0; t < times; t++)
            {
                int[,] newBoard = new int[GridSize, GridSize];
                for (int i = 0; i < GridSize; i++)
                {
                    for (int j = 0; j < GridSize; j++)
                    {
                        newBoard[j, GridSize - 1 - i] = board[i, j];
                    }
                }
                board = newBoard;
            }
        }

        private bool IsGameOver()
        {
            for (int i = 0; i < GridSize; i++)
            {
                for (int j = 0; j < GridSize; j++)
                {
                    if (board[i, j] == 0) return false;
                }
            }

            for (int i = 0; i < GridSize; i++)
            {
                for (int j = 0; j < GridSize; j++)
                {
                    if (i < GridSize - 1 && board[i, j] == board[i + 1, j]) return false;
                    if (j < GridSize - 1 && board[i, j] == board[i, j + 1]) return false;
                }
            }

            return true;
        }
    }
}