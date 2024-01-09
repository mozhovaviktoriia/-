using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;

namespace PuzzleGame
{
    class Piece : Grid
    {
        #region атрибути
        Path path;
        string imageUri;//рядок для збереження URI
        double col, row;//змінні для зберігання колонки та рядка
        int index;//цілочисельна змінна для зберігання індексу пазла
        #endregion

        #region конструктор
        public Piece(ImageSource imageSource, double col, double row)//конструктор що приймає зображення та позиції колонки та рядка для ініціалізації об'єкта пазла.
                                                                     //це параметр конструктора, який приймає зображення
        {
            ImageUri = imageUri;
            Col = col;
            Row = row;

            path = new Path();

            // Налаштування малюнку для пазла
            path.Fill = new ImageBrush()
            {
                ImageSource = imageSource,
                Stretch = Stretch.Fill,//це режим розтягування зображення
                ViewportUnits = BrushMappingMode.Absolute, // Вказує, що значення у Viewport вказано в абсолютних одиницях
                Viewport = new Rect(0, 0, 100, 100),// Визначає область зображення, яку буде видно на пазлі
                ViewboxUnits = BrushMappingMode.Absolute,
                Viewbox = new Rect(col * 100,row * 100,100,100)//шрина та висота області Viewbox
            };

            // Визначення геометрії пазла у вигляді прямокутника
            path.Data = new RectangleGeometry(new Rect(0, 0, 100, 100));
            // Додавання шляху (прямокутника з малюнком) до пазла (Grid)
            this.Children.Add(path);
        }
        #endregion

        #region властивості
        // Властивість для отримання або встановлення URI малюнку
        public string ImageUri { get { return imageUri; } set { imageUri = value; } }
        // Властивості для отримання або встановлення позиції пазла за колонкою і рядком
        public double Col { get { return col; } set { col = value; } }
        public double Row { get { return row; } set { row = value; } }
        // Властивість для отримання або встановлення індексу пазла
        public int Index { get { return index; } set { index = value; } }
        #endregion
    }
}
