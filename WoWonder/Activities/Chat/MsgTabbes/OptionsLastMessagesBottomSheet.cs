using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MaterialDialogsCore;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using Google.Android.Material.BottomSheet;
using Newtonsoft.Json;
using WoWonder.Activities.Chat.Call.Agora;
using WoWonder.Activities.Chat.Call.Twilio;
using WoWonder.Activities.Chat.GroupChat;
using WoWonder.Helpers.Controller;
using WoWonder.Helpers.Fonts;
using WoWonder.Helpers.Model;
using WoWonder.Helpers.Utils;
using WoWonder.SQLite;
using WoWonderClient.Classes.Global;
using WoWonderClient.Classes.GroupChat;
using WoWonderClient.Classes.Message;
using WoWonderClient.Requests;
using Exception = System.Exception;

namespace WoWonder.Activities.Chat.MsgTabbes
{
    public class OptionsLastMessagesBottomSheet : BottomSheetDialogFragment, MaterialDialog.IListCallback
    {
        #region Variables Basic

        private MsgTabbedMainActivity GlobalContext;
        //wael  add Mute call
        private RelativeLayout ArchiveLayout, DeleteLayout, PinLayout, MuteLayout, ReadLayout, BlockLayout, CallLayout, ProfileLayout, GroupInfoLayout, ExitGroupLayout, AddMembersLayout;
        private TextView ArchiveIcon, DeleteIcon, PinIcon, MuteIcon, ReadIcon, BlockIcon, CallIcon, ProfileIcon, GroupInfoIcon, ExitGroupIcon, AddMembersIcon;
        private TextView ArchiveText, DeleteText, PinText, MuteText, ReadText, BlockText, CallText, ProfileText, GroupInfoText, ExitGroupText, AddMembersText;

        private string Type;
        private ChatObject DataChatObjectN;
        private GetUsersListObject.User DataUserObjectOld;
        private PageClass PageClassObject;

        #endregion

        #region General

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your fragment here
            GlobalContext = (MsgTabbedMainActivity)Activity;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                Context contextThemeWrapper = AppSettings.SetTabDarkTheme ? new ContextThemeWrapper(Activity, Resource.Style.MyTheme_Dark_Base) : new ContextThemeWrapper(Activity, Resource.Style.MyTheme_Base);
                // clone the inflater using the ContextThemeWrapper
                LayoutInflater localInflater = inflater.CloneInContext(contextThemeWrapper);

