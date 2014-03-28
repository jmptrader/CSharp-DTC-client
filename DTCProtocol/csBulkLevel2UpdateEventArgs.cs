using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DTCProtocol
{
    public class csBulkLevel2UpdateEventArgs : EventArgs
    {
        #region Fields

        private DateTime timeStamp;
        private List<byte[]> updates;//make sure these are only level2 related messages...
        private List<ushort> updateTypes;
        #endregion


        #region Constructors


        public csBulkLevel2UpdateEventArgs(DateTime timeStamp, List<byte[]> updates, List<ushort> updateTypes)
        {
            this.timeStamp = timeStamp;
            this.updates = updates;
            this.updateTypes = updateTypes;
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

        public List<byte[]> Updates
        {
            get
            {
                return updates;
            }
        }

        public List<ushort> UpdateTypes
        {
            get
            {
                return updateTypes;
            }
        }

        #endregion

        #region AddUpdate

        public void AddUpdate(byte[] quote)
        {
            this.updates.Add(quote);
        }

        #endregion
        #region AddUpdateType

        public void AddUpdateType(ushort type)
        {
            this.updateTypes.Add(type);
        }

        #endregion
    }
}
