﻿using System;
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
                line.X1 = vline.V1.X*100;
                line.Y1 = Plot.ActualHeight - vline.V1.Y*100;
                line.X2 = vline.V2.X*100;
                line.Y2 = Plot.ActualHeight - vline.V2.Y*100;
                line.Opacity = vline.Intensity;

                if (vm.Visualiser.Lines[0] == vline || vm.Visualiser.Lines[vm.Visualiser.Lines.Count - 1] == vline)
                    line.Stroke = Brushes.Red;
                else
                    line.Stroke = Brushes.Black;
                Plot.Children.Add(line);
            }
        }
    }
}
