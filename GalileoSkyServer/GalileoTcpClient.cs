using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace GalileoSkyServer
{
    class GalileoTcpClient : IGalileoSkyClient
    {

        public GalileoTcpClient(TcpClient inTcpClient, Int32 inReceiveBufferLength)
        {
            mNetworkStream = inTcpClient.GetStream();
            
            Buffer = new byte[inReceiveBufferLength];
            
            OutcomingMessagesQ = new Queue<String>();
            
            isSending = false;

            mNetworkStream.BeginRead(Buffer, 0, Buffer.Length, DataReceivedCallback, null);
        }

        #region ---------SendingRegion---------

        public void SendMessage(string inMessage)
        {
            lock (OutcomingMessagesQ)
            {
                OutcomingMessagesQ.Enqueue(inMessage);
            }
            SendMessageCallback(null);
        }


        private void SendMessageCallback(IAsyncResult result)
        {
            //	result wont be null if current call to the SendMessageCallback is a continuation of BeginWrite execution
            //	result will be null if current call to the SendMessageCallback is a continuation of SendMessage execution 
            if (result != null && mNetworkStream != null)
            {
                mNetworkStream.EndWrite(result);
                lock (OutcomingMessagesQ)
                {
                    isSending = false;
                }
            }

            string CurrentMessage = null;

            lock (OutcomingMessagesQ)
            {
                //There is a possibility, that call to SendMessageCallback was made through SendMessage,
                //while another writing is in progress, so the call just returns, as inMessage was appended to the queue, and will be delivered in its turn
                if (isSending)
                {
                    return;
                }
                if (OutcomingMessagesQ.Count != 0)
                {
                    isSending = true;
                    CurrentMessage = OutcomingMessagesQ.Dequeue();
                    //Console.WriteLine(CurrentMessage);
                }
            }

            //CurrentMessage wont be null if the queue was not empty
            //CurrentMessage will be null if the queue was empty
            if (CurrentMessage != null && mNetworkStream != null)
            {

                mNetworkStream.BeginWrite(Encoding.UTF8.GetBytes(CurrentMessage), 0, Encoding.UTF8.GetBytes(CurrentMessage).Length, new AsyncCallback(SendMessageCallback), null);

            }
        }

        #endregion

        #region ---------ReceivingRegion---------

        protected void DataReceivedCallback(IAsyncResult result)
        {
            lock (mNetworkStreamLock)
            {
                if (mNetworkStream != null)
                {
                    int receivedDataLength = mNetworkStream.EndRead(result);

                    byte[] ReceivedData = new byte[receivedDataLength];
                    Array.Copy(Buffer, ReceivedData, receivedDataLength);

                    if (DataParser != null)
                    {
                        if (DataParser.AddDataAndParse(ReceivedData) != null)
                        {

                        }
                    }

                    OnPackageReceived(new ReceivedDataArgs(ReceivedData));

                    mNetworkStream.BeginRead(Buffer, 0, Buffer.Length, new AsyncCallback(DataReceivedCallback), null);
                }
            }
        }

        protected virtual void OnPackageReceived(ReceivedDataArgs ea)
        {
            if (DataReceived != null)
            {
                DataReceived(this, ea);
            }
        }

        #endregion

        #region Accessors&Setters

        public IDataParser DataParser
        {
            get {
                return mDataParser;
            }
            set {
                mDataParser = value;
            }
        }

        #endregion


        #region ---------TcpClientFields---------

        protected IDataParser mDataParser;

        protected NetworkStream mNetworkStream;

        protected object mNetworkStreamLock = new object();

        protected byte[] Buffer;

        protected Queue<String> OutcomingMessagesQ;

        protected bool isSending;

        public event EventHandler<ReceivedDataArgs> DataReceived;

        #endregion


    }
}
