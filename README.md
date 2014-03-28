Created by http://www.highfrequencyfootprint.com

```
//DTC client variables defined in your project...don't forget to add reference to DTCProtocol
//in your project

private DTCProtocol.csClient sierraDTCClient;
int sierraPortNumber;//this has to be set somewhere in your code...

bool sierraConnected = false;


private void Connect()
{
        sierraDTCClient = new DTCProtocol.csClient(sierraPortNumber);
        sierraDTCClient.BulkLevel1UpdateEvent += new EventHandler<DTCProtocol.csBulkLevel1UpdateEventArgs>(sierraDTCClient_BulkLevel1UpdateEvent);
        sierraDTCClient.BulkLevel2UpdateEvent += new EventHandler<DTCProtocol.csBulkLevel2UpdateEventArgs>(sierraDTCClient_BulkLevel2UpdateEvent);
        sierraDTCClient.ConnectedEvent += new EventHandler<DTCProtocol.csConnectedEventArgs>(sierraDTCClient_ConnectedEvent);
        sierraDTCClient.DisconnectedEvent += new EventHandler<DTCProtocol.csDisconnectedEventArgs>(sierraDTCClient_DisconnectedEvent);
        sierraDTCClient.SecurityDefinitionResponseEvent += new EventHandler<DTCProtocol.csSecurityDefinitionResponseEventArgs>(sierraDTCClient_SecurityDefinitionResponseEvent);
        sierraDTCClient.Connect(false);

}



#region Sierra DTC Client Events

List<string> dtcLinesToWrite = new List<string>();

void sierraDTCClient_SecurityDefinitionResponseEvent(object sender, DTCProtocol.csSecurityDefinitionResponseEventArgs e)
{
    dtcLinesToWrite.AddRange(e.InstrumentFromDTCMarketSymbolID);
    
}

void sierraDTCClient_ConnectedEvent(object sender, DTCProtocol.csConnectedEventArgs e)
{
    sierraConnected = true;
    //do what's needed below...
}

void sierraDTCClient_DisconnectedEvent(object sender, DTCProtocol.csDisconnectedEventArgs e)
{
    //do what's needed...
}

void sierraDTCClient_BulkLevel1UpdateEvent(object sender, DTCProtocol.csBulkLevel1UpdateEventArgs e)
{
    //record the updates as strings to be written to text file... this is just for testing...
    dtcLinesToWrite.Add(e.TimeStamp.ToString("HH:mm:ss.fff") + "," + " timeStamp");

    for (int i = 0; i < e.Updates.Count; i++)
    {
        dtcLinesToWrite.Add(DTCProtocol.csHeader.MessageToCommaDelimitedString(e.UpdateTypes[i], e.Updates[i]));
    }
}

void sierraDTCClient_BulkLevel2UpdateEvent(object sender, DTCProtocol.csBulkLevel2UpdateEventArgs e)
{
    dtcLinesToWrite.Add(e.TimeStamp.ToString("HH:mm:ss.fff") + "," + " timeStamp");

    for (int i = 0; i < e.Updates.Count; i++)
    {
        dtcLinesToWrite.Add(DTCProtocol.csHeader.MessageToCommaDelimitedString(e.UpdateTypes[i], e.Updates[i]));
    }
}

#endregion 
```
