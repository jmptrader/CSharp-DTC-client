using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DTCProtocol
{
    public class csSecurityDefinitionResponseEventArgs : EventArgs
    {
        #region Fields 

        /// <summary>
        /// Using 'MarketSymbolID' as (unique) index, we get the corresponding string for the intrument. 
        /// </summary>
        private List<string> instrumentFromDTCMarketSymbolID;

        #endregion 


        #region Constructors


        public csSecurityDefinitionResponseEventArgs(List<string> instrumentFromDTCMarketSymbolID)
        {
            this.instrumentFromDTCMarketSymbolID = instrumentFromDTCMarketSymbolID;
        }

        #endregion 

        #region Properties
        /// <summary>
        /// Using 'MarketSymbolID' as (unique) index, we get the corresponding string for the intrument. 
        /// </summary>
        public List<string> InstrumentFromDTCMarketSymbolID
        {
            get
            {
                return instrumentFromDTCMarketSymbolID;
            }
        }

        #endregion 
    }
}
