using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace PuzzleGame
{
    public partial class MainWindow : Window
    {
        //Додаткові атрибути та елементи інтерфейсу
        #region attribute 
        Image image = new Image();// Зображення та його джерело
        BitmapImage imageSource;
        List<Piece> pieces = new List<Piece>(); // Список пазлів та вибрані пазли
        List<Piece> currentSelection = new List<Piece>();
        int columns = 1; // Кількість колонок та рядків для розміщення пазлів
        int rows = 1;
        int pieceIndex = 0;//змінна, щоб кожен пазл ідент. окремол
        #endregion

        #region constructor
        public MainWindow()
        {
            InitializeComponent();

            Pole.MouseEnter += new MouseEventHandler(pole_MouseEnter);
            Pole.MouseMove += new MouseEventHandler(pole_MouseMove);
            Pole.MouseLeftButtonUp += new MouseButtonEventHandler(pole_MouseLeftButtonUp);
            Pole.MouseLeave += new MouseEventHandler(pole_MouseLeave);
            // Обробники подій для мишевих взаємодій на ігровому полі

        }
        #endregion constructor

        #region methods
        public void LoadImage(string uriImage)//метод для заванатаження фону ігр поля
        {
            BitmapImage bi = new BitmapImage(new Uri(uriImage));//зберігає зоброження на полі

            columns = 5;//розмір ігрового поля
            rows = 5;

            RenderTargetBitmap rtb = new RenderTargetBitmap(columns * 100, rows * 100, bi.DpiX, bi.DpiY, PixelFormats.Pbgra32);
            //завдяки цьому класу ми створюємо зображення розміром 500/500 пікселів
            var imgBrush = new ImageBrush(bi)//який використовується для заповнення фону зображенням.
            {
                AlignmentX = AlignmentX.Left,
                AlignmentY = AlignmentY.Top,
                Stretch = Stretch.Fill
            };
            // Створення прямокутника (Rectangle) для розміщення зображення та ств його параметрів
            var rectImage = new Rectangle
            {
                Width = columns * 100,
                Height = rows * 100,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Fill = imgBrush
            };
            rectImage.Arrange(new Rect(0, 0, columns * 100, rows * 100));

            rtb.Render(rectImage); // Малювання зображення на RenderTargetBitmap

            var png = new PngBitmapEncoder();// Створення кодера для збереження в форматі PNG
            png.Frames.Add(BitmapFrame.Create(rtb));

            System.IO.Stream ret = new System.IO.MemoryStream();
           
            png.Save(ret);
            // Ініціалізація об'єкта BitmapImage для використання у графічному інтерфейсі
            imageSource = new BitmapImage();
            imageSource.BeginInit();
            imageSource.StreamSource = ret;
            imageSource.EndInit();
            imageSource.Freeze();

        }

        public void CreatePole()// Метод для ствр ігрового поля
        // Створення гри з визначеними параметрами

        {
            Pole.IsEnabled = true;
            Pole.Children.Clear();
            Pole.Width = columns * 100;
            Pole.Height = rows * 100;
            Pole.Background = new SolidColorBrush(Colors.WhiteSmoke);
            Pole.Margin = new Thickness(50);
            Pole.Parent.SetValue(Grid.BackgroundProperty, new SolidColorBrush(Colors.LightSkyBlue));
        }

        public void CreatePieces()// Метод для ствр пазлів
        // Створення пазлів для гри та їх випадкове розташування

        {
            Podbor.Children.Clear();
            pieces.Clear();
            pieceIndex = 0;
            double pieceWidth = Pole.Width / columns;
            double pieceHeight = Pole.Height / rows;

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < columns; x++)
                {
                    Piece piece = new Piece(imageSource, x, y)
                    {
                        Margin = new Thickness(5),
                        Width = pieceWidth,
                        Height = pieceHeight
                    };
                    piece.Index = pieceIndex++;

                    piece.MouseLeftButtonUp += new MouseButtonEventHandler(piece_MouseLeftButtonUp);
                    pieces.Add(piece);
                }
            }

            RandomPiece(Podbor);
        }

        private void RandomPiece(WrapPanel podbor)// Метод для випадкового перемішування пазлів
        // Випадкове перемішування пазлів

        {
            // Випадкове перемішування пазлів
            Random rnd = new Random(DateTime.Now.Millisecond);
            for (int i = 0; i < pieces.Count; i++)
            {

                int index = rnd.Next(0, pieces.Count);
                Piece tmp = pieces[i];
                pieces[i] = pieces[index];
                pieces[index] = tmp;
            }
            foreach (var p in pieces)
            {
                podbor.Children.Add(p);
            }
        }

        private bool CanInsertPiece(int cellX, int cellY)            
            // Перевірка можливості вставки пазла в певну комірку
        {
            bool ret = true;
            foreach (Piece piece in Pole.Children)
            {
                foreach (Piece currentPiece in currentSelection)
                {
                    if (currentPiece != piece)
                    {
                        if (piece.Row == cellY && piece.Col == cellX) ret = false;
                    }
                }
            }

            return ret;
        }

        private bool IsCompleted()//завершення гри
        {
            bool ret = true;
            int index = 0;
            for (int i = 0; i < pieces.Count; i++)
            {
                ret = false;
                foreach (Piece p in Pole.Children)
                {
                    if (index == (p.Row * rows + p.Col) && index == p.Index)
                    {
                        ret = true;
                        break;
                    }
                }
                if (ret == false) break;
                index++;
            }
            return ret;
        }
        #endregion methods

        #region events
        private void piece_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        // Обробник події натискання лівої кнопки миші на пазл

        {
            var chosenPiece = (Piece)sender;

            if (chosenPiece.Parent is WrapPanel)
            {
                if (currentSelection.Count() > 0)
                {
                    var p = currentSelection[0];
                    Pole.Children.Remove(p);
                    p.Visibility = Visibility.Visible;
                    Podbor.Children.Add(p);
                    currentSelection.Clear();
                }
                else
                {
                    Podbor.Children.Remove(chosenPiece);
                    Pole.Children.Add(chosenPiece);
                    chosenPiece.Visibility = Visibility.Hidden;
                    currentSelection.Add(chosenPiece);
                }
            }
        }

        private void pole_MouseDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void pole_MouseEnter(object sender, MouseEventArgs e)
        // Обробник події наведення миші на ігрове поле

        {
            if (currentSelection.Count > 0)
            {
                foreach (var currentPiece in currentSelection)
                {
                    currentPiece.Margin = new Thickness(0);
                    currentPiece.Visibility = Visibility.Visible;
                }
            }
        }

        private void pole_MouseMove(object sender, MouseEventArgs e)
        // Обробник події руху миші на ігровому полі

        {
            var newX = Mouse.GetPosition((IInputElement)Pole).X;
            var newY = Mouse.GetPosition((IInputElement)Pole).Y;

            if (currentSelection.Count > 0)
            {
                var firstPiece = currentSelection[0];
                foreach (var currentPiece in currentSelection)
                {
                    double CellX = currentPiece.Row - firstPiece.Row;
                    double CellY = currentPiece.Col - firstPiece.Col;
                    currentPiece.SetValue(Canvas.ZIndexProperty, 2);
                    currentPiece.SetValue(Canvas.LeftProperty, newX - 50 + CellX * 100);
                    currentPiece.SetValue(Canvas.TopProperty, newY - 50 + CellY * 100);
                }
            }
        }

        private void pole_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)           
            // Обробник події відпускання лівої кнопки миші на ігровому полі
        {
            var newX = Mouse.GetPosition(Pole).X;
            var newY = Mouse.GetPosition(Pole).Y;

            double cellX = (int)((newX) / 100);
            double cellY = (int)((newY) / 100);

            if (currentSelection.Count > 0)
            {
                if (CanInsertPiece((int)cellX, (int)cellY))
                {
                    var firstPiece = currentSelection[0];

                    var relativeCellX = currentSelection[0].Col - firstPiece.Col;
                    var relativeCellY = currentSelection[0].Row - firstPiece.Row;

                    double rotatedCellX = relativeCellX;
                    double rotatedCellY = relativeCellY;

                    foreach (Piece currentPiece in currentSelection)
                    {
                        currentPiece.Col = cellX + rotatedCellX;
                        currentPiece.Row = cellY + rotatedCellY;
                        currentPiece.SetValue(Canvas.LeftProperty, currentPiece.Col * 100);
                        currentPiece.SetValue(Canvas.TopProperty, currentPiece.Row * 100);
                        currentPiece.SetValue(Canvas.ZIndexProperty, 1);
                    }
                    currentSelection.Clear();
                }
                if (Podbor.Children.Count == 0)
                    if (IsCompleted() == true)
                    {
                        MessageBox.Show("Пазл зібрано! Ти молодець! (⌒▽⌒)☆ ");
                        Pole.IsEnabled = false;
                    }
            }
            else
            {
                foreach (Piece p in Pole.Children)
                {
                    if ((p.Col == cellX) && (p.Row == cellY))
                    {
                        p.Visibility = Visibility.Visible;
                        currentSelection.Add(p);
                    }
                }
            }
        }

        private void pole_MouseLeave(object sender, MouseEventArgs e)
        // Обробник події виходу миші з ігрового поля

        {
            if (currentSelection.Count > 0)
            {
                foreach (var p in currentSelection)
                {
                    Pole.Children.Remove(p);
                    p.Margin = new Thickness(5);
                    Podbor.Children.Add(p);
                }
                currentSelection.Clear();
            }
        }

        private void btnCheckImage_Click(object sender, RoutedEventArgs e)
        // Обробник події натискання кнопки для вибору зображення
        {
            OpenFileDialog openDialog = new OpenFileDialog()
            {
                Filter = "All Image Files ( JPEG,GIF,BMP,PNG)|*.jpg;*.jpeg;*.gif;*.bmp;*.png|JPEG Files ( *.jpg;*.jpeg )|*.jpg;*.jpeg|GIF Files ( *.gif )|*.gif|BMP Files ( *.bmp )|*.bmp|PNG Files ( *.png )|*.png",
                Multiselect = false,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                Title = "Check puzzle image"
            };

            if (openDialog.ShowDialog() == true)
            {
                LoadImage(openDialog.FileName);
                btnShowImage.IsEnabled = true;
                CreatePieces();
                CreatePole();
            }
        }

        private void btnShowImage_Click(object sender, RoutedEventArgs e)
            // Обробник події натискання кнопки для перегляду або приховання зображення
        {
            if (PuzzleGrid.Visibility == Visibility.Visible)
            {
                PuzzleGrid.Visibility = Visibility.Hidden;
                PuzzleImg.Source = imageSource;
                PuzzleImg.Visibility = Visibility.Visible;
                PuzzleImg.Stretch = Stretch.Uniform;
            }
            else
            {
                PuzzleImg.Visibility = Visibility.Collapsed;
                PuzzleGrid.Visibility = Visibility.Visible;
            }

        }
        #endregion events
    }
}
