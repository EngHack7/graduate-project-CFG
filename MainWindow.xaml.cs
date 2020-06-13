using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CParser;
namespace DrawCFGraph
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        const string fontFamily = "Verdana";

        const double lineThickness = 4;

        private double deltaWidth = 0;
        private double deltaHeight = 0;

        private double moveX = 0;
        public mediator mediator = new mediator();
        public MainWindow()
        {
            InitializeComponent();
            this.Hide();
            mediator.Show();
        }

        private void resizeCanvas()
        {
            this.drawingCanvas.Width += this.deltaWidth + this.moveX;
            this.drawingCanvas.Height += this.deltaHeight;

            this.scrollViewerDisplay.ScrollToHorizontalOffset(scrollViewerDisplay.ScrollableWidth / 2);

            foreach (System.Windows.UIElement child in this.drawingCanvas.Children)
            {
                child.RenderTransform = new System.Windows.Media.TransformGroup()
                {
                    Children = new System.Windows.Media.TransformCollection()
                    {
                        child.RenderTransform,
                        new System.Windows.Media.TranslateTransform(this.moveX, 0)
                    }
                };
            }
        }


        private void updateCounters(double x1, double y1, double x2, double y2)
        {
            if (x1 < 0 && -x1 > this.moveX)
            {
                this.moveX = -x1 + lineThickness / 2;
                if (-x1 > this.deltaWidth)
                    this.deltaWidth = -x1 + lineThickness / 2;
            }

            if (x1 > this.drawingCanvas.Width && x1 - this.drawingCanvas.Width > this.deltaWidth)
                this.deltaWidth = x1 - this.drawingCanvas.Width + lineThickness / 2;
            if (y1 > this.drawingCanvas.Height && y1 - this.drawingCanvas.Height > this.deltaHeight)
                this.deltaHeight = y1 - this.drawingCanvas.Height + lineThickness / 2;

            if (x2 < 0 && -x2 > this.moveX)
            {
                this.moveX = -x2 + lineThickness / 2;
                if (-x2 > this.deltaWidth)
                    this.deltaWidth = -x2 + lineThickness / 2;
            }

            if (x2 > this.drawingCanvas.Width && x2 - this.drawingCanvas.Width > this.deltaWidth)
                this.deltaWidth = x2 - this.drawingCanvas.Width + lineThickness / 2;
            if (y2 > this.drawingCanvas.Height && y2 - this.drawingCanvas.Height > this.deltaHeight)
                this.deltaHeight = y2 - this.drawingCanvas.Height + lineThickness / 2;
        }

        private void drawGraph(IDrawable start, double startX, double startY)
        {
            start.draw(startX, startY,
                start.width(
                    (codeString, fontHeight) =>
                    {
                        System.Windows.Media.FormattedText formattedText = new System.Windows.Media.FormattedText(codeString,
                            System.Globalization.CultureInfo.GetCultureInfo("en-us"), System.Windows.FlowDirection.LeftToRight,
                            new System.Windows.Media.Typeface(fontFamily), fontHeight, System.Windows.Media.Brushes.Transparent);

                        return formattedText.Width;
                    }),

                (x1, y1, x2, y2) =>
                {
                    System.Windows.Shapes.Line line = new System.Windows.Shapes.Line()
                    {
                        Visibility = System.Windows.Visibility.Visible,
                        StrokeThickness = lineThickness,
                        Stroke = System.Windows.Media.Brushes.Black,
                        X1 = x1,
                        Y1 = y1,
                        X2 = x2,
                        Y2 = y2
                    };

                    updateCounters(x1, y1, x2, y2);
                    
                    this.drawingCanvas.Children.Add(line);
                    
                },

                (text, fontHeight, x, y) =>
                {
                    System.Windows.Controls.TextBlock textBlock = new System.Windows.Controls.TextBlock()
                    {
                        FontSize = fontHeight,
                        Text = text,
                        RenderTransform = new System.Windows.Media.TranslateTransform(x, y),

                        FontFamily = new System.Windows.Media.FontFamily(fontFamily)
                    };
                    primaryPath.Content += textBlock.Text + " || ";
                    this.drawingCanvas.Children.Add(textBlock);
                });

            resizeCanvas();
        }
        class Graph
        {
            Dictionary<char, Dictionary<char, int>> vertices = new Dictionary<char, Dictionary<char, int>>();

            public void add_vertex(char name, Dictionary<char, int> edges)
            {
                vertices[name] = edges;
            }

            public List<char> shortest_path(char start, char finish)
            {
                var previous = new Dictionary<char, char>();
                var distances = new Dictionary<char, int>();
                var nodes = new List<char>();

                List<char> path = null;

                foreach (var vertex in vertices)
                {
                    if (vertex.Key == start)
                    {
                        distances[vertex.Key] = 0;
                    }
                    else
                    {
                        distances[vertex.Key] = int.MaxValue;
                    }

                    nodes.Add(vertex.Key);
                }

                while (nodes.Count != 0)
                {
                    nodes.Sort((x, y) => distances[x] - distances[y]);

                    var smallest = nodes[0];
                    nodes.Remove(smallest);

                    if (smallest == finish)
                    {
                        path = new List<char>();
                        while (previous.ContainsKey(smallest))
                        {
                            path.Add(smallest);
                            smallest = previous[smallest];
                        }

                        break;
                    }

                    if (distances[smallest] == int.MaxValue)
                    {
                        break;
                    }

                    foreach (var neighbor in vertices[smallest])
                    {
                        var alt = distances[smallest] + neighbor.Value;
                        if (alt < distances[neighbor.Key])
                        {
                            distances[neighbor.Key] = alt;
                            previous[neighbor.Key] = smallest;
                        }
                    }
                }

                return path;
            }
        }

        public void File_Reader()
        {

            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();

            dialog.DefaultExt = ".txt";
            dialog.Filter = "C source files (*.c)|*.c;*.h|All Files|*";

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    displayProgram(System.IO.File.ReadAllText(dialog.FileName));


                }
                catch
                {
                    System.Windows.MessageBox.Show("error reading file " + dialog.FileName);
                }
            }
        }

        public void displayProgram(string program)
        {
            double currY = 10;

            this.drawingCanvas.Children.Clear();

            this.drawingCanvas.Height = 0;
            this.drawingCanvas.Width = 0;
            this.deltaWidth = 0;
            this.deltaHeight = 0;
            this.moveX = 0;

            try
            {
                CToken[] tokens = CLexer.parseTokens(program);
                
                foreach (CGrammar.CFunctionDefinition matchedFunction in CParser.CGrammar.CParser.getFunctionContents(tokens))
                {
                    drawGraph(matchedFunction, 0, currY);
                    currY += matchedFunction.height + 60;
                }
            }
            catch (System.ArgumentException exc)
            {
                System.Windows.MessageBox.Show("There is an error in your source code: " + exc.Message);
            }
            catch (System.InvalidOperationException exc)
            {
                System.Windows.MessageBox.Show("There is an error in your source code: " + exc.Message);
            }
            catch
            {
                System.Windows.MessageBox.Show("An unexpected error occured!");
            }
        }

        private void drawingCanvas_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            this.scrollViewerDisplay.ScrollToHorizontalOffset((e.NewSize.Width - this.ActualWidth) / 2);
        }

        private void Window_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            this.scrollViewerDisplay.ScrollToHorizontalOffset((this.drawingCanvas.ActualWidth - e.NewSize.Width) / 2);
        }
        private void Window_SizeChange(object sender, System.Windows.SizeChangedEventArgs e)
        {
            this.scrollViewerDisplay.ScrollToVerticalOffset((this.drawingCanvas.ActualWidth - e.NewSize.Width) / 2);
        }

       private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {

            this.File_Reader();
           
        }

        private void Button_Click_1(object sender, System.Windows.RoutedEventArgs e)
        {
            Rect bounds = VisualTreeHelper.GetDescendantBounds(drawingCanvas);
            double dpi = 96d;

            RenderTargetBitmap rtb = new RenderTargetBitmap((int)bounds.Width, (int)bounds.Height, dpi, dpi, System.Windows.Media.PixelFormats.Default);

            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(drawingCanvas);
                dc.DrawRectangle(vb, null, new Rect(new Point(), bounds.Size));
            }

            rtb.Render(dv);
            BitmapEncoder pngEncoder = new PngBitmapEncoder();
            pngEncoder.Frames.Add(BitmapFrame.Create(rtb));

            try
            {
                System.IO.MemoryStream ms = new System.IO.MemoryStream();

                pngEncoder.Save(ms);
                ms.Close();

                System.IO.File.WriteAllBytes("logo.png", ms.ToArray());
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
