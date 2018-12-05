using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Web;
using System.Data;
using System.Xml;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Runtime.Serialization.Json;
using System.Net.Mail;//dari sini
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Web;

namespace WcfJsonRestService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public class Service1 : IService1
    {
        //public string GetData(int value)
        //{
        //    return string.Format("You entered: {0}", value);
        //}

        [WebInvoke(Method = "GET",
                    ResponseFormat = WebMessageFormat.Json,
                    UriTemplate = "data/{id}")]
        public DaInventory GetData(string id)
        {
            // lookup DaInventory with the requested id 
            return new DaInventory()
            {
                Id = Convert.ToInt32(id),
                Name = "Leo Messi"
            };
        }

        [WebInvoke(Method = "GET",
                   ResponseFormat = WebMessageFormat.Json,
                   RequestFormat = WebMessageFormat.Json,
                   UriTemplate = "CekInventoryAvailable")]
        public List<SayurboxInventory> CekItemsAvaliability()
        {
            //string JSONstring = OperationContext.Current.RequestContext.RequestMessage.ToString();

            //XmlDocument xmlDoc = new XmlDocument();
            //xmlDoc.LoadXml(JSONstring);
            //string storer = xmlDoc.GetElementsByTagName("storer")[0].InnerText;
            //string Search_Parameter = xmlDoc.GetElementsByTagName("Search_Parameter")[0].InnerText;
            //string Search_Value = xmlDoc.GetElementsByTagName("Search_Value")[0].InnerText;
            //int da_page = Convert.ToInt32(xmlDoc.GetElementsByTagName("Page_Number")[0].InnerText);



            List<SayurboxInventory> list = new List<SayurboxInventory>();
            SqlConnection DBConn = new SqlConnection("Server=127.0.0.1;Database=Coba_Sayurbox_Db;UID=sa;PWD=P4ssw0rD;");
            //SqlConnection DBConn = new SqlConnection("UID=sa;PWD=P4ssw0rD;Database=Coba_Sayurbox_Db;Data Source = Coba_Sayurbox_Db.mdf");
            //SqlConnection DBConn = new SqlConnection(@"Data Source=.\SQLEXPRESS;AttachDbFilename=\bin\Debug\Coba_Sayurbox_Db.mdf;Connect Timeout=30;User Instance=True;UID=sa;PWD=P4ssw0rD;Database=Coba_Sayurbox_Db;");
            //SqlConnection DBConn = new SqlConnection(@"Server=.\SQLExpress;AttachDbFilename=\bin\Debug\Coba_Sayurbox_Db.mdf;Database=Coba_Sayurbox_Db;Trusted_Connection=Yes;UID=sa;PWD=P4ssw0rD;");
            SqlCommand DBCmd = new SqlCommand();
            SqlDataReader DBDR;
            SqlDataAdapter DA;
            DataTable DT;
            DataSet DS;
            string QUERY;
            if (DBConn.State == ConnectionState.Closed)
            {
                DBConn.Open();
            }

            DBCmd.Connection = DBConn;
            DBCmd.CommandText = "SELECT * from Da_Inventory";
            //DBCmd.Parameters.AddWithValue("storer", storer);
            //DBCmd.Parameters.AddWithValue("da_start", (da_page - 1) * da_interval);
            //DBCmd.Parameters.AddWithValue("da_end", da_page * da_interval);
            
            DBDR = DBCmd.ExecuteReader();


            while (DBDR.Read())
            {
                list.Add(new SayurboxInventory()
                {
                    Id = Convert.ToInt32(DBDR["ID"]),
                    SayurName = DBDR["ItemName"].ToString(),
                    Qty = Convert.ToInt32(DBDR["ItemQty"]),


                });
            }


            DBDR.Close();
            DBConn.Close();
            return list;


        }

        [WebInvoke(Method = "POST",
                   ResponseFormat = WebMessageFormat.Json,
                   RequestFormat = WebMessageFormat.Json,
                   UriTemplate = "CekInventory/?")]
        public string CekItemsAvaliabilityForCustomer()
        {
            string JSONstring = OperationContext.Current.RequestContext.RequestMessage.ToString();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(JSONstring);

            List<SayurboxInventoryInput> Da_inputs = new List<SayurboxInventoryInput>();

            int da_paling_awal = 0;
            bool keep_going = true;
            while (keep_going==true)
            {
                try
                {
                    Da_inputs.Add(new SayurboxInventoryInput()
                    {
                        Id = Convert.ToInt32(xmlDoc.GetElementsByTagName("Id")[da_paling_awal].InnerText),
                        SayurName = xmlDoc.GetElementsByTagName("SayurName")[da_paling_awal].InnerText,
                        Qty = Convert.ToInt32(xmlDoc.GetElementsByTagName("Qty")[da_paling_awal].InnerText),
                        CustomerId = Convert.ToInt32(xmlDoc.GetElementsByTagName("CustomerId")[da_paling_awal].InnerText),

                    });
                    da_paling_awal++;
                }
                catch (Exception e)
                {
                    keep_going = false;
                }
            }

            List<SayurboxInventory> list = new List<SayurboxInventory>();
            SqlConnection DBConn = new SqlConnection("Server=127.0.0.1;Database=Coba_Sayurbox_Db;UID=sa;PWD=P4ssw0rD;");
            
            SqlCommand DBCmd = new SqlCommand();
            SqlDataReader DBDR;
            SqlDataAdapter DA;
            DataTable DT;
            DataSet DS;
            string QUERY;
            if (DBConn.State == ConnectionState.Closed)
            {
                DBConn.Open();
            }

            DBCmd.Connection = DBConn;
            DBCmd.CommandText = "SELECT * from Da_Inventory where  ID=@da_idsayur0";
            DBCmd.Parameters.AddWithValue("da_idsayur0", Da_inputs[0].Id);

            if (Da_inputs.Count>1)
            {
                for (int i = 1; i < Da_inputs.Count; i++)
                {
                    DBCmd.CommandText += " OR ID=@da_idsayur" + i;
                    DBCmd.Parameters.AddWithValue("da_idsayur"+i, Da_inputs[i].Id);
                }
                
            }
            //DBCmd.Parameters.AddWithValue("storer", storer);
            //DBCmd.Parameters.AddWithValue("da_start", (da_page - 1) * da_interval);
            //DBCmd.Parameters.AddWithValue("da_end", da_page * da_interval);

            DBDR = DBCmd.ExecuteReader();


            while (DBDR.Read())
            {
                list.Add(new SayurboxInventory()
                {
                    Id = Convert.ToInt32(DBDR["ID"]),
                    SayurName = DBDR["ItemName"].ToString(),
                    Qty = Convert.ToInt32(DBDR["ItemQty"]),


                });
            }


            DBDR.Close();
            DBConn.Close();

            bool Available = true;
            for (int i = 1; i < Da_inputs.Count; i++)
            {
                for (int x = 1; x < list.Count; x++)
                {
                    if (Da_inputs[i].Id == list[x].Id)
                    {
                        int cek_da_thing = list[x].Qty - Da_inputs[i].Qty;
                        if (cek_da_thing<0)
                        {
                            Available = false;
                        }
                    }
                }
            }
            string da_message = "this is message";

            if (Available==true)
            {
                da_message = "yes the stock is available";
            }
            else if (Available == false)
            {
                da_message = "no the stock is not available";
            }

            return da_message;
            //JSONstring.Length; ;


        }

        [WebInvoke(Method = "POST",
                   ResponseFormat = WebMessageFormat.Json,
                   RequestFormat = WebMessageFormat.Json,
                   UriTemplate = "OrderInventory/?")]
        public string OrderForCustomer()
        {
            string JSONstring = OperationContext.Current.RequestContext.RequestMessage.ToString();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(JSONstring);

            List<SayurboxInventoryInput> Da_inputs = new List<SayurboxInventoryInput>();

            int da_paling_awal = 0;
            bool keep_going = true;
            while (keep_going == true)
            {
                try
                {
                    Da_inputs.Add(new SayurboxInventoryInput()
                    {
                        Id = Convert.ToInt32(xmlDoc.GetElementsByTagName("Id")[da_paling_awal].InnerText),
                        SayurName = xmlDoc.GetElementsByTagName("SayurName")[da_paling_awal].InnerText,
                        Qty = Convert.ToInt32(xmlDoc.GetElementsByTagName("Qty")[da_paling_awal].InnerText),
                        CustomerId = Convert.ToInt32(xmlDoc.GetElementsByTagName("CustomerId")[da_paling_awal].InnerText),

                    });
                    da_paling_awal++;
                }
                catch (Exception e)
                {
                    keep_going = false;
                }
            }

            List<SayurboxInventory> list = new List<SayurboxInventory>();
            SqlConnection DBConn = new SqlConnection("Server=127.0.0.1;Database=Coba_Sayurbox_Db;UID=sa;PWD=P4ssw0rD;");

            SqlCommand DBCmd = new SqlCommand();
            SqlDataReader DBDR;
            SqlDataAdapter DA;
            DataTable DT;
            DataSet DS;
            string QUERY;
            if (DBConn.State == ConnectionState.Closed)
            {
                DBConn.Open();
            }

            DBCmd.Connection = DBConn;
            DBCmd.CommandText = "SELECT * from Da_Inventory where  ID=@da_idsayur0";
            DBCmd.Parameters.AddWithValue("da_idsayur0", Da_inputs[0].Id);

            if (Da_inputs.Count > 1)
            {
                for (int i = 1; i < Da_inputs.Count; i++)
                {
                    DBCmd.CommandText += " OR ID=@da_idsayur" + i;
                    DBCmd.Parameters.AddWithValue("da_idsayur" + i, Da_inputs[i].Id);
                }

            }
            //DBCmd.Parameters.AddWithValue("storer", storer);
            //DBCmd.Parameters.AddWithValue("da_start", (da_page - 1) * da_interval);
            //DBCmd.Parameters.AddWithValue("da_end", da_page * da_interval);

            DBDR = DBCmd.ExecuteReader();


            while (DBDR.Read())
            {
                list.Add(new SayurboxInventory()
                {
                    Id = Convert.ToInt32(DBDR["ID"]),
                    SayurName = DBDR["ItemName"].ToString(),
                    Qty = Convert.ToInt32(DBDR["ItemQty"]),


                });
            }


            DBDR.Close();
            

            bool Available = true;
            for (int i = 1; i < Da_inputs.Count; i++)
            {
                for (int x = 1; x < list.Count; x++)
                {
                    if (Da_inputs[i].Id == list[x].Id)
                    {
                        int cek_da_thing = list[x].Qty - Da_inputs[i].Qty;
                        if (cek_da_thing < 0)
                        {
                            Available = false;
                        }
                    }
                }
            }
            string da_message = "this is message";

            if (Available == true)
            {
                da_message = "yes the stock is available";
            }
            else if (Available == false)
            {
                da_message = "no the stock is not available";
            }

            if (da_message == "yes the stock is available")
            {
                for (int i = 0; i < Da_inputs.Count; i++)
                {
                    for (int x = 0; x < list.Count; x++)
                    {
                        if (Da_inputs[i].Id == list[x].Id)
                        {
                            DBCmd.CommandText = " UPDATE Da_Inventory set ItemQty=@da_qty" + i + " where  ID=@da_idsayurForOrder" + i;
                            DBCmd.Parameters.AddWithValue("da_qty" + i, (list[x].Qty - Da_inputs[i].Qty));
                            DBCmd.Parameters.AddWithValue("da_idsayurForOrder" + i, Da_inputs[i].Id);
                            int Da_Affected = DBCmd.ExecuteNonQuery();
                        }
                        
                    }
                    
                }
                da_message = "Order success";
            }


            DBConn.Close();

            return da_message;
            //JSONstring.Length; ;


        }

        [WebInvoke(Method = "POST",
                   ResponseFormat = WebMessageFormat.Json,
                   RequestFormat = WebMessageFormat.Json,
                   UriTemplate = "SayurBoxLogin")]
        public string SayurBoxLogin()
        {
            
            string JSONstring = OperationContext.Current.RequestContext.RequestMessage.ToString();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(JSONstring);
            string username = xmlDoc.GetElementsByTagName("UserName")[0].InnerText;
            string password = xmlDoc.GetElementsByTagName("DaPassword")[0].InnerText;

            
            SqlConnection DBConn = new SqlConnection("Server=127.0.0.1;Database=Coba_Sayurbox_Db;UID=sa;PWD=P4ssw0rD;");
            
            SqlCommand DBCmd = new SqlCommand();
            SqlDataAdapter DA;
            DataTable DT;
            DataSet DS;
            string QUERY;
            if (DBConn.State == ConnectionState.Closed)
            {
                DBConn.Open();
            }

            DBCmd.Connection = DBConn;
            DBCmd.CommandText = " UPDATE Da_User set LoginStatus='Login' where UserName=@Da_Username and DaPassword=@Da_Password ";
            DBCmd.Parameters.AddWithValue("Da_Username" ,username);
            DBCmd.Parameters.AddWithValue("Da_Password", password);
            int Da_Affected = DBCmd.ExecuteNonQuery();


            string da_message = "";
            if (Da_Affected>=1)
            {
                da_message = "Login Sukses"; 
            }
            else if (Da_Affected <= 0)
            {
                da_message = "Login Gagal";
            }

            
            DBConn.Close();
            return da_message;


        }

        [WebInvoke(Method = "POST",
                   ResponseFormat = WebMessageFormat.Json,
                   RequestFormat = WebMessageFormat.Json,
                   UriTemplate = "SendCurrentPresence")]
        public string SendCurrentPresence()
        {

            string JSONstring = OperationContext.Current.RequestContext.RequestMessage.ToString();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(JSONstring);
            int ID = Convert.ToInt32(xmlDoc.GetElementsByTagName("ID")[0].InnerText);
            double LocationLat = Convert.ToDouble(xmlDoc.GetElementsByTagName("LocationLat")[0].InnerText);
            double LocationLon = Convert.ToDouble(xmlDoc.GetElementsByTagName("LocationLon")[0].InnerText);


            SqlConnection DBConn = new SqlConnection("Server=127.0.0.1;Database=Coba_Sayurbox_Db;UID=sa;PWD=P4ssw0rD;");

            SqlCommand DBCmd = new SqlCommand();
            SqlDataAdapter DA;
            DataTable DT;
            DataSet DS;
            string QUERY;
            if (DBConn.State == ConnectionState.Closed)
            {
                DBConn.Open();
            }

            DBCmd.Connection = DBConn;
            DBCmd.CommandText = " UPDATE Da_User set LocationLat=@Da_LocationLat , LocationLon=@Da_LocationLon  where ID=@Da_UserIdForLocation ";
            DBCmd.Parameters.AddWithValue("Da_LocationLat", LocationLat);
            DBCmd.Parameters.AddWithValue("Da_LocationLon", LocationLon);
            DBCmd.Parameters.AddWithValue("Da_UserIdForLocation", ID);
            int Da_Affected = DBCmd.ExecuteNonQuery();


            string da_message = "";
            if (Da_Affected >= 1)
            {
                da_message = "Location Set Lat:" + LocationLat + " Lon:" + LocationLon;
            }
            else if (Da_Affected <= 0)
            {
                da_message = "Location Not Set";
            }


            DBConn.Close();
            return da_message;


        }

        [WebInvoke(Method = "POST",
                   ResponseFormat = WebMessageFormat.Json,
                   RequestFormat = WebMessageFormat.Json,
                   UriTemplate = "CustomerMakeOrderRequest")]
        public string CustomerMakeOrderRequest()
        {

            string JSONstring = OperationContext.Current.RequestContext.RequestMessage.ToString();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(JSONstring);
            int ID = Convert.ToInt32(xmlDoc.GetElementsByTagName("ID")[0].InnerText);
            double LocationLat = Convert.ToDouble(xmlDoc.GetElementsByTagName("LocationLat")[0].InnerText);
            double LocationLon = Convert.ToDouble(xmlDoc.GetElementsByTagName("LocationLon")[0].InnerText);
            string OrderInventoryCode = xmlDoc.GetElementsByTagName("OrderInventoryCode")[0].InnerText;

            SqlConnection DBConn = new SqlConnection("Server=127.0.0.1;Database=Coba_Sayurbox_Db;UID=sa;PWD=P4ssw0rD;");

            SqlCommand DBCmd = new SqlCommand();
            SqlDataAdapter DA;
            DataTable DT;
            DataSet DS;
            string QUERY;
            if (DBConn.State == ConnectionState.Closed)
            {
                DBConn.Open();
            }

            DBCmd.Connection = DBConn;
            DBCmd.CommandText = "insert into DaOrderRequest (CustomerId,CustomerLat,CustomerLon,OrderInventoryCode) values(@Da_UserIdForRequestOrder,@Da_LocationLat,@Da_LocationLon,@Da_OrderInventoryCode);";
            DBCmd.Parameters.AddWithValue("Da_UserIdForRequestOrder", ID);
            DBCmd.Parameters.AddWithValue("Da_LocationLat", LocationLat);
            DBCmd.Parameters.AddWithValue("Da_LocationLon", LocationLon);
            DBCmd.Parameters.AddWithValue("Da_OrderInventoryCode", OrderInventoryCode);
            int Da_Affected = DBCmd.ExecuteNonQuery();
            //after ordering there should be a trigger that insert the qty ordered and sayur ordered into order table and create an item order code, I don't make it because it was part of question 1 and assumed at the time it was not connected to question 2
            //I assume after making order a code is then also submitted into order creation

            

            DBConn.Close();

            string da_message = "";
            if (Da_Affected >= 1)
            {
                da_message = "Order Created";
            }
            else if (Da_Affected <= 0)
            {
                da_message = "Order Creation Failed";
            }
            return da_message;


        }

        [WebInvoke(Method = "GET",
                   ResponseFormat = WebMessageFormat.Json,
                   RequestFormat = WebMessageFormat.Json,
                   UriTemplate = "ReceiveOpenRequest")]
        public List<SayurboxOrderRequestExchange> ReceiveOpenRequest()
        {
            //I am not 100% sure about this one so I assume the logic for this one is similar to online driver
            //I am also not sure whether the request is assigned a request automatically or not so I make the driver able to see all open order and can choose which order they want to tackle

            List<SayurboxOrderRequestExchange> list = new List<SayurboxOrderRequestExchange>();
            SqlConnection DBConn = new SqlConnection("Server=127.0.0.1;Database=Coba_Sayurbox_Db;UID=sa;PWD=P4ssw0rD;");
            
            SqlCommand DBCmd = new SqlCommand();
            SqlDataReader DBDR;
            SqlDataAdapter DA;
            DataTable DT;
            DataSet DS;
            string QUERY;
            if (DBConn.State == ConnectionState.Closed)
            {
                DBConn.Open();
            }

            DBCmd.Connection = DBConn;
            DBCmd.CommandText = "SELECT * from DaOrderRequest where Status='Open'";

            DBDR = DBCmd.ExecuteReader();


            while (DBDR.Read())
            {
                list.Add(new SayurboxOrderRequestExchange()
                {
                    OrderId = Convert.ToInt32(DBDR["OrderId"]),
                    CustomerId = Convert.ToInt32(DBDR["CustomerId"]),


                    CustomerLat = Convert.ToDouble(DBDR["CustomerLat"]),
                    CustomerLon = Convert.ToDouble(DBDR["CustomerLon"]),
                    

                    DriverAccept = DBDR["DriverAccept"].ToString(),
                    CustomerAccept = DBDR["CustomerAccept"].ToString(),
                    OrderInventoryCode = DBDR["OrderInventoryCode"].ToString(),
                    Status = DBDR["Status"].ToString(),
                });
            }


            DBDR.Close();
            DBConn.Close();
            return list;


        }

        [WebInvoke(Method = "POST",
                   ResponseFormat = WebMessageFormat.Json,
                   RequestFormat = WebMessageFormat.Json,
                   UriTemplate = "DriverAcceptRequest")]
        public string DriverAcceptRequest()
        {
            string JSONstring = OperationContext.Current.RequestContext.RequestMessage.ToString();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(JSONstring);
            Console.WriteLine(JSONstring);
            int OrderId = Convert.ToInt32(xmlDoc.GetElementsByTagName("OrderId")[0].InnerText);
            int DriverId = Convert.ToInt32(xmlDoc.GetElementsByTagName("DriverId")[0].InnerText);
            double DriverLat = Convert.ToDouble(xmlDoc.GetElementsByTagName("DriverLat")[0].InnerText);
            double DriverLon = Convert.ToDouble(xmlDoc.GetElementsByTagName("DriverLon")[0].InnerText);

            

            SqlConnection DBConn = new SqlConnection("Server=127.0.0.1;Database=Coba_Sayurbox_Db;UID=sa;PWD=P4ssw0rD;");

            SqlCommand DBCmd = new SqlCommand();
            SqlDataAdapter DA;
            DataTable DT;
            DataSet DS;
            string QUERY;
            if (DBConn.State == ConnectionState.Closed)
            {
                DBConn.Open();
            }

            DBCmd.Connection = DBConn;
            DBCmd.CommandText = "UPDATE DaOrderRequest set DriverId=@Da_DriverId,DriverLat=@Da_DriverLat,DriverLon=@Da_DriverLon,DriverAccept=1,Status='DriverAccept' WHERE OrderId=@Da_OrderId";
            DBCmd.Parameters.AddWithValue("Da_OrderId", OrderId);
            DBCmd.Parameters.AddWithValue("Da_DriverId", DriverId);
            DBCmd.Parameters.AddWithValue("Da_DriverLat", DriverLat);
            DBCmd.Parameters.AddWithValue("Da_DriverLon", DriverLon);
            int Da_Affected = DBCmd.ExecuteNonQuery();
            

            DBConn.Close();

            string da_message = "";
            if (Da_Affected >= 1)
            {
                da_message = "You (Driver have accepted this order)";
            }
            else if (Da_Affected <= 0)
            {
                da_message = "Accept Order Failed";
            }
            return da_message;


        }

        [WebInvoke(Method = "POST",
                   ResponseFormat = WebMessageFormat.Json,
                   RequestFormat = WebMessageFormat.Json,
                   UriTemplate = "CustomerAcceptRequest")]
        public string CustomerAcceptRequest()
        {
            string JSONstring = OperationContext.Current.RequestContext.RequestMessage.ToString();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(JSONstring);
            Console.WriteLine(JSONstring);
            int OrderId = Convert.ToInt32(xmlDoc.GetElementsByTagName("OrderId")[0].InnerText);
            int CustomerId = Convert.ToInt32(xmlDoc.GetElementsByTagName("CustomerId")[0].InnerText);


            SqlConnection DBConn = new SqlConnection("Server=127.0.0.1;Database=Coba_Sayurbox_Db;UID=sa;PWD=P4ssw0rD;");

            SqlCommand DBCmd = new SqlCommand();
            SqlDataAdapter DA;
            DataTable DT;
            DataSet DS;
            string QUERY;
            if (DBConn.State == ConnectionState.Closed)
            {
                DBConn.Open();
            }

            DBCmd.Connection = DBConn;
            DBCmd.CommandText = "UPDATE DaOrderRequest set CustomerAccept=1,Status='Customer Accept' WHERE OrderId=@Da_OrderId AND CustomerId=@Da_CustomerId";
            DBCmd.Parameters.AddWithValue("Da_OrderId", OrderId);
            DBCmd.Parameters.AddWithValue("Da_CustomerId", CustomerId);
            int Da_Affected = DBCmd.ExecuteNonQuery();


            DBConn.Close();

            string da_message = "";
            if (Da_Affected >= 1)
            {
                da_message = "Customer have accepted this order";
            }
            else if (Da_Affected <= 0)
            {
                da_message = "Accept Order Failed";
            }
            return da_message;


        }

        [WebInvoke(Method = "POST",
                   ResponseFormat = WebMessageFormat.Json,
                   RequestFormat = WebMessageFormat.Json,
                   UriTemplate = "SendDriverPresenceOrder")]
        public string SendDriverPresenceOrder()
        {

            string JSONstring = OperationContext.Current.RequestContext.RequestMessage.ToString();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(JSONstring);
            int OrderId = Convert.ToInt32(xmlDoc.GetElementsByTagName("OrderId")[0].InnerText);
            int DriverId = Convert.ToInt32(xmlDoc.GetElementsByTagName("DriverId")[0].InnerText);
            double DriverLat = Convert.ToDouble(xmlDoc.GetElementsByTagName("DriverLat")[0].InnerText);
            double DriverLon = Convert.ToDouble(xmlDoc.GetElementsByTagName("DriverLon")[0].InnerText);


            SqlConnection DBConn = new SqlConnection("Server=127.0.0.1;Database=Coba_Sayurbox_Db;UID=sa;PWD=P4ssw0rD;");

            SqlCommand DBCmd = new SqlCommand();
            SqlDataAdapter DA;
            DataTable DT;
            DataSet DS;
            string QUERY;
            if (DBConn.State == ConnectionState.Closed)
            {
                DBConn.Open();
            }

            DBCmd.Connection = DBConn;
            DBCmd.CommandText = " UPDATE Da_User set LocationLat=@Da_LocationLat , LocationLon=@Da_LocationLon  where ID=@Da_UserIdForLocation ";
            DBCmd.Parameters.AddWithValue("Da_LocationLat", DriverLat);
            DBCmd.Parameters.AddWithValue("Da_LocationLon", DriverLon);
            DBCmd.Parameters.AddWithValue("Da_UserIdForLocation", DriverId);
            int Da_Affected = DBCmd.ExecuteNonQuery();


            DBCmd.CommandText = " UPDATE DaOrderRequest SET DriverLat=@Da_DriverLat,DriverLon=@Da_DriverLon WHERE OrderId=@Da_OrderId";
            DBCmd.Parameters.AddWithValue("Da_DriverLat", DriverLat);
            DBCmd.Parameters.AddWithValue("Da_DriverLon", DriverLon);
            DBCmd.Parameters.AddWithValue("Da_OrderId", OrderId);
            int Da_Affected2 = DBCmd.ExecuteNonQuery();


            string da_message = "";
            if (Da_Affected2 >= 1)
            {
                da_message = "Location Set Lat:" + DriverLat + " Lon:" + DriverLon;
            }
            else if (Da_Affected2 <= 0)
            {
                da_message = "Location Not Set";
            }


            DBConn.Close();
            return da_message;


        }

        [WebInvoke(Method = "POST",
                   ResponseFormat = WebMessageFormat.Json,
                   RequestFormat = WebMessageFormat.Json,
                   UriTemplate = "GetDriverLocationOrder")]
        public string GetDriverLocationOrder()
        {
            string JSONstring = OperationContext.Current.RequestContext.RequestMessage.ToString();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(JSONstring);
            int OrderId = Convert.ToInt32(xmlDoc.GetElementsByTagName("OrderId")[0].InnerText);
            int CustomerId = Convert.ToInt32(xmlDoc.GetElementsByTagName("CustomerId")[0].InnerText);


            List<SayurboxOrderRequestExchange> list = new List<SayurboxOrderRequestExchange>();
            SqlConnection DBConn = new SqlConnection("Server=127.0.0.1;Database=Coba_Sayurbox_Db;UID=sa;PWD=P4ssw0rD;");

            SqlCommand DBCmd = new SqlCommand();
            SqlDataReader DBDR;
            SqlDataAdapter DA;
            DataTable DT;
            DataSet DS;
            string QUERY;
            if (DBConn.State == ConnectionState.Closed)
            {
                DBConn.Open();
            }

            DBCmd.Connection = DBConn;
            DBCmd.CommandText = "SELECT * from DaOrderRequest where OrderId=@Da_OrderId AND CustomerId=@Da_CustomerId";
            DBCmd.Parameters.AddWithValue("Da_OrderId", OrderId);
            DBCmd.Parameters.AddWithValue("Da_CustomerId", CustomerId);
            DBDR = DBCmd.ExecuteReader();
            string da_location = "";
            while (DBDR.Read())
            {
                da_location += "Driver Location is LAT:" + DBDR["DriverLat"].ToString() + " LON:" + DBDR["DriverLon"].ToString();
            }
            
            DBDR.Close();
            DBConn.Close();

            return da_location;


        }

        [WebInvoke(Method = "POST",
                   ResponseFormat = WebMessageFormat.Json,
                   RequestFormat = WebMessageFormat.Json,
                   UriTemplate = "DriverStartTrip")]
        public string DriverStartTrip()
        {

            string JSONstring = OperationContext.Current.RequestContext.RequestMessage.ToString();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(JSONstring);
            int OrderId = Convert.ToInt32(xmlDoc.GetElementsByTagName("OrderId")[0].InnerText);
            int CustomerId = Convert.ToInt32(xmlDoc.GetElementsByTagName("CustomerId")[0].InnerText);
            int DriverId = Convert.ToInt32(xmlDoc.GetElementsByTagName("DriverId")[0].InnerText);
            
            SqlConnection DBConn = new SqlConnection("Server=127.0.0.1;Database=Coba_Sayurbox_Db;UID=sa;PWD=P4ssw0rD;");

            SqlCommand DBCmd = new SqlCommand();
            SqlDataAdapter DA;
            DataTable DT;
            DataSet DS;
            string QUERY;
            if (DBConn.State == ConnectionState.Closed)
            {
                DBConn.Open();
            }

            DBCmd.Connection = DBConn;
            DBCmd.CommandText = " UPDATE DaOrderRequest set Status='Ongoing' where OrderId=@Da_OrderId AND CustomerId=@Da_CustomerId AND DriverId=@Da_DriverId";
            DBCmd.Parameters.AddWithValue("Da_OrderId", OrderId);
            DBCmd.Parameters.AddWithValue("Da_CustomerId", CustomerId);
            DBCmd.Parameters.AddWithValue("Da_DriverId", DriverId);
            int Da_Affected = DBCmd.ExecuteNonQuery();


            
            string da_message = "";
            if (Da_Affected >= 1)
            {
                da_message = "Driver Has Accepted Your Order";
            }
            else if (Da_Affected <= 0)
            {
                da_message = "Accept roder failed";
            }


            DBConn.Close();
            return da_message;


        }

        [WebInvoke(Method = "POST",
                   ResponseFormat = WebMessageFormat.Json,
                   RequestFormat = WebMessageFormat.Json,
                   UriTemplate = "DriverEndTrip")]
        public string DriverEndTrip()
        {

            string JSONstring = OperationContext.Current.RequestContext.RequestMessage.ToString();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(JSONstring);
            int OrderId = Convert.ToInt32(xmlDoc.GetElementsByTagName("OrderId")[0].InnerText);
            int CustomerId = Convert.ToInt32(xmlDoc.GetElementsByTagName("CustomerId")[0].InnerText);
            int DriverId = Convert.ToInt32(xmlDoc.GetElementsByTagName("DriverId")[0].InnerText);

            double DistanceInKm = Convert.ToDouble(xmlDoc.GetElementsByTagName("DistanceInKm")[0].InnerText);
            double TimeInMinutes = Convert.ToDouble(xmlDoc.GetElementsByTagName("TimeInMinutes")[0].InnerText);

            SqlConnection DBConn = new SqlConnection("Server=127.0.0.1;Database=Coba_Sayurbox_Db;UID=sa;PWD=P4ssw0rD;");

            SqlCommand DBCmd = new SqlCommand();
            SqlDataAdapter DA;
            DataTable DT;
            DataSet DS;
            string QUERY;
            if (DBConn.State == ConnectionState.Closed)
            {
                DBConn.Open();
            }

            DBCmd.Connection = DBConn;
            DBCmd.CommandText = " UPDATE DaOrderRequest set Status='Complete',DistanceInKm=@Da_DistanceInKm,TimeInMinutes=@Da_TimeInMinutes where OrderId=@Da_OrderId AND CustomerId=@Da_CustomerId AND DriverId=@Da_DriverId";
            DBCmd.Parameters.AddWithValue("Da_OrderId", OrderId);
            DBCmd.Parameters.AddWithValue("Da_CustomerId", CustomerId);
            DBCmd.Parameters.AddWithValue("Da_DriverId", DriverId);

            DBCmd.Parameters.AddWithValue("Da_DistanceInKm", DistanceInKm);
            DBCmd.Parameters.AddWithValue("Da_TimeInMinutes", TimeInMinutes);
            int Da_Affected = DBCmd.ExecuteNonQuery();



            string da_message = "";
            if (Da_Affected >= 1)
            {
                da_message = "Trip Ended Order Complete";
            }
            else if (Da_Affected <= 0)
            {
                da_message = "Trip complete failed";
            }


            DBConn.Close();
            return da_message;


        }
        


        
    }



    public class DaInventory
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class SayurboxInventory
    {
        public int Id { get; set; }
        public string SayurName { get; set; }
        public int Qty { get; set; }
    }

    public class SayurboxInventoryInput
    {
        public int Id { get; set; }
        public string SayurName { get; set; }
        public int Qty { get; set; }
        public int CustomerId { get; set; }
    }

    public class SayurboxOrderRequest
    {
        public int CustomerId { get; set; }
        public int DriverId { get; set; }
        public double CustomerLat { get; set; }
        public double CustomerLon { get; set; }
        public double DriverLat { get; set; }
        public double DriverLon { get; set; }
        public string DriverAccept { get; set; }
        public string CustomerAccept { get; set; }
        public string OrderInventoryCode { get; set; }
        public string Status { get; set; }
        public double DistanceInKm { get; set; }
        public double TimeInMinutes { get; set; }
    }

    public class SayurboxOrderRequestExchange
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public int DriverId { get; set; }
        public double CustomerLat { get; set; }
        public double CustomerLon { get; set; }
        public double DriverLat { get; set; }
        public double DriverLon { get; set; }
        public string DriverAccept { get; set; }
        public string CustomerAccept { get; set; }
        public string OrderInventoryCode { get; set; }
        public string Status { get; set; }
    }
}
