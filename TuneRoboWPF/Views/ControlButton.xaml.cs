using System;
using System.Windows.Controls;
using TuneRoboWPF.RobotService;
using TuneRoboWPF.Utility;
using TuneRoboWPF.ViewModels;

namespace TuneRoboWPF.Views
{
    /// <summary>
    /// Interaction logic for ControlButton.xaml
    /// </summary>
    public partial class ControlButton : UserControl
    {
        public delegate void UpdateParentControlEventHandler(object sender);

        public event UpdateParentControlEventHandler UpdateParentControl;

        private void OnUpdateParentControl(object sender)
        {
            UpdateParentControlEventHandler handler = UpdateParentControl;
            if (handler != null) handler(sender);
        }

        public ControlButton()
        {
            InitializeComponent();
            var viewModel = new ControlButtonViewModel();
            DataContext = viewModel;
            ViewModel = (ControlButtonViewModel)(DataContext);
            Request = null;
        }        
        public ControlButtonViewModel ViewModel;
        public RemoteRequest Request { get; set; }
        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (Request == null) return;
            ClickProcess();
            Request.ProcessSuccessfully += data =>
                                               {                                                   
                                                   OnProcessSuccessfully(data);
                                                   OnUpdateParentControl(null);
                                               };
            Request.ProcessError += (errorCode, msg) =>
                                        {
                                            Console.WriteLine(msg);
                                            OnProcessError();
                                        };
            GlobalVariables.RobotWorker.AddJob(Request);
        }

        protected virtual void OnProcessSuccessfully(ReplyData data)
        {            
        }

        protected virtual void OnProcessError()
        {
        }

        protected virtual void ClickProcess()
        {            
        }
    }
}