                View view = localInflater?.Inflate(Resource.Layout.BottomSheetLastMessagesLayout, container, false);
                return view;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return null!;
            }
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            try
            {
                base.OnViewCreated(view, savedInstanceState);
                InitComponent(view);

                LoadDataChat();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        public override void OnLowMemory()
        {
            try
            {
                GC.Collect(GC.MaxGeneration);
                base.OnLowMemory();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Functions

        private void InitComponent(View view)
        {
            try
            {
                ArchiveLayout = view.FindViewById<RelativeLayout>(Resource.Id.ArchiveLayout);
                ArchiveIcon = view.FindViewById<TextView>(Resource.Id.ArchiveIcon);
                ArchiveText = view.FindViewById<TextView>(Resource.Id.ArchiveText);
                ArchiveLayout.Click += ArchiveLayoutOnClick;

                DeleteLayout = view.FindViewById<RelativeLayout>(Resource.Id.DeleteLayout);
                DeleteIcon = view.FindViewById<TextView>(Resource.Id.DeleteIcon);
                DeleteText = view.FindViewById<TextView>(Resource.Id.DeleteText);
                DeleteLayout.Click += DeleteLayoutOnClick;

                PinLayout = view.FindViewById<RelativeLayout>(Resource.Id.PinLayout);
                PinIcon = view.FindViewById<TextView>(Resource.Id.PinIcon);
                PinText = view.FindViewById<TextView>(Resource.Id.PinText);
                PinLayout.Click += PinLayoutOnClick;

                MuteLayout = view.FindViewById<RelativeLayout>(Resource.Id.MuteLayout);
                MuteIcon = view.FindViewById<TextView>(Resource.Id.MuteIcon);
                MuteText = view.FindViewById<TextView>(Resource.Id.MuteText);
                MuteLayout.Click += MuteLayoutOnClick;

                ReadLayout = view.FindViewById<RelativeLayout>(Resource.Id.ReadLayout);
                ReadIcon = view.FindViewById<TextView>(Resource.Id.ReadIcon);
                ReadText = view.FindViewById<TextView>(Resource.Id.ReadText);
                ReadLayout.Click += ReadLayoutOnClick;

                BlockLayout = view.FindViewById<RelativeLayout>(Resource.Id.BlockLayout);
                BlockIcon = view.FindViewById<TextView>(Resource.Id.BlockIcon);
                BlockText = view.FindViewById<TextView>(Resource.Id.BlockText);
                BlockLayout.Click += BlockLayoutOnClick;

                CallLayout = view.FindViewById<RelativeLayout>(Resource.Id.CallLayout);
                CallIcon = view.FindViewById<TextView>(Resource.Id.CallIcon);
                CallText = view.FindViewById<TextView>(Resource.Id.CallText);
                CallLayout.Click += CallLayoutOnClick;

                ProfileLayout = view.FindViewById<RelativeLayout>(Resource.Id.ProfileLayout);
                ProfileIcon = view.FindViewById<TextView>(Resource.Id.ProfileIcon);
                ProfileText = view.FindViewById<TextView>(Resource.Id.ProfileText);
                ProfileLayout.Click += ProfileLayoutOnClick;

                GroupInfoLayout = view.FindViewById<RelativeLayout>(Resource.Id.GroupInfoLayout);
                GroupInfoIcon = view.FindViewById<TextView>(Resource.Id.GroupInfoIcon);
                GroupInfoText = view.FindViewById<TextView>(Resource.Id.GroupInfoText);
                GroupInfoLayout.Click += GroupInfoLayoutOnClick;

                ExitGroupLayout = view.FindViewById<RelativeLayout>(Resource.Id.ExitGroupLayout);
                ExitGroupIcon = view.FindViewById<TextView>(Resource.Id.ExitGroupIcon);
                ExitGroupText = view.FindViewById<TextView>(Resource.Id.ExitGroupText);
                ExitGroupLayout.Click += ExitGroupLayoutOnClick;

                AddMembersLayout = view.FindViewById<RelativeLayout>(Resource.Id.AddMembersLayout);
                AddMembersIcon = view.FindViewById<TextView>(Resource.Id.AddMembersIcon);
                AddMembersText = view.FindViewById<TextView>(Resource.Id.AddMembersText);
                AddMembersLayout.Click += AddMembersLayoutOnClick;

                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, ArchiveIcon, IonIconsFonts.Archive);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, DeleteIcon, IonIconsFonts.Trash);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeSolid, PinIcon, FontAwesomeIcon.Thumbtack);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, MuteIcon, IonIconsFonts.NotificationsOff); //Notifications
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, ReadIcon, IonIconsFonts.MailUnread); //Mail
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, BlockIcon, IonIconsFonts.Lock);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, CallIcon, IonIconsFonts.Call);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, ProfileIcon, IonIconsFonts.Contact);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, GroupInfoIcon, IonIconsFonts.InformationCircle);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, ExitGroupIcon, IonIconsFonts.Exit);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, AddMembersIcon, IonIconsFonts.PersonAdd);

                if (!AppSettings.EnableChatArchive)
                    ArchiveLayout.Visibility = ViewStates.Gone;

                if (!AppSettings.EnableChatPin)
                    PinLayout.Visibility = ViewStates.Gone;

                if (!AppSettings.EnableChatMute)
                    MuteLayout.Visibility = ViewStates.Gone;

                if (!AppSettings.EnableChatMakeAsRead)
                    ReadLayout.Visibility = ViewStates.Gone;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Event
         
        //Add Members to group
        private void AddMembersLayoutOnClick(object sender, EventArgs e)
        {
            try
            {
                Intent intent = new Intent(Activity, typeof(EditGroupChatActivity));
                intent.PutExtra("GroupObject", JsonConvert.SerializeObject(DataChatObjectN));
                intent.PutExtra("Type", "Edit");
                Activity.StartActivity(intent);

                Dismiss();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Exit Group
        private void ExitGroupLayoutOnClick(object sender, EventArgs e)
        {
            try
            {
                if (!Methods.CheckConnectivity())
                {
                    ToastUtils.ShowToast(Activity, Activity.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short);
                }
                else
                {
                    var dialog = new MaterialDialog.Builder(Activity).Theme(AppSettings.SetTabDarkTheme ? MaterialDialogsCore.Theme.Dark : MaterialDialogsCore.Theme.Light);
                    dialog.Content(GetText(Resource.String.Lbl_AreYouSureExitGroup));
                    dialog.PositiveText(GetText(Resource.String.Lbl_Exit)).OnPositive(async (materialDialog, action) =>
                    {
                        try
                        {
                            //Show a progress
                            AndHUD.Shared.Show(Activity, GetText(Resource.String.Lbl_Loading));

                            var (apiStatus, respond) = await RequestsAsync.GroupChat.ExitGroupChatAsync(DataChatObjectN.GroupId);
                            if (apiStatus == 200)
                            {
                                if (respond is AddOrRemoveUserToGroupObject result)
                                {
                                    Console.WriteLine(result.MessageData);

                                    ToastUtils.ShowToast(Activity, Activity.GetString(Resource.String.Lbl_GroupSuccessfullyLeaved), ToastLength.Short);

                                    //remove item to my Group list  
                                    if (AppSettings.LastChatSystem == SystemApiGetLastChat.New)
                                    {
                                        var adapter = GlobalContext?.LastChatTab.MAdapter;
                                        var data = adapter?.LastChatsList?.FirstOrDefault(a => a.LastChat?.GroupId == DataChatObjectN.GroupId);
                                        if (data != null)
                                        {
                                            adapter?.LastChatsList.Remove(data);
                                            adapter?.NotifyItemRemoved(adapter.LastChatsList.IndexOf(data));
                                        }
                                    }
                                    else
                                    {
                                        var adapter = GlobalContext?.LastGroupChatsTab.MAdapter;
                                        var data = adapter?.LastChatsList?.FirstOrDefault(a => a.LastChat?.GroupId == DataChatObjectN.GroupId);
                                        if (data != null)
                                        {
                                            adapter.LastChatsList.Remove(data);
                                            adapter.NotifyItemRemoved(adapter.LastChatsList.IndexOf(data));
                                        }
                                    }

                                    AndHUD.Shared.ShowSuccess(Activity);
                                }
                            }
                            else Methods.DisplayReportResult(Activity, respond);

                            AndHUD.Shared.Dismiss();

                            Dismiss();
                        }
                        catch (Exception e)
                        {
                            Methods.DisplayReportResultTrack(e);
                        }
                    });
                    dialog.NegativeText(GetText(Resource.String.Lbl_Cancel)).OnNegative(new WoWonderTools.MyMaterialDialog());
                    dialog.AlwaysCallSingleChoiceCallback();
                    dialog.Build().Show();
                } 
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Group Info (Profile)  
        private void GroupInfoLayoutOnClick(object sender, EventArgs e)
        {
            try
            {
                Intent intent = new Intent(Activity, typeof(EditGroupChatActivity));
                intent.PutExtra("GroupObject", JsonConvert.SerializeObject(DataChatObjectN));
                intent.PutExtra("Type", "Profile");
                Activity.StartActivity(intent);

                Dismiss();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //View Profile
        private void ProfileLayoutOnClick(object sender, EventArgs e)
        {
            try
            {
                if (AppSettings.LastChatSystem == SystemApiGetLastChat.New)
                    WoWonderTools.OpenProfile(Activity, DataChatObjectN.UserId, DataChatObjectN.UserData);
                else
                    WoWonderTools.OpenProfile(Activity, DataUserObjectOld.UserId, DataUserObjectOld);

                Dismiss();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Call
        private void CallLayoutOnClick(object sender, EventArgs e)
        {
            try
            { 
                if (AppSettings.EnableAudioCall && AppSettings.EnableVideoCall)
                {
                    var arrayAdapter = new List<string>();
                    var dialogList = new MaterialDialog.Builder(Context).Theme(AppSettings.SetTabDarkTheme ? MaterialDialogsCore.Theme.Dark : MaterialDialogsCore.Theme.Light);

                    if (AppSettings.EnableAudioVideoCall)
                    {
                        var dataSettings = ListUtils.SettingsSiteList;
                        if (dataSettings?.WhoCall == "pro") //just pro user can chat 
                        {
                            var dataUser = ListUtils.MyProfileList?.FirstOrDefault()?.IsPro;
                            if (dataUser == "0") // Not Pro remove call
                            {
                                //CallLayout.Visibility = ViewStates.Gone;
                                return;
                            }
                        }
                        else //all users can chat
                        {
                            if (dataSettings?.VideoChat == "0" || !AppSettings.EnableVideoCall)
                            {
                                //VideoCallButton.Visibility = ViewStates.Gone;
                            }
                            else
                            {
                                arrayAdapter.Add(Context.GetText(Resource.String.Lbl_Video_call));
                            }

                            if (dataSettings?.AudioChat == "0" || !AppSettings.EnableAudioCall)
                            {
                                //AudioCallButton.Visibility = ViewStates.Gone;
                            }
                            else
                            {
                                arrayAdapter.Add(Context.GetText(Resource.String.Lbl_Voice_call));
                            }
                        }
                    }
                    else
                    {
                        //CallLayout.Visibility = ViewStates.Gone;
                        return;
                    }

                    dialogList.Title(GetText(Resource.String.Lbl_Call));
                    //dialogList.Content(GetText(Resource.String.Lbl_Select_Type_Call));
                    dialogList.Items(arrayAdapter);
                    dialogList.NegativeText(GetText(Resource.String.Lbl_Close)).OnNegative(new WoWonderTools.MyMaterialDialog());
                    dialogList.AlwaysCallSingleChoiceCallback();
                    dialogList.ItemsCallback(this).Build().Show();
                }
                else if (AppSettings.EnableAudioCall == false && AppSettings.EnableVideoCall)
                {
                    try
                    {
                        Intent intentVideoCall = new Intent(Context, typeof(TwilioVideoCallActivity));
                        switch (AppSettings.UseLibrary)
                        {
                            case SystemCall.Agora:
                                intentVideoCall = new Intent(Context, typeof(AgoraVideoCallActivity));
                                intentVideoCall.PutExtra("type", "Agora_video_calling_start");
                                break;
                            case SystemCall.Twilio:
                                intentVideoCall = new Intent(Context, typeof(TwilioVideoCallActivity));
                                intentVideoCall.PutExtra("type", "Twilio_video_calling_start");
                                break;
                        }

                        if (AppSettings.LastChatSystem == SystemApiGetLastChat.New)
                        {
                            var callUserObject = new CallUserObject
                            {
                                UserId = DataChatObjectN.UserId,
                                Avatar = DataChatObjectN.Avatar,
                                Name = DataChatObjectN.Name,
                                Data = new CallUserObject.DataCallUser()
                            };
                            intentVideoCall.PutExtra("callUserObject", JsonConvert.SerializeObject(callUserObject));  
                        }
                        else
                        {
                            var callUserObject = new CallUserObject
                            {
                                UserId = DataUserObjectOld.UserId,
                                Avatar = DataUserObjectOld.Avatar,
                                Name = DataUserObjectOld.Name,
                                Data = new CallUserObject.DataCallUser()
                            };
                            intentVideoCall.PutExtra("callUserObject", JsonConvert.SerializeObject(callUserObject)); 
                        } 
                        Activity.StartActivity(intentVideoCall);
                        Dismiss();
                    }
                    catch (Exception exception)
                    {
                        Methods.DisplayReportResultTrack(exception);
                    }
                }
                else if (AppSettings.EnableAudioCall && AppSettings.EnableVideoCall == false)
                {
                    try
                    {
                        Intent intentVideoCall = new Intent(Context, typeof(TwilioVideoCallActivity));
                        switch (AppSettings.UseLibrary)
                        {
                            case SystemCall.Agora:
                                intentVideoCall = new Intent(Context, typeof(AgoraAudioCallActivity));
                                intentVideoCall.PutExtra("type", "Agora_audio_calling_start");
                                break;
                            case SystemCall.Twilio:
                                intentVideoCall = new Intent(Context, typeof(TwilioAudioCallActivity));
                                intentVideoCall.PutExtra("type", "Twilio_audio_calling_start");
                                break;
                        }

                        if (AppSettings.LastChatSystem == SystemApiGetLastChat.New)
                        {
                            var callUserObject = new CallUserObject
                            {
                                UserId = DataChatObjectN.UserId,
                                Avatar = DataChatObjectN.Avatar,
                                Name = DataChatObjectN.Name,
                                Data = new CallUserObject.DataCallUser()
                            };
                            intentVideoCall.PutExtra("callUserObject", JsonConvert.SerializeObject(callUserObject)); 
                        }
                        else
                        {
                            var callUserObject = new CallUserObject
                            {
                                UserId = DataUserObjectOld.UserId,
                                Avatar = DataUserObjectOld.Avatar,
                                Name = DataUserObjectOld.Name,
                                Data = new CallUserObject.DataCallUser()
                            };
                            intentVideoCall.PutExtra("callUserObject", JsonConvert.SerializeObject(callUserObject));
                             
                        }
                         
                        Activity.StartActivity(intentVideoCall);
                        Dismiss();
                    }
                    catch (Exception exception)
                    {
                        Methods.DisplayReportResultTrack(exception);
                    }
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Block User
        private async void BlockLayoutOnClick(object sender, EventArgs e)
        {
            try
            {
                string userId = AppSettings.LastChatSystem == SystemApiGetLastChat.New ? DataChatObjectN.UserId : DataUserObjectOld.UserId;

                if (Methods.CheckConnectivity())
                {
                    var (apiStatus, respond) = await RequestsAsync.Global.BlockUserAsync(userId, true); //true >> "block" 
                    if (apiStatus == 200)
                    {
                        Methods.DisplayReportResultTrack(respond);

                        var dbDatabase = new SqLiteDatabase();
                        //dbDatabase.Insert_Or_Replace_OR_Delete_UsersContact(DataUserChat, "Delete"); 
                        dbDatabase.DeleteAllMessagesUser(UserDetails.UserId, userId);

                        Methods.Path.DeleteAll_FolderUser(userId);

                        ToastUtils.ShowToast(Context, Context.GetText(Resource.String.Lbl_Blocked_successfully), ToastLength.Short);
                    }
                }
                else
                {
                    ToastUtils.ShowToast(Context, Context.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short);
                }

                Dismiss();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Mark As Read/UnRead //wael
        //if (Seen == "0") //not read Change to read (Normal) >> Seen = "1"; 
        //else //read Change to unread (Bold) >> Seen = "0";
        private void ReadLayoutOnClick(object sender, EventArgs e)
        {
            try
            {
                if (AppSettings.LastChatSystem == SystemApiGetLastChat.New)
                {
                    var mAdapter = GlobalContext?.LastChatTab?.MAdapter;
                    var seen = DataChatObjectN.LastMessage.LastMessageClass.Seen == "0" ? DataChatObjectN.LastMessage.LastMessageClass.Seen = "1" : DataChatObjectN.LastMessage.LastMessageClass.Seen = "0";

                    //wael add api  
                    Classes.LastChatsClass checkUser = null!;
                    switch (Type)
                    {
                        case "user":
                            checkUser = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChat?.UserId == DataChatObjectN.UserId);
                            break;
                        case "page":
                            var checkPage = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChat?.PageId == DataChatObjectN.PageId && a.LastChat?.LastMessage.LastMessageClass?.ToData?.UserId == DataChatObjectN.LastMessage.LastMessageClass?.ToData?.UserId);
                            if (checkPage != null)
                            {
                                var userAdminPage = DataChatObjectN.UserId;
                                if (userAdminPage == DataChatObjectN.LastMessage.LastMessageClass.ToData.UserId)
                                {
                                    var userId = DataChatObjectN.LastMessage.LastMessageClass.UserData?.UserId;
                                    checkUser = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChat?.LastMessage.LastMessageClass.UserData?.UserId == userId);

                                    var name = DataChatObjectN.LastMessage.LastMessageClass.UserData?.Name + "(" + DataChatObjectN.PageName + ")";
                                    Console.WriteLine(name);
                                }
                                else
                                {
                                    var userId = DataChatObjectN.LastMessage.LastMessageClass.ToData.UserId;
                                    checkUser = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChat?.LastMessage.LastMessageClass.ToData.UserId == userId);

                                    var name = DataChatObjectN.LastMessage.LastMessageClass.ToData.Name + "(" + DataChatObjectN.PageName + ")";
                                    Console.WriteLine(name);
                                }
                            }
                            else
                                checkUser = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChat?.PageId == DataChatObjectN.PageId);
                            break;
                        //break;
                        case "group":
                            checkUser = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChat?.GroupId == DataChatObjectN.GroupId);
                            break;
                    }
                    if (checkUser != null)
                    {
                        checkUser.LastChat.LastMessage.LastMessageClass.Seen = seen;
                        mAdapter?.NotifyItemChanged(mAdapter.LastChatsList.IndexOf(checkUser), "WithoutBlobRead");
                    }
                }
                else
                {
                    //wael add api  
                    switch (Type)
                    {
                        case "user":
                            {
                                var mAdapter = GlobalContext?.LastChatTab?.MAdapter;
                                var seen = DataUserObjectOld.LastMessage.Seen == "0" ? DataUserObjectOld.LastMessage.Seen = "1" : DataUserObjectOld.LastMessage.Seen = "0";

                                var checkUser = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastMessagesUser?.UserId == DataUserObjectOld.UserId);
                                if (checkUser != null)
                                {
                                    checkUser.LastMessagesUser.LastMessage.Seen = seen;
                                    mAdapter?.NotifyItemChanged(mAdapter.LastChatsList.IndexOf(checkUser), "WithoutBlobRead");
                                }
                                break;
                            }
                        case "page":
                            {
                                var mAdapter = GlobalContext?.LastPageChatsTab?.MAdapter;
                                var seen = PageClassObject.LastMessage.Seen == "0" ? PageClassObject.LastMessage.Seen = "1" : PageClassObject.LastMessage.Seen = "0";

                                var checkPage = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChatPage?.PageId == PageClassObject.PageId && a.LastChatPage?.LastMessage?.ToData?.UserId == PageClassObject.LastMessage?.ToData?.UserId);
                                if (checkPage != null)
                                {
                                    var userAdminPage = PageClassObject.UserId;
                                    if (userAdminPage == PageClassObject.LastMessage.ToData.UserId)
                                    {
                                        var userId = PageClassObject.LastMessage.UserData?.UserId;
                                        var checkUser = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChatPage?.LastMessage.UserData?.UserId == userId);
                                        if (checkUser != null)
                                        {
                                            checkUser.LastChatPage.LastMessage.Seen = seen;
                                            mAdapter?.NotifyItemChanged(mAdapter.LastChatsList.IndexOf(checkUser), "WithoutBlobRead");
                                        }

                                        var name = PageClassObject.LastMessage.UserData?.Name + "(" + PageClassObject.PageName + ")";
                                        Console.WriteLine(name);
                                    }
                                    else
                                    {
                                        var userId = PageClassObject.LastMessage.ToData.UserId;
                                        var checkUser = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChatPage?.LastMessage.ToData.UserId == userId);
                                        if (checkUser != null)
                                        {
                                            checkUser.LastChatPage.LastMessage.Seen = seen;
                                            mAdapter?.NotifyItemChanged(mAdapter.LastChatsList.IndexOf(checkUser), "WithoutBlobRead");
                                        }

                                        var name = PageClassObject.LastMessage.ToData.Name + "(" + PageClassObject.PageName + ")";
                                        Console.WriteLine(name);
                                    }
                                }
                                break;
                            }
                        //break;
                        case "group":
                            {
                                var mAdapter = GlobalContext?.LastGroupChatsTab?.MAdapter;
                                var seen = DataChatObjectN.LastMessage.LastMessageClass.Seen == "0" ? DataChatObjectN.LastMessage.LastMessageClass.Seen = "1" : DataChatObjectN.LastMessage.LastMessageClass.Seen = "0";

                                var checkGroup = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChat?.GroupId == DataChatObjectN.GroupId);
                                if (checkGroup?.LastChat != null)
                                {
                                    checkGroup.LastChat.LastMessage.LastMessageClass.Seen = seen;
                                    mAdapter?.NotifyItemChanged(mAdapter.LastChatsList.IndexOf(checkGroup), "WithoutBlobRead");
                                }
                                break;
                            }
                    }
                }

                Dismiss();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Mark As Mute/UnMute
        private void MuteLayoutOnClick(object sender, EventArgs e)
        {
            try
            {
                bool isMute = false;
                Classes.OptionLastChat muteObject = null!;
                Mute globalMute = null!;
                string idChat = null!;

                if (AppSettings.LastChatSystem == SystemApiGetLastChat.New)
                {
                    var mAdapter = GlobalContext?.LastChatTab?.MAdapter;
                    isMute = !DataChatObjectN.IsMute;
                    idChat = DataChatObjectN.ChatId;
                    globalMute = DataChatObjectN.Mute;

                    Classes.LastChatsClass checkUser = null!;
                    switch (Type)
                    {
                        case "user":
                            checkUser = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChat?.UserId == DataChatObjectN.UserId);
                            muteObject = new Classes.OptionLastChat
                            {
                                ChatType = "user",
                                ChatId = DataChatObjectN.ChatId,
                                UserId = DataChatObjectN.UserId,
                                GroupId = "",
                                PageId = "",
                                Name = DataChatObjectN.Name
                            };
                            break;
                        case "page":
                            var checkPage = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChat?.PageId == DataChatObjectN.PageId && a.LastChat?.LastMessage.LastMessageClass?.ToData?.UserId == DataChatObjectN.LastMessage.LastMessageClass?.ToData?.UserId);
                            if (checkPage != null)
                            {
                                var userAdminPage = DataChatObjectN.UserId;
                                if (userAdminPage == DataChatObjectN.LastMessage.LastMessageClass.ToData.UserId)
                                {
                                    var userId = DataChatObjectN.LastMessage.LastMessageClass.UserData?.UserId;
                                    checkUser = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChat?.LastMessage.LastMessageClass.UserData?.UserId == userId);

                                    var name = DataChatObjectN.LastMessage.LastMessageClass.UserData?.Name + "(" + DataChatObjectN.PageName + ")";
                                    Console.WriteLine(name);

                                    muteObject = new Classes.OptionLastChat
                                    {
                                        ChatType = "page",
                                        ChatId = DataChatObjectN.ChatId,
                                        UserId = userId,
                                        GroupId = "",
                                        PageId = DataChatObjectN.PageId,
                                        Name = name
                                    };
                                }
                                else
                                {
                                    var userId = DataChatObjectN.LastMessage.LastMessageClass.ToData.UserId;
                                    checkUser = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChat?.LastMessage.LastMessageClass.ToData.UserId == userId);

                                    var name = DataChatObjectN.LastMessage.LastMessageClass.ToData.Name + "(" + DataChatObjectN.PageName + ")";
                                    Console.WriteLine(name);

                                    muteObject = new Classes.OptionLastChat
                                    {
                                        ChatType = "page",
                                        ChatId = DataChatObjectN.ChatId,
                                        UserId = userId,
                                        GroupId = "",
                                        PageId = DataChatObjectN.PageId,
                                        Name = name
                                    };
                                }
                            }
                            else
                            {
                                checkUser = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChat?.PageId == DataChatObjectN.PageId);
                                muteObject = new Classes.OptionLastChat
                                {
                                    ChatType = "page",
                                    ChatId = DataChatObjectN.PageId,
                                    UserId = "",
                                    GroupId = "",
                                    PageId = DataChatObjectN.PageId,
                                    Name = DataChatObjectN.PageName
                                };
                            }
                            break;
                        //break;
                        case "group":
                            {
                                checkUser = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChat?.GroupId == DataChatObjectN.GroupId);
                                muteObject = new Classes.OptionLastChat
                                {
                                    ChatType = "group",
                                    ChatId = DataChatObjectN.ChatId,
                                    UserId = "",
                                    GroupId = DataChatObjectN.GroupId,
                                    PageId = "",
                                    Name = DataChatObjectN.GroupName
                                };
                            }
                            break;
                    }
                    if (checkUser != null)
                    {
                        checkUser.LastChat.IsMute = isMute;
                        checkUser.LastChat.Mute.Notify = isMute ? "no" : "yes";
                        globalMute = checkUser.LastChat.Mute;

                        mAdapter?.NotifyItemChanged(mAdapter.LastChatsList.IndexOf(checkUser), "WithoutBlobMute");
                    }
                }
                else
                { 
                    switch (Type)
                    {
                        case "user":
                            {
                                var mAdapter = GlobalContext?.LastChatTab?.MAdapter;
                                isMute = !DataUserObjectOld.IsMute;
                                idChat = DataUserObjectOld.UserId;
                                globalMute = DataUserObjectOld.Mute;

                                var checkUser = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastMessagesUser?.UserId == DataUserObjectOld.UserId);
                                if (checkUser != null)
                                {
                                    checkUser.LastMessagesUser.IsMute = isMute;

                                    checkUser.LastMessagesUser.Mute.Notify = isMute ? "no" : "yes";
                                    globalMute = checkUser.LastChat.Mute;

                                    mAdapter?.NotifyItemChanged(mAdapter.LastChatsList.IndexOf(checkUser), "WithoutBlobMute");
                                    muteObject = new Classes.OptionLastChat
                                    {
                                        ChatType = "user",
                                        ChatId = DataUserObjectOld.UserId,
                                        UserId = DataUserObjectOld.UserId,
                                        GroupId = "",
                                        PageId = "",
                                        Name = DataUserObjectOld.Name
                                    };
                                }
                                break;
                            }
                        case "page":
                            {
                                var mAdapter = GlobalContext?.LastPageChatsTab?.MAdapter;
                                isMute = !PageClassObject.IsMute;
                                idChat = PageClassObject.PageId;
                                 
                                var checkPage = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChatPage?.PageId == PageClassObject.PageId && a.LastChatPage?.LastMessage?.ToData?.UserId == PageClassObject.LastMessage?.ToData?.UserId);
                                if (checkPage != null)
                                {
                                    var userAdminPage = PageClassObject.UserId;
                                    if (userAdminPage == PageClassObject.LastMessage.ToData.UserId)
                                    {
                                        var userId = PageClassObject.LastMessage.UserData?.UserId;
                                        var checkUser = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChatPage?.LastMessage.UserData?.UserId == userId);
                                        if (checkUser != null)
                                        {
                                            checkUser.LastChatPage.IsMute = isMute;
                                            //checkUser.LastChatPage.Mute.Notify = isMute ? "no" : "yes";
                                            //GlobalMute = checkUser.LastChatPage.Mute;

                                            mAdapter?.NotifyItemChanged(mAdapter.LastChatsList.IndexOf(checkUser), "WithoutBlobMute");
                                        }

                                        var name = PageClassObject.LastMessage.UserData?.Name + "(" + PageClassObject.PageName + ")";
                                        Console.WriteLine(name);

                                        muteObject = new Classes.OptionLastChat
                                        {
                                            ChatType = "page",
                                            ChatId = PageClassObject.PageId + userId,
                                            UserId = userId,
                                            GroupId = "",
                                            PageId = PageClassObject.PageId,
                                            Name = name
                                        };
                                    }
                                    else
                                    {
                                        var userId = PageClassObject.LastMessage.ToData.UserId;
                                        var checkUser = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChatPage?.LastMessage.ToData.UserId == userId);
                                        if (checkUser != null)
                                        {
                                            checkUser.LastChatPage.IsMute = isMute;

                                            //checkUser.LastChatPage.Mute.Notify = isMute ? "no" : "yes";
                                            //GlobalMute = checkUser.LastChatPage.Mute;

                                            mAdapter?.NotifyItemChanged(mAdapter.LastChatsList.IndexOf(checkUser), "WithoutBlobMute");
                                        }

                                        var name = PageClassObject.LastMessage.ToData.Name + "(" + PageClassObject.PageName + ")";
                                        Console.WriteLine(name);

                                        muteObject = new Classes.OptionLastChat
                                        {
                                            ChatType = "page",
                                            ChatId = PageClassObject.PageId + userId,
                                            UserId = userId,
                                            GroupId = "",
                                            PageId = PageClassObject.PageId,
                                            Name = name
                                        };
                                    }
                                }
                                break;
                            }
                        //break;
                        case "group":
                            {
                                var mAdapter = GlobalContext?.LastGroupChatsTab?.MAdapter;
                                isMute = !DataChatObjectN.IsMute;
                                idChat = DataChatObjectN.GroupId;
                                 
                                var checkGroup = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChat?.GroupId == DataChatObjectN.GroupId);
                                if (checkGroup != null)
                                {
                                    checkGroup.LastChat.IsMute = isMute;

                                    checkGroup.LastChat.Mute.Notify = isMute ? "no" : "yes";
                                    globalMute = checkGroup.LastChat.Mute;
                                     
                                    mAdapter?.NotifyItemChanged(mAdapter.LastChatsList.IndexOf(checkGroup), "WithoutBlobMute");

                                    muteObject = new Classes.OptionLastChat
                                    {
                                        ChatType = "group",
                                        ChatId = DataChatObjectN.ChatId,
                                        UserId = "",
                                        GroupId = DataChatObjectN.GroupId,
                                        PageId = "",
                                        Name = DataChatObjectN.GroupName
                                    };
                                }
                                break;
                            }
                    }
                }

                if (isMute)
                {
                    if (muteObject != null)
                    {
                        ListUtils.MuteList.Add(muteObject);

                        var sqLiteDatabase = new SqLiteDatabase();
                        sqLiteDatabase.InsertORDelete_Mute(muteObject);
                    }
                     
                    var dictionary = new Dictionary<string, string>
                    {
                        {"notify", "no"}, 
                    };

                    if (globalMute != null)
                    {
                        dictionary.Add("call_chat", globalMute.CallChat);
                        dictionary.Add("archive", globalMute.Archive);
                        dictionary.Add("pin", globalMute.Pin); 
                    }

                    if (Methods.CheckConnectivity())
                        PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Message.MuteChatsInfoAsync(idChat, Type, dictionary) });
                     
                    ToastUtils.ShowToast(Context, Context.GetText(Resource.String.Lbl_AddedMute), ToastLength.Long);
                }
                else
                {
                    var checkMute = ListUtils.MuteList.FirstOrDefault(a => muteObject != null && a.ChatId == muteObject.ChatId && a.ChatType == muteObject.ChatType);
                    if (checkMute != null)
                    {
                        ListUtils.MuteList.Remove(checkMute);

                        var sqLiteDatabase = new SqLiteDatabase();
                        sqLiteDatabase.InsertORDelete_Mute(checkMute);
                    }
                     
                    var dictionary = new Dictionary<string, string>
                    {
                        {"notify", "yes"},
                    };

                    if (globalMute != null)
                    {
                        dictionary.Add("call_chat", globalMute.CallChat);
                        dictionary.Add("archive", globalMute.Archive);
                        dictionary.Add("pin", globalMute.Pin);
                    }

                    if (Methods.CheckConnectivity())
                        PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Message.MuteChatsInfoAsync(idChat, Type, dictionary) });
                     
                    ToastUtils.ShowToast(Context, Context.GetText(Resource.String.Lbl_RemovedMute), ToastLength.Long);
                }

                Dismiss();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Mark Pin
        private void PinLayoutOnClick(object sender, EventArgs e)
        {
            try
            {
                bool isPin = false;
                Classes.OptionLastChat pinObject = null!;
                Mute globalMute = null!;
                string idChat = null!;
                
                if (AppSettings.LastChatSystem == SystemApiGetLastChat.New)
                {
                    var mAdapter = GlobalContext?.LastChatTab?.MAdapter;
                    isPin = !DataChatObjectN.IsPin;
                    idChat = DataChatObjectN.ChatId;
                    globalMute = DataChatObjectN.Mute;

                    Classes.LastChatsClass checkUser = null!;
                    switch (Type)
                    {
                        case "user":
                            checkUser = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChat?.UserId == DataChatObjectN.UserId);
                            pinObject = new Classes.OptionLastChat
                            {
                                ChatType = "user",
                                ChatId = DataChatObjectN.ChatId,
                                UserId = DataChatObjectN.UserId,
                                GroupId = "",
                                PageId = "",
                                Name = DataChatObjectN.Name
                            };
                            break;
                        case "page":
                            var checkPage = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChat?.PageId == DataChatObjectN.PageId && a.LastChat?.LastMessage.LastMessageClass?.ToData?.UserId == DataChatObjectN.LastMessage.LastMessageClass?.ToData?.UserId);
                            if (checkPage != null)
                            {
                                var userAdminPage = DataChatObjectN.UserId;
                                if (userAdminPage == DataChatObjectN.LastMessage.LastMessageClass.ToData.UserId)
                                {
                                    var userId = DataChatObjectN.LastMessage.LastMessageClass.UserData?.UserId;
                                    checkUser = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChat?.LastMessage.LastMessageClass.UserData?.UserId == userId);

                                    var name = DataChatObjectN.LastMessage.LastMessageClass.UserData?.Name + "(" + DataChatObjectN.PageName + ")";
                                    Console.WriteLine(name);

                                    pinObject = new Classes.OptionLastChat
                                    {
                                        ChatType = "page",
                                        ChatId = DataChatObjectN.ChatId,
                                        UserId = userId,
                                        GroupId = "",
                                        PageId = DataChatObjectN.PageId,
                                        Name = name
                                    };
                                }
                                else
                                {
                                    var userId = DataChatObjectN.LastMessage.LastMessageClass.ToData.UserId;
                                    checkUser = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChat?.LastMessage.LastMessageClass.ToData.UserId == userId);

                                    var name = DataChatObjectN.LastMessage.LastMessageClass.ToData.Name + "(" + DataChatObjectN.PageName + ")";
                                    Console.WriteLine(name);

                                    pinObject = new Classes.OptionLastChat
                                    {
                                        ChatType = "page",
                                        ChatId = DataChatObjectN.ChatId,
                                        UserId = userId,
                                        GroupId = "",
                                        PageId = DataChatObjectN.PageId,
                                        Name = name
                                    };
                                }
                            }
                            else
                            {
                                checkUser = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChat?.PageId == DataChatObjectN.PageId);
                                pinObject = new Classes.OptionLastChat
                                {
                                    ChatType = "page",
                                    ChatId = DataChatObjectN.ChatId,
                                    UserId = "",
                                    GroupId = "",
                                    PageId = DataChatObjectN.PageId,
                                    Name = DataChatObjectN.PageName
                                };
                            }
                            break;
                        //break;
                        case "group":
                            {
                                checkUser = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChat?.GroupId == DataChatObjectN.GroupId);
                                pinObject = new Classes.OptionLastChat
                                {
                                    ChatType = "group",
                                    ChatId = DataChatObjectN.ChatId,
                                    UserId = "",
                                    GroupId = DataChatObjectN.GroupId,
                                    PageId = "",
                                    Name = DataChatObjectN.GroupName
                                };
                            }
                            break;
                    }
                 
                    if (checkUser != null)
                    {
                        checkUser.LastChat.IsPin = isPin;
                        checkUser.LastChat.Mute.Pin = isPin ? "yes" : "no";
                        globalMute = checkUser.LastChat.Mute;
                        
                        var index = mAdapter.LastChatsList.IndexOf(checkUser);
                        if (isPin)
                        {
                            if (ListUtils.FriendRequestsList.Count > 0)
                            {
                                mAdapter.LastChatsList.Move(index, 1);
                                mAdapter.NotifyItemMoved(index, 1);
                                mAdapter.NotifyItemChanged(1, "WithoutBlobPin");
                            }
                            else
                            {
                                mAdapter.LastChatsList.Move(index, 0);
                                mAdapter.NotifyItemMoved(index, 0);
                                mAdapter.NotifyItemChanged(0, "WithoutBlobPin");
                            }
                             
                            //var checkPin = mAdapter.LastChatsList.LastOrDefault(o => o.LastChat != null && o.LastChat.IsPin);
                            //if (checkPin != null)
                            //{
                            //    var toIndex = mAdapter.LastChatsList.IndexOf(checkPin) + 1;

                            //    mAdapter.LastChatsList.Move(index, toIndex);
                            //    mAdapter.NotifyItemMoved(index, toIndex);
                            //    mAdapter.NotifyItemChanged(toIndex);
                            //}
                            //else
                            //{
                            //    if (ListUtils.FriendRequestsList.Count > 0)
                            //    {
                            //        mAdapter.LastChatsList.Move(index, 1);
                            //        mAdapter.NotifyItemMoved(index, 1);
                            //        mAdapter.NotifyItemChanged(1);
                            //    }
                            //    else
                            //    {
                            //        mAdapter.LastChatsList.Move(index, 0);
                            //        mAdapter.NotifyItemMoved(index, 0);
                            //        mAdapter.NotifyItemChanged(0);
                            //    }
                            //}
                        }
                        else
                        {
                            mAdapter.NotifyItemChanged(index, "WithoutBlobPin");
                        }
                    }
                }
                else
                {
                    switch (Type)
                    {
                        case "user":
                            {
                                var mAdapter = GlobalContext?.LastChatTab?.MAdapter;
                                isPin = !DataUserObjectOld.IsPin;
                                idChat = DataUserObjectOld.UserId;
                                globalMute = DataUserObjectOld.Mute;

                                var checkUser = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastMessagesUser?.UserId == DataUserObjectOld.UserId);
                                if (checkUser != null)
                                {
                                    checkUser.LastMessagesUser.IsPin = isPin;

                                    checkUser.LastMessagesUser.Mute.Pin = isPin ? "yes" : "no";
                                    globalMute = checkUser.LastMessagesUser.Mute;

                                    var index = mAdapter.LastChatsList.IndexOf(checkUser);
                                    if (isPin)
                                    {
                                        var checkPin = mAdapter.LastChatsList.LastOrDefault(o => o.LastMessagesUser != null && o.LastMessagesUser.IsPin);
                                        if (checkPin != null)
                                        {
                                            var toIndex = mAdapter.LastChatsList.IndexOf(checkPin) + 1;

                                            if (ListUtils.FriendRequestsList.Count > 0)
                                                toIndex++;

                                            if (mAdapter.LastChatsList.Count > toIndex)
                                            {
                                                mAdapter.LastChatsList.Move(index, toIndex);
                                                mAdapter.NotifyItemMoved(index, toIndex);
                                            }

                                            mAdapter.NotifyItemChanged(toIndex, "WithoutBlobPin");
                                        }
                                        else
                                        {
                                            if (ListUtils.FriendRequestsList.Count > 0)
                                            {
                                                mAdapter.LastChatsList.Move(index, 1);
                                                mAdapter.NotifyItemMoved(index, 1);
                                                mAdapter.NotifyItemChanged(1, "WithoutBlobPin");
                                            }
                                            else
                                            {
                                                mAdapter.LastChatsList.Move(index, 0);
                                                mAdapter.NotifyItemMoved(index, 0);
                                                mAdapter.NotifyItemChanged(0, "WithoutBlobPin");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        mAdapter.NotifyItemChanged(index, "WithoutBlobPin");
                                    }

                                    pinObject = new Classes.OptionLastChat
                                    {
                                        ChatType = "user",
                                        ChatId = DataUserObjectOld.UserId,
                                        UserId = DataUserObjectOld.UserId,
                                        GroupId = "",
                                        PageId = "",
                                        Name = DataUserObjectOld.Name
                                    };
                                }
                                break;
                            }
                        case "page":
                            {
                                var mAdapter = GlobalContext?.LastPageChatsTab?.MAdapter;
                                isPin = !PageClassObject.IsPin;
                                idChat = PageClassObject.PageId;

                                var checkPage = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChatPage?.PageId == PageClassObject.PageId && a.LastChatPage?.LastMessage?.ToData?.UserId == PageClassObject.LastMessage?.ToData?.UserId);
                                if (checkPage != null)
                                {
                                    var userAdminPage = PageClassObject.UserId;
                                    if (userAdminPage == PageClassObject.LastMessage.ToData.UserId)
                                    {
                                        var userId = PageClassObject.LastMessage.UserData?.UserId;
                                        var checkUser = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChatPage?.LastMessage.UserData?.UserId == userId);
                                        if (checkUser != null)
                                        {
                                            checkUser.LastChatPage.IsPin = isPin;
                                            //checkUser.LastChatPage.Mute.Pin = isPin ? "yes" : "no";
                                            //globalMute = checkUser.LastChatPage.Mute;

                                            var index = mAdapter.LastChatsList.IndexOf(checkUser);
                                            if (isPin)
                                            {
                                                var checkPin = mAdapter.LastChatsList.LastOrDefault(o => o.LastChatPage != null && o.LastChatPage.IsPin);
                                                if (checkPin != null)
                                                {
                                                    var toIndex = mAdapter.LastChatsList.IndexOf(checkPin) + 1;

                                                    if (ListUtils.FriendRequestsList.Count > 0)
                                                        toIndex++;

                                                    if (mAdapter.LastChatsList.Count > toIndex)
                                                    {
                                                        mAdapter.LastChatsList.Move(index, toIndex);
                                                        mAdapter.NotifyItemMoved(index, toIndex);
                                                    }
                                                    mAdapter.NotifyItemChanged(toIndex, "WithoutBlobPin");
                                                }
                                                else
                                                {
                                                    if (ListUtils.FriendRequestsList.Count > 0)
                                                    {
                                                        mAdapter.LastChatsList.Move(index, 1);
                                                        mAdapter.NotifyItemMoved(index, 1);
                                                        mAdapter.NotifyItemChanged(1, "WithoutBlobPin");
                                                    }
                                                    else
                                                    {
                                                        mAdapter.LastChatsList.Move(index, 0);
                                                        mAdapter.NotifyItemMoved(index, 0);
                                                        mAdapter.NotifyItemChanged(0, "WithoutBlobPin");
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                mAdapter.NotifyItemChanged(index, "WithoutBlobPin");
                                            }
                                        }

                                        var name = PageClassObject.LastMessage.UserData?.Name + "(" + PageClassObject.PageName + ")";
                                        Console.WriteLine(name);

                                        pinObject = new Classes.OptionLastChat
                                        {
                                            ChatType = "page",
                                            ChatId = PageClassObject.PageId + userId,
                                            UserId = userId,
                                            GroupId = "",
                                            PageId = PageClassObject.PageId,
                                            Name = name
                                        };
                                    }
                                    else
                                    {
                                        var userId = PageClassObject.LastMessage.ToData.UserId;
                                        var checkUser = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChatPage?.LastMessage.ToData.UserId == userId);
                                        if (checkUser != null)
                                        {
                                            checkUser.LastChatPage.IsPin = isPin;
                                            //checkUser.LastChatPage.Mute.Pin = isPin ? "yes" : "no";
                                            //globalMute = checkUser.LastChatPage.Mute;

                                            var index = mAdapter.LastChatsList.IndexOf(checkUser);

                                            if (isPin)
                                            {
                                                var checkPin = mAdapter.LastChatsList.LastOrDefault(o => o.LastChatPage != null && o.LastChatPage.IsPin);
                                                if (checkPin != null)
                                                {
                                                    var toIndex = mAdapter.LastChatsList.IndexOf(checkPin) + 1;

                                                    if (ListUtils.FriendRequestsList.Count > 0)
                                                        toIndex++;

                                                    if (mAdapter.LastChatsList.Count > toIndex)
                                                    {
                                                        mAdapter.LastChatsList.Move(index, toIndex);
                                                        mAdapter.NotifyItemMoved(index, toIndex);
                                                    }
                                                   
                                                    mAdapter.NotifyItemChanged(toIndex, "WithoutBlobPin");
                                                }
                                                else
                                                {
                                                    if (ListUtils.FriendRequestsList.Count > 0)
                                                    {
                                                        mAdapter.LastChatsList.Move(index, 1);
                                                        mAdapter.NotifyItemMoved(index, 1);
                                                        mAdapter.NotifyItemChanged(1, "WithoutBlobPin");
                                                    }
                                                    else
                                                    {
                                                        mAdapter.LastChatsList.Move(index, 0);
                                                        mAdapter.NotifyItemMoved(index, 0);
                                                        mAdapter.NotifyItemChanged(0, "WithoutBlobPin");
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                mAdapter.NotifyItemChanged(index, "WithoutBlobPin");
                                            }
                                        }

                                        var name = PageClassObject.LastMessage.ToData.Name + "(" + PageClassObject.PageName + ")";
                                        Console.WriteLine(name);

                                        pinObject = new Classes.OptionLastChat
                                        {
                                            ChatType = "page",
                                            ChatId = PageClassObject.PageId + userId,
                                            UserId = userId,
                                            GroupId = "",
                                            PageId = PageClassObject.PageId,
                                            Name = name
                                        };
                                    }
                                }
                                break;
                            }
                        //break;
                        case "group":
                            {
                                var mAdapter = GlobalContext?.LastGroupChatsTab?.MAdapter;
                                isPin = !DataChatObjectN.IsPin;
                                idChat = DataChatObjectN.GroupId;
                                globalMute = DataChatObjectN.Mute;

                                var checkGroup = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChat?.GroupId == DataChatObjectN.GroupId);
                                if (checkGroup?.LastChat != null)
                                {
                                    checkGroup.LastChat.IsPin = isPin;
                                    checkGroup.LastChat.Mute.Pin = isPin ? "yes" : "no";
                                    globalMute = checkGroup.LastChat.Mute;

                                    if (isPin)
                                    {
                                        var index = mAdapter.LastChatsList.IndexOf(checkGroup);

                                        if (isPin)
                                        {
                                            var checkPin = mAdapter.LastChatsList.LastOrDefault(o => o.LastChat != null && o.LastChat.IsPin);
                                            if (checkPin != null)
                                            {
                                                var toIndex = mAdapter.LastChatsList.IndexOf(checkPin) + 1;

                                                if (ListUtils.FriendRequestsList.Count > 0)
                                                    toIndex++;

                                                if (mAdapter.LastChatsList.Count > toIndex)
                                                {
                                                    mAdapter.LastChatsList.Move(index, toIndex);
                                                    mAdapter.NotifyItemMoved(index, toIndex);
                                                }
                                                mAdapter.NotifyItemChanged(toIndex, "WithoutBlobPin");
                                            }
                                            else
                                            {
                                                if (ListUtils.FriendRequestsList.Count > 0)
                                                {
                                                    mAdapter.LastChatsList.Move(index, 1);
                                                    mAdapter.NotifyItemMoved(index, 1);
                                                    mAdapter.NotifyItemChanged(1, "WithoutBlobPin");
                                                }
                                                else
                                                {
                                                    mAdapter.LastChatsList.Move(index, 0);
                                                    mAdapter.NotifyItemMoved(index, 0);
                                                    mAdapter.NotifyItemChanged(0, "WithoutBlobPin");
                                                }
                                            }
                                        }
                                        else
                                        {
                                            mAdapter.NotifyItemChanged(index, "WithoutBlobPin");
                                        }
                                    }

                                    pinObject = new Classes.OptionLastChat
                                    {
                                        ChatType = "group",
                                        ChatId = DataChatObjectN.GroupId,
                                        UserId = "",
                                        GroupId = DataChatObjectN.GroupId,
                                        PageId = "",
                                        Name = DataChatObjectN.GroupName
                                    };
                                }
                                break;
                            }
                    }
                }

                if (isPin)
                {
                    if (pinObject != null)
                    {
                        ListUtils.PinList.Add(pinObject);

                        var sqLiteDatabase = new SqLiteDatabase();
                        sqLiteDatabase.InsertORDelete_Pin(pinObject);
                    }

                    var dictionary = new Dictionary<string, string>
                    {
                        {"pin", "yes"},
                    };

                    if (globalMute != null)
                    {
                        dictionary.Add("call_chat", globalMute.CallChat);
                        dictionary.Add("archive", globalMute.Archive);
                        dictionary.Add("notify", globalMute.Notify);
                    }

                    if (Methods.CheckConnectivity())
                        PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Message.MuteChatsInfoAsync(idChat, Type, dictionary) });

                    ToastUtils.ShowToast(Context, Context.GetText(Resource.String.Lbl_MessagePinned), ToastLength.Long);
                }
                else
                {
                    var checkPin = ListUtils.PinList.FirstOrDefault(a => pinObject != null && a.ChatId == pinObject.ChatId && a.ChatType == pinObject.ChatType);
                    if (checkPin != null)
                    {
                        ListUtils.PinList.Remove(checkPin);

                        var sqLiteDatabase = new SqLiteDatabase();
                        sqLiteDatabase.InsertORDelete_Pin(checkPin);
                    }

                    var dictionary = new Dictionary<string, string>
                    {
                        {"pin", "no"},
                    };

                    if (globalMute != null)
                    {
                        dictionary.Add("call_chat", globalMute.CallChat);
                        dictionary.Add("archive", globalMute.Archive);
                        dictionary.Add("notify", globalMute.Notify);
                    }

                    if (Methods.CheckConnectivity())
                        PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Message.MuteChatsInfoAsync(idChat, Type, dictionary) });

                    ToastUtils.ShowToast(Context, Context.GetText(Resource.String.Lbl_MessageUnPinned), ToastLength.Long);
                }

                Dismiss();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Delete Chat
        private void DeleteLayoutOnClick(object sender, EventArgs e)
        {
            try
            {
                var dialog = new MaterialDialog.Builder(Context).Theme(AppSettings.SetTabDarkTheme ? MaterialDialogsCore.Theme.Dark : MaterialDialogsCore.Theme.Light);
                dialog.Title(GetText(Resource.String.Lbl_DeleteTheEntireConversation));
                dialog.Content(GetText(Resource.String.Lbl_OnceYouDeleteConversation));
                dialog.PositiveText(GetText(Resource.String.Lbl_Yes)).OnPositive((materialDialog, action) =>
                {
                    try
                    {
                        if (!Methods.CheckConnectivity())
                        {
                            ToastUtils.ShowToast(Context, Context.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short);
                            return;
                        }

                        switch (Type)
                        {
                            case "user":
                                {
                                    if (AppSettings.LastChatSystem == SystemApiGetLastChat.New)
                                    {
                                        var mAdapter = GlobalContext?.LastChatTab?.MAdapter;
                                        var userToDelete = mAdapter?.LastChatsList?.FirstOrDefault(a => a.LastChat?.UserId == DataChatObjectN.UserId);
                                        if (userToDelete != null)
                                        {
                                            var index = mAdapter.LastChatsList.IndexOf(userToDelete);
                                            if (index > -1)
                                            {
                                                mAdapter?.LastChatsList?.Remove(userToDelete);
                                                mAdapter?.NotifyItemRemoved(index);
                                            }
                                        }

                                        var dbDatabase = new SqLiteDatabase();
                                        dbDatabase.Delete_LastUsersChat(DataChatObjectN.UserId, "user");
                                        dbDatabase.DeleteAllMessagesUser(UserDetails.UserId, DataChatObjectN.UserId);

                                        Methods.Path.DeleteAll_FolderUser(DataChatObjectN.UserId);

                                        PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Message.DeleteConversationAsync(DataChatObjectN.UserId) });
                                        ToastUtils.ShowToast(Context, Context.GetText(Resource.String.Lbl_TheConversationHasBeenDeleted), ToastLength.Long);
                                    }
                                    else
                                    {
                                        var mAdapter = GlobalContext?.LastChatTab?.MAdapter;
                                        var userToDelete = mAdapter?.LastChatsList?.FirstOrDefault(a => a.LastMessagesUser?.UserId == DataUserObjectOld.UserId);
                                        if (userToDelete != null)
                                        {
                                            var index = mAdapter.LastChatsList.IndexOf(userToDelete);
                                            if (index > -1)
                                            {
                                                mAdapter?.LastChatsList.Remove(userToDelete);
                                                mAdapter?.NotifyItemRemoved(index);
                                            }
                                        }

                                        var dbDatabase = new SqLiteDatabase();
                                        dbDatabase.Delete_LastUsersChat(DataUserObjectOld.UserId, "user");
                                        dbDatabase.DeleteAllMessagesUser(UserDetails.UserId, DataUserObjectOld.UserId);

                                        Methods.Path.DeleteAll_FolderUser(DataUserObjectOld.UserId);

                                        PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Message.DeleteConversationAsync(DataUserObjectOld.UserId) });
                                        ToastUtils.ShowToast(Context, Context.GetText(Resource.String.Lbl_TheConversationHasBeenDeleted), ToastLength.Long);
                                    }
                                    break;
                                }
                            case "page":
                                {
                                    string userId;
                                    //remove item to my Group list  
                                    if (AppSettings.LastChatSystem == SystemApiGetLastChat.New)
                                    {
                                        var mAdapter = GlobalContext?.LastChatTab?.MAdapter;

                                        var checkPage = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChat?.PageId == DataChatObjectN.PageId);
                                        if (checkPage != null)
                                        {
                                            var userAdminPage = DataChatObjectN.UserId;
                                            if (userAdminPage == DataChatObjectN.LastMessage.LastMessageClass.ToData.UserId)
                                            {
                                                userId = DataChatObjectN.LastMessage.LastMessageClass.UserData.UserId;
                                                var data = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChat?.LastMessage.LastMessageClass.UserData.UserId == userId);
                                                if (data != null)
                                                {
                                                    mAdapter?.LastChatsList.Remove(data);
                                                    mAdapter?.NotifyItemRemoved(mAdapter.LastChatsList.IndexOf(data));
                                                }
                                            }
                                            else
                                            {
                                                userId = DataChatObjectN.LastMessage.LastMessageClass.ToData.UserId;
                                                var data = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChat?.LastMessage.LastMessageClass.ToData.UserId == userId);
                                                if (data != null)
                                                {
                                                    mAdapter?.LastChatsList.Remove(data);
                                                    mAdapter?.NotifyItemRemoved(mAdapter.LastChatsList.IndexOf(data));
                                                }
                                            }

                                            var dbDatabase = new SqLiteDatabase();
                                            dbDatabase.Delete_LastUsersChat(DataChatObjectN.PageId, "page", userId);

                                            Methods.Path.DeleteAll_FolderUser(DataChatObjectN.PageId);

                                            PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.PageChat.DeletePageChatAsync(DataChatObjectN.PageId, userId) });
                                        }
                                    }
                                    else
                                    {
                                        var adapter = GlobalContext?.LastPageChatsTab.MAdapter;
                                        var data = adapter?.LastChatsList?.FirstOrDefault(a => a.LastChatPage?.PageId == PageClassObject.PageId);
                                        if (data != null)
                                        {
                                            if (data.LastChatPage?.LastMessage != null)
                                                userId = data.LastChatPage?.IsPageOnwer != null && data.LastChatPage.IsPageOnwer.Value ? data.LastChatPage?.LastMessage.FromId == UserDetails.UserId ? data.LastChatPage?.LastMessage.ToId : data.LastChatPage?.LastMessage.FromId : UserDetails.UserId ?? UserDetails.UserId;
                                            else
                                                userId = data.LastChatPage?.IsPageOnwer != null && data.LastChatPage.IsPageOnwer.Value ? data.LastChatPage?.UserId : UserDetails.UserId;

                                            adapter.LastChatsList.Remove(data);
                                            adapter.NotifyItemRemoved(adapter.LastChatsList.IndexOf(data));

                                            var dbDatabase = new SqLiteDatabase();
                                            dbDatabase.Delete_LastUsersChat(PageClassObject.PageId, "page", userId);

                                            Methods.Path.DeleteAll_FolderUser(PageClassObject.PageId);

                                            PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.PageChat.DeletePageChatAsync(PageClassObject.PageId, userId) });
                                        }
                                    }

                                    ToastUtils.ShowToast(Activity, Activity.GetString(Resource.String.Lbl_TheConversationHasBeenDeleted), ToastLength.Short);

                                    break;
                                }
                            case "group":
                                {
                                    //remove item to my Group list  
                                    if (AppSettings.LastChatSystem == SystemApiGetLastChat.New)
                                    {
                                        var mAdapter = GlobalContext?.LastChatTab?.MAdapter;
                                        var data = mAdapter?.LastChatsList?.FirstOrDefault(a => a.LastChat?.GroupId == DataChatObjectN.GroupId);
                                        if (data != null)
                                        {
                                            mAdapter.LastChatsList.Remove(data);
                                            mAdapter.NotifyItemRemoved(mAdapter.LastChatsList.IndexOf(data));
                                        }
                                    }
                                    else
                                    {
                                        var adapter = GlobalContext?.LastGroupChatsTab.MAdapter;
                                        var data = adapter?.LastChatsList?.FirstOrDefault(a => a.LastChat?.GroupId == DataChatObjectN.GroupId);
                                        if (data != null)
                                        {
                                            adapter.LastChatsList.Remove(data);
                                            adapter.NotifyItemRemoved(adapter.LastChatsList.IndexOf(data));
                                        }
                                    }

                                    var dbDatabase = new SqLiteDatabase();
                                    dbDatabase.Delete_LastUsersChat(DataChatObjectN.GroupId, "group");
                                    dbDatabase.DeleteAllMessagesUser(UserDetails.UserId, DataChatObjectN.GroupId);

                                    Methods.Path.DeleteAll_FolderUser(DataChatObjectN.GroupId);

                                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.GroupChat.DeleteGroupChatAsync(DataChatObjectN.GroupId) });

                                    ToastUtils.ShowToast(Activity, Activity.GetString(Resource.String.Lbl_GroupSuccessfullyLeaved), ToastLength.Short);
                                    break;
                                }
                        }

                        Dismiss();
                    }
                    catch (Exception exception)
                    {
                        Methods.DisplayReportResultTrack(exception);
                    }
                });
                dialog.NegativeText(GetText(Resource.String.Lbl_No)).OnNegative(new WoWonderTools.MyMaterialDialog());
                dialog.AlwaysCallSingleChoiceCallback();
                dialog.Build().Show(); 
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Archive chat 
        private void ArchiveLayoutOnClick(object sender, EventArgs e)
        {
            try
            {
                bool isArchive = false;
                Classes.LastChatArchive archiveObject = null!;
                Mute globalMute = null!;
                Classes.LastChatsClass checkUser = null!;
                string idChat = null!;

                if (AppSettings.LastChatSystem == SystemApiGetLastChat.New)
                {
                    var mAdapter = GlobalContext?.LastChatTab?.MAdapter;
                    isArchive = !DataChatObjectN.IsArchive;
                    DataChatObjectN.IsArchive = isArchive;
                    idChat = DataChatObjectN.ChatId;
                    globalMute = DataChatObjectN.Mute;

                    switch (Type)
                    {
                        case "user":
                            checkUser = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChat?.UserId == DataChatObjectN.UserId);
                            archiveObject = new Classes.LastChatArchive
                            {
                                ChatType = "user",
                                ChatId = DataChatObjectN.ChatId,
                                UserId = DataChatObjectN.UserId,
                                GroupId = "",
                                PageId = "",
                                Name = DataChatObjectN.Name,
                                IdLastMessage = DataChatObjectN.LastMessage.LastMessageClass?.Id ?? "",
                                LastChat = DataChatObjectN
                            };
                            break;
                        case "page":
                            var checkPage = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChat?.PageId == DataChatObjectN.PageId && a.LastChat?.LastMessage.LastMessageClass?.ToData?.UserId == DataChatObjectN.LastMessage.LastMessageClass?.ToData?.UserId);
                            if (checkPage != null)
                            {
                                var userAdminPage = DataChatObjectN.UserId;
                                if (userAdminPage == DataChatObjectN.LastMessage.LastMessageClass.ToData.UserId)
                                {
                                    var userId = DataChatObjectN.LastMessage.LastMessageClass.UserData?.UserId;
                                    checkUser = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChat?.LastMessage.LastMessageClass.UserData?.UserId == userId);

                                    var name = DataChatObjectN.LastMessage.LastMessageClass.UserData?.Name + "(" + DataChatObjectN.PageName + ")";
                                    Console.WriteLine(name);

                                    archiveObject = new Classes.LastChatArchive
                                    {
                                        ChatType = "page",
                                        ChatId = DataChatObjectN.ChatId,
                                        UserId = userId,
                                        GroupId = "",
                                        PageId = DataChatObjectN.PageId,
                                        Name = name,
                                        IdLastMessage = DataChatObjectN.LastMessage.LastMessageClass?.Id ?? "",
                                        LastChat = DataChatObjectN
                                    };
                                }
                                else
                                {
                                    var userId = DataChatObjectN.LastMessage.LastMessageClass.ToData.UserId;
                                    checkUser = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChat?.LastMessage.LastMessageClass.ToData.UserId == userId);

                                    var name = DataChatObjectN.LastMessage.LastMessageClass.ToData.Name + "(" + DataChatObjectN.PageName + ")";
                                    Console.WriteLine(name);
                                    
                                    archiveObject = new Classes.LastChatArchive
                                    {
                                        ChatType = "page",
                                        ChatId = DataChatObjectN.ChatId,
                                        UserId = userId,
                                        GroupId = "",
                                        PageId = DataChatObjectN.PageId,
                                        Name = name,
                                        IdLastMessage = DataChatObjectN.LastMessage.LastMessageClass?.Id ?? "",
                                        LastChat = DataChatObjectN
                                    };
                                }
                            }
                            else
                            {
                                checkUser = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChat?.PageId == DataChatObjectN.PageId);
                                archiveObject = new Classes.LastChatArchive
                                {
                                    ChatType = "page",
                                    ChatId = DataChatObjectN.ChatId,
                                    UserId = "",
                                    GroupId = "",
                                    PageId = DataChatObjectN.PageId,
                                    Name = DataChatObjectN.PageName,
                                    IdLastMessage = DataChatObjectN.LastMessage.LastMessageClass?.Id ?? "",
                                    LastChat = DataChatObjectN
                                };
                            }
                            break;
                        //break;
                        case "group":
                            {
                                checkUser = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChat?.GroupId == DataChatObjectN.GroupId);
                                archiveObject = new Classes.LastChatArchive
                                {
                                    ChatType = "group",
                                    ChatId = DataChatObjectN.GroupId,
                                    UserId = "",
                                    GroupId = DataChatObjectN.GroupId,
                                    PageId = "",
                                    Name = DataChatObjectN.GroupName,
                                    IdLastMessage = DataChatObjectN.LastMessage.LastMessageClass?.Id ?? "",
                                    LastChat = DataChatObjectN
                                };
                            }
                            break;
                    }
                    if (checkUser != null)
                    {
                        checkUser.LastChat.IsArchive = isArchive;
                        checkUser.LastChat.Mute.Archive = isArchive ? "yes" : "no";
                        globalMute = checkUser.LastChat.Mute;

                        var index = mAdapter.LastChatsList.IndexOf(checkUser);
                        mAdapter.LastChatsList.Remove(checkUser);
                        mAdapter.NotifyItemRemoved(index);
                    }
                }
                else
                {
                    switch (Type)
                    {
                        case "user":
                            {
                                var mAdapter = GlobalContext?.LastChatTab?.MAdapter;
                                isArchive = !DataUserObjectOld.IsArchive;
                                DataUserObjectOld.IsArchive = isArchive;
                                globalMute = DataUserObjectOld.Mute;

                                checkUser = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastMessagesUser?.UserId == DataUserObjectOld.UserId);
                                if (checkUser?.LastMessagesUser != null)
                                {
                                    checkUser.LastMessagesUser.IsArchive = isArchive;

                                    checkUser.LastMessagesUser.Mute.Archive = isArchive ? "yes" : "no";
                                    globalMute = checkUser.LastMessagesUser.Mute;

                                    var index = mAdapter.LastChatsList.IndexOf(checkUser);
                                    mAdapter.LastChatsList.Remove(checkUser);
                                    mAdapter.NotifyItemRemoved(index);

                                    idChat = DataUserObjectOld.UserId;
                                    archiveObject = new Classes.LastChatArchive
                                    {
                                        ChatType = "user",
                                        ChatId = DataUserObjectOld.UserId,
                                        UserId = DataUserObjectOld.UserId,
                                        GroupId = "",
                                        PageId = "",
                                        Name = DataUserObjectOld.Name,
                                        IdLastMessage = DataUserObjectOld?.LastMessage?.Id ?? "",
                                        LastMessagesUser = DataUserObjectOld
                                    };
                                }
                                break;
                            }
                        case "page":
                            {
                                var mAdapter = GlobalContext?.LastPageChatsTab?.MAdapter;
                                isArchive = !PageClassObject.IsArchive;
                                PageClassObject.IsArchive = isArchive;
                                //globalMute = PageClassObject.Mute;

                                var checkPage = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChatPage?.PageId == PageClassObject.PageId && a.LastChatPage?.LastMessage?.ToData?.UserId == PageClassObject.LastMessage?.ToData?.UserId);
                                if (checkPage != null)
                                {
                                    var userAdminPage = PageClassObject.UserId;
                                    if (userAdminPage == PageClassObject.LastMessage.ToData.UserId)
                                    {
                                        var userId = PageClassObject.LastMessage.UserData?.UserId;
                                        checkUser = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChatPage?.LastMessage.UserData?.UserId == userId);
                                        if (checkUser != null)
                                        {
                                            checkUser.LastChatPage.IsArchive = isArchive;

                                            //checkUser.LastChatPage.Mute.Archive = isArchive ? "yes" : "no";
                                            //globalMute = checkUser.LastChatPage.Mute;
                                             
                                            var index = mAdapter.LastChatsList.IndexOf(checkUser);
                                            mAdapter.LastChatsList.Remove(checkUser);
                                            mAdapter.NotifyItemRemoved(index);
                                        }

                                        var name = PageClassObject.LastMessage.UserData?.Name + "(" + PageClassObject.PageName + ")";
                                        Console.WriteLine(name);

                                        idChat = PageClassObject.PageId;
                                        archiveObject = new Classes.LastChatArchive
                                        {
                                            ChatType = "page",
                                            ChatId = PageClassObject.PageId + userId,
                                            UserId = userId,
                                            GroupId = "",
                                            PageId = PageClassObject.PageId,
                                            Name = name,
                                            IdLastMessage = PageClassObject.LastMessage?.Id ?? "",
                                            LastChatPage = PageClassObject
                                        };
                                    }
                                    else
                                    {
                                        var userId = PageClassObject.LastMessage.ToData.UserId;
                                        checkUser = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChatPage?.LastMessage.ToData.UserId == userId);
                                        if (checkUser != null)
                                        {
                                            checkUser.LastChatPage.IsArchive = isArchive;

                                            //checkUser.LastChatPage.Mute.Archive = isArchive ? "yes" : "no";
                                            //globalMute = checkUser.LastChatPage.Mute;

                                            var index = mAdapter.LastChatsList.IndexOf(checkUser);
                                            mAdapter.LastChatsList.Remove(checkUser);
                                            mAdapter.NotifyItemRemoved(index);
                                        }

                                        var name = PageClassObject.LastMessage.ToData.Name + "(" + PageClassObject.PageName + ")";
                                        Console.WriteLine(name);

                                        idChat = PageClassObject.PageId;
                                        archiveObject = new Classes.LastChatArchive
                                        {
                                            ChatType = "page",
                                            ChatId = PageClassObject.PageId + userId,
                                            UserId = userId,
                                            GroupId = "",
                                            PageId = PageClassObject.PageId,
                                            Name = name,
                                            IdLastMessage = PageClassObject.LastMessage?.Id ?? "",
                                            LastChatPage = PageClassObject
                                        };
                                    }
                                }
                                break;
                            }
                        //break;
                        case "group":
                            {
                                var mAdapter = GlobalContext?.LastGroupChatsTab?.MAdapter;
                                isArchive = !DataChatObjectN.IsArchive;
                                DataChatObjectN.IsArchive = isArchive;
                                globalMute = DataChatObjectN.Mute;

                                checkUser = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChat?.GroupId == DataChatObjectN.GroupId);
                                if (checkUser?.LastChat != null)
                                {
                                    checkUser.LastChat.IsArchive = isArchive;

                                    checkUser.LastChat.Mute.Archive = isArchive ? "yes" : "no";
                                    globalMute = checkUser.LastChat.Mute;

                                    var index = mAdapter.LastChatsList.IndexOf(checkUser);
                                    mAdapter.LastChatsList.Remove(checkUser);
                                    mAdapter.NotifyItemRemoved(index);

                                    idChat = DataChatObjectN.GroupId;
                                    archiveObject = new Classes.LastChatArchive
                                    {
                                        ChatType = "group",
                                        ChatId = DataChatObjectN.GroupId,
                                        UserId = "",
                                        GroupId = DataChatObjectN.GroupId,
                                        PageId = "",
                                        Name = DataChatObjectN.GroupName,
                                        IdLastMessage = DataChatObjectN.LastMessage.LastMessageClass?.Id ?? "",
                                        LastChat = DataChatObjectN
                                    };
                                }
                                break;
                            }
                    }
                }

                if (isArchive)
                {
                    if (archiveObject != null)
                    {
                        ListUtils.ArchiveList.Add(archiveObject);

                        var sqLiteDatabase = new SqLiteDatabase();
                        sqLiteDatabase.InsertORDelete_Archive(archiveObject);

                        ArchivedActivity.GetInstance()?.GetArchivedList();

                        var dictionary = new Dictionary<string, string>
                        {
                            {"archive", "yes"},
                        };

                        if (globalMute != null)
                        {
                            dictionary.Add("call_chat", globalMute.CallChat);
                            dictionary.Add("pin", globalMute.Archive);
                            dictionary.Add("notify", globalMute.Notify);
                        }

                        if (Methods.CheckConnectivity())
                            PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Message.MuteChatsInfoAsync(idChat , Type, dictionary) });
                    }

                    ToastUtils.ShowToast(Context, Context.GetText(Resource.String.Lbl_Archive), ToastLength.Long);
                }
                else
                {
                    var checkArchive = ListUtils.ArchiveList.FirstOrDefault(a => archiveObject != null && a.ChatId == archiveObject.ChatId && a.ChatType == archiveObject.ChatType);
                    if (checkArchive != null)
                    {
                        ListUtils.ArchiveList.Remove(checkArchive);

                        var sqLiteDatabase = new SqLiteDatabase();
                        sqLiteDatabase.InsertORDelete_Archive(checkArchive);

                        ArchivedActivity.GetInstance()?.GetArchivedList();

                        var dictionary = new Dictionary<string, string>
                        {
                            {"archive", "no"}, 
                        };

                        if (globalMute != null)
                        {
                            dictionary.Add("call_chat", globalMute.CallChat);
                            dictionary.Add("pin", globalMute.Archive);
                            dictionary.Add("notify", globalMute.Notify);
                        }

                        if (Methods.CheckConnectivity())
                            PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Message.MuteChatsInfoAsync(idChat, Type, dictionary) });
                    }

                    ToastUtils.ShowToast(Context, Context.GetText(Resource.String.Lbl_UnArchive), ToastLength.Long);
                }

                GlobalContext?.LastChatTab?.ShowEmptyPage();

                Dismiss();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion

        #region MaterialDialog

        public void OnSelection(MaterialDialog dialog, View itemView, int position, string itemString)
        {
            try
            {
                string text = itemString; 
                if (text == Context.GetText(Resource.String.Lbl_Voice_call))
                { 
                    Intent intentVideoCall = new Intent(Context, typeof(TwilioVideoCallActivity));
                    switch (AppSettings.UseLibrary)
                    {
                        case SystemCall.Agora:
                            intentVideoCall = new Intent(Context, typeof(AgoraAudioCallActivity));
                            intentVideoCall.PutExtra("type", "Agora_audio_calling_start");
                            break;
                        case SystemCall.Twilio:
                            intentVideoCall = new Intent(Context, typeof(TwilioAudioCallActivity));
                            intentVideoCall.PutExtra("type", "Twilio_audio_calling_start");
                            break;
                    }

                    CallUserObject callUserObject;

                    switch (AppSettings.LastChatSystem)
                    {
                        case SystemApiGetLastChat.New:
                            callUserObject = new CallUserObject
                            {
                                UserId = DataChatObjectN.UserId,
                                Avatar = DataChatObjectN.Avatar,
                                Name = DataChatObjectN.Name,
                                Data = new CallUserObject.DataCallUser()
                            };
                            intentVideoCall.PutExtra("callUserObject", JsonConvert.SerializeObject(callUserObject)); 
                            break;
                        default:
                            callUserObject = new CallUserObject
                            {
                                UserId = DataUserObjectOld.UserId,
                                Avatar = DataUserObjectOld.Avatar,
                                Name = DataUserObjectOld.Name,
                                Data = new CallUserObject.DataCallUser()
                            };
                            intentVideoCall.PutExtra("callUserObject", JsonConvert.SerializeObject(callUserObject));
                             
                            break;
                    }
                      
                    Activity.StartActivity(intentVideoCall);
                }
                else if (text == Context.GetText(Resource.String.Lbl_Video_call))
                { 
                    Intent intentVideoCall = new Intent(Context, typeof(TwilioVideoCallActivity));
                    switch (AppSettings.UseLibrary)
                    {
                        case SystemCall.Agora:
                            intentVideoCall = new Intent(Context, typeof(AgoraVideoCallActivity));
                            intentVideoCall.PutExtra("type", "Agora_video_calling_start");
                            break;
                        case SystemCall.Twilio:
                            intentVideoCall = new Intent(Context, typeof(TwilioVideoCallActivity));
                            intentVideoCall.PutExtra("type", "Twilio_video_calling_start");
                            break;
                    }

                    CallUserObject callUserObject;

                    switch (AppSettings.LastChatSystem)
                    {
                        case SystemApiGetLastChat.New:
                            callUserObject = new CallUserObject
                            {
                                UserId = DataChatObjectN.UserId,
                                Avatar = DataChatObjectN.Avatar,
                                Name = DataChatObjectN.Name,
                                Data = new CallUserObject.DataCallUser()
                            };
                            intentVideoCall.PutExtra("callUserObject", JsonConvert.SerializeObject(callUserObject)); 
                            break;
                        default:
                            callUserObject = new CallUserObject
                            {
                                UserId = DataUserObjectOld.UserId,
                                Avatar = DataUserObjectOld.Avatar,
                                Name = DataUserObjectOld.Name,
                                Data = new CallUserObject.DataCallUser()
                            };
                            intentVideoCall.PutExtra("callUserObject", JsonConvert.SerializeObject(callUserObject)); 
                            break;
                    }
                     
                    Activity.StartActivity(intentVideoCall);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        private void LoadDataChat()
        {
            try
            {
                var page = Arguments.GetString("Page") ?? "";
                Type = Arguments.GetString("Type") ?? "";
                switch (Type)
                {
                    case "user":
                        {
                            if (AppSettings.LastChatSystem == SystemApiGetLastChat.New)
                            {
                                DataChatObjectN = JsonConvert.DeserializeObject<ChatObject>(Arguments.GetString("ItemObject") ?? "");
                                if (DataChatObjectN != null) //not read Change to read (Normal)
                                {
                                    if (DataChatObjectN.LastMessage.LastMessageClass?.Seen == "0")
                                    {
                                        ReadText.Text = Context.GetText(Resource.String.Lbl_MarkAsRead);
                                        FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, ReadIcon, IonIconsFonts.Mail);
                                    }
                                    else
                                    {
                                        ReadText.Text = Context.GetText(Resource.String.Lbl_MarkAsUnRead);
                                        FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, ReadIcon, IonIconsFonts.MailUnread);
                                    }

                                    if (DataChatObjectN.IsMute)
                                    {
                                        MuteText.Text = Context.GetText(Resource.String.Lbl_UnMuteNotification);
                                        FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, MuteIcon, IonIconsFonts.Notifications);
                                    }
                                    else
                                    {
                                        MuteText.Text = Context.GetText(Resource.String.Lbl_MuteNotification);
                                        FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, MuteIcon, IonIconsFonts.NotificationsOff);
                                    }

                                    PinText.Text = Context.GetText(DataChatObjectN.IsPin ? Resource.String.Lbl_UnPin : Resource.String.Lbl_Pin);
                                    ArchiveText.Text = Context.GetText(DataChatObjectN.IsArchive ? Resource.String.Lbl_UnArchive : Resource.String.Lbl_Archive);

                                }
                            }
                            else
                            {
                                DataUserObjectOld = JsonConvert.DeserializeObject<GetUsersListObject.User>(Arguments.GetString("ItemObject") ?? "");
                                if (DataUserObjectOld != null) //not read Change to read (Normal)
                                {
                                    if (DataUserObjectOld.LastMessage?.Seen == "0")
                                    {
                                        ReadText.Text = Context.GetText(Resource.String.Lbl_MarkAsRead);
                                        FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, ReadIcon, IonIconsFonts.Mail);
                                    }
                                    else
                                    {
                                        ReadText.Text = Context.GetText(Resource.String.Lbl_MarkAsUnRead);
                                        FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, ReadIcon, IonIconsFonts.MailUnread);
                                    }

                                    if (DataUserObjectOld.IsMute)
                                    {
                                        MuteText.Text = Context.GetText(Resource.String.Lbl_UnMuteNotification);
                                        FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, MuteIcon, IonIconsFonts.Notifications);
                                    }
                                    else
                                    {
                                        MuteText.Text = Context.GetText(Resource.String.Lbl_MuteNotification);
                                        FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, MuteIcon, IonIconsFonts.NotificationsOff);
                                    }

                                    PinText.Text = Context.GetText(DataUserObjectOld.IsPin ? Resource.String.Lbl_UnPin : Resource.String.Lbl_Pin);
                                    ArchiveText.Text = Context.GetText(DataUserObjectOld.IsArchive ? Resource.String.Lbl_UnArchive : Resource.String.Lbl_Archive);

                                }
                            }

                            GroupInfoLayout.Visibility = ViewStates.Gone;
                            ExitGroupLayout.Visibility = ViewStates.Gone;
                            AddMembersLayout.Visibility = ViewStates.Gone;

                            if (page == "Archived")
                                PinLayout.Visibility = ViewStates.Gone;
                             
                            if (AppSettings.EnableAudioVideoCall)
                            {
                                var dataSettings = ListUtils.SettingsSiteList;
                                if (dataSettings?.WhoCall == "pro") //just pro user can chat 
                                {
                                    var dataUser = ListUtils.MyProfileList?.FirstOrDefault()?.IsPro;
                                    if (dataUser == "0") // Not Pro remove call
                                    {
                                        CallLayout.Visibility = ViewStates.Gone;
                                    }
                                }
                                else //all users can chat
                                {
                                    if (dataSettings?.VideoChat == "0" || !AppSettings.EnableVideoCall)
                                    {
                                        //VideoCallButton.Visibility = ViewStates.Gone;
                                        AppSettings.EnableVideoCall = false;
                                    }

                                    if (dataSettings?.AudioChat == "0" || !AppSettings.EnableAudioCall)
                                    {
                                        //AudioCallButton.Visibility = ViewStates.Gone;
                                        AppSettings.EnableAudioCall = false;
                                    }
                                }
                            }
                            else
                            {
                                CallLayout.Visibility = ViewStates.Gone;
                            }
                             
                            break;
                        }
                    case "page":
                        {
                            if (AppSettings.LastChatSystem == SystemApiGetLastChat.New)
                            {
                                DataChatObjectN = JsonConvert.DeserializeObject<ChatObject>(Arguments.GetString("ItemObject") ?? "");

                                if (DataChatObjectN != null) //not read Change to read (Normal)  
                                {
                                    if (DataChatObjectN.LastMessage.LastMessageClass?.Seen == "0")
                                    {
                                        ReadText.Text = Context.GetText(Resource.String.Lbl_MarkAsRead);
                                        FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, ReadIcon, IonIconsFonts.Mail);
                                    }
                                    else
                                    {
                                        ReadText.Text = Context.GetText(Resource.String.Lbl_MarkAsUnRead);
                                        FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, ReadIcon, IonIconsFonts.MailUnread);
                                    }

                                    if (DataChatObjectN.IsMute)
                                    {
                                        MuteText.Text = Context.GetText(Resource.String.Lbl_UnMuteNotification);
                                        FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, MuteIcon, IonIconsFonts.Notifications);
                                    }
                                    else
                                    {
                                        MuteText.Text = Context.GetText(Resource.String.Lbl_MuteNotification);
                                        FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, MuteIcon, IonIconsFonts.NotificationsOff);
                                    }

                                    PinText.Text = Context.GetText(DataChatObjectN.IsPin ? Resource.String.Lbl_UnPin : Resource.String.Lbl_Pin);
                                    ArchiveText.Text = Context.GetText(DataChatObjectN.IsArchive ? Resource.String.Lbl_UnArchive : Resource.String.Lbl_Archive);

                                }
                            }
                            else
                            {
                                PageClassObject = JsonConvert.DeserializeObject<PageClass>(Arguments.GetString("ItemObject") ?? "");
                                if (PageClassObject != null) //not read Change to read (Normal)  
                                {
                                    if (PageClassObject.LastMessage?.Seen == "0")
                                    {
                                        ReadText.Text = Context.GetText(Resource.String.Lbl_MarkAsRead);
                                        FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, ReadIcon, IonIconsFonts.Mail);
                                    }
                                    else
                                    {
                                        ReadText.Text = Context.GetText(Resource.String.Lbl_MarkAsUnRead);
                                        FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, ReadIcon, IonIconsFonts.MailUnread);
                                    }

                                    if (PageClassObject.IsMute)
                                    {
                                        MuteText.Text = Context.GetText(Resource.String.Lbl_UnMuteNotification);
                                        FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, MuteIcon, IonIconsFonts.Notifications);
                                    }
                                    else
                                    {
                                        MuteText.Text = Context.GetText(Resource.String.Lbl_MuteNotification);
                                        FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, MuteIcon, IonIconsFonts.NotificationsOff);
                                    }

                                    PinText.Text = Context.GetText(PageClassObject.IsPin ? Resource.String.Lbl_UnPin : Resource.String.Lbl_Pin);
                                    ArchiveText.Text = Context.GetText(PageClassObject.IsArchive ? Resource.String.Lbl_UnArchive : Resource.String.Lbl_Archive);

                                }
                            }

                            BlockLayout.Visibility = ViewStates.Gone;
                            CallLayout.Visibility = ViewStates.Gone;
                            ProfileLayout.Visibility = ViewStates.Gone;
                            GroupInfoLayout.Visibility = ViewStates.Gone;
                            ExitGroupLayout.Visibility = ViewStates.Gone;
                            AddMembersLayout.Visibility = ViewStates.Gone; 

                            if (page == "Archived")
                                PinLayout.Visibility = ViewStates.Gone;

                            break;
                        }
                    case "group":
                        {
                            DataChatObjectN = JsonConvert.DeserializeObject<ChatObject>(Arguments.GetString("ItemObject") ?? "");
                            if (DataChatObjectN != null) //not read Change to read (Normal)  
                            {
                                if (DataChatObjectN.LastMessage.LastMessageClass?.Seen == "0")
                                {
                                    ReadText.Text = Context.GetText(Resource.String.Lbl_MarkAsRead);
                                    FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, ReadIcon, IonIconsFonts.Mail);
                                }
                                else
                                {
                                    ReadText.Text = Context.GetText(Resource.String.Lbl_MarkAsUnRead);
                                    FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, ReadIcon, IonIconsFonts.MailUnread);
                                }

                                if (DataChatObjectN.IsMute)
                                {
                                    MuteText.Text = Context.GetText(Resource.String.Lbl_UnMuteNotification);
                                    FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, MuteIcon, IonIconsFonts.Notifications);
                                }
                                else
                                {
                                    MuteText.Text = Context.GetText(Resource.String.Lbl_MuteNotification);
                                    FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, MuteIcon, IonIconsFonts.NotificationsOff);
                                }

                                PinText.Text = Context.GetText(DataChatObjectN.IsPin ? Resource.String.Lbl_UnPin : Resource.String.Lbl_Pin);
                                ArchiveText.Text = Context.GetText(DataChatObjectN.IsArchive ? Resource.String.Lbl_UnArchive : Resource.String.Lbl_Archive);

                                if (DataChatObjectN?.Owner != null && !DataChatObjectN.Owner.Value)
                                    AddMembersLayout.Visibility = ViewStates.Gone;
                            }

                            BlockLayout.Visibility = ViewStates.Gone;
                            CallLayout.Visibility = ViewStates.Gone;
                            ProfileLayout.Visibility = ViewStates.Gone;

                            if (page == "Archived")
                                PinLayout.Visibility = ViewStates.Gone;

                            break;
                        }
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }
    }
}