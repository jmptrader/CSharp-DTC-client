using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
// added for access to socket classes
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace DTCProtocol
{
    public class csClient
    {
        #region member variables

        Timer heartBeatTimer;
        //int countTimer = 0;

        /// <summary>
        /// Seconds since we received last server heartbeat message, used for reconnection logic.
        /// </summary>
        int secondsSinceLastServerHeartbeat = 0;
        /// <summary>
        /// If no heartbeat is received for this many seconds, fire a disconnected event.
        /// </summary>
        int secondsToFireDisconnectedEvent = 3;
        /// <summary>
        /// If disconnected, try to reconnect every 'secondsToAttemptReconnection' seconds.
        /// </summary>
        int secondsToAttemptReconnection = 10;

        object reconnectionLogicLock = new object();

        volatile bool dtcServerConnected = false;
        readonly string portDTC = "DTCPort";
        readonly int portNumber = 10000;
        //dtc sockets etc.
        AsyncCallback dtcCallback;
        Socket dtcSocket;
        byte[] dtcSocketBuffer = new byte[100000];
        private enum DTCSocketName { Level1, Level2, Lookup, Admin }

        string dtcIPAddress = "127.0.0.1";//localhost


        //needs to be marked volatile so that different threads pickup the correct values/changes
        volatile bool unprocessedDataExists = false;
        //this needs to be set to false properly before exiting application!-so that we don't have this thread hanging...
        volatile bool dataProcessingThreadRunning = true;                           // flag that keeps the message processing thread running
        Thread dataProcessingThread;                    // the data processing thread
        //this buffer will be (re)filled with coming messages in the data processing thread
        List<byte[]> dataToProcessBuffer;
        List<UInt16> typesToProcessBuffer;
        
        
        //note: each byte[] array is a complete message
        List<byte[]> dtcUnprocessedMessagesRead;//data received-complete messages ready to be processed by data processing thread
        List<UInt16> dtcUnprocessedMessageTypesRead;//their corresponding message type numbers
        DateTime timeStampOfUnprocessedData;
        Object dtcUnprocessedMessagesReadLockObject;//this lock will synchronize access to 'dtcUnprocessedMessagesRead'


        List<byte[]> dtcMessagesRead;//complete messages
        List<UInt16> dtcMessageTypesRead;//their corresponding message type numbers
        List<byte> dtcPartialMessage;
        List<byte> dtcBytesToAddToBytesReadBuffer;
        UInt16 dtcSizeOfCurrentMessageToRead = 0;//in bytes
        UInt16 dtcTypeOfCurrentMessageToRead = 0;//in bytes
        
        readonly int dtcMinBytesForSizeAndType = 4;//2 for size and 2 for type
        byte[] dtcSizeBytes = new byte[2];//size of message bytes
        byte[] dtcTypeBytes = new byte[2];//type of message bytes

        /// <summary>
        /// Using 'MarketSymbolID' as (unique) index, we get the corresponding string for the intrument. 
        /// </summary>
        public List<string> instrumentFromDTCMarketSymbolID;

        #endregion 

        #region constructor

        public csClient(int portNumber)
        {
            this.portNumber = portNumber;
        }

        #endregion 

        #region Connect
        /// <summary>
        /// Will try to connect the socket to the server. If it is a reconnection, it will clean up/initialize resources first.
        /// </summary>
        /// <param name="isReconnectionAttempt"></param>
        public void Connect(bool isReconnectionAttempt)
        {
            if (isReconnectionAttempt)
            {
                OrderlyDTCShutdown(!isReconnectionAttempt);
            }
            Initialize();
            ConnectToDTCPort(portDTC);
        }

        #endregion 

        #region ConnectToDTCPort

        /// <summary>
        /// Connects local DTC socket to the port and starts receiving data via a callback.
        /// </summary>
        /// <param name="sPort"></param>
        /// <returns></returns>
        /// 
        private void ConnectToDTCPort(string portName)
        {

            try
            {
                
                if (portName == portDTC)
                {

                    #region create socket, callback, connect, and start receiving data


                    // create the socket
                    dtcSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);



                    IPAddress ipLocalhost = IPAddress.Parse(dtcIPAddress);

                    // pull the level 1 port out of the registry.  we use the Level 1 port because we want streaming updates

                    int iPort = GetDTCPort(portName);
                    IPEndPoint ipendLocalhost = new IPEndPoint(ipLocalhost, iPort);

                    try
                    {

                        // connect
                        dtcSocket.Connect(ipendLocalhost);

                        //fire a connected even
                        OnConnected(new csConnectedEventArgs(DateTime.Now));

                        // this example is using asynchronous sockets to communicate with the feed.  As a result, we are using .NET's BeginReceive and EndReceive calls with a callback.

                        dtcCallback = new AsyncCallback(DTCOnReceive);

                        #region  begin receiving data

                        dtcSocket.BeginReceive(dtcSocketBuffer, 0, dtcSocketBuffer.Length, SocketFlags.None, dtcCallback, null);// DTCSocketName.Level1);

                        #endregion
                    }
                    catch (Exception ex)
                    {
                        //note: have to catch error in case the specified port is already in use, 
                        //instruct user to select a port one higher than existing one, keep repeating till finding an available port

                    }

                    #endregion

                }
                
            }
            catch (Exception ex)
            {
                
            }
        }

        #endregion

        #region GetDTCPort

        /// <summary>
        /// Gets local DTC socket port
        /// </summary>
        /// <param name="sPort"></param>
        /// <returns></returns>
        /// 
        private int GetDTCPort(string portName)
        {
            return portNumber;
        }

        #endregion

        //define events that will be fired and pass on level1 and level2 data to subscribers (and potentially connected/disconnected event)

        #region Bulk Level1 Update Event
        // Private instance field created to serve as thread synchronization lock
        private readonly Object bulkLevel1UpdateEventLock = new Object();

        // Add private field which refers to the head of the delegate linked-list
        private EventHandler<csBulkLevel1UpdateEventArgs> bulkLevel1UpdateEvent;
        /// <summary>
        /// Event fired on level1 data arrival, passing the received data in csBulkLevel1UpdateEventArgs.
        /// </summary>
        public event EventHandler<csBulkLevel1UpdateEventArgs> BulkLevel1UpdateEvent
        {
            // Explicitly implement the 'add' method
            add
            {
                // Take the private lock and add a handler 
                // (passed as 'value') to the delegate linked list
                lock (bulkLevel1UpdateEventLock) { bulkLevel1UpdateEvent += value; }
            }
            // Explicitly implement the 'remove' method
            remove
            {
                // Take the private lock and remove a handler 
                // (passed as 'value') from the delegate linked list
                lock (bulkLevel1UpdateEventLock) { bulkLevel1UpdateEvent -= value; }
            }
        }

        #region Notify Registered Objects that event has occured
        private void OnBulkLevel1Update(csBulkLevel1UpdateEventArgs o)
        {
            EventHandler<csBulkLevel1UpdateEventArgs> temp = bulkLevel1UpdateEvent;
            if (temp != null) temp(this, o);
        }
        #endregion



        #endregion

        #region Bulk Level2 Update Event
        // Private instance field created to serve as thread synchronization lock
        private readonly Object bulkLevel2UpdateEventLock = new Object();

        // Add private field which refers to the head of the delegate linked-list
        private EventHandler<csBulkLevel2UpdateEventArgs> bulkLevel2UpdateEvent;
        /// <summary>
        /// Event fired on level2 data arrival, passing the received data in csBulkLevel2UpdateEventArgs.
        /// </summary>
        public event EventHandler<csBulkLevel2UpdateEventArgs> BulkLevel2UpdateEvent
        {
            // Explicitly implement the 'add' method
            add
            {
                // Take the private lock and add a handler 
                // (passed as 'value') to the delegate linked list
                lock (bulkLevel2UpdateEventLock) { bulkLevel2UpdateEvent += value; }
            }
            // Explicitly implement the 'remove' method
            remove
            {
                // Take the private lock and remove a handler 
                // (passed as 'value') from the delegate linked list
                lock (bulkLevel2UpdateEventLock) { bulkLevel2UpdateEvent -= value; }
            }
        }

        #region Notify Registered Objects that event has occured
        private void OnBulkLevel2Update(csBulkLevel2UpdateEventArgs o)
        {
            EventHandler<csBulkLevel2UpdateEventArgs> temp = bulkLevel2UpdateEvent;
            if (temp != null) temp(this, o);
        }
        #endregion



        #endregion

        #region Connected Event
        // Private instance field created to serve as thread synchronization lock
        private readonly Object connectedEventLock = new Object();

        // Add private field which refers to the head of the delegate linked-list
        private EventHandler<csConnectedEventArgs> connectedEvent;
        /// <summary>
        /// Event fired on connecting to the server.
        /// </summary>
        public event EventHandler<csConnectedEventArgs> ConnectedEvent
        {
            // Explicitly implement the 'add' method
            add
            {
                // Take the private lock and add a handler 
                // (passed as 'value') to the delegate linked list
                lock (connectedEventLock) { connectedEvent += value; }
            }
            // Explicitly implement the 'remove' method
            remove
            {
                // Take the private lock and remove a handler 
                // (passed as 'value') from the delegate linked list
                lock (connectedEventLock) { connectedEvent -= value; }
            }
        }

        #region Notify Registered Objects that event has occured
        private void OnConnected(csConnectedEventArgs o)
        {
            EventHandler<csConnectedEventArgs> temp = connectedEvent;
            if (temp != null) temp(this, o);
        }
        #endregion



        #endregion

        #region Disconnected Event
        // Private instance field created to serve as thread synchronization lock
        private readonly Object disconnectedEventLock = new Object();

        // Add private field which refers to the head of the delegate linked-list
        private EventHandler<csDisconnectedEventArgs> disconnectedEvent;
        /// <summary>
        /// Event fired on disconnecting from the server.
        /// </summary>
        public event EventHandler<csDisconnectedEventArgs> DisconnectedEvent
        {
            // Explicitly implement the 'add' method
            add
            {
                // Take the private lock and add a handler 
                // (passed as 'value') to the delegate linked list
                lock (disconnectedEventLock) { disconnectedEvent += value; }
            }
            // Explicitly implement the 'remove' method
            remove
            {
                // Take the private lock and remove a handler 
                // (passed as 'value') from the delegate linked list
                lock (disconnectedEventLock) { disconnectedEvent -= value; }
            }
        }

        #region Notify Registered Objects that event has occured
        private void OnDisconnected(csDisconnectedEventArgs o)
        {
            EventHandler<csDisconnectedEventArgs> temp = disconnectedEvent;
            if (temp != null) temp(this, o);
        }
        #endregion



        #endregion

        #region SecurityDefinitionResponse Event
        // Private instance field created to serve as thread synchronization lock
        private readonly Object securityDefinitionResponseEventLock = new Object();

        // Add private field which refers to the head of the delegate linked-list
        private EventHandler<csSecurityDefinitionResponseEventArgs> securityDefinitionResponseEvent;
        /// <summary>
        /// Event fired on connecting to the server.
        /// </summary>
        public event EventHandler<csSecurityDefinitionResponseEventArgs> SecurityDefinitionResponseEvent
        {
            // Explicitly implement the 'add' method
            add
            {
                // Take the private lock and add a handler 
                // (passed as 'value') to the delegate linked list
                lock (securityDefinitionResponseEventLock) { securityDefinitionResponseEvent += value; }
            }
            // Explicitly implement the 'remove' method
            remove
            {
                // Take the private lock and remove a handler 
                // (passed as 'value') from the delegate linked list
                lock (securityDefinitionResponseEventLock) { securityDefinitionResponseEvent -= value; }
            }
        }

        #region Notify Registered Objects that event has occured
        private void OnSecurityDefinitionResponse(csSecurityDefinitionResponseEventArgs o)
        {
            EventHandler<csSecurityDefinitionResponseEventArgs> temp = securityDefinitionResponseEvent;
            if (temp != null) temp(this, o);
        }
        #endregion



        #endregion



        #region DTCOnReceive

        /// <summary>
        /// This is our callback that gets called by the .NET socket class when new data arrives on the socket
        /// </summary>
        /// <param name="asyn"></param>
        private void DTCOnReceive(IAsyncResult asyn)
        {
            try
            {
                //DTCSocketName dtcSocketName = (DTCSocketName)asyn.AsyncState;


                if (true)//dtcSocketName == DTCSocketName.Level2)
                {
                    DateTime now = DateTime.Now;

                    try
                    {
                        #region process the data received

                        // read data from the socket.
                        int iReceivedBytes = 0;
                        iReceivedBytes = dtcSocket.EndReceive(asyn);

                        if (iReceivedBytes > 0)
                        {
                            #region if this is the first message received (we just connected), take whatever action is necessary
                            //for now, based on current implementation, this doesn't have any effect, so skip it...

                            //if (!dtcServerConnected)//first message received
                            //{
                            //    ////fire a connected even
                            //    //OnConnected(new csConnectedEventArgs(now));

                            //    //send messages for instruments etc.
                            //    //try
                            //    //{
                            //    //    if (dtcSocket != null && dtcSocket.Connected)
                            //    //    {
                            //    //        //csHeader.s_ExchangeListRequest request = new csHeader.s_ExchangeListRequest(0);
                            //    //        //byte[] bytesToSend = csHeader.GetBytesFromObject(request);
                            //    //        //dtcSocket.Send(bytesToSend);
                            //    //        //csHeader.s_SymbolsForExchangeRequest request2 = new csHeader.s_SymbolsForExchangeRequest(0, "CME", csHeader.SecurityTypeEnum.ST_FUTURE);
                            //    //        //bytesToSend = csHeader.GetBytesFromObject(request2);
                            //    //        //dtcSocket.Send(bytesToSend);
                            //    //        //csHeader.s_MarketDepthRequest request3 = new csHeader.s_MarketDepthRequest(csHeader.RequestActionEnum.SNAPSHOT, 0, "QCLH14","CME", 20);
                            //    //        //bytesToSend = csHeader.GetBytesFromObject(request3);
                            //    //        //dtcSocket.Send(bytesToSend);


                            //    //    }
                            //    //}
                            //    //catch (Exception ex)
                            //    //{
                            //    //    string message = ex.Message;
                            //    //}
                            //}

                            #endregion 

                            dtcServerConnected = true;
                           
                            //A DTC message consists of a 2 byte Size and 2 byte message Type at the beginning of every message structure.
                            //These 2 variables are automatically set by the message constructor in the standard header file 

                            //read off message size and type if available

                            dtcBytesToAddToBytesReadBuffer.Clear();

                            #region prepend existing partial messages and add messages received

                            //prepend any existing partial messages(if applicable)
                            if (dtcPartialMessage.Count > 0)
                            {
                                dtcBytesToAddToBytesReadBuffer.AddRange(dtcPartialMessage);
                                dtcPartialMessage.Clear();
                            }
                            //add messages received
                            for (int i = 0; i < iReceivedBytes; i++)
                            {
                                dtcBytesToAddToBytesReadBuffer.Add(dtcSocketBuffer[i]);
                            }


                            #endregion


                            #region read off full messages

                            int currentIndex = 0;

                            while (currentIndex + dtcMinBytesForSizeAndType <= dtcBytesToAddToBytesReadBuffer.Count)
                            {
                                dtcSizeBytes[0] = dtcBytesToAddToBytesReadBuffer[currentIndex++];
                                dtcSizeBytes[1] = dtcBytesToAddToBytesReadBuffer[currentIndex++];
                                dtcTypeBytes[0] = dtcBytesToAddToBytesReadBuffer[currentIndex++];
                                dtcTypeBytes[1] = dtcBytesToAddToBytesReadBuffer[currentIndex++];
                                //set index back to starting location for ease of use
                                currentIndex -= 4;

                                try
                                {
                                    dtcSizeOfCurrentMessageToRead = BitConverter.ToUInt16(dtcSizeBytes, 0);
                                    dtcTypeOfCurrentMessageToRead = BitConverter.ToUInt16(dtcTypeBytes, 0);


                                    if (currentIndex + dtcSizeOfCurrentMessageToRead <= dtcBytesToAddToBytesReadBuffer.Count)
                                    {
                                        byte[] currentMessageComplete = new byte[dtcSizeOfCurrentMessageToRead];
                                        for (int i = 0; i < dtcSizeOfCurrentMessageToRead; i++)
                                        {
                                            currentMessageComplete[i] = dtcBytesToAddToBytesReadBuffer[currentIndex + i];
                                        }
                                        currentIndex += dtcSizeOfCurrentMessageToRead;

                                        //need to add the message to processing queue  now...
                                        //maybe will have to add locking here, depending on how it's used in other parts of code
                                        dtcMessagesRead.Add(currentMessageComplete);
                                        dtcMessageTypesRead.Add(dtcTypeOfCurrentMessageToRead);
                                    }
                                    else//whatever remains will be partial message
                                    {
                                        byte[] currentMessagePartial = new byte[dtcBytesToAddToBytesReadBuffer.Count - currentIndex];

                                        for (int i = 0; i < currentMessagePartial.Length; i++)
                                        {
                                            currentMessagePartial[i] = dtcBytesToAddToBytesReadBuffer[currentIndex + i];
                                        }
                                        dtcPartialMessage.AddRange(currentMessagePartial);
                                        currentIndex = dtcBytesToAddToBytesReadBuffer.Count;
                                    }

                                }
                                catch { }
                            }
                            if (currentIndex < dtcBytesToAddToBytesReadBuffer.Count)//the rest has to be added to partial message)
                            {
                                byte[] currentMessagePartial = new byte[dtcBytesToAddToBytesReadBuffer.Count - currentIndex];

                                for (int i = 0; i < currentMessagePartial.Length; i++)
                                {
                                    currentMessagePartial[i] = dtcBytesToAddToBytesReadBuffer[currentIndex + i];
                                }
                                dtcPartialMessage.AddRange(currentMessagePartial);
                            }

                            #endregion 

                            if (dtcMessagesRead.Count > 0)
                            {//add complete messages to data to process buffer...
                                lock (dtcUnprocessedMessagesReadLockObject)
                                {
                                    dtcUnprocessedMessagesRead.AddRange(dtcMessagesRead);
                                    dtcUnprocessedMessageTypesRead.AddRange(dtcMessageTypesRead);
                                    timeStampOfUnprocessedData = now;

                                    dtcMessagesRead.Clear();
                                    dtcMessageTypesRead.Clear();

                                    unprocessedDataExists = true;
                                }
                            }


                        }

                        #endregion
                    }
                    catch (Exception ex)
                    {
                    }
                    // call wait for data to notify the socket that we are ready to receive another callback
                    #region  begin receiving  data (again)

                   
                    dtcSocket.BeginReceive(dtcSocketBuffer, 0, dtcSocketBuffer.Length, SocketFlags.None, dtcCallback, null);// DTCSocketName.Level2);
                    
                    #endregion
                }


               
            }
            catch (Exception ex)
            {
               
            }

        }




        #endregion 

        #region OrderlyDTCShutdown
        /// <summary>
        /// Performs an orderly shutdown of timers/sockets/data processing threads etc. (only if a previous connection has been attempted).
        /// </summary>
        /// <param name="closingApplication">If true, hearbeat timer will be disposed too.</param>
        public void OrderlyDTCShutdown(bool closingApplication)
        {
            //
            //closingApplication
            //let the data processing thread exit gracefully..
            dataProcessingThreadRunning = false;
            int waitTime = 0;

            try
            {
                while (dataProcessingThread != null && dataProcessingThread.IsAlive && waitTime < 100)
                {
                    Thread.Sleep(20);
                    waitTime += 20;
                }
            }
            catch { }

            try
            {
                if (heartBeatTimer != null && closingApplication)
                {
                    heartBeatTimer.Dispose();
                    //Thread.Sleep(2000);//wait 2 seconds for any outstanding tick events to fire...
                }

            }
            catch (Exception ex)
            {

            }
            
            try
            {
                if (dtcSocket != null)
                {
                    dtcSocket.Shutdown(SocketShutdown.Both);
                    dtcSocket.Close();
                }
                
            }
            catch (Exception ex)
            {
               
            }

        }

        #endregion 

        #region Initialize
        /// <summary>
        /// Reinitializes all variables to their starting values, starts a new dataprocessing thread and starts the hearbeat timer.
        /// </summary>
        public void Initialize()
        {
            //reinitialize all lists/variables

            secondsSinceLastServerHeartbeat = 0;

            unprocessedDataExists = false;
            dataProcessingThreadRunning = true; 
            dataToProcessBuffer = new List<byte[]>();
            typesToProcessBuffer = new List<ushort>();
            
            
            dtcUnprocessedMessagesRead = new List<byte[]>();//data received-complete messages ready to be processed by data processing thread
            dtcUnprocessedMessageTypesRead = new List<ushort>();//their corresponding message type numbers
            timeStampOfUnprocessedData = DateTime.Now;
            dtcUnprocessedMessagesReadLockObject = new object();//this lock will synchronize access to 'dtcUnprocessedMessagesRead'


            dtcMessagesRead = new List<byte[]>();//complete messages
            dtcMessageTypesRead = new List<ushort>();//their corresponding message type numbers
            dtcPartialMessage = new List<byte>();
            dtcBytesToAddToBytesReadBuffer = new List<byte>();

            instrumentFromDTCMarketSymbolID = new List<string>();

            try
            {
                //begin checking the inbound message queue
                dataProcessingThreadRunning = true;
                dataProcessingThread = new Thread(DataProcessing);
                dataProcessingThread.IsBackground = false; //this is a ui thread: stop running it when the form closes
                dataProcessingThread.Start();
            }
            catch (Exception ex)
            {

            } 

            //initialize timer and send the first heartbeat message

            try
            {
                //this sets heartbeat message bytes
                csHeader.s_Heartbeat.SetHeartbeatBytesToSend();
                if (heartBeatTimer == null)//only start a new timer the very first time
                {
                    heartBeatTimer = new Timer(new TimerCallback(heartBeatTimerTick), null, 0, 1000);
                }
            }
            catch (Exception ex)
            {

            } 

        }

        #endregion 

        #region heartBeatTimerTick
        //heartBeatTimerTick
        private void heartBeatTimerTick(Object stateInfo)
        {
            #region  send heartbeat message

            try
            {
                if (dtcServerConnected && dtcSocket != null && dtcSocket.Connected)
                {
                    dtcSocket.Send(csHeader.HeartbeatBytesToSend);
                }
                else
                {
                    bool reconnect = false;

                    lock (reconnectionLogicLock)
                    {
                        secondsSinceLastServerHeartbeat++;
                        if (secondsSinceLastServerHeartbeat == secondsToFireDisconnectedEvent)
                        { 
                            //fire disconnected event
                            if (dtcServerConnected)
                            {
                                dtcServerConnected = false;
                                OnDisconnected(new csDisconnectedEventArgs(DateTime.Now));
                            }
                        }
                        else if (secondsSinceLastServerHeartbeat >= secondsToAttemptReconnection)
                        { 
                            //try to reconnect
                            secondsSinceLastServerHeartbeat = 0;
                            dtcServerConnected = false;
                            reconnect = true;
                        }
                    }
                    if (reconnect)
                    {
                        Connect(true);
                    }
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }

            #endregion
        }

        #endregion

        #region RecordedData
        //heartBeatTimerTick
        public List<string> RecordedData()
        {
            List<string> linesToWrite = new List<string>();

            linesToWrite.Add("********************* NEW ENTRY ********************************************************");

            #region  send heartbeat message

            try
            {
                string filePath = "C:\\SpeedTest\\dtcTest.txt";

                for (int i = 0; i < dtcMessagesRead.Count; i++)
                {
                    linesToWrite.Add(csHeader.MessageToCommaDelimitedString(dtcMessageTypesRead[i], dtcMessagesRead[i]));
                }
                
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }

            #endregion

            return linesToWrite;
        }

        #endregion


        //Data processing thread

        #region DataProcessing

        private void DataProcessing()
        {
            //This method is being run by dataProcessingThread

            // continuous loop while our application is running
            while (dataProcessingThreadRunning)
            {

                dataToProcessBuffer.Clear();
                typesToProcessBuffer.Clear();


                #region wait while there is no new data (and the thread still wants to run... via dataProcessingThreadRunning)

                while (!unprocessedDataExists)
                {
                    Thread.Sleep(20);
                    //if shutdown is being executed from outside, terminate this thread by just leaving it
                    if (!dataProcessingThreadRunning)
                    {
                        return;
                    }
                }


                #endregion

                DateTime timeOfUpdate = DateTime.Now;//the update that came happend 'now' (within 1 millisecond precision...)

                //if we are here, unprocessed data exists
                //this is the only place in the code that removes from the queue! and sets 'unprocessedDataExists' to false

                #region get read data from the socket, add them to 'dataToProcess', clear read data queue

                try
                {
                    lock (dtcUnprocessedMessagesReadLockObject)
                    {
                        dataToProcessBuffer.AddRange(dtcUnprocessedMessagesRead);
                        typesToProcessBuffer.AddRange(dtcUnprocessedMessageTypesRead);
                        timeOfUpdate = timeStampOfUnprocessedData;

                        dtcUnprocessedMessagesRead.Clear();
                        dtcUnprocessedMessageTypesRead.Clear();

                        unprocessedDataExists = false;
                    }
                }
                catch { }


                #endregion

                ProcessData(timeOfUpdate, dataToProcessBuffer, typesToProcessBuffer);
            }

            // if here, we fell out of the While loop because dataProcessingThreadRunning has been set to false

        } // thread terminates

        #endregion 


        #region ProcessData
        /// <summary>
        /// Processes the data and fires appropriate update events to subscribers.
        /// </summary>
        /// <param name="dataToProcess"></param>
        private void ProcessData(DateTime timeOfUpdate, List<byte[]> dataToProcess, List<ushort> typesToProcess)
        {

            csBulkLevel1UpdateEventArgs level1Updates = new csBulkLevel1UpdateEventArgs(timeOfUpdate, new List<byte[]>(), new List<ushort>());
            csBulkLevel2UpdateEventArgs level2Updates = new csBulkLevel2UpdateEventArgs(timeOfUpdate, new List<byte[]>(), new List<ushort>());

            for (int i = 0; i < typesToProcess.Count; i++)
            {
                //if statements are ordered in order of frequency of occurrence...
                if (typesToProcess[i] == csHeader.MARKET_DEPTH_INCREMENTAL_UPDATE_COMPACT)
                {
                    level2Updates.AddUpdate(dataToProcess[i]);
                    level2Updates.AddUpdateType(typesToProcess[i]);
                }
                else if (typesToProcess[i] == csHeader.QUOTE_INCREMENTAL_UPDATE_COMPACT)
                {
                    level1Updates.AddUpdate(dataToProcess[i]);
                    level1Updates.AddUpdateType(typesToProcess[i]);

                    //for now since they are not included in 'detph' updates we include them there too
                    //now, it claims to be fixed...so remove it
                    //level2Updates.AddUpdate(dataToProcess[i]);
                    //level2Updates.AddUpdateType(typesToProcess[i]);
                }
                else if (typesToProcess[i] == csHeader.TRADE_INCREMENTAL_UPDATE_COMPACT)
                {
                    level1Updates.AddUpdate(dataToProcess[i]);
                    level1Updates.AddUpdateType(typesToProcess[i]);
                }
                else if (typesToProcess[i] == csHeader.MARKET_DEPTH_SNAPSHOT_LEVEL)
                {
                    level2Updates.AddUpdate(dataToProcess[i]);
                    level2Updates.AddUpdateType(typesToProcess[i]);
                }
                else if (typesToProcess[i] == csHeader.SECURITY_DEFINITION_RESPONSE)
                {//here need to find the correspondence of marketSymbolID and its string
                    try
                    {
                        csHeader.s_SecurityDefinitionResponse securityResponse = new csHeader.s_SecurityDefinitionResponse();
                        securityResponse.CopyFromManual(dataToProcess[i]);
                        //securityResponse.RequestID//this is then used as 'marketSymbolID
                        while (instrumentFromDTCMarketSymbolID.Count <= securityResponse.RequestID)
                        {
                            instrumentFromDTCMarketSymbolID.Add(string.Empty);
                        }
                        
                        instrumentFromDTCMarketSymbolID[securityResponse.RequestID] = securityResponse.Symbol;

                        //create a copy of this list
                        List<string> copy = new List<string>(instrumentFromDTCMarketSymbolID);
                        //pass it to the potential subscribers
                        OnSecurityDefinitionResponse(new csSecurityDefinitionResponseEventArgs(copy));
                    }
                    catch { }
                }
                else if (typesToProcess[i] == csHeader.HEARTBEAT)
                {
                    lock (reconnectionLogicLock)
                    {
                        secondsSinceLastServerHeartbeat = 0;
                    }
                }
                else if (typesToProcess[i] == csHeader.MARKET_DATA_FEED_STATUS)
                {
                    //see what is the status of the feed and act accordingly...
                }
            }

            if (level1Updates.Updates.Count > 0)
            {
                OnBulkLevel1Update(level1Updates);
            }
            if (level2Updates.Updates.Count > 0)
            {
                OnBulkLevel2Update(level2Updates);
            }

        }

        #endregion

    }
}
