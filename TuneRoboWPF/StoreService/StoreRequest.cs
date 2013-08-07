using System;
using System.Diagnostics;
using System.Diagnostics.Eventing;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using MessageBoxUtils;
using TuneRoboWPF.StoreService.SimpleRequest;
using TuneRoboWPF.Utility;
using comm;
using user;

namespace TuneRoboWPF.StoreService
{
    public class StoreRequest : IRequest
    {
        //public delegate void StoreRequestSuccessfullyEventHandler(Reply sender);
        FastSmartWeakEvent<EventHandler> _eEvent = new FastSmartWeakEvent<EventHandler>();
        public event EventHandler SuccesfullyEvent
        {
            add{_eEvent.Add(value);}
            remove {_eEvent.Remove(value);}
        }
        public void RaiseEvent(Reply sender)
        {
            _eEvent.Raise(sender,EventArgs.Empty);
        }

        public delegate void SuccessfullyEventHandler(Reply sender);

        public event SuccessfullyEventHandler ProcessSuccessfully;

        public void OnProcessSuccessfully(Reply sender)
        {
            SuccessfullyEventHandler handler = ProcessSuccessfully;
            if (handler != null) handler(sender);
        }

        public delegate void ErrorEventHandler(Reply sender, string msg);

        public event ErrorEventHandler ProcessError;

        public void OnProcessError(Reply sender, string msg)
        {
            ErrorEventHandler handler = ProcessError;
            if (handler != null) handler(sender, msg);
        }

        protected byte[] Packet { get; set; }
        protected string RequestKey { get; set; }

        private void AutoSignin()
        {
            //var signinRequest = new SigninStoreRequest(GlobalVariables.CurrentUser.Email, GlobalVariables.CurrentUser.Password,
            //                                           SigninRequest.Type.USER);
            //signinRequest.ProcessSuccessfully += (s) =>
            //{


            //};
            //signinRequest.ProcessError += (s, msg) => { };
            //GlobalVariables.StoreWorker.ForceAddRequest(signinRequest);
        }

        public object Process()
        {
            if (!string.IsNullOrEmpty(RequestKey))
            {
                if (GlobalVariables.RequestDictionary.ContainsKey(RequestKey))
                {
                    OnProcessSuccessfully(GlobalVariables.RequestDictionary[RequestKey]);
                    RaiseEvent(GlobalVariables.RequestDictionary[RequestKey]);
                    return GlobalVariables.RequestDictionary[RequestKey];
                }
            }
            var count = 0;
            StoreConnection connection;
            do
            {
                connection = GlobalVariables.ServerConnection;
                if (connection.SocketAlive)
                {
                    if (GlobalVariables.UserOnline && count > 0)
                    {
                        AutoSignin();
                        //GlobalVariables.StoreWorker.ForceAddRequest(this);
                        return null;
                    }
                    //GlobalVariables.ServerConnection.Connection.Close();                    
                    break;
                }
                GlobalVariables.ServerConnection.ConfigAndConnectSocket();
                count++;
            } while (count < StoreConnection.RetryTime);
            if (count == StoreConnection.RetryTime)
            {
                //if (GlobalVariables)
                OnProcessError(null, "Lost connection");
                return null;
            }
            BuildPacket();
            try
            {
                connection.SendPacket(Packet);
            }
            catch (SocketException se)
            {
                //if (se.SocketErrorCode == SocketError.TimedOut)
                //{
                //    return;
                //}
                OnProcessError(null, "Send packet error");
                return null;
            }

            StoreReply reply = null;
            try
            {
                var packet = connection.ReceiveReply();
                reply = new StoreReply(packet, GlobalVariables.CountRequest);
            }
            catch (SocketException se)
            {
                //if (se.SocketErrorCode == SocketError.TimedOut)
                //{
                //    return;
                //}
                OnProcessError(null, "Receive packet error");
                return null;
            }
            Debug.Assert(reply != null, "reply packet is null");
            Reply dataReply = reply.ProcessReply();
            if (dataReply == null)
            {
                OnProcessError(null, "Reply is null");
                return null;
            }
            if (dataReply.type == (decimal)Reply.Type.OK)
            {
                OnProcessSuccessfully(dataReply);
                RaiseEvent(dataReply);
                if (!string.IsNullOrEmpty(RequestKey))
                {
                    GlobalVariables.RequestDictionary.Add(RequestKey, dataReply);
                }
            }
            else
            {
                OnProcessError(dataReply, "Reply is failed");
            }
            return dataReply;
        }

        public virtual void BuildPacket()
        {
            Packet = new byte[] { };
        }

    }

    //public sealed class StoreRequestSuccesfullyEventManager:WeakEventManagerBase<StoreRequestSuccesfullyEventManager,StoreRequest>
    //{
    //    //protected override void StartListening(StoreRequest source)
    //    //{
    //    //    source.ProcessSuccessfully += DeliverEvent;
    //    //}

    //    //protected override void StopListening(StoreRequest source)
    //    //{
    //    //    source.ProcessSuccessfully -= DeliverEvent;
    //    //}
    //}

    public class StoreRequestSuccesfullyFastSmartEventSource
    {
        public delegate void StoreRequestSuccessfullyEventHandler(Reply sender);
        FastSmartWeakEvent<StoreRequestSuccessfullyEventHandler> _eEvent= new FastSmartWeakEvent<StoreRequestSuccessfullyEventHandler>();
        public event StoreRequestSuccessfullyEventHandler SuccesfullyEvent
        {
            add{_eEvent.Add(value);}
            remove {_eEvent.Remove(value);}
        }
        public void RaiseEvent(Reply sender)
        {
            _eEvent.Raise(sender,EventArgs.Empty);
        }

    }
}
