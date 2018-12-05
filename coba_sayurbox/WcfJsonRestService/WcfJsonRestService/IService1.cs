using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Data;

namespace WcfJsonRestService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService1
    {
        //[OperationContract]
        //string GetData(int value);

        [OperationContract]
        DaInventory GetData(string id);

        [OperationContract]
        List<SayurboxInventory> CekItemsAvaliability();

        [OperationContract]
        string CekItemsAvaliabilityForCustomer();

        [OperationContract]
        string OrderForCustomer();

        [OperationContract]
        string SayurBoxLogin();

        [OperationContract]
        string SendCurrentPresence();

        [OperationContract]
        string CustomerMakeOrderRequest();

        [OperationContract]
        List<SayurboxOrderRequestExchange> ReceiveOpenRequest();

        [OperationContract]
        string DriverAcceptRequest();

        [OperationContract]
        string CustomerAcceptRequest();

        [OperationContract]
        string SendDriverPresenceOrder();

        [OperationContract]
        string GetDriverLocationOrder();

        [OperationContract]
        string DriverStartTrip();

        [OperationContract]
        string DriverEndTrip();


        // TODO: Add your service operations here
    }

    // Use a data contract as illustrated in the sample below to add composite types to service operations
    
}
