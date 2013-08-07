using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TuneRoboWPF.Utility;
using Application = System.Windows.Application;

namespace TuneRoboWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Preprocess(null, null);
            //var preprocessThread = new BackgroundWorker();
            //preprocessThread.DoWork += Preprocess;                           
            //preprocessThread.RunWorkerAsync();
        }


        private void Preprocess(object sender, DoWorkEventArgs e)
        {
            GlobalFunction.ReadConfig();
            GlobalFunction.CheckExistAndCreateDirectory(GlobalFunction.GetSavedDir());
            GlobalFunction.GetTempDataFolder();
            GlobalFunction.ClearCacheImage();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            StaticMainWindow.Window = new MainWindow();
            StaticMainWindow.Window.Show();

#if RELEASE
            NBug.Settings.Destination1 = "Type=Mail;From=huupc@tosy.com;FromName=NBug Error Reporter;To=huupc@tosy.com;Cc=huongptt@tosy.com;SmtpServer=mail.tosy.com;Port=587;Priority=Normal;Username=huupc@tosy.com;Password=ph0ngv4n;";
            //NBug.Settings.Destination2 = "Type=Mail;From=huupc@tosy.com;To=huongptt@tosy.com;SmtpServer=mail.tosy.com;";
            AppDomain.CurrentDomain.UnhandledException += NBug.Handler.UnhandledException;
            Current.DispatcherUnhandledException += NBug.Handler.DispatcherUnhandledException;
            System.Threading.Tasks.TaskScheduler.UnobservedTaskException += NBug.Handler.UnobservedTaskException; 
#endif
            EventManager.RegisterClassHandler(typeof(TextBox), UIElement.GotFocusEvent, new RoutedEventHandler(TextBox_SelectAllText));
            EventManager.RegisterClassHandler(typeof(TextBox), UIElement.PreviewMouseDownEvent, new MouseButtonEventHandler(TextBox_SelectivelyIgnoreMouseButton));
        }

        /// <summary>
        /// Handles PreviewMouseDown Event.  Selects all on Triple click.  
        /// If SelectAllOnEnter is true, when the textbox is clicked and doesn't already have keyboard focus, selects all
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_SelectivelyIgnoreMouseButton(object sender, MouseButtonEventArgs e)
        {
            // If its a triple click, select all text for the user.
            if (e.ClickCount == 3)
            {
                TextBox_SelectAllText(sender, new RoutedEventArgs());
                return;
            }

            // Find the TextBox
            DependencyObject parent = e.OriginalSource as UIElement;
            while (parent != null && !(parent is TextBox))
            {
                parent = System.Windows.Media.VisualTreeHelper.GetParent(parent);
            }

            if (parent != null)
            {
                var textBox = parent as TextBox;
                if (!textBox.IsKeyboardFocusWithin)
                {
                    // If the text box is not yet focused, give it the focus and
                    // stop further processing of this click event.
                    textBox.Focus();
                    e.Handled = true;
                }
            }
        }
        private void TextBox_SelectAllText(object sender, RoutedEventArgs e)
        {
            ((TextBox)sender).SelectAll();
        }
    }
}
