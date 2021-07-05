using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Widget;
using Com.OneSignal.Abstractions;
using WoWonder.Activities.Tabbes;
using OSNotification = Com.OneSignal.Abstractions.OSNotification;
using OSNotificationPayload = Com.OneSignal.Abstractions.OSNotificationPayload;
using WoWonder.Helpers.Model;
using WoWonder.Helpers.Utils;
using WoWonder.SQLite;

namespace WoWonder.Library.OneSignal
{
    public static class MsgOneSignalNotification
    {
        //Force your app to Register notification additionalData without loading it from server (For Best Result) 
        private static string UserId, PostId, PageId, GroupId, EventId, Type;

        public static void RegisterNotificationDevice()
        {
            try
            {
                if (UserDetails.NotificationPopup)
                {
                    if (!string.IsNullOrEmpty(AppSettings.MsgOneSignalAppId) || !string.IsNullOrWhiteSpace(AppSettings.MsgOneSignalAppId))
                    {
                        Com.OneSignal.OneSignal.Current.StartInit(AppSettings.MsgOneSignalAppId)
                            .InFocusDisplaying(OSInFocusDisplayOption.Notification)
                            .HandleNotificationReceived(HandleNotificationReceived)
                            .HandleNotificationOpened(HandleNotificationOpened)
                            .EndInit();
                        Com.OneSignal.OneSignal.Current.IdsAvailable(IdsAvailable);
                        Com.OneSignal.OneSignal.Current.RegisterForPushNotifications();
                        Com.OneSignal.OneSignal.Current.SetSubscription(true);
                        AppSettings.ShowNotification = true;
                    }
                }
                else
                {
                    Un_RegisterNotificationDevice();
                }
            }
            catch (Exception ex)
            {
                Methods.DisplayReportResultTrack(ex);
            }
        }

        public static void Un_RegisterNotificationDevice()
        {
            try
            {
                Com.OneSignal.OneSignal.Current.SetSubscription(false);
                AppSettings.ShowNotification = false;
            }
            catch (Exception ex)
            {
                Methods.DisplayReportResultTrack(ex);
            }
        }

        private static void IdsAvailable(string userId, string pushToken)
        {
            try
            {
                UserDetails.DeviceMsgId = userId;
            }
            catch (Exception ex)
            {
                Methods.DisplayReportResultTrack(ex);
            }
        }

        private static void HandleNotificationReceived(OSNotification notification)
        {
            try
            {
                OSNotificationPayload payload = notification.payload;
                Dictionary<string, object> additionalData = payload.additionalData;
                if (additionalData?.Count > 0)
                {
                    string chatType = "", IdChat = "";
                    foreach (var item in additionalData)
                    {
                        switch (item.Key)
                        {
                            case "post_id":
                                PostId = item.Value.ToString();
                                break;
                            case "user_id":
                                UserId = item.Value.ToString();
                                chatType = "user";
                                IdChat = UserId;
                                break;
                            case "page_id":
                                PageId = item.Value.ToString();
                                chatType = "page";
                                IdChat = PageId + UserId;
                                break;
                            case "group_id":
                                GroupId = item.Value.ToString();
                                chatType = "group";
                                IdChat = GroupId;
                                break;
                            case "event_id":
                                EventId = item.Value.ToString();
                                break;
                            case "type":
                                Type = item.Value.ToString();
                                break;
                        }
                    }

                    if (!string.IsNullOrEmpty(IdChat))
                    {
                        if (ListUtils.MuteList.Count == 0)
                        {
                            var sqLiteDatabase = new SqLiteDatabase();
                            ListUtils.MuteList = sqLiteDatabase.Get_MuteList();
                        }

                        var check = ListUtils.MuteList.FirstOrDefault(a => a.ChatId == IdChat && a.ChatType == chatType);
                        if (check != null)
                        {
                            notification.shown = false;
                            notification.displayType = OSNotification.DisplayType.None;
                            Com.OneSignal.OneSignal.Current.ClearAndroidOneSignalNotifications();
                        }
                    }
                }

                string message = payload.body;
                if (message.Contains("call") || message.Contains("Calling"))
                {
                    notification.shown = false;
                    notification.displayType = OSNotification.DisplayType.None;
                    Com.OneSignal.OneSignal.Current.ClearAndroidOneSignalNotifications();
                }
            }
            catch (Exception ex)
            {
                ToastUtils.ShowToast(Application.Context, ex.ToString(), ToastLength.Long); //Allen
                Methods.DisplayReportResultTrack(ex);
            }
        }

        private static void HandleNotificationOpened(OSNotificationOpenedResult result)
        {
            try
            {
                OSNotificationPayload payload = result.notification.payload;
                Dictionary<string, object> additionalData = payload.additionalData;
                //string message = payload.body; 
                string actionId = result.action.actionID;

                if (additionalData?.Count > 0)
                {
                    foreach (var item in additionalData)
                    {
                        switch (item.Key)
                        {
                            case "post_id":
                                PostId = item.Value.ToString();
                                break;
                            case "user_id":
                                UserId = item.Value.ToString();
                                break;
                            case "page_id":
                                PageId = item.Value.ToString();
                                break;
                            case "group_id":
                                GroupId = item.Value.ToString();
                                break;
                            case "event_id":
                                EventId = item.Value.ToString();
                                break;
                            case "type":
                                Type = item.Value.ToString();
                                break;
                        }
                    }

                    Intent intent = new Intent(Application.Context, typeof(TabbedMainActivity));
                    intent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
                    intent.AddFlags(ActivityFlags.SingleTop);
                    intent.SetAction(Intent.ActionView);
                    intent.PutExtra("userId", UserId);
                    intent.PutExtra("PostId", PostId);
                    intent.PutExtra("PageId", PageId);
                    intent.PutExtra("GroupId", GroupId);
                    intent.PutExtra("EventId", EventId);
                    intent.PutExtra("type", Type);
                    intent.PutExtra("Notifier", "Chat");
                    Application.Context.StartActivity(intent);

                    if (additionalData.ContainsKey("discount"))
                    {
                        // Take user to your store..
                    }
                }
                if (actionId != null)
                {
                    // actionSelected equals the id on the button the user pressed.
                    // actionSelected will equal "__DEFAULT__" when the notification itself was tapped when buttons were present. 
                }
            }
            catch (Exception ex)
            {
                Methods.DisplayReportResultTrack(ex);
            }
        }
    }
}