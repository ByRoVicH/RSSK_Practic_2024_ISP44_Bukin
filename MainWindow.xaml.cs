using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
namespace PressureChartApp
{
    public partial class MainWindow : Window
    {
        private const int MaxPoints = 100;
        private const double CanvasHeight = 300;
        private const double CanvasWidth = 475;
        private Random random = new Random();
        private List<double> pressureValues = new List<double>();
        private DispatcherTimer timer;
        private Polyline graphLine;
        private Line xAxisLine;
        private Line yAxisLine;
        private List<TextBlock> xLabels = new List<TextBlock>();
        private List<TextBlock> yLabels = new List<TextBlock>();
        private int totalPoints = 0;
        private bool isPaused = false;
        public MainWindow()
        {
            InitializeComponent();
            InitializeCanvas();
            InitializeTimer();
        }
        private void InitializeCanvas()
        {
            // Создаем линию графика
            graphLine = new Polyline
            {
                Stroke = Brushes.Magenta,
                StrokeThickness = 2
            };
            PressureCanvas.Children.Add(graphLine);
            // Создаем ось X
            xAxisLine = new Line
            {
                Stroke = Brushes.Black,
                StrokeThickness = 1,
                X1 = 0,
                Y1 = CanvasHeight,
                X2 = CanvasWidth,
                Y2 = CanvasHeight
            };
            PressureCanvas.Children.Add(xAxisLine);
            // Создаем ось Y
            yAxisLine = new Line
            {
                Stroke = Brushes.Black,
                StrokeThickness = 1,
                X1 = 0,
                Y1 = 0,
                X2 = 0,
                Y2 = CanvasHeight
            };
            PressureCanvas.Children.Add(yAxisLine);
        }
        private void InitializeTimer()
        {
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(500)
            };
            timer.Tick += UpdateGraph;
            timer.Start();
        }
        private void UpdateGraph(object sender, EventArgs e)
        {
            double currentPressure = random.Next(720, 780); // Генерация значения давления
            pressureValues.Add(currentPressure);
            totalPoints++;
            if (pressureValues.Count > MaxPoints)
            {
                pressureValues.RemoveAt(0);
            }
            UpdateGraphLine();
            UpdateAxesLabels();
            UpdateTextDisplays(currentPressure);
        }
        private void UpdateGraphLine()
        {
            graphLine.Points.Clear();
            double step = CanvasWidth / MaxPoints;
            for (int i = 0; i < pressureValues.Count; i++)
            {
                double x = i * step;
                double y = CanvasHeight - (pressureValues[i] - 700) * CanvasHeight / 100;
                graphLine.Points.Add(new Point(x, y));
            }
        }
        private void UpdateAxesLabels()
        {
            // Очистка старых меток
            foreach (var label in xLabels) PressureCanvas.Children.Remove(label);
            foreach (var label in yLabels) PressureCanvas.Children.Remove(label);
            xLabels.Clear();
            yLabels.Clear();
            // Добавляем метки на ось X
            double xStep = CanvasWidth / 10;
            int startValue = totalPoints > MaxPoints ? totalPoints - MaxPoints : 0;
            for (int i = 0; i <= 10; i++)
            {
                int value = startValue + i * MaxPoints / 10; // Значения на оси X
                TextBlock label = new TextBlock
                {
                    Text = $"{value:F0}",
                    Foreground = Brushes.Black
                };
                Canvas.SetLeft(label, i * xStep - 10);
                Canvas.SetTop(label, CanvasHeight + 5);
                PressureCanvas.Children.Add(label);
                xLabels.Add(label);
            }
            // Добавляем метки на ось Y
            double yStep = CanvasHeight / 5;
            for (int i = 0; i <= 5; i++)
            {
                double value = 700 + (100 - i * 20); // Значения на оси Y
                TextBlock label = new TextBlock
                {
                    Text = $"{value:F0}",
                    Foreground = Brushes.Black
                };
                Canvas.SetLeft(label, -25);
                Canvas.SetTop(label, i * yStep - 10);
                PressureCanvas.Children.Add(label);
                yLabels.Add(label);
            }
        }
        private void UpdateTextDisplays(double currentPressure)
        {
            CurrentPressureText.Text = $"Давление в данный момент: {currentPressure:F2}";
            double averagePressure = pressureValues.Average();
            AveragePressureText.Text = $"Среднее давление: {averagePressure:F2}";
        }
        private void StartPauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (isPaused)
            {
                timer.Start();
                StartPauseButton.Content = "Стоп";
                isPaused = false;
            }
            else
            {
                timer.Stop();
                StartPauseButton.Content = "Старт";
                isPaused = true;
            }
        }
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void InfoButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Построить график давления в Туле на основе ГСЧ. График должен строиться постоянно, однако, для грамотного отображения нужно использовать паузу. Должны выводиться текущие показания и средние прогнозируемые (источник текущих показаний заменить ГСЧ)");
        }
    }
}
