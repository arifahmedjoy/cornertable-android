﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Android.Content;
using Android.OS;
using Android.Widget;
using Iceteck.SiliCompressorrLib;
using Java.Net;
using WoWonder.Activities.Chat.Adapters;
using WoWonder.Activities.Chat.GroupChat;
using WoWonder.Activities.Chat.MsgTabbes;
using WoWonder.Helpers.Model;
using WoWonder.Helpers.Utils;
using WoWonderClient.Classes.GroupChat;
using WoWonderClient.Classes.Message;
using WoWonderClient.Requests;
using File = Java.IO.File;
using MessageData = WoWonderClient.Classes.Message.MessageData;

namespace WoWonder.Helpers.Controller
{
    public static class GroupMessageController
    {
        //############# DONT'T MODIFY HERE ############# 
        private static ChatObject GroupData;
        private static GroupChatWindowActivity MainWindowActivity;
        private static MsgTabbedMainActivity GlobalContext;

        //========================= Functions ========================= 
        public static async Task SendMessageTask(GroupChatWindowActivity windowActivity, string id, string messageId, string text = "", string contact = "", string pathFile = "", string imageUrl = "", string stickerId = "", string gifUrl = "", string lat = "", string lng = "", string replyId = "")
        {
            try
            {
                MainWindowActivity = windowActivity;
                if (windowActivity.GroupData != null)
                    GroupData = windowActivity.GroupData;

                GlobalContext = MsgTabbedMainActivity.GetInstance();

                if (AppSettings.EnableVideoCompress && Methods.AttachmentFiles.Check_FileExtension(pathFile) == "Video")
                {
                    File destinationPath = new File(Methods.Path.FolderDcimVideo + "/Compressor");

                    if (!Directory.Exists(destinationPath.Path))
                        Directory.CreateDirectory(destinationPath.Path);

                    await Task.Factory.StartNew(() => new VideoCompressAsyncTask(windowActivity, id, messageId, text, pathFile).Execute("false", pathFile, destinationPath.Path));
                }
                else
                {
                    StartApiService(id, messageId, text, contact, pathFile, imageUrl, stickerId, gifUrl, lat, lng, replyId);
                }

                await Task.Delay(0);
            }
            catch (Exception ex)
            {
                Methods.DisplayReportResultTrack(ex);
            }
        }

