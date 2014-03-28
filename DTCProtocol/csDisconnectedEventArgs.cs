using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DTCProtocol
{
    public class csDisconnectedEventArgs : EventArgs
    {
        #region Fields

        private DateTime timeStamp;

        #endregion


        #region Constructors


        public csDisconnectedEventArgs(DateTime timeStamp)
        {
            this.timeStamp = timeStamp;
        }

        #endregion

        #region Properties

        public DateTime TimeStamp
        {
            get
            {
                return timeStamp;
            }
        }


        #endregion

    }
}
