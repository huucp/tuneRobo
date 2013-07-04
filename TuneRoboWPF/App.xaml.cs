using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
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
            var preprocessThread = new BackgroundWorker();
            preprocessThread.DoWork += Preprocess;                           
            preprocessThread.RunWorkerAsync();
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

            if (false)
            {
                NBug.Settings.Destination1 = "Type=Mail;From=huupc@tosy.com;To=huupc@tosy.com;SmtpServer=mail.tosy.com;";

                AppDomain.CurrentDomain.UnhandledException += NBug.Handler.UnhandledException;
                Current.DispatcherUnhandledException += NBug.Handler.DispatcherUnhandledException;
                System.Threading.Tasks.TaskScheduler.UnobservedTaskException += NBug.Handler.UnobservedTaskException; 
            }

        }
    }
}