        private static void StartApiService(string id, string messageId, string text = "", string contact = "", string pathFile = "", string imageUrl = "", string stickerId = "", string gifUrl = "", string lat = "", string lng = "", string replyId = "")
        {
            if (!Methods.CheckConnectivity())
                ToastUtils.ShowToast(MainWindowActivity, MainWindowActivity.GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short);
            else
                PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => SendMessage(id, messageId, text, contact, pathFile, imageUrl, stickerId, gifUrl, lat, lng, replyId) });
        }

        private static async Task SendMessage(string id, string messageId, string text = "", string contact = "", string pathFile = "", string imageUrl = "", string stickerId = "", string gifUrl = "", string lat = "", string lng = "", string replyId = "")
        {
            var (apiStatus, respond) = await RequestsAsync.GroupChat.Send_MessageToGroupChatAsync(id, messageId, text, contact, pathFile, imageUrl, stickerId, gifUrl, lat, lng, replyId);
            if (apiStatus == 200)
            {
                if (respond is GroupSendMessageObject result)
                {
                    UpdateLastIdMessage(result.Data);
                }
            }
            else Methods.DisplayReportResult(MainWindowActivity, respond);
        }

        private static void UpdateLastIdMessage(List<MessageData> chatMessages)
        {
            try
            {
                MessageData messageInfo = chatMessages?.FirstOrDefault();
                if (messageInfo != null)
                {
                    var typeModel = Holders.GetTypeModel(messageInfo);
                    if (typeModel == MessageModelType.None)
                        return;

                    var checker = MainWindowActivity?.MAdapter.DifferList?.FirstOrDefault(a => a.MesData.Id == messageInfo.MessageHashId);
                    if (checker != null)
                    {
                        var message = WoWonderTools.MessageFilter(messageInfo.ToId, messageInfo, typeModel, true);
                        message.ModelType = typeModel;

                        checker.MesData = message;
                        checker.Id = Java.Lang.Long.ParseLong(message.Id);
                        checker.TypeView = typeModel;
                         
                        #region LastChat

                        //if (AppSettings.LastChatSystem == SystemApiGetLastChat.New)
                        //{
                        //    var updaterUser = GlobalContext?.LastChatTab?.MAdapter?.LastChatsList.FirstOrDefault(a => a.LastChat?.UserId == message.ToId);
                        //    if (updaterUser != null)
                        //    {
                        //        var index = GlobalContext.LastChatTab.MAdapter.LastChatsList.IndexOf(GlobalContext.LastChatTab.MAdapter.LastChatsList.FirstOrDefault(x => x.LastChat?.GroupId == message.GroupId));
                        //        if (index > -1)
                        //        {
                        //            updaterUser.LastChat.LastMessage.LastMessageClass.Text = typeModel switch
                        //            {
                        //                MessageModelType.RightGif => GlobalContext?.GetText(Resource.String.Lbl_SendGifFile),
                        //                MessageModelType.RightText => !string.IsNullOrEmpty(message.Text) ? Methods.FunString.DecodeString(message.Text) : GlobalContext?.GetText(Resource.String.Lbl_SendMessage),
                        //                MessageModelType.RightSticker => GlobalContext?.GetText(Resource.String.Lbl_SendStickerFile),
                        //                MessageModelType.RightContact => GlobalContext?.GetText(Resource.String.Lbl_SendContactnumber),
                        //                MessageModelType.RightFile => GlobalContext?.GetText(Resource.String.Lbl_SendFile),
                        //                MessageModelType.RightVideo => GlobalContext?.GetText(Resource.String.Lbl_SendVideoFile),
                        //                MessageModelType.RightImage => GlobalContext?.GetText(Resource.String.Lbl_SendImageFile),
                        //                MessageModelType.RightAudio => GlobalContext?.GetText(Resource.String.Lbl_SendAudioFile),
                        //                MessageModelType.RightMap => GlobalContext?.GetText(Resource.String.Lbl_SendLocationFile),
                        //                _ => updaterUser.LastChat.LastMessage.LastMessageClass.Text
                        //            };

                        //            GlobalContext?.RunOnUiThread(() =>
                        //            {
                        //                try
                        //                {
                        //                    if (!updaterUser.LastMessagesUser.IsPin)
                        //                    {
                        //                        var checkPin = GlobalContext?.LastChatTab?.MAdapter.LastChatsList.LastOrDefault(o => o.LastChat != null && o.LastChat.IsPin);
                        //                        if (checkPin != null)
                        //                        {
                        //                            var toIndex = GlobalContext.LastChatTab.MAdapter.LastChatsList.IndexOf(checkPin) + 1;
                        //                            GlobalContext?.LastChatTab?.MAdapter.LastChatsList.Move(index, toIndex);
                        //                            GlobalContext?.LastChatTab?.MAdapter.NotifyItemMoved(index, toIndex);
                        //                        }
                        //                        else
                        //                        {
                        //                            if (ListUtils.GroupRequestsList.Count > 0)
                        //                            {
                        //                                GlobalContext?.LastChatTab?.MAdapter?.LastChatsList.Move(index, 0);
                        //                                GlobalContext?.LastChatTab?.MAdapter?.NotifyItemMoved(index, 0);
                        //                                GlobalContext?.LastChatTab?.MAdapter?.NotifyItemChanged(index, "WithoutBlob");
                        //                            }
                        //                            else
                        //                            {
                        //                                GlobalContext?.LastChatTab?.MAdapter?.LastChatsList.Move(index, 1);
                        //                                GlobalContext?.LastChatTab?.MAdapter?.NotifyItemMoved(index, 1);
                        //                                GlobalContext?.LastChatTab?.MAdapter?.NotifyItemChanged(index, "WithoutBlob");
                        //                            }
                        //                        }
                        //                    } 
                        //                }
                        //                catch (Exception e)
                        //                {
                        //                    Methods.DisplayReportResultTrack(e);
                        //                }
                        //            });
                        //        }
                        //    }
                        //    else
                        //    {
                        //        GlobalContext?.RunOnUiThread(() =>
                        //        {
                        //            try
                        //            {
                        //                if (GroupData != null)
                        //                { 
                        //                    if (ListUtils.GroupRequestsList.Count > 0)
                        //                    {
                        //                        GlobalContext?.LastChatTab.MAdapter.LastChatsList.Insert(0, new Classes.LastChatsClass()
                        //                        {
                        //                            LastChat = GroupData,
                        //                            Type = Classes.ItemType.LastChatNewV
                        //                        });

                        //                        GlobalContext?.LastChatTab.MAdapter.NotifyItemInserted(0);
                        //                        GlobalContext?.LastChatTab.MRecycler.ScrollToPosition(0);
                        //                    }
                        //                    else
                        //                    {
                        //                        GlobalContext?.LastChatTab.MAdapter.LastChatsList.Insert(1, new Classes.LastChatsClass()
                        //                        {
                        //                            LastChat = GroupData,
                        //                            Type = Classes.ItemType.LastChatNewV
                        //                        });

                        //                        GlobalContext?.LastChatTab.MAdapter.NotifyItemInserted(1);
                        //                        GlobalContext?.LastChatTab.MRecycler.ScrollToPosition(1);
                        //                    } 
                        //                }
                        //            }
                        //            catch (Exception e)
                        //            {
                        //                Methods.DisplayReportResultTrack(e);
                        //            }
                        //        });
                        //    }
                        //}
                        //else
                        //{
                        //    var updaterUser = GlobalContext?.LastGroupChatsTab?.MAdapter?.LastChatsList.FirstOrDefault(a => a.LastChat?.UserId == message.ToId);
                        //    if (updaterUser != null)
                        //    {
                        //        var index = GlobalContext.LastGroupChatsTab.MAdapter.LastChatsList.IndexOf(GlobalContext.LastGroupChatsTab.MAdapter.LastChatsList.FirstOrDefault(x => x.LastChat?.GroupId == message.GroupId));
                        //        if (index > -1)
                        //        {
                        //            updaterUser.LastChat.LastMessage.LastMessageClass.Text = typeModel switch
                        //            {
                        //                MessageModelType.RightGif => GlobalContext?.GetText(Resource.String.Lbl_SendGifFile),
                        //                MessageModelType.RightText => !string.IsNullOrEmpty(message.Text) ? Methods.FunString.DecodeString(message.Text) : GlobalContext?.GetText(Resource.String.Lbl_SendMessage),
                        //                MessageModelType.RightSticker => GlobalContext?.GetText(Resource.String.Lbl_SendStickerFile),
                        //                MessageModelType.RightContact => GlobalContext?.GetText(Resource.String.Lbl_SendContactnumber),
                        //                MessageModelType.RightFile => GlobalContext?.GetText(Resource.String.Lbl_SendFile),
                        //                MessageModelType.RightVideo => GlobalContext?.GetText(Resource.String.Lbl_SendVideoFile),
                        //                MessageModelType.RightImage => GlobalContext?.GetText(Resource.String.Lbl_SendImageFile),
                        //                MessageModelType.RightAudio => GlobalContext?.GetText(Resource.String.Lbl_SendAudioFile),
                        //                MessageModelType.RightMap => GlobalContext?.GetText(Resource.String.Lbl_SendLocationFile),
                        //                _ => updaterUser.LastChat.LastMessage.LastMessageClass.Text
                        //            };

                        //            GlobalContext?.RunOnUiThread(() =>
                        //            {
                        //                try
                        //                {
                        //                    if (!updaterUser.LastMessagesUser.IsPin)
                        //                    {
                        //                        var checkPin = GlobalContext?.LastGroupChatsTab?.MAdapter.LastChatsList.LastOrDefault(o => o.LastChat != null && o.LastChat.IsPin);
                        //                        if (checkPin != null)
                        //                        {
                        //                            var toIndex = GlobalContext.LastGroupChatsTab.MAdapter.LastChatsList.IndexOf(checkPin) + 1;
                        //                            GlobalContext?.LastGroupChatsTab?.MAdapter.LastChatsList.Move(index, toIndex);
                        //                            GlobalContext?.LastGroupChatsTab?.MAdapter.NotifyItemMoved(index, toIndex);
                        //                        }
                        //                        else
                        //                        {
                        //                            if (ListUtils.GroupRequestsList.Count > 0)
                        //                            {
                        //                                GlobalContext?.LastGroupChatsTab?.MAdapter?.LastChatsList.Move(index, 0);
                        //                                GlobalContext?.LastGroupChatsTab?.MAdapter?.NotifyItemMoved(index, 0);
                        //                                GlobalContext?.LastGroupChatsTab?.MAdapter?.NotifyItemChanged(index, "WithoutBlob");
                        //                            }
                        //                            else
                        //                            {
                        //                                GlobalContext?.LastGroupChatsTab?.MAdapter?.LastChatsList.Move(index, 1);
                        //                                GlobalContext?.LastGroupChatsTab?.MAdapter?.NotifyItemMoved(index, 1);
                        //                                GlobalContext?.LastGroupChatsTab?.MAdapter?.NotifyItemChanged(index, "WithoutBlob");
                        //                            }
                        //                        }
                        //                    } 
                        //                }
                        //                catch (Exception e)
                        //                {
                        //                    Methods.DisplayReportResultTrack(e);
                        //                }
                        //            });
                        //        }
                        //    }
                        //    else
                        //    {
                        //        //GlobalContext?.RunOnUiThread(() =>
                        //        //{
                        //        //    try
                        //        //    {
                        //        //        GlobalContext?.LastGroupChatsTab?.MAdapter.LastGroupList.Insert(0, GroupData);
                        //        //        GlobalContext?.LastGroupChatsTab?.MAdapter.NotifyItemInserted(0);
                        //        //        GlobalContext?.LastGroupChatsTab?.MRecycler.ScrollToPosition(GlobalContext.LastGroupChatsTab.MAdapter.LastGroupList.IndexOf(GroupData));
                        //        //    }
                        //        //    catch (Exception e)
                        //        //    {
                        //        //        Methods.DisplayReportResultTrack(e);
                        //        //    }
                        //        //});
                        //    }
                        //}

                        #endregion

                        GlobalContext?.RunOnUiThread(() =>
                        {
                            try
                            {
                                //Update data RecyclerView Messages.
                                if (message.ModelType != MessageModelType.RightSticker || message.ModelType != MessageModelType.RightImage || message.ModelType != MessageModelType.RightMap || message.ModelType != MessageModelType.RightVideo)
                                    MainWindowActivity.Update_One_Messages(checker.MesData);

                                if (UserDetails.SoundControl)
                                    Methods.AudioRecorderAndPlayer.PlayAudioFromAsset("Popup_SendMesseges.mp3");
                            }
                            catch (Exception e)
                            {
                                Methods.DisplayReportResultTrack(e);
                            }
                        });
                    }

                    GroupData = null!;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private class VideoCompressAsyncTask : AsyncTask<string, string, string>
        {
            private readonly Context MContext;
            private readonly string Id;
            private readonly string MessageHashId;
            private readonly string Text;
            private string FilePath;
            public VideoCompressAsyncTask(Context context, string id, string messageHashId, string text, string filePath)
            {
                try
                {
                    MContext = context;
                    Id = id;
                    MessageHashId = messageHashId;
                    Text = text;
                    FilePath = filePath;
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e);
                }
            }

            protected override string RunInBackground(params string[] paths)
            {
                string filePath = null!;
                try
                {
                    //This bellow is just a temporary solution to test that method call works
                    var b = bool.Parse(paths[0]);
                    if (b)
                    {
                        filePath = SiliCompressor.With(MContext).CompressVideo(paths[1], paths[2]);
                    }
                    else
                    {
                        Android.Net.Uri videoContentUri = Android.Net.Uri.Parse(paths[1]);

                        // Example using the bitrate and video size parameters = >> filePath = SiliCompressor.with(mContext).compressVideo(videoContentUri, paths[2], 1280,720,1500000);*/
                        filePath = SiliCompressor.With(MContext).CompressVideo(videoContentUri?.ToString(), paths[2]);
                    }
                }
                catch (URISyntaxException e)
                {
                    Methods.DisplayReportResultTrack(e);
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e);
                }
                return filePath;
            }

            protected override void OnPostExecute(string compressedFilePath)
            {
                try
                {
                    base.OnPostExecute(compressedFilePath);

                    File imageFile = new File(compressedFilePath);
                    //float length = imageFile.Length() / 1024f; // Size in KB
                    //string value;
                    //if (length >= 1024)
                    //    value = length / 1024f + " MB";
                    //else
                    //    value = length + " KB";

                    //Methods.DisplayReportResultTrack("Name: " + imageFile.Name + " Size: " + value);

                    //Methods.DisplayReportResultTrack("Silicompressor Path: " + compressedFilePath);

                    var attach = imageFile.Path;
                    if (attach != null)
                    {
                        FilePath = imageFile.Path;
                        StartApiService(Id, MessageHashId, Text, "", FilePath);
                    }
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e);
                }
            }
        }
    }
}