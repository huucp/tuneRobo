using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CoverFlow
{
    /// <summary>
    /// Interaction logic for CoverFlowControl.xaml
    /// </summary>
    public partial class CoverFlowControl : UserControl
    {
        private List<Cover> coverList = new List<Cover>();
        private int index;
        public CoverFlowControl()
        {
            InitializeComponent();

            var image = new FileInfo("Katy Perry.jpg");
            for (int i = 0; i < 5; i++)
            {
                var cover = new Cover(image.FullName, i);
                coverList.Add(cover);
                visualModel.Children.Add(cover);
            }
            UpdateIndex(1);
        }

        private void RotateCover(int pos)
        {
            coverList[pos].Animate(index);
        }
        private void UpdateIndex(int newIndex)
        {
            if (index != newIndex)
            {
                int oldIndex = index;
                index = newIndex;
                RotateCover(oldIndex);
                RotateCover(index);
                camera.Position = new Point3D(.2 * index, camera.Position.Y, camera.Position.Z);
            }
        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            int newIndex = index;
            switch (e.Key)
            {
                case Key.Right:
                    if (newIndex < coverList.Count - 1)
                        newIndex++;
                    break;
                case Key.Left:
                    if (newIndex > 0)
                        newIndex--;
                    break;
            }
            UpdateIndex(newIndex);
        }

        public void GoToNext()
        {
            int newIndex = index;
            if (newIndex < coverList.Count - 1)
            {
                newIndex++;
            }else
            {
                newIndex = 0;
            }
            UpdateIndex(newIndex);

        }

        public void GoToPreviouse()
        {
            int newIndex = index;
            if (newIndex > 0)
            {
                newIndex--;                
            }else
            {
                newIndex = coverList.Count - 1;
            }
            UpdateIndex(newIndex);
        }
    }
}
