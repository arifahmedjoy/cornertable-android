using System;
using System.Net;
using System.Net.Security;
using Newtonsoft.Json.Linq;
using SocketIOClient;
using SocketIOClient.WebSocketClient;
using WoWonder.Helpers.Utils;
using WoWonderClient;

namespace WoWonder.SocketSystem
{
    public class WoSocketHandler
    {  
        private SocketIO Client;
        private bool IsStarted; 
        private bool IsSocketWithSsl;
         
        public void InitStart()
        {
            try
            {
                Client = new SocketIO(InitializeWoWonder.WebsiteUrl + "/:" + ListUtils.SettingsSiteList.NodejsPort + "/");
                
                UseSocketWithSsl();

                WoSocketEvents events = new WoSocketEvents();
                events.InitEvents(Client);

                //Add all On_ functions here 
                Socket_On_Alert(Client);
                Socket_On_Private_Message(Client);
                Socket_On_Private_Message_Page(Client);
                Socket_On_User_Status_Change(Client);
                Socket_On_TypingEvent(Client);
            }
            catch (OperationCanceledException e)
            {
                IsStarted = false;
                Console.WriteLine(e);
            }
            catch (Exception ex)
            {
                IsStarted = false;
                Methods.DisplayReportResultTrack(ex);
            }
        }

        public void UseSocketWithSsl()
        {
            try
            {
                if (IsSocketWithSsl)
                {
                    //********* For Proxy settings ***********
                    //var proxy = new System.Net.WebProxy("http://example.com");
                    //proxy.Credentials = new NetworkCredential("username", "password");
                    //********************

                    if (Client.Socket is ClientWebSocket websocket)
                    {
                        websocket.Config = options =>
                        { 
                            options.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) =>
                            {
                                Console.WriteLine("SslPolicyErrors: " + sslPolicyErrors);
                                if (sslPolicyErrors == SslPolicyErrors.None)
                                {
                                    return true;
                                }
                                return true;
                            };

                            // Set Proxy
                            //options.Proxy = proxy;
                        }; 
                    }
                    else
                    { 
                        ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) =>
                        {
                            if (sslPolicyErrors == SslPolicyErrors.None)
                            {
                                return true;
                            }
                            Console.WriteLine(sslPolicyErrors);
                            return false;
                        };

                        //********* For Proxy settings ***********
                        //websocket.Config = options => options.Proxy = proxy;
                        //********************
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e); 
            }
        }

        public void Emit_Join(string username, string acessToken)
        {
            try
            {
                Client.OnConnected += async (sender, e) =>
                {
                    JObject value = new JObject {["username"] = username, ["user_id"] = acessToken};

                    await Client.EmitAsync("join", response =>
                    {
                        var result = response;
                        Console.WriteLine(result);
                    }, value);
                };

                Client.ConnectAsync();
            }
            catch (Exception ex)
            {
                Methods.DisplayReportResultTrack(ex);
            }
        }

        //set type text
        public async void EmitAsync_TypingEvent(string recipientId, string acessToken)
        {
            try
            {
                JObject value = new JObject {["recipient_id"] = recipientId, ["user_id"] = acessToken};

                await Client.EmitAsync("typing", response =>
                {
                    var result = response.GetValue();
                    Console.WriteLine(result);
                }, value); 
            }
            catch (Exception ex)
            {
                Methods.DisplayReportResultTrack(ex);
            }
        }

        //Send Message text
        public async void EmitAsync_SendMessage(string toId, string acessToken, string username, string msg, string color, bool isSticker)
        {
            try
            {
                JObject value = new JObject
                {
                    ["to_id"] = toId,
                    ["from_id"] = acessToken,
                    ["username"] = username,
                    ["msg"] = msg,
                    ["color"] = color,
                    ["isSticker"] = false
                };

                await Client.EmitAsync("private_message", response =>
                {
                    var result = response.GetValue();
                    Console.WriteLine(result);
                }, value);
            }
            catch (Exception ex)
            {
                Methods.DisplayReportResultTrack(ex);
            }
        }

        //Get New Message 
        public static void Socket_On_Private_Message(SocketIO client)
        {
            try
            {
                client.On("private_message", response => {
                    var result = response;

                    Console.WriteLine(result);
                });
            }
            catch (Exception ex)
            {
                Methods.DisplayReportResultTrack(ex);
            }
        }
         
        public static void Socket_On_Alert(SocketIO client)
        {
            try
            {
                client.On("alert", response => {
                    var result = response;

                    Console.WriteLine(result);
                });
            }
            catch (Exception ex)
            {
                Methods.DisplayReportResultTrack(ex);
            }
        }

        //Get New Message 
        public static void Socket_On_Private_Message_Page(SocketIO client)
        {
            try
            {
                client.On("private_message_page", response => {
                    var result = response;

                    Console.WriteLine(result);
                });
            }
            catch (Exception ex)
            {
                Methods.DisplayReportResultTrack(ex);
            }
        }



        //Send Message text
        public async void EmitAsync_SendMessagepage(string toId, string acessToken, string username, string msg, string color, bool isSticker)
        {
            try
            {
                JObject value = new JObject
                {
                    ["to_id"] = toId,
                    ["from_id"] = acessToken,
                    ["username"] = username,
                    ["msg"] = msg,
                    ["color"] = color,
                    ["isSticker"] = false
                };

                await Client.EmitAsync("private_message_page", response =>
                {
                    var result = response.GetValue();
                    Console.WriteLine(result);
                }, value);
            }
            catch (Exception ex)
            {
                Methods.DisplayReportResultTrack(ex);
            }
        }


        //Get new user in last user messages 
        public static void Socket_On_User_Status_Change(SocketIO client)
        {
            try
            {
                client.On("user_status_change", response => {
                    var result = response;

                    Console.WriteLine(result);
                });
            }
            catch (Exception ex)
            {
                Methods.DisplayReportResultTrack(ex);
            }
        }

        //Check who is Typing now
        public void Socket_On_TypingEvent(SocketIO client)
        {
            try
            {
                client.On("typing", response => {
                    var result = response;

                    Console.WriteLine(result);
                }); 
            }
            catch (Exception ex)
            {
                Methods.DisplayReportResultTrack(ex);
            }
        }
           
        //online
        public void Socket_On_loggedintEvent(SocketIO client)
        {
            try
            {
                client.On("onloggedin", response => {
                    var result = response;

                    Console.WriteLine(result);
                }); 
            }
            catch (Exception ex)
            {
                Methods.DisplayReportResultTrack(ex);
            }
        }
        
        //offline
        public void Socket_On_loggedoutEvent(SocketIO client)
        {
            try
            {
                client.On("onloggedout", response => {
                    var result = response;

                    Console.WriteLine(result);
                }); 
            }
            catch (Exception ex)
            {
                Methods.DisplayReportResultTrack(ex);
            }
        }

    }
} 