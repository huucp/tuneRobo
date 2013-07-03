using System;
using System.Diagnostics;
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
            ClickProcess();
            if (Request == null) return;
            Request.ProcessSuccessfully += data =>
            {
                //OnProcessSuccessfully(data);
                //OnUpdateParentControl(null);
                GetState();
            };
            Request.ProcessError += (errorCode, msg) =>
                                        {
                                            Debug.Fail(msg);
                                            switch (errorCode)
                                            {
                                                case RobotRequest.ErrorCode.SetupConnection:
                                                case RobotRequest.ErrorCode.WrongSessionID:
                                                    OnUpdateParentControl("MustReconnect");
                                                    break;
                                            }
                                            OnProcessError();
                                        };
            GlobalVariables.RobotWorker.AddJob(Request);
        }

        private void GetState()
        {
            var stateRequest = new GetStateRequest();
            stateRequest.ProcessSuccessfully += data =>
            {
                OnProcessSuccessfully(data);
                OnUpdateParentControl(null);
            };
            stateRequest.ProcessError += (errorCode, msg) =>
            {
                Debug.Fail(msg);
                switch (errorCode)
                {
                    case RobotRequest.ErrorCode.SetupConnection:
                    case RobotRequest.ErrorCode.WrongSessionID:
                        OnUpdateParentControl("MustReconnect");
                        break;
                }
                OnProcessError();
            };
            GlobalVariables.RobotWorker.AddJob(stateRequest);
        }

        protected virtual void OnProcessSuccessfully(RobotReplyData data)
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
