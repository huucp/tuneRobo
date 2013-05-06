using System.Diagnostics;
using System.Net.Sockets;
using System.Windows.Forms;
using TuneRoboWPF.Utility;
using comm;

namespace TuneRoboWPF.StoreService
{
    public class StoreRequest : IRequest
    {
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
        public object Process()
        {

            var count = 0;
            StoreConnection connection;
            do
            {
                connection = GlobalVariables.ServerConnection;
                if (connection.Connected)
                {
                    break;
                }
                count++;
            } while (count < StoreConnection.RetryTime);
            if (count == StoreConnection.RetryTime)
            {
                MessageBox.Show("You lost connection, please check your connection", "Connection error",
                                MessageBoxButtons.OK);
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
            if (dataReply.type == (decimal) Reply.Type.OK)
            {
                OnProcessSuccessfully(dataReply);
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
}
