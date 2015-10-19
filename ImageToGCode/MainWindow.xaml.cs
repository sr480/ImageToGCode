using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ImageToGCode
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var vm = (MainViewModel)DataContext;

            if (vm.Presenter == null)
                return;

            var maxY = vm.Presenter.Lines.SelectMany(x => x.Pixels).Max(x => x.Y);

            Plot.Children.Clear();
            //foreach (var line in vm.Presenter.Lines)
            //{
            //    foreach (var point in line.Pixels)
            //    {
            //        var el = new Ellipse();
            //        el.Margin = new Thickness(point.X, point.Y, 0, 0);
            //        el.Width = 1;
            //        el.Height = 1;
            //        el.Fill = Brushes.Black;
            //        el.Opacity = 1 - point.Intensity;
            //        Plot.Children.Add(el);
            //    }
            //}

            foreach (var vline in vm.Visualiser.Lines)
            {
                var line = new Line();
                line.X1 = vline.V1.X;
                line.Y1 = Plot.ActualHeight - vline.V1.Y;
                line.X2 = vline.V2.X;
                line.Y2 = Plot.ActualHeight - vline.V2.Y;
                line.Opacity = vline.Intensity;
                line.Stroke = Brushes.Black;
                Plot.Children.Add(line);

                foreach (var point in line.Pixels)
                {
                    var el = new Ellipse();
                    el.Margin = new Thickness(point.X, maxY - point.Y, 0, 0);

                    if (vm.Presenter.Lines[0] == line || vm.Presenter.Lines[vm.Presenter.Lines.Count-1] == line)
                        el.Width = 4;
                    else
                        el.Width = 1;
                    
                    
                    
                    el.Height = 1;

                    if (vm.Presenter.Lines[0] == line || vm.Presenter.Lines[vm.Presenter.Lines.Count - 1] == line)
                        el.Fill = Brushes.Red;
                    else
                        el.Fill = Brushes.Black;
                    el.Opacity = 1 - point.Intensity;
                    Plot.Children.Add(el);
                }
            }
        }
    }
}
