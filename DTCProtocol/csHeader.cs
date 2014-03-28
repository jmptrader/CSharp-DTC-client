using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace DTCProtocol
{
    public class csHeader
    {
        static System.Text.ASCIIEncoding ascii = new System.Text.ASCIIEncoding();
        static byte[] heartbeatBytesToSend = new byte[16];
        const byte ZEROBYTE = 0;

        #region constructor

        public csHeader()
        {
           
        }

        #endregion

        #region properties

        public static byte[] HeartbeatBytesToSend
        {
            get { return heartbeatBytesToSend; }
        }

        
        #endregion

        //replaced:
        //unsigned Int16 with UInt16
        //unsigned Int32 with UInt32
        //__int16 with Int16
        //__int32 with Int32


        #region helpful notes

        //        signed char    c; -> SByte    c; 
        //unsigned char  b; -> Byte     b;
        //signed short   s; -> Int16    s;
        //unsigned short w; -> UInt16   w;
        //signed long    l; -> Int32    l;
        //unsigned long  u; -> UInt32   u;
        //float          f; -> Single   f; (though 'float' still works)
        //double        *d; -> Double   d; (was this meant to be a pointer???)
        //char          *p; -> String   s; (assuming its a string here, in the marshaling you can tell it whether it is ASCII or wide char format)



        //**************************NOTE on Pack = 8
        //Pack - Specifies the value, in bytes, to be used for packing. 
        //If the compiler option /Zp is not set for the module, the default value for n is 8. 
        //Valid values are 1, 2, 4, 8, and 16. The alignment of a member will be on a boundary that is 
        //either a multiple of n or a multiple of the size of the member, whichever is smaller.


        #endregion

        #region constants

        const int PACK_SIZE = 8;

        	// DTC protocol version
	    public const Int32 CURRENT_VERSION = 3;

	    // Text string lengths. The protocol is intended to be updated to support variable length strings making these irrelevant at that time.
	    const Int32 SYMBOL_LENGTH = 64;
	    const Int32 EXCHANGE_LENGTH= 16;
	    const Int32 UNDERLYING_SYMBOL_LENGTH= 32;
	    const Int32 SYMBOL_DESCRIPTION_LENGTH = 48;
	    const Int32 ORDER_ID_LENGTH = 32;
	    const Int32 TRADE_ACCOUNT_LENGTH = 32;
	    const Int32 TEXT_DESCRIPTION_LENGTH = 96;
	    const Int32 TEXT_MESSAGE_LENGTH = 256;

	    //----Message types----

	    // Authentication and connection monitoring
        public const UInt16 LOGON_REQUEST = 1;
        public const UInt16 LOGON_RESPONSE = 2;
        public const UInt16 HEARTBEAT = 3;
        public const UInt16 DISCONNECT_FROM_SERVER_NO_RECONNECT = 4;

	    // Market data
        public const UInt16 MARKET_DATA_FEED_STATUS = 100;
	    const UInt16 MARKET_DATA_REQUEST = 101;
	    const UInt16 MARKET_DEPTH_REQUEST = 102;
	    const UInt16 MARKET_DATA_REJECT = 103;
        public const UInt16 MARKET_DATA_SNAPSHOT = 104;
	    const UInt16 MARKET_DEPTH_FULL_UPDATE = 105;
        public const UInt16 MARKET_DEPTH_INCREMENTAL_UPDATE = 106;
	    const UInt16 TRADE_INCREMENTAL_UPDATE = 107;
	    const UInt16 QUOTE_INCREMENTAL_UPDATE = 108;
	    const UInt16 FUNDAMENTAL_DATA_RESPONSE = 110;

        public const UInt16 TRADE_INCREMENTAL_UPDATE_COMPACT = 112;
	    const UInt16 DAILY_VOLUME_INCREMENTAL_UPDATE = 113;
	    const UInt16 DAILY_HIGH_INCREMENTAL_UPDATE = 114;
	    const UInt16 DAILY_LOW_INCREMENTAL_UPDATE = 115;
	    const UInt16 MARKET_DATA_FEED_SYMBOL_STATUS = 116;
        public const UInt16 QUOTE_INCREMENTAL_UPDATE_COMPACT = 117;
        public const UInt16 MARKET_DEPTH_INCREMENTAL_UPDATE_COMPACT = 118;
	    const UInt16 SETTLEMENT_INCREMENTAL_UPDATE = 119;
	    const UInt16 DAILY_OPEN_INCREMENTAL_UPDATE = 120;
	    const UInt16 MARKET_DEPTH_REJECT = 121;
        public const UInt16 MARKET_DEPTH_SNAPSHOT_LEVEL = 122;

	    // Order entry and modification
	    const UInt16 SUBMIT_NEW_SINGLE_ORDER = 200;
	    const UInt16 SUBMIT_NEW_OCO_ORDER = 201;
	    const UInt16 CANCEL_REPLACE_ORDER = 202;
	    const UInt16 CANCEL_ORDER = 203;

	    // Trading related
	    const UInt16 OPEN_ORDERS_REQUEST = 300;
	    const UInt16 ORDER_UPDATE_REPORT = 301;
	    const UInt16 OPEN_ORDERS_REQUEST_REJECT = 302;
	    const UInt16 HISTORICAL_ORDER_FILLS_REQUEST = 303;
	    const UInt16 HISTORICAL_ORDER_FILL_REPORT = 304;
	    const UInt16 CURRENT_POSITIONS_REQUEST = 305;
	    const UInt16 POSITION_REPORT = 306;
	    const UInt16 CURRENT_POSITIONS_REQUEST_REJECT = 307;

	    // Account list
	    const UInt16 ACCOUNTS_REQUEST = 400;
	    const UInt16 ACCOUNT_LIST_RESPONSE = 401;

	    // Symbol discovery and security definitions
	    const UInt16 EXCHANGE_LIST_REQUEST = 500;
	    const UInt16 EXCHANGE_LIST_RESPONSE = 501;
	    const UInt16 SYMBOLS_FOR_EXCHANGE_REQUEST = 502;
	    const UInt16 UNDERLYING_SYMBOLS_FOR_EXCHANGE_REQUEST = 503;
	    const UInt16 SYMBOLS_FOR_UNDERLYING_REQUEST = 504;
	    const UInt16 SECURITY_DEFINITION_FOR_SYMBOL_REQUEST = 506;
        public const UInt16 SECURITY_DEFINITION_RESPONSE = 507;

	    // Account balance
	    const UInt16 ACCOUNT_BALANCE_UPDATE = 600;

	    // Logging
	    const UInt16 USER_MESSAGE = 700;
	    const UInt16 GENERAL_LOG_MESSAGE = 701;

	    // Historical price data
	    const UInt16 HISTORICAL_PRICE_DATA_REQUEST= 800;
	    const UInt16 HISTORICAL_PRICE_DATA_HEADER_RESPONSE = 801;
	    const UInt16 HISTORICAL_PRICE_DATA_REJECT = 802;
	    const UInt16 HISTORICAL_PRICE_DATA_RECORD_RESPONSE = 803;
	    const UInt16 HISTORICAL_PRICE_DATA_TICK_RECORD_RESPONSE = 804;

        #endregion

	    /*==========================================================================*/
	    //Standard UNIX date and time value
	    //typedef Int64 t_DateTime;

	    // This is a 4 byte DateTime value used in messages where compactness is an important consideration.
	    //typedef UInt32 t_DateTime4Byte;
            
        #region enums
	    /*==========================================================================*/
	    public enum LogonStatusEnum : int
	    { LOGON_SUCCESS = 1
	    , LOGON_ERROR = 2
	    , LOGON_ERROR_NO_RECONNECT = 3
	    , LOGON_RECONNECT_NEW_ADDRESS = 4
	    };

	    /*==========================================================================*/
	    public enum MessageSupportedEnum : int
	    { MESSAGE_UNSUPPORTED = 0
	    , MESSAGE_SUPPORTED = 1
	    };

	    /*==========================================================================*/
	    public enum TradeModeEnum : int
	    { TRADE_MODE_DEMO = 1
	    , TRADE_MODE_SIMULATED = 2
	    , TRADE_MODE_LIVE = 3
	    };

	    /*==========================================================================*/
	    public enum RequestActionEnum : int
	    { SUBSCRIBE = 1
	    , UNSUBSCRIBE = 2
	    , SNAPSHOT = 3
	    };

	    /*==========================================================================*/
	    public enum OrderStatusEnum : int
	    { ORDER_STATUS_UNSPECIFIED = 0
	    , ORDER_STATUS_ORDERSENT = 1
	    , ORDER_STATUS_PENDINGOPEN = 2
	    , ORDER_STATUS_PENDINGCHILD = 3
	    , ORDER_STATUS_OPEN = 4
	    , ORDER_STATUS_PENDING_CANCEL_REPLACE = 5
	    , ORDER_STATUS_PENDING_CANCEL = 6
	    , ORDER_STATUS_FILLED = 7
	    , ORDER_STATUS_CANCELED = 8
	    , ORDER_STATUS_REJECTED = 9
	    };

	    /*==========================================================================*/
	    public enum ExecutionTypeEnum : int
	    { ET_UNSET = 0
	    , ET_OPEN_ORDERS_REQUEST = 1
	    , ET_NEW_ORDER_ACCEPTED = 2
	    , ET_ORDER_UPDATE = 3
	    , ET_FILLED = 4
	    , ET_PARTIAL_FILL = 5
	    , ET_CANCELED = 6
	    , ET_CANCEL_REPLACE_COMPLETE = 7
	    , ET_NEW_ORDER_REJECT = 8
	    , ET_ORDER_CANCEL_REJECT = 9
	    , ET_ORDER_CANCEL_REPLACE_REJECT = 10
	    };
	    //ushort = UInt16
	    /*==========================================================================*/
	    public enum BidOrAskEnum : ushort
	    { BID_ASK_UNSET = 0
	    , AT_BID = 1
	    , AT_ASK = 2
	    };
	    //unsigned char
	    /*==========================================================================*/
	    public enum MarketDepthIncrementalUpdateTypeEnum : byte
	    { DEPTH_UNSET = 0
	    , DEPTH_INSERT_UPDATE = 1 // Insert or update depth at the given price level
	    , DEPTH_DELETE = 2 // Delete depth at the given price level
	    };
    	
	    /*==========================================================================*/
	    public enum OrderTypeEnum : int
	    { ORDER_TYPE_UNSET = 0
	    , ORDER_TYPE_MARKET = 1
	    , ORDER_TYPE_LIMIT = 2
	    , ORDER_TYPE_STOP = 3
	    , ORDER_TYPE_STOP_LIMIT = 4
	    , ORDER_TYPE_MARKET_IF_TOUCHED = 5
	    };
    	
	    /*==========================================================================*/
	    public enum TimeInForceEnum : int
	    { TIF_UNSET = 0
	    , TIF_DAY = 1
	    , TIF_GOOD_TILL_CANCELED = 2
	    , TIF_GOOD_TILL_DATE_TIME = 3
	    , TIF_IMMEDIATE_OR_CANCEL = 4
	    , TIF_ALL_OR_NONE = 5
	    , TIF_FILL_OR_KILL = 6
	    };
    	
	    /*==========================================================================*/
	    public enum BuySellEnum : int
	    { BUY_SELL_UNSET = 0
	    , BUY = 1
	    , SELL = 2
	    };

	    /*==========================================================================*/
	    public enum OpenCloseTradeEnum : int
	    { TRADE_UNSET = 0
	    , TRADE_OPEN = 1
	    , TRADE_CLOSE = 2
	    };

	    /*==========================================================================*/
    	
	    public enum MarketDataFeedStatusEnum : int
	    { MARKET_DATA_FEED_LOST = 1
	    , MARKET_DATA_FEED_RESTORED = 2
	    };

	    /*==========================================================================*/
	    public enum DisplayFormatEnum : int
	    { DISPLAY_FORMAT_UNSET =  -1
	    //The following formats indicate the number of decimal places to be displayed
	    , DISPLAY_FORMAT_DECIMAL_0 = 0
	    , DISPLAY_FORMAT_DECIMAL_1 = 1
	    , DISPLAY_FORMAT_DECIMAL_2 = 2
	    , DISPLAY_FORMAT_DECIMAL_3 = 3
	    , DISPLAY_FORMAT_DECIMAL_4 = 4
	    , DISPLAY_FORMAT_DECIMAL_5 = 5
	    , DISPLAY_FORMAT_DECIMAL_6 = 6
	    , DISPLAY_FORMAT_DECIMAL_7 = 7
	    , DISPLAY_FORMAT_DECIMAL_8 = 8
	    , DISPLAY_FORMAT_DECIMAL_9 = 9
	    //The following formats are fractional formats
	    , DISPLAY_FORMAT_DENOMINATOR_256 = 356
	    , DISPLAY_FORMAT_DENOMINATOR_128 = 228
	    , DISPLAY_FORMAT_DENOMINATOR_64 = 164
	    , DISPLAY_FORMAT_DENOMINATOR_32_QUARTERS = 136
	    , DISPLAY_FORMAT_DENOMINATOR_32_HALVES = 134
	    , DISPLAY_FORMAT_DENOMINATOR_32 = 132 
	    , DISPLAY_FORMAT_DENOMINATOR_16 = 116
	    , DISPLAY_FORMAT_DENOMINATOR_8 = 108
	    , DISPLAY_FORMAT_DENOMINATOR_4 = 104
	    , DISPLAY_FORMAT_DENOMINATOR_2 = 102
	    };

	    /*==========================================================================*/
	    public enum SecurityTypeEnum : int
	    { ST_UNSET = 0
	    , ST_FUTURE = 1
	    , ST_STOCK = 2
	    , ST_FOREX = 3 // Bitcoins also go into this category
	    , ST_INDEX = 4
	    , ST_FUTURES_STRATEGY = 5
	    , ST_STOCK_OPTION = 6
	    , ST_FUTURES_OPTION = 7
	    , ST_INDEX_OPTION = 8
	    };
    	
	    /*==========================================================================*/
	    public enum HistoricalDataIntervalEnum : int
	    { INTERVAL_TICK = 0
	    , INTERVAL_1_SECOND = 1
	    , INTERVAL_2_SECONDS = 2
	    , INTERVAL_4_SECONDS = 4
	    , INTERVAL_5_SECONDS = 5
	    , INTERVAL_10_SECONDS = 10
	    , INTERVAL_30_SECONDS = 30
	    , INTERVAL_1_MINUTE = 60
	    , INTERVAL_1_DAY = 86400
	    , INTERVAL_1_WEEK = 604800
	    };

        #endregion



        #region Process

        public static void Process()
        {
            //if (BitConverter.IsLittleEndian)
            //{ 
            //}
            //csHeader.s_LogonRequest request = new csHeader.s_LogonRequest();
            //byte[] source = new byte[280];
            //request.CopyFrom(source);
            //int size = Marshal.SizeOf(typeof(csHeader.s_Heartbeat));
            //csHeader.s_LogonRequest request = new csHeader.s_LogonRequest();
            //csHeader.s_Heartbeat request = new csHeader.s_Heartbeat();
            //s_QuoteIncrementalUpdateCompact request = new s_QuoteIncrementalUpdateCompact();
            //s_MarketDataFeedStatus request = new s_MarketDataFeedStatus();
            //s_MarketDepthSnapshotLevel request = new s_MarketDepthSnapshotLevel();
            //s_MarketDepthIncrementalUpdate request = new s_MarketDepthIncrementalUpdate();
            //s_MarketDepthIncrementalUpdateCompact request = new s_MarketDepthIncrementalUpdateCompact();
            //s_TradeIncrementalUpdateCompact request = new s_TradeIncrementalUpdateCompact();
            //s_SecurityDefinitionResponse request = new s_SecurityDefinitionResponse();
            //csHeader.s_ExchangeListRequest request2 = new csHeader.s_ExchangeListRequest(0);
            //request2.MessageSize = (ushort)System.Runtime.InteropServices.Marshal.SizeOf(request2);
            //byte[] bytesToSend = csHeader.GetBytesFromObject(request2);
            //csHeader.s_ExchangeListRequest request3 = new s_ExchangeListRequest();
            //request3.CopyFrom(bytesToSend);

            s_MarketDepthRequest request = new s_MarketDepthRequest();

            //request.TotalDailyVolume = 11;
            //request.TradeAtBidOrAsk = BidOrAskEnum.AT_BID;
            //request.TradeVolume = 22;
            //request.TradeDateTimeUnix = 875;
            //request.Price = 100.99f;

            request.Exchange = "cme";
            request.Symbol = "6E";
            request.RequestActionValue = RequestActionEnum.SUBSCRIBE;
            request.NumberOfLevels = 20;
            

            //request.AskPrice = 4.123F;
            //request.BidPrice = 1.33F;
            //request.BidSize = 25;
            //request.AskSize = 78;
            //request.QuoteDateTimeUnix = 555;
            //request.UpdateType = MarketDepthIncrementalUpdateTypeEnum.DEPTH_DELETE;
            
            int size = Marshal.SizeOf(request);

            byte[] source = GetBytesFromObject(request);

            //s_Heartbeat.SetHeartbeatBytesToSend();
            //size = Marshal.SizeOf(typeof(s_QuoteIncrementalUpdate));

            //byte[] source = new byte[280];
            //source[8] = 65;
            //source[9] = 66;
            //source[10] = 67;
            //source[11] = 68;
            //source[136] = 1;
            //source[137] = 1;
            //source[138] = 1;
            //source[139] = 1;
            //source[140] = 1;
            //source[141] = 1;
            //source[142] = 1;
            //source[143] = 1;
            //byte[] source = new byte[16];
            //source[8] = 65;
            //source[9] = 66;
            //source[10] = 67;
            //source[11] = 68;
            //byte[] source = new byte[48];
            //source[8] = 65;
            //source[9] = 66;
            //source[10] = 67;
            //source[11] = 68;
            //for (byte i = 48; i < 96; i++)
            //{
            //    source[i-48] = i;
            //}
            //byte[] source = new byte[32];
            //source[4] = 2;
            //source[5] = 0;
            //source[6] = 0;
            //source[7] = 0;
            
            
            DateTime dateStart = DateTime.Now;
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();


            for (int i = 0; i < 100000; i++)
            {
                //csHeader.s_LogonRequest request = new csHeader.s_LogonRequest();
                //csHeader.s_Heartbeat request = new csHeader.s_Heartbeat();

                //request.CopyFrom(source);
                request.CopyFromManual(source);
                //int size = Marshal.SizeOf(request);
                //int size = Marshal.SizeOf(typeof(s_LogonRequest));
            }

            sw.Stop();
            TimeSpan ts = sw.Elapsed;
            double timeElapsed = ts.TotalMilliseconds;
        }

        #endregion 

        
        #region GetBytesFromObject
        /// <summary>
        /// Returns byte array 'equivalent' to this object.
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static byte[] GetBytesFromObject(object o)
        {
            int size = Marshal.SizeOf(o);
            byte[] arr = new byte[size];
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(o, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);
            return arr;
        }

        #endregion 


        //************************** Authentication and Connection Monitoring ****************

        #region s_LogonRequest

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 8)]
        public struct s_LogonRequest
	    {
            [MarshalAs(UnmanagedType.U2)]
            //[FieldOffset(0)]
		    ushort size;
            [MarshalAs(UnmanagedType.U2)]
            //[FieldOffset(0)]
		    ushort type;
            [MarshalAs(UnmanagedType.I4)]
            //[FieldOffset(0)]
		    int protocolVersion;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            //[FieldOffset(0)]
            string username;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            //[FieldOffset(0)]
            string password;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            //[FieldOffset(0)]
            string generalTextData;

            [MarshalAs(UnmanagedType.I4)]
            //[FieldOffset(0)]
		    int integer_1;
            [MarshalAs(UnmanagedType.I4)]
            //[FieldOffset(0)]
		    int integer_2;
            [MarshalAs(UnmanagedType.I4)]
            //[FieldOffset(0)]
		    int heartbeatIntervalInSeconds;

            [MarshalAs(UnmanagedType.I4)]
            //[FieldOffset(0)]
		    TradeModeEnum tradeMode;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = TRADE_ACCOUNT_LENGTH)]
            //[FieldOffset(0)]
            string tradeAccount;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            //[FieldOffset(0)]
            string hardwareIdentifier;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            //[FieldOffset(0)]
            string clientName;

        


            #region properties

            public ushort MessageSize
            {
                get { return size; }
                set { size = value; }
            }

            public int ProtocolVersion
            {
                get { return protocolVersion; }
            }
            public string Username
            {
                get { return username; }
                set { username = value; }
            }
            public string Password
            {
                get { return password; }
                set { password = value; }
            }
            public string GeneralTextData
            {
                get { return generalTextData; }
                set { generalTextData = value; }
            }
            public int Integer_1
            {
                get { return integer_1; }
            }
            public int Integer_2
            {
                get { return integer_2; }
            }
            public int HeartbeatIntervalInSeconds
            {
                get { return heartbeatIntervalInSeconds; }
            }

            public string HardwareIdentifier
            {
                get { return hardwareIdentifier; }
                set { hardwareIdentifier = value; }
            }
            public string ClientName
            {
                get { return clientName; }
                set { clientName = value; }
            }
            

            #endregion 

            #region CopyFrom


            public void CopyFrom(byte[] p_SourceData)
            {
                
                //object str = new object();
                int size = Marshal.SizeOf(this);
                if(p_SourceData.Length < size)
                {
                    size = p_SourceData.Length;
                }
                IntPtr ptr = Marshal.AllocHGlobal(size);
                Marshal.Copy(p_SourceData, 0, ptr, size);
                this = (s_LogonRequest)Marshal.PtrToStructure(ptr, typeof(s_LogonRequest));//(object)Marshal.PtrToStructure(ptr, typeof(s_LogonRequest));
                Marshal.FreeHGlobal(ptr);

                //this.protocolVersion = CURRENT_VERSION;
                //this.type = LOGON_REQUEST;
            }

            #endregion 

            //NOTE: this works despite not checking any 'Pack' related allignments because all the members 'naturally' satisfy it
            //i.e., each members position is a multiple of min(its byte size, PACK_SIZE)

            #region CopyFromManual
            /// <summary>
            /// Manually converts (using BitConverter/ascii) bytes based on structure layout.
            /// </summary>
            /// <param name="p_SourceData"></param>

            public void CopyFromManual(byte[] p_SourceData)
            {
            
                int startIndex = 0;//current index in byte array
                int max = p_SourceData.Length;

                if (startIndex + 2 <= max)
                {

                    size = BitConverter.ToUInt16(p_SourceData, startIndex);
                    startIndex += 2;
                }
                if (startIndex + 2 <= max)
                {

                    type = BitConverter.ToUInt16(p_SourceData, startIndex);
                    startIndex += 2;
                }
                if (startIndex + 4 <= max)
                {

                    protocolVersion = BitConverter.ToInt32(p_SourceData, startIndex);
                    startIndex += 4;
                }

                if (startIndex + 32 <= max)
                {
                    int endIndex = startIndex + 32;
                    int byteCount = 0;
                    for (int i = startIndex; i < endIndex; i++)
                    {
                        if (p_SourceData[i] == ZEROBYTE)//end of string
                        {
                            byteCount = i - startIndex;
                            break;
                        }
                    }
                    if (byteCount > 0)
                    {
                        username = ascii.GetString(p_SourceData, startIndex, byteCount);
                    }
                    startIndex += 32;
                }

                if (startIndex + 32 <= max)
                {
                    int endIndex = startIndex + 32;
                    int byteCount = 0;
                    for (int i = startIndex; i < endIndex; i++)
                    {
                        if (p_SourceData[i] == ZEROBYTE)//end of string
                        {
                            byteCount = i - startIndex;
                            break;
                        }
                    }
                    if (byteCount > 0)
                    {
                        password = ascii.GetString(p_SourceData, startIndex, byteCount);
                    }
                    startIndex += 32;
                }
                if (startIndex + 64 <= max)
                {
                    int endIndex = startIndex + 64;
                    int byteCount = 0;
                    for (int i = startIndex; i < endIndex; i++)
                    {
                        if (p_SourceData[i] == ZEROBYTE)//end of string
                        {
                            byteCount = i - startIndex;
                            break;
                        }
                    }
                    if (byteCount > 0)
                    {
                        generalTextData = ascii.GetString(p_SourceData, startIndex, byteCount);
                    }
                    startIndex += 64;
                }

                if (startIndex + 4 <= max)
                {

                    integer_1 = BitConverter.ToInt32(p_SourceData, startIndex);
                    startIndex += 4;
                }
                if (startIndex + 4 <= max)
                {

                    integer_2 = BitConverter.ToInt32(p_SourceData, startIndex);
                    startIndex += 4;
                }
                if (startIndex + 4 <= max)
                {

                    heartbeatIntervalInSeconds = BitConverter.ToInt32(p_SourceData, startIndex);
                    startIndex += 4;
                }
                if (startIndex + 4 <= max)
                {

                    tradeMode = (TradeModeEnum)BitConverter.ToInt32(p_SourceData, startIndex);
                    startIndex += 4;
                }


                if (startIndex + TRADE_ACCOUNT_LENGTH <= max)
                {
                    int endIndex = startIndex + TRADE_ACCOUNT_LENGTH;
                    int byteCount = 0;
                    for (int i = startIndex; i < endIndex; i++)
                    {
                        if (p_SourceData[i] == ZEROBYTE)//end of string
                        {
                            byteCount = i - startIndex;
                            break;
                        }
                    }
                    if (byteCount > 0)
                    {
                        tradeAccount = ascii.GetString(p_SourceData, startIndex, byteCount);
                    }
                    startIndex += TRADE_ACCOUNT_LENGTH;
                }
                if (startIndex + 64 <= max)
                {
                    int endIndex = startIndex + 64;
                    int byteCount = 0;
                    for (int i = startIndex; i < endIndex; i++)
                    {
                        if (p_SourceData[i] == ZEROBYTE)//end of string
                        {
                            byteCount = i - startIndex;
                            break;
                        }
                    }
                    if (byteCount > 0)
                    {
                        hardwareIdentifier = ascii.GetString(p_SourceData, startIndex, byteCount);
                    }
                    startIndex += 64;
                }
                if (startIndex + 32 <= max)
                {
                    int endIndex = startIndex + 32;
                    int byteCount = 0;
                    for (int i = startIndex; i < endIndex; i++)
                    {
                        if (p_SourceData[i] == ZEROBYTE)//end of string
                        {
                            byteCount = i - startIndex;
                            break;
                        }
                    }
                    if (byteCount > 0)
                    {
                        clientName = ascii.GetString(p_SourceData, startIndex, byteCount);
                    }
                    startIndex += 32;
                }


            }

            #endregion 
         
	    };

        #endregion 

        #region s_Heartbeat

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 8)]
        public struct s_Heartbeat
	    {

            [MarshalAs(UnmanagedType.U2)]
            //[FieldOffset(0)]
            ushort size;
            [MarshalAs(UnmanagedType.U2)]
            //[FieldOffset(0)]
            ushort type;
            [MarshalAs(UnmanagedType.U4)]
            //[FieldOffset(0)]
            uint droppedMessages;
            [MarshalAs(UnmanagedType.I8)]
            //[FieldOffset(0)]
            long currentDateTime;


            //Standard UNIX date and time value
	        //typedef __int64 t_DateTime;

	        // This is a 4 byte DateTime value used in messages where compactness is an important consideration.
	        //typedef unsigned __int32 t_DateTime4Byte;
            //This can be optionally set by the Client/Server when sending this message. This is a UNIX timestamp indicating the current date and time. Time zone needs to be GMT/UTC.

            #region properties

            public ushort MessageSize
            {
                get { return size; }
            }

            public uint DroppedMessages
            {
                get { return droppedMessages; }
            }

            #endregion


            #region CopyFrom


            public void CopyFrom(byte[] p_SourceData)
            {
                int size = Marshal.SizeOf(this);
                if (p_SourceData.Length < size)
                {
                    size = p_SourceData.Length;
                }
                IntPtr ptr = Marshal.AllocHGlobal(size);
                Marshal.Copy(p_SourceData, 0, ptr, size);
                this = (s_Heartbeat)Marshal.PtrToStructure(ptr, typeof(s_Heartbeat));//(object)Marshal.PtrToStructure(ptr, typeof(s_LogonRequest));
                Marshal.FreeHGlobal(ptr);
            }

            #endregion

            #region CopyFromManual
            /// <summary>
            /// Manually converts (using BitConverter/ascii) bytes based on structure layout.
            /// </summary>
            /// <param name="p_SourceData"></param>

            public void CopyFromManual(byte[] p_SourceData)
            {
                int startIndex = 0;//current index in byte array
                int max = p_SourceData.Length;

                if (startIndex + 2 <= max)
                {
                    size = BitConverter.ToUInt16(p_SourceData, startIndex);
                    startIndex += 2;
                }
                if (startIndex + 2 <= max)
                {
                    type = BitConverter.ToUInt16(p_SourceData, startIndex);
                    startIndex += 2;
                }
                if (startIndex + 4 <= max)
                {
                    droppedMessages = BitConverter.ToUInt32(p_SourceData, startIndex);
                    startIndex += 4;
                }
                if (startIndex + 8 <= max)
                {
                    currentDateTime = BitConverter.ToInt64(p_SourceData, startIndex);
                    startIndex += 8;
                }
            }

            #endregion 

            #region SetHeartbeatBytesToSend

            public static void SetHeartbeatBytesToSend()
            {
                ushort size = 16;
                ushort type = HEARTBEAT;
                byte[] sizeBytes = BitConverter.GetBytes(size);
                byte[] typeBytes = BitConverter.GetBytes(type);
                heartbeatBytesToSend[0] = sizeBytes[0];
                heartbeatBytesToSend[1] = sizeBytes[1];
                heartbeatBytesToSend[2] = typeBytes[0];
                heartbeatBytesToSend[3] = typeBytes[1];
            }

            #endregion 
        };




        #endregion 

        #region s_MarketDataFeedStatus

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 8)]
        public struct s_MarketDataFeedStatus
        {

            [MarshalAs(UnmanagedType.U2)]
            //[FieldOffset(0)]
            ushort size;
            [MarshalAs(UnmanagedType.U2)]
            //[FieldOffset(0)]
            ushort type;
            [MarshalAs(UnmanagedType.I4)]
            //[FieldOffset(0)]
            //int status;
            MarketDataFeedStatusEnum status;


            #region properties

            public ushort MessageSize
            {
                get { return size; }
            }

            public MarketDataFeedStatusEnum Status
            {
                get { return status; }
                set { status = value; }
            }

            #endregion


            #region CopyFrom


            public void CopyFrom(byte[] p_SourceData)
            {
                int size = Marshal.SizeOf(this);
                if (p_SourceData.Length < size)
                {
                    size = p_SourceData.Length;
                }
                IntPtr ptr = Marshal.AllocHGlobal(size);
                Marshal.Copy(p_SourceData, 0, ptr, size);
                this = (s_MarketDataFeedStatus)Marshal.PtrToStructure(ptr, typeof(s_MarketDataFeedStatus));
                Marshal.FreeHGlobal(ptr);
            }

            #endregion

            #region CopyFromManual
            /// <summary>
            /// Manually converts (using BitConverter/ascii) bytes based on structure layout.
            /// </summary>
            /// <param name="p_SourceData"></param>

            public void CopyFromManual(byte[] p_SourceData)
            {
                int startIndex = 0;//current index in byte array
                int max = p_SourceData.Length;

                if (startIndex + 2 <= max)
                {
                    size = BitConverter.ToUInt16(p_SourceData, startIndex);
                    startIndex += 2;
                }
                if (startIndex + 2 <= max)
                {
                    type = BitConverter.ToUInt16(p_SourceData, startIndex);
                    startIndex += 2;
                }
                if (startIndex + 4 <= max)
                {
                    //MarketDataFeedStatusEnum 
                    status = (MarketDataFeedStatusEnum)BitConverter.ToInt32(p_SourceData, startIndex);
                    startIndex += 4;
                }

            }

            #endregion

        };




        #endregion 

        

   
        //*************** Symbol Discovery and Security Definitions *******************

        #region s_ExchangeListRequest

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 8)]
        public struct s_ExchangeListRequest
        {
            [MarshalAs(UnmanagedType.U2)]
            ushort size;
            [MarshalAs(UnmanagedType.U2)]
            ushort type;
            [MarshalAs(UnmanagedType.I4)]
            int requestID;
            

            #region properties

            public ushort MessageSize
            {
                get { return size; }
                set { size = value; }
            }

            public int RequestID
            {
                get { return requestID; }
                set { requestID = value; }
            }
            

            #endregion

            #region constructors
            /// <summary>
            /// Automatically sets fields 'size', 'type' to their correct values.
            /// </summary>
            /// <param name="requestID"></param>
            public s_ExchangeListRequest(int requestID)
                : this()//first automatically calls the default parameterless constructor that initializes all fields to their default values
            {
                this.size = (ushort)System.Runtime.InteropServices.Marshal.SizeOf(this);
                this.type = EXCHANGE_LIST_REQUEST;
                this.requestID = requestID;
            }

            #endregion

            #region CopyFrom


            public void CopyFrom(byte[] p_SourceData)
            {
                int size = Marshal.SizeOf(this);
                if (p_SourceData.Length < size)
                {
                    size = p_SourceData.Length;
                }
                IntPtr ptr = Marshal.AllocHGlobal(size);
                Marshal.Copy(p_SourceData, 0, ptr, size);
                this = (s_ExchangeListRequest)Marshal.PtrToStructure(ptr, typeof(s_ExchangeListRequest));
                Marshal.FreeHGlobal(ptr);
            }

            #endregion

         
            #region CopyFromManual
            /// <summary>
            /// Manually converts (using BitConverter/ascii) bytes based on structure layout.
            /// </summary>
            /// <param name="p_SourceData"></param>

            public void CopyFromManual(byte[] p_SourceData)
            {
                int startIndex = 0;//current index in byte array
                int max = p_SourceData.Length;

                if (startIndex + 2 <= max)
                {
                    size = BitConverter.ToUInt16(p_SourceData, startIndex);
                    startIndex += 2;
                }
                if (startIndex + 2 <= max)
                {
                    type = BitConverter.ToUInt16(p_SourceData, startIndex);
                    startIndex += 2;
                }
                if (startIndex + 4 <= max)
                {
                    requestID = BitConverter.ToInt32(p_SourceData, startIndex);
                    startIndex += 4;
                }
            }

            #endregion
        };

        #endregion 

        #region s_ExchangeListResponse

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 8)]
        public struct s_ExchangeListResponse
        {
            [MarshalAs(UnmanagedType.U2)]
            ushort size;
            [MarshalAs(UnmanagedType.U2)]
            ushort type;
            [MarshalAs(UnmanagedType.I4)]
            int requestID;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EXCHANGE_LENGTH)]
            string exchange;
            [MarshalAs(UnmanagedType.I1)]
            sbyte finalMessage;
            

            #region properties

            public ushort MessageSize
            {
                get { return size; }
            }

            public int RequestID
            {
                get { return requestID; }
                set { requestID = value; }
            }
             public string Exchange
            {
                get { return exchange; }
                set { exchange = value; }
            }

            #endregion

            #region CopyFrom


            public void CopyFrom(byte[] p_SourceData)
            {
                int size = Marshal.SizeOf(this);
                if (p_SourceData.Length < size)
                {
                    size = p_SourceData.Length;
                }
                IntPtr ptr = Marshal.AllocHGlobal(size);
                Marshal.Copy(p_SourceData, 0, ptr, size);
                this = (s_ExchangeListResponse)Marshal.PtrToStructure(ptr, typeof(s_ExchangeListResponse));
                Marshal.FreeHGlobal(ptr);
            }

            #endregion

         
            #region CopyFromManual
            /// <summary>
            /// Manually converts (using BitConverter/ascii) bytes based on structure layout.
            /// </summary>
            /// <param name="p_SourceData"></param>

            public void CopyFromManual(byte[] p_SourceData)
            {
                int startIndex = 0;//current index in byte array
                int max = p_SourceData.Length;

                if (startIndex + 2 <= max)
                {
                    size = BitConverter.ToUInt16(p_SourceData, startIndex);
                    startIndex += 2;
                }
                if (startIndex + 2 <= max)
                {
                    type = BitConverter.ToUInt16(p_SourceData, startIndex);
                    startIndex += 2;
                }
                if (startIndex + 4 <= max)
                {
                    requestID = BitConverter.ToInt32(p_SourceData, startIndex);
                    startIndex += 4;
                }
                if (startIndex + EXCHANGE_LENGTH <= max)
                {
                    int endIndex = startIndex + EXCHANGE_LENGTH;
                    int byteCount = 0;
                    for (int i = startIndex; i < endIndex; i++)
                    {
                        if (p_SourceData[i] == ZEROBYTE)//end of string
                        {
                            byteCount = i - startIndex;
                            break;
                        }
                    }
                    if (byteCount > 0)
                    {
                        exchange = ascii.GetString(p_SourceData, startIndex, byteCount);
                    }
                    startIndex += EXCHANGE_LENGTH;
                }
                if (startIndex + 1 <= max)
                {
                    finalMessage = (sbyte)p_SourceData[startIndex];
                    startIndex += 1;
                }
            }

            #endregion
        };

        #endregion 

        #region s_SymbolsForExchangeRequest

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 8)]
        public struct s_SymbolsForExchangeRequest
        {
            [MarshalAs(UnmanagedType.U2)]
            ushort size;
            [MarshalAs(UnmanagedType.U2)]
            ushort type;
            [MarshalAs(UnmanagedType.I4)]
            int requestID;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EXCHANGE_LENGTH)]
            string exchange;
            [MarshalAs(UnmanagedType.I4)]
            SecurityTypeEnum securityType;
            

            #region properties

            public ushort MessageSize
            {
                get { return size; }
                set { size = value; }
            }

            public int RequestID
            {
                get { return requestID; }
                set { requestID = value; }
            }
            public string Exchange
            {
                get { return exchange; }
                set { exchange = value; }
            }
            public SecurityTypeEnum SecurityType
            {
                get { return securityType; }
                set { securityType = value; }
            }

            #endregion

            #region constructors
            /// <summary>
            /// Automatically sets fields 'size', 'type' to their correct values.
            /// </summary>
            /// <param name="requestID"></param>
            public s_SymbolsForExchangeRequest(int requestID, string exchange, SecurityTypeEnum securityType)
                : this()//first automatically calls the default parameterless constructor that initializes all fields to their default values
            {
                this.size = (ushort)System.Runtime.InteropServices.Marshal.SizeOf(this);
                this.type = SYMBOLS_FOR_EXCHANGE_REQUEST;
                this.requestID = requestID;
                this.exchange = exchange;
                this.securityType = securityType;
            }

            #endregion

            #region CopyFrom


            public void CopyFrom(byte[] p_SourceData)
            {
                int size = Marshal.SizeOf(this);
                if (p_SourceData.Length < size)
                {
                    size = p_SourceData.Length;
                }
                IntPtr ptr = Marshal.AllocHGlobal(size);
                Marshal.Copy(p_SourceData, 0, ptr, size);
                this = (s_SymbolsForExchangeRequest)Marshal.PtrToStructure(ptr, typeof(s_SymbolsForExchangeRequest));
                Marshal.FreeHGlobal(ptr);
            }

            #endregion
         
            #region CopyFromManual
            /// <summary>
            /// Manually converts (using BitConverter/ascii) bytes based on structure layout.
            /// </summary>
            /// <param name="p_SourceData"></param>

            public void CopyFromManual(byte[] p_SourceData)
            {
                int startIndex = 0;//current index in byte array
                int max = p_SourceData.Length;

                if (startIndex + 2 <= max)
                {
                    size = BitConverter.ToUInt16(p_SourceData, startIndex);
                    startIndex += 2;
                }
                if (startIndex + 2 <= max)
                {
                    type = BitConverter.ToUInt16(p_SourceData, startIndex);
                    startIndex += 2;
                }
                if (startIndex + 4 <= max)
                {
                    requestID = BitConverter.ToInt32(p_SourceData, startIndex);
                    startIndex += 4;
                }
                if (startIndex + EXCHANGE_LENGTH <= max)
                {
                    int endIndex = startIndex + EXCHANGE_LENGTH;
                    int byteCount = 0;
                    for (int i = startIndex; i < endIndex; i++)
                    {
                        if (p_SourceData[i] == ZEROBYTE)//end of string
                        {
                            byteCount = i - startIndex;
                            break;
                        }
                    }
                    if (byteCount > 0)
                    {
                        exchange = ascii.GetString(p_SourceData, startIndex, byteCount);
                    }
                    startIndex += EXCHANGE_LENGTH;
                }
                if (startIndex + 4 <= max)
                {
                    securityType = (SecurityTypeEnum)BitConverter.ToInt32(p_SourceData, startIndex);
                    startIndex += 4;
                }
            }

            #endregion
        };

        #endregion 

        #region s_UnderlyingSymbolsForExchangeRequest

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 8)]
        public struct s_UnderlyingSymbolsForExchangeRequest
        {
            [MarshalAs(UnmanagedType.U2)]
            ushort size;
            [MarshalAs(UnmanagedType.U2)]
            ushort type;
            [MarshalAs(UnmanagedType.I4)]
            int requestID;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EXCHANGE_LENGTH)]
            string exchange;
            [MarshalAs(UnmanagedType.I4)]
            SecurityTypeEnum securityType;
            

            #region properties

            public ushort MessageSize
            {
                get { return size; }
                set { size = value; }
            }

            public int RequestID
            {
                get { return requestID; }
                set { requestID = value; }
            }
            public string Exchange
            {
                get { return exchange; }
                set { exchange = value; }
            }
            public SecurityTypeEnum SecurityType
            {
                get { return securityType; }
                set { securityType = value; }
            }

            #endregion

            #region constructors
            /// <summary>
            /// Automatically sets fields 'size', 'type' to their correct values.
            /// </summary>
            /// <param name="requestID"></param>
            public s_UnderlyingSymbolsForExchangeRequest(int requestID, string exchange, SecurityTypeEnum securityType)
                : this()//first automatically calls the default parameterless constructor that initializes all fields to their default values
            {
                this.size = (ushort)System.Runtime.InteropServices.Marshal.SizeOf(this);
                this.type = UNDERLYING_SYMBOLS_FOR_EXCHANGE_REQUEST;
                this.requestID = requestID;
                this.exchange = exchange;
                this.securityType = securityType;
            }

            #endregion

            #region CopyFrom


            public void CopyFrom(byte[] p_SourceData)
            {
                int size = Marshal.SizeOf(this);
                if (p_SourceData.Length < size)
                {
                    size = p_SourceData.Length;
                }
                IntPtr ptr = Marshal.AllocHGlobal(size);
                Marshal.Copy(p_SourceData, 0, ptr, size);
                this = (s_UnderlyingSymbolsForExchangeRequest)Marshal.PtrToStructure(ptr, typeof(s_UnderlyingSymbolsForExchangeRequest));
                Marshal.FreeHGlobal(ptr);
            }

            #endregion
         
            #region CopyFromManual
            /// <summary>
            /// Manually converts (using BitConverter/ascii) bytes based on structure layout.
            /// </summary>
            /// <param name="p_SourceData"></param>

            public void CopyFromManual(byte[] p_SourceData)
            {
                int startIndex = 0;//current index in byte array
                int max = p_SourceData.Length;

                if (startIndex + 2 <= max)
                {
                    size = BitConverter.ToUInt16(p_SourceData, startIndex);
                    startIndex += 2;
                }
                if (startIndex + 2 <= max)
                {
                    type = BitConverter.ToUInt16(p_SourceData, startIndex);
                    startIndex += 2;
                }
                if (startIndex + 4 <= max)
                {
                    requestID = BitConverter.ToInt32(p_SourceData, startIndex);
                    startIndex += 4;
                }
                if (startIndex + EXCHANGE_LENGTH <= max)
                {
                    int endIndex = startIndex + EXCHANGE_LENGTH;
                    int byteCount = 0;
                    for (int i = startIndex; i < endIndex; i++)
                    {
                        if (p_SourceData[i] == ZEROBYTE)//end of string
                        {
                            byteCount = i - startIndex;
                            break;
                        }
                    }
                    if (byteCount > 0)
                    {
                        exchange = ascii.GetString(p_SourceData, startIndex, byteCount);
                    }
                    startIndex += EXCHANGE_LENGTH;
                }
                if (startIndex + 4 <= max)
                {
                    securityType = (SecurityTypeEnum)BitConverter.ToInt32(p_SourceData, startIndex);
                    startIndex += 4;
                }
            }

            #endregion
        };

        #endregion 

        #region s_SymbolsForUnderlyingRequest

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 8)]
        public struct s_SymbolsForUnderlyingRequest
        {
            [MarshalAs(UnmanagedType.U2)]
            ushort size;
            [MarshalAs(UnmanagedType.U2)]
            ushort type;
            [MarshalAs(UnmanagedType.I4)]
            int requestID;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = UNDERLYING_SYMBOL_LENGTH)]
            string underlyingSymbol;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EXCHANGE_LENGTH)]
            string exchange;
            [MarshalAs(UnmanagedType.I4)]
            SecurityTypeEnum securityType;
            


            #region properties

            public ushort MessageSize
            {
                get { return size; }
                set { size = value; }
            }

            public int RequestID
            {
                get { return requestID; }
                set { requestID = value; }
            }
             public string UnderlyingSymbol
            {
                get { return underlyingSymbol; }
                set { underlyingSymbol = value; }
            }
            public string Exchange
            {
                get { return exchange; }
                set { exchange = value; }
            }
            public SecurityTypeEnum SecurityType
            {
                get { return securityType; }
                set { securityType = value; }
            }

            #endregion

            #region constructors
            /// <summary>
            /// Automatically sets fields 'size', 'type' to their correct values.
            /// </summary>
            /// <param name="requestID"></param>
            public s_SymbolsForUnderlyingRequest(int requestID, string underlyingSymbol, string exchange, SecurityTypeEnum securityType)
                : this()//first automatically calls the default parameterless constructor that initializes all fields to their default values
            {
                this.size = (ushort)System.Runtime.InteropServices.Marshal.SizeOf(this);
                this.type = SYMBOLS_FOR_UNDERLYING_REQUEST;
                this.requestID = requestID;
                this.underlyingSymbol = underlyingSymbol;
                this.exchange = exchange;
                this.securityType = securityType;
            }

            #endregion

            #region CopyFrom


            public void CopyFrom(byte[] p_SourceData)
            {
                int size = Marshal.SizeOf(this);
                if (p_SourceData.Length < size)
                {
                    size = p_SourceData.Length;
                }
                IntPtr ptr = Marshal.AllocHGlobal(size);
                Marshal.Copy(p_SourceData, 0, ptr, size);
                this = (s_SymbolsForUnderlyingRequest)Marshal.PtrToStructure(ptr, typeof(s_SymbolsForUnderlyingRequest));
                Marshal.FreeHGlobal(ptr);
            }

            #endregion
         
            #region CopyFromManual
            /// <summary>
            /// Manually converts (using BitConverter/ascii) bytes based on structure layout.
            /// </summary>
            /// <param name="p_SourceData"></param>

            public void CopyFromManual(byte[] p_SourceData)
            {
                int startIndex = 0;//current index in byte array
                int max = p_SourceData.Length;

                if (startIndex + 2 <= max)
                {
                    size = BitConverter.ToUInt16(p_SourceData, startIndex);
                    startIndex += 2;
                }
                if (startIndex + 2 <= max)
                {
                    type = BitConverter.ToUInt16(p_SourceData, startIndex);
                    startIndex += 2;
                }
                if (startIndex + 4 <= max)
                {
                    requestID = BitConverter.ToInt32(p_SourceData, startIndex);
                    startIndex += 4;
                }
                if (startIndex + UNDERLYING_SYMBOL_LENGTH <= max)
                {
                    int endIndex = startIndex + UNDERLYING_SYMBOL_LENGTH;
                    int byteCount = 0;
                    for (int i = startIndex; i < endIndex; i++)
                    {
                        if (p_SourceData[i] == ZEROBYTE)//end of string
                        {
                            byteCount = i - startIndex;
                            break;
                        }
                    }
                    if (byteCount > 0)
                    {
                        underlyingSymbol = ascii.GetString(p_SourceData, startIndex, byteCount);
                    }
                    startIndex += UNDERLYING_SYMBOL_LENGTH;
                }
                if (startIndex + EXCHANGE_LENGTH <= max)
                {
                    int endIndex = startIndex + EXCHANGE_LENGTH;
                    int byteCount = 0;
                    for (int i = startIndex; i < endIndex; i++)
                    {
                        if (p_SourceData[i] == ZEROBYTE)//end of string
                        {
                            byteCount = i - startIndex;
                            break;
                        }
                    }
                    if (byteCount > 0)
                    {
                        exchange = ascii.GetString(p_SourceData, startIndex, byteCount);
                    }
                    startIndex += EXCHANGE_LENGTH;
                }
                if (startIndex + 4 <= max)
                {
                    securityType = (SecurityTypeEnum)BitConverter.ToInt32(p_SourceData, startIndex);
                    startIndex += 4;
                }
            }

            #endregion
        };

        #endregion 

        #region s_SecurityDefinitionForSymbolRequest

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 8)]
        public struct s_SecurityDefinitionForSymbolRequest
        {
            [MarshalAs(UnmanagedType.U2)]
            ushort size;
            [MarshalAs(UnmanagedType.U2)]
            ushort type;
            [MarshalAs(UnmanagedType.I4)]
            int requestID;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = SYMBOL_LENGTH)]
            string symbol;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EXCHANGE_LENGTH)]
            string exchange;
            [MarshalAs(UnmanagedType.I4)]
            SecurityTypeEnum securityType;
            

            #region properties

            public ushort MessageSize
            {
                get { return size; }
                set { size = value; }
            }

            public int RequestID
            {
                get { return requestID; }
                set { requestID = value; }
            }
             public string Symbol
            {
                get { return symbol; }
                set { symbol = value; }
            }
            public string Exchange
            {
                get { return exchange; }
                set { exchange = value; }
            }
            public SecurityTypeEnum SecurityType
            {
                get { return securityType; }
                set { securityType = value; }
            }

            #endregion

            #region constructors
            /// <summary>
            /// Automatically sets fields 'size', 'type' to their correct values.
            /// </summary>
            /// <param name="requestID"></param>
            public s_SecurityDefinitionForSymbolRequest(int requestID, string symbol, string exchange, SecurityTypeEnum securityType)
                : this()//first automatically calls the default parameterless constructor that initializes all fields to their default values
            {
                this.size = (ushort)System.Runtime.InteropServices.Marshal.SizeOf(this);
                this.type = SECURITY_DEFINITION_FOR_SYMBOL_REQUEST;
                this.requestID = requestID;
                this.symbol = symbol;
                this.exchange = exchange;
                this.securityType = securityType;
            }

            #endregion

            #region CopyFrom


            public void CopyFrom(byte[] p_SourceData)
            {
                int size = Marshal.SizeOf(this);
                if (p_SourceData.Length < size)
                {
                    size = p_SourceData.Length;
                }
                IntPtr ptr = Marshal.AllocHGlobal(size);
                Marshal.Copy(p_SourceData, 0, ptr, size);
                this = (s_SecurityDefinitionForSymbolRequest)Marshal.PtrToStructure(ptr, typeof(s_SecurityDefinitionForSymbolRequest));
                Marshal.FreeHGlobal(ptr);
            }

            #endregion
         
            #region CopyFromManual
            /// <summary>
            /// Manually converts (using BitConverter/ascii) bytes based on structure layout.
            /// </summary>
            /// <param name="p_SourceData"></param>

            public void CopyFromManual(byte[] p_SourceData)
            {
                int startIndex = 0;//current index in byte array
                int max = p_SourceData.Length;

                if (startIndex + 2 <= max)
                {
                    size = BitConverter.ToUInt16(p_SourceData, startIndex);
                    startIndex += 2;
                }
                if (startIndex + 2 <= max)
                {
                    type = BitConverter.ToUInt16(p_SourceData, startIndex);
                    startIndex += 2;
                }
                if (startIndex + 4 <= max)
                {
                    requestID = BitConverter.ToInt32(p_SourceData, startIndex);
                    startIndex += 4;
                }
                if (startIndex + UNDERLYING_SYMBOL_LENGTH <= max)
                {
                    int endIndex = startIndex + UNDERLYING_SYMBOL_LENGTH;
                    int byteCount = 0;
                    for (int i = startIndex; i < endIndex; i++)
                    {
                        if (p_SourceData[i] == ZEROBYTE)//end of string
                        {
                            byteCount = i - startIndex;
                            break;
                        }
                    }
                    if (byteCount > 0)
                    {
                        symbol = ascii.GetString(p_SourceData, startIndex, byteCount);
                    }
                    startIndex += UNDERLYING_SYMBOL_LENGTH;
                }
                if (startIndex + EXCHANGE_LENGTH <= max)
                {
                    int endIndex = startIndex + EXCHANGE_LENGTH;
                    int byteCount = 0;
                    for (int i = startIndex; i < endIndex; i++)
                    {
                        if (p_SourceData[i] == ZEROBYTE)//end of string
                        {
                            byteCount = i - startIndex;
                            break;
                        }
                    }
                    if (byteCount > 0)
                    {
                        exchange = ascii.GetString(p_SourceData, startIndex, byteCount);
                    }
                    startIndex += EXCHANGE_LENGTH;
                }
                if (startIndex + 4 <= max)
                {
                    securityType = (SecurityTypeEnum)BitConverter.ToInt32(p_SourceData, startIndex);
                    startIndex += 4;
                }
            }

            #endregion
        };

        #endregion 

        #region s_SecurityDefinitionResponse

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 8)]
        public struct s_SecurityDefinitionResponse
        {
            [MarshalAs(UnmanagedType.U2)]
            ushort size;
            [MarshalAs(UnmanagedType.U2)]
            ushort type;
            [MarshalAs(UnmanagedType.I4)]
            int requestID;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = SYMBOL_LENGTH)]
            string symbol;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EXCHANGE_LENGTH)]
            string exchange;
            [MarshalAs(UnmanagedType.I4)]
            SecurityTypeEnum securityType;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = SYMBOL_DESCRIPTION_LENGTH)]
            string symbolDescription;
            [MarshalAs(UnmanagedType.R4)]
            float tickSize;
            [MarshalAs(UnmanagedType.I4)]
            DisplayFormatEnum priceDisplayFormat;
            [MarshalAs(UnmanagedType.R4)]
            float tickCurrencyValue;
            [MarshalAs(UnmanagedType.I1)]
            sbyte finalMessage;


            #region properties

            public ushort MessageSize
            {
                get { return size; }
            }

            public int RequestID
            {
                get { return requestID; }
            }

            public string Symbol
            {
                get { return symbol; }
                set { symbol = value; }
            }
            public string Exchange
            {
                get { return exchange; }
                set { exchange = value; }
            }
            public string SymbolDescription
            {
                get { return symbolDescription; }
                set { symbolDescription = value; }
            }
            public SecurityTypeEnum SecurityType
            {
                get { return securityType; }
                set { securityType = value; }
            }
            public DisplayFormatEnum PriceDisplayFormat
            {
                get { return priceDisplayFormat; }
                set { priceDisplayFormat = value; }
            }
            public float TickCurrencyValue
            {
                get { return tickCurrencyValue; }
                set { tickCurrencyValue = value; }
            }
            public float TickSize
            {
                get { return tickSize; }
                set { tickSize = value; }
            }
            public sbyte FinalMessage
            {
                get { return finalMessage; }
                set { finalMessage = value; }
            }

            #endregion

            #region CopyFrom


            public void CopyFrom(byte[] p_SourceData)
            {
                int size = Marshal.SizeOf(this);
                if (p_SourceData.Length < size)
                {
                    size = p_SourceData.Length;
                }
                IntPtr ptr = Marshal.AllocHGlobal(size);
                Marshal.Copy(p_SourceData, 0, ptr, size);
                this = (s_SecurityDefinitionResponse)Marshal.PtrToStructure(ptr, typeof(s_SecurityDefinitionResponse));
                Marshal.FreeHGlobal(ptr);
            }

            #endregion


            #region CopyFromManual
            /// <summary>
            /// Manually converts (using BitConverter/ascii) bytes based on structure layout.
            /// </summary>
            /// <param name="p_SourceData"></param>

            public void CopyFromManual(byte[] p_SourceData)
            {
                int startIndex = 0;//current index in byte array
                int max = p_SourceData.Length;

                if (startIndex + 2 <= max)
                {
                    size = BitConverter.ToUInt16(p_SourceData, startIndex);
                    startIndex += 2;
                }
                if (startIndex + 2 <= max)
                {
                    type = BitConverter.ToUInt16(p_SourceData, startIndex);
                    startIndex += 2;
                }
                if (startIndex + 4 <= max)
                {
                    requestID = BitConverter.ToInt32(p_SourceData, startIndex);
                    startIndex += 4;
                }
                if (startIndex + SYMBOL_LENGTH <= max)
                {
                    int endIndex = startIndex + SYMBOL_LENGTH;
                    int byteCount = 0;
                    for (int i = startIndex; i < endIndex; i++)
                    {
                        if (p_SourceData[i] == ZEROBYTE)//end of string
                        {
                            byteCount = i - startIndex;
                            break;
                        }
                    }
                    if (byteCount > 0)
                    {
                        symbol = ascii.GetString(p_SourceData, startIndex, byteCount);
                    }
                    startIndex += SYMBOL_LENGTH;
                }
                if (startIndex + EXCHANGE_LENGTH <= max)
                {
                    int endIndex = startIndex + EXCHANGE_LENGTH;
                    int byteCount = 0;
                    for (int i = startIndex; i < endIndex; i++)
                    {
                        if (p_SourceData[i] == ZEROBYTE)//end of string
                        {
                            byteCount = i - startIndex;
                            break;
                        }
                    }
                    if (byteCount > 0)
                    {
                        exchange = ascii.GetString(p_SourceData, startIndex, byteCount);
                    }
                    startIndex += EXCHANGE_LENGTH;
                }
                if (startIndex + 4 <= max)
                {
                    securityType = (SecurityTypeEnum)BitConverter.ToInt32(p_SourceData, startIndex);
                    startIndex += 4;
                }
                if (startIndex + SYMBOL_DESCRIPTION_LENGTH <= max)
                {
                    int endIndex = startIndex + SYMBOL_DESCRIPTION_LENGTH;
                    int byteCount = 0;
                    for (int i = startIndex; i < endIndex; i++)
                    {
                        if (p_SourceData[i] == ZEROBYTE)//end of string
                        {
                            byteCount = i - startIndex;
                            break;
                        }
                    }
                    if (byteCount > 0)
                    {
                        symbolDescription = ascii.GetString(p_SourceData, startIndex, byteCount);
                    }
                    startIndex += SYMBOL_DESCRIPTION_LENGTH;
                }
                if (startIndex + 4 <= max)
                {
                    tickSize = BitConverter.ToSingle(p_SourceData, startIndex);
                    startIndex += 4;
                }
                if (startIndex + 4 <= max)
                {
                    priceDisplayFormat = (DisplayFormatEnum)BitConverter.ToInt32(p_SourceData, startIndex);
                    startIndex += 4;
                }
                if (startIndex + 4 <= max)
                {
                    tickCurrencyValue = BitConverter.ToSingle(p_SourceData, startIndex);
                    startIndex += 4;
                }
                if (startIndex + 1 <= max)
                {
                    finalMessage = (sbyte)p_SourceData[startIndex];
                    startIndex += 1;
                }
            }

            #endregion
        };

        #endregion 


        
        //********************* Market Data *******************

        #region s_MarketDataRequest

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 8)]
        public struct s_MarketDataRequest
        {
            [MarshalAs(UnmanagedType.U2)]
            ushort size;
            [MarshalAs(UnmanagedType.U2)]
            ushort type;
            [MarshalAs(UnmanagedType.I4)]
            RequestActionEnum requestActionValue;
            [MarshalAs(UnmanagedType.U2)]
            ushort marketDataSymbolID;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = SYMBOL_LENGTH)]
            string symbol;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EXCHANGE_LENGTH)]
            string exchange;


            #region properties

            public ushort MessageSize
            {
                get { return size; }
                set { size = value; }
            }

            public ushort MarketDataSymbolID
            {
                get { return marketDataSymbolID; }
                set { marketDataSymbolID = value; }
            }

            public string Symbol
            {
                get { return symbol; }
                set { symbol = value; }
            }
            public string Exchange
            {
                get { return exchange; }
                set { exchange = value; }
            }
            public RequestActionEnum RequestActionValue
            {
                get { return requestActionValue; }
                set { requestActionValue = value; }
            }

            #endregion

            #region constructors
            /// <summary>
            /// Automatically sets fields 'size', 'type' to their correct values.
            /// </summary>
            /// <param name="requestID"></param>
            public s_MarketDataRequest(RequestActionEnum requestActionValue, ushort marketDataSymbolID, string symbol, string exchange)
                : this()//first automatically calls the default parameterless constructor that initializes all fields to their default values
            {
                this.size = (ushort)System.Runtime.InteropServices.Marshal.SizeOf(this);
                this.type = MARKET_DATA_REQUEST;
                this.requestActionValue = requestActionValue;
                this.marketDataSymbolID = marketDataSymbolID;
                this.symbol = symbol;
                this.exchange = exchange;
            }

            #endregion

            #region CopyFrom


            public void CopyFrom(byte[] p_SourceData)
            {
                int size = Marshal.SizeOf(this);
                if (p_SourceData.Length < size)
                {
                    size = p_SourceData.Length;
                }
                IntPtr ptr = Marshal.AllocHGlobal(size);
                Marshal.Copy(p_SourceData, 0, ptr, size);
                this = (s_MarketDataRequest)Marshal.PtrToStructure(ptr, typeof(s_MarketDataRequest));
                Marshal.FreeHGlobal(ptr);
            }

            #endregion

            #region CopyFromManual
            /// <summary>
            /// Manually converts (using BitConverter/ascii) bytes based on structure layout.
            /// </summary>
            /// <param name="p_SourceData"></param>

            public void CopyFromManual(byte[] p_SourceData)
            {
                int startIndex = 0;//current index in byte array
                int max = p_SourceData.Length;

                if (startIndex + 2 <= max)
                {
                    size = BitConverter.ToUInt16(p_SourceData, startIndex);
                    startIndex += 2;
                }
                if (startIndex + 2 <= max)
                {
                    type = BitConverter.ToUInt16(p_SourceData, startIndex);
                    startIndex += 2;
                }
                if (startIndex + 4 <= max)
                {
                    requestActionValue = (RequestActionEnum)BitConverter.ToInt32(p_SourceData, startIndex);
                    startIndex += 4;
                }
                if (startIndex + 2 <= max)
                {
                    marketDataSymbolID = BitConverter.ToUInt16(p_SourceData, startIndex);
                    startIndex += 8;//allign boundaries...
                }
                if (startIndex + SYMBOL_LENGTH <= max)
                {
                    int endIndex = startIndex + SYMBOL_LENGTH;
                    int byteCount = 0;
                    for (int i = startIndex; i < endIndex; i++)
                    {
                        if (p_SourceData[i] == ZEROBYTE)//end of string
                        {
                            byteCount = i - startIndex;
                            break;
                        }
                    }
                    if (byteCount > 0)
                    {
                        symbol = ascii.GetString(p_SourceData, startIndex, byteCount);
                    }
                    startIndex += SYMBOL_LENGTH;
                }
                if (startIndex + EXCHANGE_LENGTH <= max)
                {
                    int endIndex = startIndex + EXCHANGE_LENGTH;
                    int byteCount = 0;
                    for (int i = startIndex; i < endIndex; i++)
                    {
                        if (p_SourceData[i] == ZEROBYTE)//end of string
                        {
                            byteCount = i - startIndex;
                            break;
                        }
                    }
                    if (byteCount > 0)
                    {
                        exchange = ascii.GetString(p_SourceData, startIndex, byteCount);
                    }
                    startIndex += EXCHANGE_LENGTH;
                }
            }

            #endregion
        };

        #endregion

        #region s_MarketDepthRequest

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 8)]
        public struct s_MarketDepthRequest
        {
            [MarshalAs(UnmanagedType.U2)]
            ushort size;
            [MarshalAs(UnmanagedType.U2)]
            ushort type;
            [MarshalAs(UnmanagedType.I4)]
            RequestActionEnum requestActionValue;
            [MarshalAs(UnmanagedType.U2)]
            ushort marketDataSymbolID;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = SYMBOL_LENGTH)]
            string symbol;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EXCHANGE_LENGTH)]
            string exchange;
            [MarshalAs(UnmanagedType.I4)]
            int numberOfLevels;


            #region properties

            public ushort MessageSize
            {
                get { return size; }
                set { size = value; }
            }

            public ushort MarketDataSymbolID
            {
                get { return marketDataSymbolID; }
                set { marketDataSymbolID = value; }
            }

            public string Symbol
            {
                get { return symbol; }
                set { symbol = value; }
            }
            public string Exchange
            {
                get { return exchange; }
                set { exchange = value; }
            }
            public RequestActionEnum RequestActionValue
            {
                get { return requestActionValue; }
                set { requestActionValue = value; }
            }
            public int NumberOfLevels
            {
                get { return numberOfLevels; }
                set { numberOfLevels = value; }
            }

            #endregion

            #region constructors
            /// <summary>
            /// Automatically sets fields 'size', 'type' to their correct values.
            /// </summary>
            /// <param name="requestID"></param>
            public s_MarketDepthRequest(RequestActionEnum requestActionValue, ushort marketDataSymbolID, string symbol, string exchange, int numberOfLevels)
                : this()//first automatically calls the default parameterless constructor that initializes all fields to their default values
            {
                this.size = (ushort)System.Runtime.InteropServices.Marshal.SizeOf(this);
                this.type = MARKET_DEPTH_REQUEST;
                this.requestActionValue = requestActionValue;
                this.marketDataSymbolID = marketDataSymbolID;
                this.symbol = symbol;
                this.exchange = exchange;
                this.numberOfLevels = numberOfLevels;
            }

            #endregion

            #region CopyFrom


            public void CopyFrom(byte[] p_SourceData)
            {
                int size = Marshal.SizeOf(this);
                if (p_SourceData.Length < size)
                {
                    size = p_SourceData.Length;
                }
                IntPtr ptr = Marshal.AllocHGlobal(size);
                Marshal.Copy(p_SourceData, 0, ptr, size);
                this = (s_MarketDepthRequest)Marshal.PtrToStructure(ptr, typeof(s_MarketDepthRequest));
                Marshal.FreeHGlobal(ptr);
            }

            #endregion

            #region CopyFromManual
            /// <summary>
            /// Manually converts (using BitConverter/ascii) bytes based on structure layout.
            /// </summary>
            /// <param name="p_SourceData"></param>

            public void CopyFromManual(byte[] p_SourceData)
            {
                int startIndex = 0;//current index in byte array
                int max = p_SourceData.Length;

                if (startIndex + 2 <= max)
                {
                    size = BitConverter.ToUInt16(p_SourceData, startIndex);
                    startIndex += 2;
                }
                if (startIndex + 2 <= max)
                {
                    type = BitConverter.ToUInt16(p_SourceData, startIndex);
                    startIndex += 2;
                }
                if (startIndex + 4 <= max)
                {
                    requestActionValue = (RequestActionEnum)BitConverter.ToInt32(p_SourceData, startIndex);
                    startIndex += 4;
                }
                if (startIndex + 2 <= max)
                {
                    marketDataSymbolID = BitConverter.ToUInt16(p_SourceData, startIndex);
                    startIndex += 8;//allign boundaries...
                }
                if (startIndex + SYMBOL_LENGTH <= max)
                {
                    int endIndex = startIndex + SYMBOL_LENGTH;
                    int byteCount = 0;
                    for (int i = startIndex; i < endIndex; i++)
                    {
                        if (p_SourceData[i] == ZEROBYTE)//end of string
                        {
                            byteCount = i - startIndex;
                            break;
                        }
                    }
                    if (byteCount > 0)
                    {
                        symbol = ascii.GetString(p_SourceData, startIndex, byteCount);
                    }
                    startIndex += SYMBOL_LENGTH;
                }
                if (startIndex + EXCHANGE_LENGTH <= max)
                {
                    int endIndex = startIndex + EXCHANGE_LENGTH;
                    int byteCount = 0;
                    for (int i = startIndex; i < endIndex; i++)
                    {
                        if (p_SourceData[i] == ZEROBYTE)//end of string
                        {
                            byteCount = i - startIndex;
                            break;
                        }
                    }
                    if (byteCount > 0)
                    {
                        exchange = ascii.GetString(p_SourceData, startIndex, byteCount);
                    }
                    startIndex += EXCHANGE_LENGTH;
                }
                if (startIndex + 4 <= max)
                {
                    numberOfLevels = BitConverter.ToInt32(p_SourceData, startIndex);
                    startIndex += 4;
                }

            }

            #endregion
        };

        #endregion 


        #region s_QuoteIncrementalUpdate

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 8)]
        public struct s_QuoteIncrementalUpdate
        {
            [MarshalAs(UnmanagedType.U2)]
            ushort size;
            [MarshalAs(UnmanagedType.U2)]
            ushort type;
            [MarshalAs(UnmanagedType.U2)]
            ushort marketDataSymbolID;
            [MarshalAs(UnmanagedType.R8)]
            double bidPrice;
            [MarshalAs(UnmanagedType.U4)]
            uint bidSize;
            [MarshalAs(UnmanagedType.R8)]
            double askPrice;
            [MarshalAs(UnmanagedType.U4)]
            uint askSize;
            [MarshalAs(UnmanagedType.R8)]
            double quoteDateTimeUnix;

            #region properties

            public ushort MessageSize
            {
                get { return size; }
            }

            public uint MarketDataSymbolID
            {
                get { return marketDataSymbolID; }
            }

            public double BidPrice
            {
                get { return bidPrice; }
                set { bidPrice = value; }
            }
            public double AskPrice
            {
                get { return askPrice; }
                set { askPrice = value; }
            }
            public uint BidSize
            {
                get { return bidSize; }
                set { bidSize = value; }
            }
            public uint AskSize
            {
                get { return askSize; }
                set { askSize = value; }
            }
            public double QuoteDateTimeUnix
            {
                get { return quoteDateTimeUnix; }
                set { quoteDateTimeUnix = value; }
            }

            #endregion

            #region CopyFrom


            public void CopyFrom(byte[] p_SourceData)
            {
                int size = Marshal.SizeOf(this);
                if (p_SourceData.Length < size)
                {
                    size = p_SourceData.Length;
                }
                IntPtr ptr = Marshal.AllocHGlobal(size);
                Marshal.Copy(p_SourceData, 0, ptr, size);
                this = (s_QuoteIncrementalUpdate)Marshal.PtrToStructure(ptr, typeof(s_QuoteIncrementalUpdate));
                Marshal.FreeHGlobal(ptr);
            }

            #endregion

            //note: this one is most likely incorrect, will test how 
            #region CopyFromManualWithoutAllignment
            /// <summary>
            /// Manually converts (using BitConverter/ascii) bytes based on structure layout.
            /// </summary>
            /// <param name="p_SourceData"></param>

            public void CopyFromManualWithoutAllignment(byte[] p_SourceData)
            {
                int startIndex = 0;//current index in byte array
                int max = p_SourceData.Length;

                if (startIndex + 2 <= max)
                {
                    size = BitConverter.ToUInt16(p_SourceData, startIndex);
                    startIndex += 2;
                }
                if (startIndex + 2 <= max)
                {
                    type = BitConverter.ToUInt16(p_SourceData, startIndex);
                    startIndex += 2;
                }
                if (startIndex + 2 <= max)
                {
                    marketDataSymbolID = BitConverter.ToUInt16(p_SourceData, startIndex);
                    startIndex += 2;
                }
                if (startIndex + 8 <= max)
                {
                    bidPrice = BitConverter.ToDouble(p_SourceData, startIndex);
                    startIndex += 8;
                }
                if (startIndex + 4 <= max)
                {
                    bidSize = BitConverter.ToUInt32(p_SourceData, startIndex);
                    startIndex += 4;
                }
                if (startIndex + 8 <= max)
                {
                    askPrice = BitConverter.ToDouble(p_SourceData, startIndex);
                    startIndex += 8;
                }
                if (startIndex + 4 <= max)
                {
                    askSize = BitConverter.ToUInt32(p_SourceData, startIndex);
                    startIndex += 4;
                }
                if (startIndex + 8 <= max)
                {
                    quoteDateTimeUnix = BitConverter.ToDouble(p_SourceData, startIndex);
                    startIndex += 8;
                }
            }

            #endregion

            //this one adjusts boundary allignments with respect to packsize
            #region CopyFromManual
            /// <summary>
            /// Manually converts (using BitConverter/ascii) bytes based on structure layout.
            /// </summary>
            /// <param name="p_SourceData"></param>

            public void CopyFromManual(byte[] p_SourceData)
            {
                int startIndex = 0;//current index in byte array
                int max = p_SourceData.Length;

                if (startIndex + 2 <= max)
                {
                    size = BitConverter.ToUInt16(p_SourceData, startIndex);
                    startIndex += 2;
                }
                if (startIndex + 2 <= max)
                {
                    type = BitConverter.ToUInt16(p_SourceData, startIndex);
                    startIndex += 2;
                }
                if (startIndex + 2 <= max)
                {
                    marketDataSymbolID = BitConverter.ToUInt16(p_SourceData, startIndex);
                    startIndex += 4;//to allign to a multiple of 8!
                }
                if (startIndex + 8 <= max)
                {
                    bidPrice = BitConverter.ToDouble(p_SourceData, startIndex);
                    startIndex += 8;
                }
                if (startIndex + 4 <= max)
                {
                    bidSize = BitConverter.ToUInt32(p_SourceData, startIndex);
                    startIndex += 8;//to allign to a multiple of 8!
                }
                if (startIndex + 8 <= max)
                {
                    askPrice = BitConverter.ToDouble(p_SourceData, startIndex);
                    startIndex += 8;
                }
                if (startIndex + 4 <= max)
                {
                    askSize = BitConverter.ToUInt32(p_SourceData, startIndex);
                    startIndex += 8;//to allign to a multiple of 8!
                }
                if (startIndex + 8 <= max)
                {
                    quoteDateTimeUnix = BitConverter.ToDouble(p_SourceData, startIndex);
                    startIndex += 8;
                }
            }

            #endregion
        };

        #endregion 

        #region s_QuoteIncrementalUpdateCompact

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 8)]
        public struct s_QuoteIncrementalUpdateCompact
        {
            [MarshalAs(UnmanagedType.U2)]
            ushort size;
            [MarshalAs(UnmanagedType.U2)]
            ushort type;
            [MarshalAs(UnmanagedType.R4)]
            float bidPrice;
            [MarshalAs(UnmanagedType.U4)]
            uint bidSize;
            [MarshalAs(UnmanagedType.R4)]
            float askPrice;
            [MarshalAs(UnmanagedType.U4)]
            uint askSize;
            [MarshalAs(UnmanagedType.U4)]
            uint quoteDateTimeUnix;
            [MarshalAs(UnmanagedType.U2)]
            ushort marketDataSymbolID;

            
            #region properties

            public ushort MessageSize
            {
                get { return size; }
            }

            public uint MarketDataSymbolID
            {
                get { return marketDataSymbolID; }
            }

            public float BidPrice
            {
                get { return bidPrice; }
                set { bidPrice = value; }
            }
            public float AskPrice
            {
                get { return askPrice; }
                set { askPrice = value; }
            }
            public uint BidSize
            {
                get { return bidSize; }
                set { bidSize = value; }
            }
            public uint AskSize
            {
                get { return askSize; }
                set { askSize = value; }
            }
            public uint QuoteDateTimeUnix
            {
                get { return quoteDateTimeUnix; }
                set { quoteDateTimeUnix = value; }
            }

            #endregion

            #region CopyFrom


            public void CopyFrom(byte[] p_SourceData)
            {
                int size = Marshal.SizeOf(this);
                if (p_SourceData.Length < size)
                {
                    size = p_SourceData.Length;
                }
                IntPtr ptr = Marshal.AllocHGlobal(size);
                Marshal.Copy(p_SourceData, 0, ptr, size);
                this = (s_QuoteIncrementalUpdateCompact)Marshal.PtrToStructure(ptr, typeof(s_QuoteIncrementalUpdateCompact));
                Marshal.FreeHGlobal(ptr);
            }

            #endregion

           

            //this one adjusts boundary allignments with respect to packsize
            #region CopyFromManual
            /// <summary>
            /// Manually converts (using BitConverter/ascii) bytes based on structure layout.
            /// </summary>
            /// <param name="p_SourceData"></param>

            public void CopyFromManual(byte[] p_SourceData)
            {
                int startIndex = 0;//current index in byte array
                int max = p_SourceData.Length;

                if (startIndex + 2 <= max)
                {
                    size = BitConverter.ToUInt16(p_SourceData, startIndex);
                    startIndex += 2;
                }
                if (startIndex + 2 <= max)
                {
                    type = BitConverter.ToUInt16(p_SourceData, startIndex);
                    startIndex += 2;
                }
                
                if (startIndex + 4 <= max)
                {
                    bidPrice = BitConverter.ToSingle(p_SourceData, startIndex);
                    startIndex += 4;
                }
                if (startIndex + 4 <= max)
                {
                    bidSize = BitConverter.ToUInt32(p_SourceData, startIndex);
                    startIndex += 4;
                }
                if (startIndex + 4 <= max)
                {
                    askPrice = BitConverter.ToSingle(p_SourceData, startIndex);
                    startIndex += 4;
                }
                if (startIndex + 4 <= max)
                {
                    askSize = BitConverter.ToUInt32(p_SourceData, startIndex);
                    startIndex += 4;
                }
                if (startIndex + 4 <= max)
                {
                    quoteDateTimeUnix = BitConverter.ToUInt32(p_SourceData, startIndex);
                    startIndex += 4;
                }
                if (startIndex + 2 <= max)
                {
                    marketDataSymbolID = BitConverter.ToUInt16(p_SourceData, startIndex);
                    startIndex += 2;
                }
            }

            #endregion
        };

        #endregion 


        #region s_TradeIncrementalUpdate

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 8)]
        public struct s_TradeIncrementalUpdate
        {
            [MarshalAs(UnmanagedType.U2)]
            ushort size;
            [MarshalAs(UnmanagedType.U2)]
            ushort type;
            [MarshalAs(UnmanagedType.U2)]
            ushort marketDataSymbolID;
            [MarshalAs(UnmanagedType.U2)]
            BidOrAskEnum tradeAtBidOrAsk;
            [MarshalAs(UnmanagedType.R8)]
            double price;
            [MarshalAs(UnmanagedType.U4)]
            uint tradeVolume;
            [MarshalAs(UnmanagedType.U4)]
            uint totalDailyVolume;
            [MarshalAs(UnmanagedType.R8)]
            double tradeDateTimeUnix;

            #region properties

            public ushort MessageSize
            {
                get { return size; }
            }

            public uint MarketDataSymbolID
            {
                get { return marketDataSymbolID; }
            }
            
            public BidOrAskEnum TradeAtBidOrAsk
            {
                get { return tradeAtBidOrAsk; }
                set { tradeAtBidOrAsk = value; }
            }
            public double Price
            {
                get { return price; }
                set { price = value; }
            }
            public uint TradeVolume
            {
                get { return tradeVolume; }
                set { tradeVolume = value; }
            }
            public uint TotalDailyVolume
            {
                get { return totalDailyVolume; }
                set { totalDailyVolume = value; }
            }
            public double TradeDateTimeUnix
            {
                get { return tradeDateTimeUnix; }
                set { tradeDateTimeUnix = value; }
            }

            #endregion

            #region CopyFrom


            public void CopyFrom(byte[] p_SourceData)
            {
                int size = Marshal.SizeOf(this);
                if (p_SourceData.Length < size)
                {
                    size = p_SourceData.Length;
                }
                IntPtr ptr = Marshal.AllocHGlobal(size);
                Marshal.Copy(p_SourceData, 0, ptr, size);
                this = (s_TradeIncrementalUpdate)Marshal.PtrToStructure(ptr, typeof(s_TradeIncrementalUpdate));
                Marshal.FreeHGlobal(ptr);
            }

            #endregion

         
            #region CopyFromManual
            /// <summary>
            /// Manually converts (using BitConverter/ascii) bytes based on structure layout.
            /// </summary>
            /// <param name="p_SourceData"></param>

            public void CopyFromManual(byte[] p_SourceData)
            {
                int startIndex = 0;//current index in byte array
                int max = p_SourceData.Length;

                if (startIndex + 2 <= max)
                {
                    size = BitConverter.ToUInt16(p_SourceData, startIndex);
                    startIndex += 2;
                }
                if (startIndex + 2 <= max)
                {
                    type = BitConverter.ToUInt16(p_SourceData, startIndex);
                    startIndex += 2;
                }
                if (startIndex + 2 <= max)
                {
                    marketDataSymbolID = BitConverter.ToUInt16(p_SourceData, startIndex);
                    startIndex += 2;
                }
                if (startIndex + 2 <= max)
                {
                    tradeAtBidOrAsk = (BidOrAskEnum)BitConverter.ToUInt16(p_SourceData, startIndex);
                    startIndex += 2;
                }
                if (startIndex + 8 <= max)
                {
                    price = BitConverter.ToDouble(p_SourceData, startIndex);
                    startIndex += 8;
                }
                if (startIndex + 4 <= max)
                {
                    tradeVolume = BitConverter.ToUInt32(p_SourceData, startIndex);
                    startIndex += 4;
                }
                if (startIndex + 4 <= max)
                {
                    totalDailyVolume = BitConverter.ToUInt32(p_SourceData, startIndex);
                    startIndex += 4;
                }
                if (startIndex + 8 <= max)
                {
                    tradeDateTimeUnix = BitConverter.ToDouble(p_SourceData, startIndex);
                    startIndex += 8;
                }
            }

            #endregion
        };

        #endregion 

        #region s_TradeIncrementalUpdateCompact

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 8)]
        public struct s_TradeIncrementalUpdateCompact
        {
            [MarshalAs(UnmanagedType.U2)]
            ushort size;
            [MarshalAs(UnmanagedType.U2)]
            ushort type;
            [MarshalAs(UnmanagedType.R4)]
            float price;
            [MarshalAs(UnmanagedType.U4)]
            uint tradeVolume;
            [MarshalAs(UnmanagedType.U4)]
            uint tradeDateTimeUnix;
            [MarshalAs(UnmanagedType.U2)]
            ushort marketDataSymbolID;
            [MarshalAs(UnmanagedType.U2)]
            BidOrAskEnum tradeAtBidOrAsk;


            #region properties

            public ushort MessageSize
            {
                get { return size; }
            }

            public uint MarketDataSymbolID
            {
                get { return marketDataSymbolID; }
            }

            public BidOrAskEnum TradeAtBidOrAsk
            {
                get { return tradeAtBidOrAsk; }
                set { tradeAtBidOrAsk = value; }
            }
            public float Price
            {
                get { return price; }
                set { price = value; }
            }
            public uint TradeVolume
            {
                get { return tradeVolume; }
                set { tradeVolume = value; }
            }
           
            public uint TradeDateTimeUnix
            {
                get { return tradeDateTimeUnix; }
                set { tradeDateTimeUnix = value; }
            }

            #endregion

            #region CopyFrom


            public void CopyFrom(byte[] p_SourceData)
            {
                int size = Marshal.SizeOf(this);
                if (p_SourceData.Length < size)
                {
                    size = p_SourceData.Length;
                }
                IntPtr ptr = Marshal.AllocHGlobal(size);
                Marshal.Copy(p_SourceData, 0, ptr, size);
                this = (s_TradeIncrementalUpdateCompact)Marshal.PtrToStructure(ptr, typeof(s_TradeIncrementalUpdateCompact));
                Marshal.FreeHGlobal(ptr);
            }

            #endregion


            #region CopyFromManual
            /// <summary>
            /// Manually converts (using BitConverter/ascii) bytes based on structure layout.
            /// </summary>
            /// <param name="p_SourceData"></param>

            public void CopyFromManual(byte[] p_SourceData)
            {
                int startIndex = 0;//current index in byte array
                int max = p_SourceData.Length;

                if (startIndex + 2 <= max)
                {
                    size = BitConverter.ToUInt16(p_SourceData, startIndex);
                    startIndex += 2;
                }
                if (startIndex + 2 <= max)
                {
                    type = BitConverter.ToUInt16(p_SourceData, startIndex);
                    startIndex += 2;
                }
                if (startIndex + 4 <= max)
                {
                    price = BitConverter.ToSingle(p_SourceData, startIndex);
                    startIndex += 4;
                }
                if (startIndex + 4 <= max)
                {
                    tradeVolume = BitConverter.ToUInt32(p_SourceData, startIndex);
                    startIndex += 4;
                }
                if (startIndex + 4 <= max)
                {
                    tradeDateTimeUnix = BitConverter.ToUInt32(p_SourceData, startIndex);
                    startIndex += 4;
                }
                if (startIndex + 2 <= max)
                {
                    marketDataSymbolID = BitConverter.ToUInt16(p_SourceData, startIndex);
                    startIndex += 2;
                }
                if (startIndex + 2 <= max)
                {
                    tradeAtBidOrAsk = (BidOrAskEnum)BitConverter.ToUInt16(p_SourceData, startIndex);
                    startIndex += 2;
                }
                
            }

            #endregion
        };

        #endregion 



        #region s_MarketDepthSnapshotLevel

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 8)]
        public struct s_MarketDepthSnapshotLevel
	    {

            [MarshalAs(UnmanagedType.U2)]
            ushort size;
            [MarshalAs(UnmanagedType.U2)]
            ushort type;
            [MarshalAs(UnmanagedType.U2)]
            ushort marketDataSymbolID;
            [MarshalAs(UnmanagedType.U2)]
            BidOrAskEnum side;
            [MarshalAs(UnmanagedType.R8)]
            double price;
            [MarshalAs(UnmanagedType.R8)]
            double volume;
            [MarshalAs(UnmanagedType.U2)]
            ushort level;
            [MarshalAs(UnmanagedType.U1)]
            byte firstMessageInBatch;
            [MarshalAs(UnmanagedType.U1)]
            byte lastMessageInBatch;
           
		

            #region properties

            public ushort MessageSize
            {
                get { return size; }
            }

            public ushort MarketDataSymbolID
            {
                get { return marketDataSymbolID; }
                set { marketDataSymbolID = value; }
            }

            public BidOrAskEnum Side
            {
                get { return side; }
                set { side = value; }
            }

            public double Price
            {
                get { return price; }
                set { price = value; }
            }
            public double Volume
            {
                get { return volume; }
                set { volume = value; }
            }
            public ushort Level
            {
                get { return level; }
                set { level = value; }
            }
            public byte FirstMessageInBatch
            {
                get { return firstMessageInBatch; }
                set { firstMessageInBatch = value; }
            }
            public byte LastMessageInBatch
            {
                get { return lastMessageInBatch; }
                set { lastMessageInBatch = value; }
            }

            #endregion


            #region CopyFrom


            public void CopyFrom(byte[] p_SourceData)
            {
                int size = Marshal.SizeOf(this);
                if (p_SourceData.Length < size)
                {
                    size = p_SourceData.Length;
                }
                IntPtr ptr = Marshal.AllocHGlobal(size);
                Marshal.Copy(p_SourceData, 0, ptr, size);
                this = (s_MarketDepthSnapshotLevel)Marshal.PtrToStructure(ptr, typeof(s_MarketDepthSnapshotLevel));
                Marshal.FreeHGlobal(ptr);
            }

            #endregion

            #region CopyFromManual
            /// <summary>
            /// Manually converts (using BitConverter/ascii) bytes based on structure layout.
            /// </summary>
            /// <param name="p_SourceData"></param>

            public void CopyFromManual(byte[] p_SourceData)
            {
                int startIndex = 0;//current index in byte array
                int max = p_SourceData.Length;

                if (startIndex + 2 <= max)
                {
                    size = BitConverter.ToUInt16(p_SourceData, startIndex);
                    startIndex += 2;
                }
                if (startIndex + 2 <= max)
                {
                    type = BitConverter.ToUInt16(p_SourceData, startIndex);
                    startIndex += 2;
                }
                if (startIndex + 2 <= max)
                {
                    marketDataSymbolID = BitConverter.ToUInt16(p_SourceData, startIndex);
                    startIndex += 2;
                }
                if (startIndex + 2 <= max)
                {
                    side = (BidOrAskEnum)BitConverter.ToUInt16(p_SourceData, startIndex);
                    startIndex += 2;
                }
                if (startIndex + 8 <= max)
                {
                    price = BitConverter.ToDouble(p_SourceData, startIndex);
                    startIndex += 8;
                }
                if (startIndex + 8 <= max)
                {
                    volume = BitConverter.ToDouble(p_SourceData, startIndex);
                    startIndex += 8;
                }
                if (startIndex + 2 <= max)
                {
                    level = BitConverter.ToUInt16(p_SourceData, startIndex);
                    startIndex += 2;
                }
                if (startIndex + 1 <= max)
                {
                    firstMessageInBatch = p_SourceData[startIndex];
                    startIndex += 1;
                }
                if (startIndex + 1 <= max)
                {
                    lastMessageInBatch = p_SourceData[startIndex];
                    startIndex += 1;
                }
                
            }

            #endregion 

        };




        #endregion 

        #region s_MarketDepthIncrementalUpdate

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 8)]
        public struct s_MarketDepthIncrementalUpdate
	    {

            [MarshalAs(UnmanagedType.U2)]
            ushort size;
            [MarshalAs(UnmanagedType.U2)]
            ushort type;
            [MarshalAs(UnmanagedType.U2)]
            ushort marketDataSymbolID;
            [MarshalAs(UnmanagedType.U2)]
            BidOrAskEnum side;
            [MarshalAs(UnmanagedType.R8)]
            double price;
            [MarshalAs(UnmanagedType.U4)]
            uint volume;
            [MarshalAs(UnmanagedType.U1)]
            MarketDepthIncrementalUpdateTypeEnum updateType;
           

            #region properties

            public ushort MessageSize
            {
                get { return size; }
            }

            public ushort MarketDataSymbolID
            {
                get { return marketDataSymbolID; }
                set { marketDataSymbolID = value; }
            }

            public BidOrAskEnum Side
            {
                get { return side; }
                set { side = value; }
            }

            public double Price
            {
                get { return price; }
                set { price = value; }
            }
            public uint Volume
            {
                get { return volume; }
                set { volume = value; }
            }
            public MarketDepthIncrementalUpdateTypeEnum UpdateType
            {
                get { return updateType; }
                set { updateType = value; }
            }
            
            #endregion


            #region CopyFrom


            public void CopyFrom(byte[] p_SourceData)
            {
                int size = Marshal.SizeOf(this);
                if (p_SourceData.Length < size)
                {
                    size = p_SourceData.Length;
                }
                IntPtr ptr = Marshal.AllocHGlobal(size);
                Marshal.Copy(p_SourceData, 0, ptr, size);
                this = (s_MarketDepthIncrementalUpdate)Marshal.PtrToStructure(ptr, typeof(s_MarketDepthIncrementalUpdate));
                Marshal.FreeHGlobal(ptr);
            }

            #endregion

            #region CopyFromManual
            /// <summary>
            /// Manually converts (using BitConverter/ascii) bytes based on structure layout.
            /// </summary>
            /// <param name="p_SourceData"></param>

            public void CopyFromManual(byte[] p_SourceData)
            {
                int startIndex = 0;//current index in byte array
                int max = p_SourceData.Length;

                if (startIndex + 2 <= max)
                {
                    size = BitConverter.ToUInt16(p_SourceData, startIndex);
                    startIndex += 2;
                }
                if (startIndex + 2 <= max)
                {
                    type = BitConverter.ToUInt16(p_SourceData, startIndex);
                    startIndex += 2;
                }
                if (startIndex + 2 <= max)
                {
                    marketDataSymbolID = BitConverter.ToUInt16(p_SourceData, startIndex);
                    startIndex += 2;
                }
                if (startIndex + 2 <= max)
                {
                    side = (BidOrAskEnum)BitConverter.ToUInt16(p_SourceData, startIndex);
                    startIndex += 2;
                }
                if (startIndex + 8 <= max)
                {
                    price = BitConverter.ToDouble(p_SourceData, startIndex);
                    startIndex += 8;
                }
                if (startIndex + 4 <= max)
                {
                    volume = BitConverter.ToUInt32(p_SourceData, startIndex);
                    startIndex += 4;
                }
                if (startIndex + 1 <= max)
                {
                    updateType = (MarketDepthIncrementalUpdateTypeEnum)p_SourceData[startIndex];
                    startIndex += 1;
                }
                
            }

            #endregion 

        };




        #endregion 

        #region s_MarketDepthIncrementalUpdateCompact

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 8)]
        public struct s_MarketDepthIncrementalUpdateCompact
        {

            [MarshalAs(UnmanagedType.U2)]
            ushort size;
            [MarshalAs(UnmanagedType.U2)]
            ushort type;
            [MarshalAs(UnmanagedType.U2)]
            ushort marketDataSymbolID;
            [MarshalAs(UnmanagedType.U2)]
            BidOrAskEnum side;
            [MarshalAs(UnmanagedType.R4)]
            float price;
            [MarshalAs(UnmanagedType.U4)]
            uint volume;
            [MarshalAs(UnmanagedType.U1)]
            MarketDepthIncrementalUpdateTypeEnum updateType;


            #region properties

            public ushort MessageSize
            {
                get { return size; }
            }

            public ushort MarketDataSymbolID
            {
                get { return marketDataSymbolID; }
                set { marketDataSymbolID = value; }
            }

            public BidOrAskEnum Side
            {
                get { return side; }
                set { side = value; }
            }

            public float Price
            {
                get { return price; }
                set { price = value; }
            }
            public uint Volume
            {
                get { return volume; }
                set { volume = value; }
            }
            public MarketDepthIncrementalUpdateTypeEnum UpdateType
            {
                get { return updateType; }
                set { updateType = value; }
            }

            #endregion


            #region CopyFrom


            public void CopyFrom(byte[] p_SourceData)
            {
                int size = Marshal.SizeOf(this);
                if (p_SourceData.Length < size)
                {
                    size = p_SourceData.Length;
                }
                IntPtr ptr = Marshal.AllocHGlobal(size);
                Marshal.Copy(p_SourceData, 0, ptr, size);
                this = (s_MarketDepthIncrementalUpdateCompact)Marshal.PtrToStructure(ptr, typeof(s_MarketDepthIncrementalUpdateCompact));
                Marshal.FreeHGlobal(ptr);
            }

            #endregion

            #region CopyFromManual
            /// <summary>
            /// Manually converts (using BitConverter/ascii) bytes based on structure layout.
            /// </summary>
            /// <param name="p_SourceData"></param>

            public void CopyFromManual(byte[] p_SourceData)
            {
                int startIndex = 0;//current index in byte array
                int max = p_SourceData.Length;

                if (startIndex + 2 <= max)
                {
                    size = BitConverter.ToUInt16(p_SourceData, startIndex);
                    startIndex += 2;
                }
                if (startIndex + 2 <= max)
                {
                    type = BitConverter.ToUInt16(p_SourceData, startIndex);
                    startIndex += 2;
                }
                if (startIndex + 2 <= max)
                {
                    marketDataSymbolID = BitConverter.ToUInt16(p_SourceData, startIndex);
                    startIndex += 2;
                }
                if (startIndex + 2 <= max)
                {
                    side = (BidOrAskEnum)BitConverter.ToUInt16(p_SourceData, startIndex);
                    startIndex += 2;
                }
                if (startIndex + 4 <= max)
                {
                    price = BitConverter.ToSingle(p_SourceData, startIndex);
                    startIndex += 4;
                }
                if (startIndex + 4 <= max)
                {
                    volume = BitConverter.ToUInt32(p_SourceData, startIndex);
                    startIndex += 4;
                }
                if (startIndex + 1 <= max)
                {
                    updateType = (MarketDepthIncrementalUpdateTypeEnum)p_SourceData[startIndex];
                    startIndex += 1;
                }

            }

            #endregion

        };




        #endregion 

       

        //********************* Process Data *******************

        #region MessageToCommaDelimitedString
        //heartBeatTimerTick
        public static string MessageToCommaDelimitedString(ushort type, byte[] sourceData)
        {
            string result = type.ToString() + ",";

        //    const UInt16 LOGON_REQUEST = 1;
        //const UInt16 LOGON_RESPONSE = 2;
        //const UInt16 HEARTBEAT = 3;
        //const UInt16 DISCONNECT_FROM_SERVER_NO_RECONNECT = 4;

        //// Market data
        //const UInt16 MARKET_DATA_FEED_STATUS = 100;
        //const UInt16 MARKET_DATA_REQUEST = 101;
        //const UInt16 MARKET_DEPTH_REQUEST = 102;
        //const UInt16 MARKET_DATA_REJECT = 103;
        //const UInt16 MARKET_DATA_SNAPSHOT = 104;
        //const UInt16 MARKET_DEPTH_FULL_UPDATE = 105;
        //const UInt16 MARKET_DEPTH_INCREMENTAL_UPDATE = 106;
        //const UInt16 TRADE_INCREMENTAL_UPDATE = 107;
        //const UInt16 QUOTE_INCREMENTAL_UPDATE = 108;
        //const UInt16 FUNDAMENTAL_DATA_RESPONSE = 110;
    	
        //const UInt16 TRADE_INCREMENTAL_UPDATE_COMPACT = 112;
        //const UInt16 DAILY_VOLUME_INCREMENTAL_UPDATE = 113;
        //const UInt16 DAILY_HIGH_INCREMENTAL_UPDATE = 114;
        //const UInt16 DAILY_LOW_INCREMENTAL_UPDATE = 115;
        //const UInt16 MARKET_DATA_FEED_SYMBOL_STATUS = 116;
        //const UInt16 QUOTE_INCREMENTAL_UPDATE_COMPACT = 117;
        //const UInt16 MARKET_DEPTH_INCREMENTAL_UPDATE_COMPACT = 118;
        //const UInt16 SETTLEMENT_INCREMENTAL_UPDATE = 119;
        //const UInt16 DAILY_OPEN_INCREMENTAL_UPDATE = 120;
        //const UInt16 MARKET_DEPTH_REJECT = 121;
        //const UInt16 MARKET_DEPTH_SNAPSHOT_LEVEL = 122;

            if (type == MARKET_DEPTH_INCREMENTAL_UPDATE_COMPACT)
            {
                try
                {
                    s_MarketDepthIncrementalUpdateCompact s = new s_MarketDepthIncrementalUpdateCompact();
                    s.CopyFromManual(sourceData);
                    //result
                    result += "MARKET_DEPTH_INCREMENTAL_UPDATE_COMPACT" + "," + s.MessageSize.ToString() + "," + s.MarketDataSymbolID.ToString() + "," + s.Price.ToString() + "," + s.Side.ToString() + "," + s.Volume.ToString() + "," + s.UpdateType.ToString();
                }
                catch (Exception ex)
                {
                    result += ex.Message;
                }
            }
            else if (type == MARKET_DEPTH_INCREMENTAL_UPDATE)
            {
                try
                {
                    s_MarketDepthIncrementalUpdate s = new s_MarketDepthIncrementalUpdate();
                    s.CopyFromManual(sourceData);
                    //result
                    result += "MARKET_DEPTH_INCREMENTAL_UPDATE" + "," + s.MessageSize.ToString() + "," + s.MarketDataSymbolID.ToString() + "," + s.Price.ToString() + "," + s.Side.ToString() + "," + s.Volume.ToString() + "," + s.UpdateType.ToString();
                }
                catch (Exception ex)
                {
                    result += ex.Message;
                }
            }
            else if (type == MARKET_DEPTH_SNAPSHOT_LEVEL)
            {
                try
                {
                    s_MarketDepthSnapshotLevel s = new s_MarketDepthSnapshotLevel();
                    s.CopyFromManual(sourceData);
                    //result
                    result += "MARKET_DEPTH_SNAPSHOT_LEVEL" + "," + s.MessageSize.ToString() + "," + s.MarketDataSymbolID.ToString() + "," + s.Price.ToString() + "," + s.Side.ToString() + "," + s.Volume.ToString() + "," + s.Level.ToString() + "," + s.FirstMessageInBatch.ToString() + "," + s.LastMessageInBatch.ToString();
                }
                catch (Exception ex)
                {
                    result += ex.Message;
                }
            }
            else if (type == QUOTE_INCREMENTAL_UPDATE)
            {
                try
                {
                    s_QuoteIncrementalUpdate s = new s_QuoteIncrementalUpdate();
                    s.CopyFromManual(sourceData);
                    //result
                    result += "QUOTE_INCREMENTAL_UPDATE" + "," + s.MessageSize.ToString() + "," + s.MarketDataSymbolID.ToString() + "," + s.BidPrice.ToString() + "," + s.AskPrice.ToString() + "," + s.BidSize.ToString() + "," + s.AskSize.ToString() + "," + s.QuoteDateTimeUnix.ToString();
                }
                catch (Exception ex)
                {
                    result += ex.Message;
                }
            }
            else if (type == QUOTE_INCREMENTAL_UPDATE_COMPACT)
            {
                try
                {
                    s_QuoteIncrementalUpdateCompact s = new s_QuoteIncrementalUpdateCompact();
                    s.CopyFromManual(sourceData);
                    //result
                    result += "QUOTE_INCREMENTAL_UPDATE_COMPACT" + "," + s.MessageSize.ToString() + "," + s.MarketDataSymbolID.ToString() + "," + s.BidPrice.ToString() + "," + s.AskPrice.ToString() + "," + s.BidSize.ToString() + "," + s.AskSize.ToString() + "," + s.QuoteDateTimeUnix.ToString();
                }
                catch (Exception ex)
                {
                    result += ex.Message;
                }
            }
            else if (type == TRADE_INCREMENTAL_UPDATE)
            {
                try
                {
                    s_TradeIncrementalUpdate s = new s_TradeIncrementalUpdate();
                    s.CopyFromManual(sourceData);
                    //result
                    result += "TRADE_INCREMENTAL_UPDATE" + "," + s.MessageSize.ToString() + "," + s.MarketDataSymbolID.ToString() + "," + s.Price.ToString() + "," + s.TradeVolume.ToString() + "," + s.TotalDailyVolume.ToString() + "," + s.TradeAtBidOrAsk.ToString() + "," + s.TradeDateTimeUnix.ToString();
                }
                catch (Exception ex)
                {
                    result += ex.Message;
                }
            }
            else if (type == TRADE_INCREMENTAL_UPDATE_COMPACT)
            {
                try
                {
                    s_TradeIncrementalUpdateCompact s = new s_TradeIncrementalUpdateCompact();
                    s.CopyFromManual(sourceData);
                    //result
                    result += "TRADE_INCREMENTAL_UPDATE_COMPACT" + "," + s.MessageSize.ToString() + "," + s.MarketDataSymbolID.ToString() + "," + s.Price.ToString() + "," + s.TradeVolume.ToString() + "," + s.TradeAtBidOrAsk.ToString() + "," + s.TradeDateTimeUnix.ToString();
                }
                catch (Exception ex)
                {
                    result += ex.Message;
                }
            }
            else if (type == MARKET_DATA_FEED_STATUS)
            { 
                try
                {
                    s_MarketDataFeedStatus s = new s_MarketDataFeedStatus();
                    s.CopyFrom(sourceData);
                    //result
                    result += "MARKET_DATA_FEED_STATUS" + "," + s.MessageSize.ToString() + "," + s.Status.ToString();
                }
                catch(Exception ex)
                {
                    result += ex.Message;
                }
            }
            else if (type == HEARTBEAT)
            {
                try
                {
                    s_Heartbeat s = new s_Heartbeat();
                    s.CopyFrom(sourceData);
                    //result
                    result += "HEARTBEAT" + "," + s.MessageSize.ToString() + "," + s.DroppedMessages.ToString();
                }
                catch (Exception ex)
                {
                    result += ex.Message;
                }
            }
            else if (type == EXCHANGE_LIST_RESPONSE)
            {
                try
                {
                    s_ExchangeListResponse s = new s_ExchangeListResponse();
                    s.CopyFrom(sourceData);
                    //result
                    result += "EXCHANGE_LIST_RESPONSE" + "," + s.MessageSize.ToString() + "," + s.RequestID.ToString() + "," + s.Exchange.ToString();
                }
                catch (Exception ex)
                {
                    result += ex.Message;
                }
            }
            else if (type == SECURITY_DEFINITION_RESPONSE)
            {
                try
                {
                    s_SecurityDefinitionResponse s = new s_SecurityDefinitionResponse();
                    s.CopyFrom(sourceData);
                    //result
                    result += "SECURITY_DEFINITION_RESPONSE" + "," + s.MessageSize.ToString() + "," + s.RequestID.ToString() + "," + s.Exchange.ToString() + "," + s.Symbol.ToString() + "," + s.SymbolDescription.ToString() + "," + s.SecurityType.ToString() + "," + s.TickSize.ToString() + "," + s.TickCurrencyValue.ToString() + "," + s.PriceDisplayFormat.ToString() + "," + s.FinalMessage.ToString();
                }
                catch (Exception ex)
                {
                    result += ex.Message;
                }
            }
            //if (type == LOGON_RESPONSE)
            //{
               
            //}
            
            return result;
        }

        #endregion
	

    }


}
