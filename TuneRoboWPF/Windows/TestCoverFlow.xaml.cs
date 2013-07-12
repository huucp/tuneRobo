using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace TuneRoboWPF.Windows
{
    /// <summary>
    /// Interaction logic for TestCoverFlow.xaml
    /// </summary>
    public partial class TestCoverFlow : Window
    {
        public TestCoverFlow()
        {
            InitializeComponent();

            Image1.Source = new BitmapImage(new Uri(new FileInfo("1.jpg").FullName, UriKind.RelativeOrAbsolute));
            Image2.Source = new BitmapImage(new Uri(new FileInfo("2.jpg").FullName, UriKind.RelativeOrAbsolute));
            Image3.Source = new BitmapImage(new Uri(new FileInfo("3.jpg").FullName, UriKind.RelativeOrAbsolute));
            Image4.Source = new BitmapImage(new Uri(new FileInfo("4.jpg").FullName, UriKind.RelativeOrAbsolute));

            thumnailList.Add(Image1);
            thumnailList.Add(Image2);
            thumnailList.Add(Image3);
            thumnailList.Add(Image4);

            AddTransformList();

            firstMargin = thumnailList[0].Margin;

            Cover1.Source = Image2.Source;
            Cover1.Opacity = 0;
            Cover2.Source = Image3.Source;

            Panel.SetZIndex(Cover1,1);
            Panel.SetZIndex(Cover2,2);

            //Console.WriteLine(Cover1.Opacity);
            //Console.WriteLine(Cover2.Opacity);


            timer.Interval = new TimeSpan(0, 0, 5);
            timer.Tick += timer_Tick;
            //timer.Start();
        }

        private List<List<int>> transformList = new List<List<int>>();
        private int currentTransform = 0;
        private void AddTransformList()
        {
            var l1 = new List<int>() { 0, 1, 2, 3 };
            var l2 = new List<int>() { 3, 0, 1, 2 };
            var l3 = new List<int>() { 2, 3, 0, 1 };
            var l4 = new List<int>() { 1, 2, 3, 0 };
            transformList.Add(l1);
            transformList.Add(l2);
            transformList.Add(l3);
            transformList.Add(l4);
        }

        void timer_Tick(object sender, EventArgs e)
        {
            CoverFlowing();
        }
        List<Image> thumnailList = new List<Image>();

        private Thickness firstMargin;

        private DispatcherTimer timer = new DispatcherTimer();

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Right:
                    //CoverFlowControlTest.GoToNext();
                    break;
                case Key.Left:
                    //CoverFlowControlTest.GoToPreviouse();
                    break;
            }

        }

        private void TranslationAnimate(int index, double from, double to)
        {
            var da = new DoubleAnimation();
            da.From = from;
            da.To = to;
            da.Duration = new Duration(TimeSpan.FromSeconds(1));
            var tt = new TranslateTransform();
            var image = thumnailList[index];
            image.RenderTransform = tt;
            da.Completed += (sender, e) =>
            {
                tt.BeginAnimation(TranslateTransform.YProperty, null);
                if (index == nextReserveImageIndex)
                {
                    image.Margin = firstMargin;
                }
                else image.Margin = new Thickness(0, image.Margin.Top + 90, 0, 0);
                if (index == thumnailList.Count - 1)
                {
                    if (nextReserveImageIndex == 0) nextReserveImageIndex = thumnailList.Count - 1;
                    else nextReserveImageIndex--;
                }
            };
            tt.BeginAnimation(TranslateTransform.YProperty, da);
        }

        private void OpacityAnimate(Image image, double from, double to)
        {
            var da = new DoubleAnimation()
                         {
                             From = from,
                             To = to,
                             Duration = new Duration(TimeSpan.FromSeconds(1))
                         };
            da.Completed += (sender, e) =>
            {
                image.BeginAnimation(OpacityProperty, null);
                if (Cover1CanVisible)
                {
                    //Bring cover1 to front   
                    var tmpZIndex = Panel.GetZIndex(Cover1);
                    Panel.SetZIndex(Cover1, Panel.GetZIndex(Cover2));
                    Panel.SetZIndex(Cover2, tmpZIndex);
                    //Change to next image
                    Cover2.Source = thumnailList[nextImageIndex].Source;

                    Cover1CanVisible = false;
                }
                else
                {
                    //Bring cover 2 to front
                    var tmpZIndex = Panel.GetZIndex(Cover2);
                    Panel.SetZIndex(Cover2, Panel.GetZIndex(Cover1));
                    Panel.SetZIndex(Cover1, tmpZIndex);
                    //Change to next image
                    Cover1.Source = thumnailList[nextImageIndex].Source;

                    Cover1CanVisible = true;
                }
                //Console.WriteLine(Panel.GetZIndex(Cover1));
                //Console.WriteLine(Panel.GetZIndex(Cover2));
                nextImageIndex--;
                if (nextImageIndex < 0) nextImageIndex = thumnailList.Count - 1;
                Console.WriteLine(nextImageIndex);
            };
            image.BeginAnimation(OpacityProperty, da);
        }


        private int nextReserveImageIndex = 3;
        private int nextImageIndex = 0;
        private bool Cover1CanVisible = true;

        private void CoverFlowing()
        {
            for (int i = 0; i < thumnailList.Count; i++)
            {
                TranslationAnimate(i, 0, 90);
            }            
            if (Cover1CanVisible)
            {
                Cover1.Opacity = 1;
                OpacityAnimate(Cover2, 1, 0);
                //Cover2.Opacity = 0;
                //Cover2.Source = thumnailList[nextReserveImageIndex].Source;
                //Cover1CanVisible = false;
            }
            else
            {
                Cover2.Opacity = 1;
                OpacityAnimate(Cover1, 1, 0);
                //Cover1.Opacity = 0;
                //Cover1.Source = thumnailList[nextReserveImageIndex].Source;
                //Cover1CanVisible = true;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CoverFlowing();
        }

        private void PrintThumnailMargin()
        {
            foreach (var image in thumnailList)
            {
                Console.WriteLine(image.Margin.Top);
            }
        }
    }
}
