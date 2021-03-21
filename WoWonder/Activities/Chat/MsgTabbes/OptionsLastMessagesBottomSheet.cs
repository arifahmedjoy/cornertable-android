using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AFollestad.MaterialDialogs;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using Google.Android.Material.BottomSheet;
using Java.Lang;
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

        private RelativeLayout ArchiveLayout, DeleteLayout, PinLayout, MuteLayout, ReadLayout, BlockLayout, CallLayout, ProfileLayout, GroupInfoLayout, ExitGroupLayout, AddMembersLayout, ReportLayout;
        private TextView ArchiveIcon, DeleteIcon, PinIcon, MuteIcon, ReadIcon, BlockIcon, CallIcon, ProfileIcon, GroupInfoIcon, ExitGroupIcon, AddMembersIcon, ReportIcon;
        private TextView ArchiveText, DeleteText, PinText, MuteText, ReadText, BlockText, CallText, ProfileText, GroupInfoText, ExitGroupText, AddMembersText, ReportText;

        private string Type;
        private ChatObject DataChatObject;
        private GetUsersListObject.User DataUserObject;
        private PageClass PageClassObject;

        #endregion

        #region General

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your fragment here
            GlobalContext = MsgTabbedMainActivity.GetInstance();
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
                return null;
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

                ReportLayout = view.FindViewById<RelativeLayout>(Resource.Id.ReportLayout);
                ReportIcon = view.FindViewById<TextView>(Resource.Id.ReportIcon);
                ReportText = view.FindViewById<TextView>(Resource.Id.ReportText);
                ReportLayout.Click += ReportLayoutOnClick;

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
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, ReportIcon, IonIconsFonts.IosBug);

                if (!AppSettings.EnableChatArchive)
                    ArchiveLayout.Visibility = ViewStates.Gone;

                if (!AppSettings.EnableChatPin)
                    PinLayout.Visibility = ViewStates.Gone;

                if (!AppSettings.EnableChatMute)
                    MuteLayout.Visibility = ViewStates.Gone;

                if (!AppSettings.EnableChatMakeAsRead)
                    ReadLayout.Visibility = ViewStates.Gone;

                if (!AppSettings.EnableChatReport)
                    ReportLayout.Visibility = ViewStates.Gone;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Event

        //Report user // wael add new api and check if Reported
        private void ReportLayoutOnClick(object sender, EventArgs e)
        {
            try
            {

                Dismiss();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Add Members to group
        private void AddMembersLayoutOnClick(object sender, EventArgs e)
        {
            try
            {
                Intent intent = new Intent(Activity, typeof(EditGroupChatActivity));
                intent.PutExtra("GroupObject", JsonConvert.SerializeObject(DataChatObject));
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
                    Toast.MakeText(Activity, Activity.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                }
                else
                {
                    var dialog = new MaterialDialog.Builder(Activity).Theme(AppSettings.SetTabDarkTheme ? AFollestad.MaterialDialogs.Theme.Dark : AFollestad.MaterialDialogs.Theme.Light);
                    dialog.Content(GetText(Resource.String.Lbl_AreYouSureExitGroup));
                    dialog.PositiveText(GetText(Resource.String.Lbl_Exit)).OnPositive(async (materialDialog, action) =>
                    {
                        try
                        {
                            //Show a progress
                            AndHUD.Shared.Show(Activity, GetText(Resource.String.Lbl_Loading));

                            var (apiStatus, respond) = await RequestsAsync.GroupChat.ExitGroupChat(DataChatObject.GroupId);
                            if (apiStatus == 200)
                            {
                                if (respond is AddOrRemoveUserToGroupObject result)
                                {
                                    Console.WriteLine(result.MessageData);

                                    Toast.MakeText(Activity, Activity.GetString(Resource.String.Lbl_GroupSuccessfullyLeaved), ToastLength.Short)?.Show();

                                    //remove item to my Group list  
                                    if (AppSettings.LastChatSystem == SystemApiGetLastChat.New)
                                    {
                                        var adapter = GlobalContext?.LastChatTab.MAdapter;
                                        var data = adapter?.LastChatsList?.FirstOrDefault(a => a.LastChat?.GroupId == DataChatObject.GroupId);
                                        if (data != null)
                                        {
                                            adapter?.LastChatsList.Remove(data);
                                            adapter?.NotifyItemRemoved(adapter.LastChatsList.IndexOf(data));
                                        }
                                    }
                                    else
                                    {
                                        var adapter = GlobalContext?.LastGroupChatsTab.MAdapter;
                                        var data = adapter?.LastChatsList?.FirstOrDefault(a => a.LastChat?.GroupId == DataChatObject.GroupId);
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
                intent.PutExtra("GroupObject", JsonConvert.SerializeObject(DataChatObject));
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
                    WoWonderTools.OpenProfile(Activity, DataChatObject.UserId, DataChatObject.UserData);
                else
                    WoWonderTools.OpenProfile(Activity, DataUserObject.UserId, DataUserObject);

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
                string timeNow = DateTime.Now.ToString("hh:mm");
                var unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                string time = Convert.ToString(unixTimestamp);

                switch (AppSettings.EnableAudioCall)
                {
                    case true when AppSettings.EnableVideoCall:
                        {
                            var arrayAdapter = new List<string>();
                            var dialogList = new MaterialDialog.Builder(Context).Theme(AppSettings.SetTabDarkTheme ? AFollestad.MaterialDialogs.Theme.Dark : AFollestad.MaterialDialogs.Theme.Light);

                            arrayAdapter.Add(Context.GetText(Resource.String.Lbl_Voice_call));
                            arrayAdapter.Add(Context.GetText(Resource.String.Lbl_Video_call));

                            dialogList.Title(GetText(Resource.String.Lbl_Call));
                            //dialogList.Content(GetText(Resource.String.Lbl_Select_Type_Call));
                            dialogList.Items(arrayAdapter);
                            dialogList.NegativeText(GetText(Resource.String.Lbl_Close)).OnNegative(new WoWonderTools.MyMaterialDialog());
                            dialogList.AlwaysCallSingleChoiceCallback();
                            dialogList.ItemsCallback(this).Build().Show();
                            break;
                        }
                    // Video Call On
                    case false when AppSettings.EnableVideoCall:
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
                                intentVideoCall.PutExtra("UserID", DataChatObject.UserId);
                                intentVideoCall.PutExtra("avatar", DataChatObject.Avatar);
                                intentVideoCall.PutExtra("name", DataChatObject.Name);
                            }
                            else
                            {
                                intentVideoCall.PutExtra("UserID", DataUserObject.UserId);
                                intentVideoCall.PutExtra("avatar", DataUserObject.Avatar);
                                intentVideoCall.PutExtra("name", DataUserObject.Name);
                            }

                            intentVideoCall.PutExtra("time", timeNow);
                            intentVideoCall.PutExtra("CallID", time);
                            intentVideoCall.PutExtra("access_token", "YOUR_TOKEN");
                            intentVideoCall.PutExtra("access_token_2", "YOUR_TOKEN");
                            intentVideoCall.PutExtra("from_id", "0");
                            intentVideoCall.PutExtra("active", "0");
                            intentVideoCall.PutExtra("status", "0");
                            intentVideoCall.PutExtra("room_name", "TestRoom");
                            Activity.StartActivity(intentVideoCall);
                        }
                        catch (Exception exception)
                        {
                            Methods.DisplayReportResultTrack(exception);
                        }

                        break;
                    // Audio Call On
                    case true when AppSettings.EnableVideoCall == false:
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
                                intentVideoCall.PutExtra("UserID", DataChatObject.UserId);
                                intentVideoCall.PutExtra("avatar", DataChatObject.Avatar);
                                intentVideoCall.PutExtra("name", DataChatObject.Name);
                            }
                            else
                            {
                                intentVideoCall.PutExtra("UserID", DataUserObject.UserId);
                                intentVideoCall.PutExtra("avatar", DataUserObject.Avatar);
                                intentVideoCall.PutExtra("name", DataUserObject.Name);
                            }

                            intentVideoCall.PutExtra("time", timeNow);
                            intentVideoCall.PutExtra("CallID", time);
                            intentVideoCall.PutExtra("access_token", "YOUR_TOKEN");
                            intentVideoCall.PutExtra("access_token_2", "YOUR_TOKEN");
                            intentVideoCall.PutExtra("from_id", "0");
                            intentVideoCall.PutExtra("active", "0");
                            intentVideoCall.PutExtra("status", "0");
                            intentVideoCall.PutExtra("room_name", "TestRoom");
                            Activity.StartActivity(intentVideoCall);
                        }
                        catch (Exception exception)
                        {
                            Methods.DisplayReportResultTrack(exception);
                        }

                        break;
                }

                Dismiss();
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
                string userId = AppSettings.LastChatSystem == SystemApiGetLastChat.New ? DataChatObject.UserId : DataUserObject.UserId;

                if (Methods.CheckConnectivity())
                {
                    var (apiStatus, respond) = await RequestsAsync.Global.Block_User(userId, true); //true >> "block" 
                    if (apiStatus == 200)
                    {
                        Methods.DisplayReportResultTrack(respond);

                        var dbDatabase = new SqLiteDatabase();
                        //dbDatabase.Insert_Or_Replace_OR_Delete_UsersContact(DataUserChat, "Delete"); 
                        dbDatabase.DeleteAllMessagesUser(UserDetails.UserId, userId);

                        Methods.Path.DeleteAll_FolderUser(userId);

                        Toast.MakeText(Context, Context.GetText(Resource.String.Lbl_Blocked_successfully), ToastLength.Short)?.Show();

                        Dismiss();
                    }
                }
                else
                {
                    Toast.MakeText(Context, Context.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                }
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
                    var seen = DataChatObject.LastMessage.LastMessageClass.Seen == "0" ? DataChatObject.LastMessage.LastMessageClass.Seen = "1" : DataChatObject.LastMessage.LastMessageClass.Seen = "0";

                    //wael add api  
                    Classes.LastChatsClass checkUser = null;
                    switch (Type)
                    {
                        case "user":
                            checkUser = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChat?.UserId == DataChatObject.UserId);
                            break;
                        case "page":
                            var checkPage = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChat?.PageId == DataChatObject.PageId && a.LastChat?.LastMessage.LastMessageClass?.ToData?.UserId == DataChatObject.LastMessage.LastMessageClass?.ToData?.UserId);
                            if (checkPage != null)
                            {
                                var userAdminPage = DataChatObject.UserId;
                                if (userAdminPage == DataChatObject.LastMessage.LastMessageClass.ToData.UserId)
                                {
                                    var userId = DataChatObject.LastMessage.LastMessageClass.UserData?.UserId;
                                    checkUser = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChat?.LastMessage.LastMessageClass.UserData?.UserId == userId);

                                    var name = DataChatObject.LastMessage.LastMessageClass.UserData?.Name + "(" + DataChatObject.PageName + ")";
                                    Console.WriteLine(name);
                                }
                                else
                                {
                                    var userId = DataChatObject.LastMessage.LastMessageClass.ToData.UserId;
                                    checkUser = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChat?.LastMessage.LastMessageClass.ToData.UserId == userId);

                                    var name = DataChatObject.LastMessage.LastMessageClass.ToData.Name + "(" + DataChatObject.PageName + ")";
                                    Console.WriteLine(name);
                                }
                            }
                            else
                                checkUser = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChat?.PageId == DataChatObject.PageId);
                            break;
                        //break;
                        case "group":
                            checkUser = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChat?.GroupId == DataChatObject.GroupId);
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
                                var seen = DataUserObject.LastMessage.Seen == "0" ? DataUserObject.LastMessage.Seen = "1" : DataUserObject.LastMessage.Seen = "0";

                                var checkUser = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastMessagesUser?.UserId == DataUserObject.UserId);
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
                                var seen = DataChatObject.LastMessage.LastMessageClass.Seen == "0" ? DataChatObject.LastMessage.LastMessageClass.Seen = "1" : DataChatObject.LastMessage.LastMessageClass.Seen = "0";

                                var checkGroup = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChat?.GroupId == DataChatObject.GroupId);
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

        //Mark As Mute/UnMute //wael
        private void MuteLayoutOnClick(object sender, EventArgs e)
        {
            try
            {
                bool isMute = false;
                Classes.OptionLastChat muteObject = null;
                if (AppSettings.LastChatSystem == SystemApiGetLastChat.New)
                {
                    var mAdapter = GlobalContext?.LastChatTab?.MAdapter;
                    isMute = !DataChatObject.IsMute;

                    //wael add api  
                    Classes.LastChatsClass checkUser = null;
                    switch (Type)
                    {
                        case "user":
                            checkUser = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChat?.UserId == DataChatObject.UserId);
                            muteObject = new Classes.OptionLastChat
                            {
                                ChatType = "user",
                                IdChat = DataChatObject.UserId,
                                UserId = DataChatObject.UserId,
                                GroupId = "",
                                PageId = "",
                                Name = DataChatObject.Name
                            };
                            break;
                        case "page":
                            var checkPage = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChat?.PageId == DataChatObject.PageId && a.LastChat?.LastMessage.LastMessageClass?.ToData?.UserId == DataChatObject.LastMessage.LastMessageClass?.ToData?.UserId);
                            if (checkPage != null)
                            {
                                var userAdminPage = DataChatObject.UserId;
                                if (userAdminPage == DataChatObject.LastMessage.LastMessageClass.ToData.UserId)
                                {
                                    var userId = DataChatObject.LastMessage.LastMessageClass.UserData?.UserId;
                                    checkUser = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChat?.LastMessage.LastMessageClass.UserData?.UserId == userId);

                                    var name = DataChatObject.LastMessage.LastMessageClass.UserData?.Name + "(" + DataChatObject.PageName + ")";
                                    Console.WriteLine(name);

                                    muteObject = new Classes.OptionLastChat
                                    {
                                        ChatType = "page",
                                        IdChat = DataChatObject.PageId + userId,
                                        UserId = userId,
                                        GroupId = "",
                                        PageId = DataChatObject.PageId,
                                        Name = name
                                    };
                                }
                                else
                                {
                                    var userId = DataChatObject.LastMessage.LastMessageClass.ToData.UserId;
                                    checkUser = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChat?.LastMessage.LastMessageClass.ToData.UserId == userId);

                                    var name = DataChatObject.LastMessage.LastMessageClass.ToData.Name + "(" + DataChatObject.PageName + ")";
                                    Console.WriteLine(name);

                                    muteObject = new Classes.OptionLastChat
                                    {
                                        ChatType = "page",
                                        IdChat = DataChatObject.PageId + userId,
                                        UserId = userId,
                                        GroupId = "",
                                        PageId = DataChatObject.PageId,
                                        Name = name
                                    };
                                }
                            }
                            else
                            {
                                checkUser = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChat?.PageId == DataChatObject.PageId);
                                muteObject = new Classes.OptionLastChat
                                {
                                    ChatType = "page",
                                    IdChat = DataChatObject.PageId,
                                    UserId = "",
                                    GroupId = "",
                                    PageId = DataChatObject.PageId,
                                    Name = DataChatObject.PageName
                                };
                            }
                            break;
                        //break;
                        case "group":
                            {
                                checkUser = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChat?.GroupId == DataChatObject.GroupId);
                                muteObject = new Classes.OptionLastChat
                                {
                                    ChatType = "group",
                                    IdChat = DataChatObject.GroupId,
                                    UserId = "",
                                    GroupId = DataChatObject.GroupId,
                                    PageId = "",
                                    Name = DataChatObject.GroupName
                                };
                            }
                            break;
                    }
                    if (checkUser != null)
                    {
                        checkUser.LastChat.IsMute = isMute;
                        mAdapter?.NotifyItemChanged(mAdapter.LastChatsList.IndexOf(checkUser), "WithoutBlobMute");
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
                                isMute = !DataUserObject.IsMute;

                                var checkUser = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastMessagesUser?.UserId == DataUserObject.UserId);
                                if (checkUser != null)
                                {
                                    checkUser.LastMessagesUser.IsMute = isMute;
                                    mAdapter?.NotifyItemChanged(mAdapter.LastChatsList.IndexOf(checkUser), "WithoutBlobMute");
                                    muteObject = new Classes.OptionLastChat
                                    {
                                        ChatType = "user",
                                        IdChat = DataUserObject.UserId,
                                        UserId = DataUserObject.UserId,
                                        GroupId = "",
                                        PageId = "",
                                        Name = DataUserObject.Name
                                    };
                                }
                                break;
                            }
                        case "page":
                            {
                                var mAdapter = GlobalContext?.LastPageChatsTab?.MAdapter;
                                isMute = !PageClassObject.IsMute;

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
                                            mAdapter?.NotifyItemChanged(mAdapter.LastChatsList.IndexOf(checkUser), "WithoutBlobMute");
                                        }

                                        var name = PageClassObject.LastMessage.UserData?.Name + "(" + PageClassObject.PageName + ")";
                                        Console.WriteLine(name);

                                        muteObject = new Classes.OptionLastChat
                                        {
                                            ChatType = "page",
                                            IdChat = PageClassObject.PageId + userId,
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
                                            mAdapter?.NotifyItemChanged(mAdapter.LastChatsList.IndexOf(checkUser), "WithoutBlobMute");
                                        }

                                        var name = PageClassObject.LastMessage.ToData.Name + "(" + PageClassObject.PageName + ")";
                                        Console.WriteLine(name);

                                        muteObject = new Classes.OptionLastChat
                                        {
                                            ChatType = "page",
                                            IdChat = PageClassObject.PageId + userId,
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
                                isMute = !DataChatObject.IsMute;

                                var checkGroup = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChat?.GroupId == DataChatObject.GroupId);
                                if (checkGroup != null)
                                {
                                    checkGroup.LastChat.IsMute = isMute;
                                    mAdapter?.NotifyItemChanged(mAdapter.LastChatsList.IndexOf(checkGroup), "WithoutBlobMute");

                                    muteObject = new Classes.OptionLastChat
                                    {
                                        ChatType = "group",
                                        IdChat = DataChatObject.GroupId,
                                        UserId = "",
                                        GroupId = DataChatObject.GroupId,
                                        PageId = "",
                                        Name = DataChatObject.GroupName
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

                    Toast.MakeText(Context, Context.GetText(Resource.String.Lbl_AddedMute), ToastLength.Long)?.Show();
                }
                else
                {
                    var checkMute = ListUtils.MuteList.FirstOrDefault(a => muteObject != null && a.IdChat == muteObject.IdChat && a.ChatType == muteObject.ChatType);
                    if (checkMute != null)
                    {
                        ListUtils.MuteList.Remove(checkMute);

                        var sqLiteDatabase = new SqLiteDatabase();
                        sqLiteDatabase.InsertORDelete_Mute(checkMute);
                    }

                    Toast.MakeText(Context, Context.GetText(Resource.String.Lbl_RemovedMute), ToastLength.Long)?.Show();
                }

                Dismiss();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Mark Pin //wael
        private void PinLayoutOnClick(object sender, EventArgs e)
        {
            try
            {
                bool isPin = false;
                Classes.OptionLastChat pinObject = null;
                if (AppSettings.LastChatSystem == SystemApiGetLastChat.New)
                {
                    var mAdapter = GlobalContext?.LastChatTab?.MAdapter;
                    isPin = !DataChatObject.IsPin;

                    //wael add api  
                    Classes.LastChatsClass checkUser = null;
                    switch (Type)
                    {
                        case "user":
                            checkUser = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChat?.UserId == DataChatObject.UserId);
                            pinObject = new Classes.OptionLastChat
                            {
                                ChatType = "user",
                                IdChat = DataChatObject.UserId,
                                UserId = DataChatObject.UserId,
                                GroupId = "",
                                PageId = "",
                                Name = DataChatObject.Name
                            };
                            break;
                        case "page":
                            var checkPage = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChat?.PageId == DataChatObject.PageId && a.LastChat?.LastMessage.LastMessageClass?.ToData?.UserId == DataChatObject.LastMessage.LastMessageClass?.ToData?.UserId);
                            if (checkPage != null)
                            {
                                var userAdminPage = DataChatObject.UserId;
                                if (userAdminPage == DataChatObject.LastMessage.LastMessageClass.ToData.UserId)
                                {
                                    var userId = DataChatObject.LastMessage.LastMessageClass.UserData?.UserId;
                                    checkUser = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChat?.LastMessage.LastMessageClass.UserData?.UserId == userId);

                                    var name = DataChatObject.LastMessage.LastMessageClass.UserData?.Name + "(" + DataChatObject.PageName + ")";
                                    Console.WriteLine(name);

                                    pinObject = new Classes.OptionLastChat
                                    {
                                        ChatType = "page",
                                        IdChat = DataChatObject.PageId + userId,
                                        UserId = userId,
                                        GroupId = "",
                                        PageId = DataChatObject.PageId,
                                        Name = name
                                    };
                                }
                                else
                                {
                                    var userId = DataChatObject.LastMessage.LastMessageClass.ToData.UserId;
                                    checkUser = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChat?.LastMessage.LastMessageClass.ToData.UserId == userId);

                                    var name = DataChatObject.LastMessage.LastMessageClass.ToData.Name + "(" + DataChatObject.PageName + ")";
                                    Console.WriteLine(name);

                                    pinObject = new Classes.OptionLastChat
                                    {
                                        ChatType = "page",
                                        IdChat = DataChatObject.PageId + userId,
                                        UserId = userId,
                                        GroupId = "",
                                        PageId = DataChatObject.PageId,
                                        Name = name
                                    };
                                }
                            }
                            else
                            {
                                checkUser = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChat?.PageId == DataChatObject.PageId);
                                pinObject = new Classes.OptionLastChat
                                {
                                    ChatType = "page",
                                    IdChat = DataChatObject.PageId,
                                    UserId = "",
                                    GroupId = "",
                                    PageId = DataChatObject.PageId,
                                    Name = DataChatObject.PageName
                                };
                            }
                            break;
                        //break;
                        case "group":
                            {
                                checkUser = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChat?.GroupId == DataChatObject.GroupId);
                                pinObject = new Classes.OptionLastChat
                                {
                                    ChatType = "group",
                                    IdChat = DataChatObject.GroupId,
                                    UserId = "",
                                    GroupId = DataChatObject.GroupId,
                                    PageId = "",
                                    Name = DataChatObject.GroupName
                                };
                            }
                            break;
                    }
                    if (checkUser != null)
                    {
                        checkUser.LastChat.IsPin = isPin;
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
                    //wael add api  
                    switch (Type)
                    {
                        case "user":
                            {
                                var mAdapter = GlobalContext?.LastChatTab?.MAdapter;
                                isPin = !DataUserObject.IsPin;

                                var checkUser = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastMessagesUser?.UserId == DataUserObject.UserId);
                                if (checkUser != null)
                                {
                                    checkUser.LastChat.IsPin = isPin;

                                    var index = mAdapter.LastChatsList.IndexOf(checkUser);
                                    if (isPin)
                                    {
                                        var checkPin = mAdapter.LastChatsList.LastOrDefault(o => o.LastMessagesUser != null && o.LastMessagesUser.IsPin);
                                        if (checkPin != null)
                                        {
                                            var toIndex = mAdapter.LastChatsList.IndexOf(checkPin) + 1;

                                            if (ListUtils.FriendRequestsList.Count > 0)
                                                toIndex++;

                                            mAdapter.LastChatsList.Move(index, toIndex);
                                            mAdapter.NotifyItemMoved(index, toIndex);
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
                                        IdChat = DataUserObject.UserId,
                                        UserId = DataUserObject.UserId,
                                        GroupId = "",
                                        PageId = "",
                                        Name = DataUserObject.Name
                                    };
                                }
                                break;
                            }
                        case "page":
                            {
                                var mAdapter = GlobalContext?.LastPageChatsTab?.MAdapter;
                                isPin = !PageClassObject.IsPin;

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

                                            var index = mAdapter.LastChatsList.IndexOf(checkUser);
                                            if (isPin)
                                            {
                                                var checkPin = mAdapter.LastChatsList.LastOrDefault(o => o.LastChatPage != null && o.LastChatPage.IsPin);
                                                if (checkPin != null)
                                                {
                                                    var toIndex = mAdapter.LastChatsList.IndexOf(checkPin) + 1;

                                                    if (ListUtils.FriendRequestsList.Count > 0)
                                                        toIndex++;

                                                    mAdapter.LastChatsList.Move(index, toIndex);
                                                    mAdapter.NotifyItemMoved(index, toIndex);
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
                                            IdChat = PageClassObject.PageId + userId,
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
                                            var index = mAdapter.LastChatsList.IndexOf(checkUser);

                                            if (isPin)
                                            {
                                                var checkPin = mAdapter.LastChatsList.LastOrDefault(o => o.LastChatPage != null && o.LastChatPage.IsPin);
                                                if (checkPin != null)
                                                {
                                                    var toIndex = mAdapter.LastChatsList.IndexOf(checkPin) + 1;

                                                    if (ListUtils.FriendRequestsList.Count > 0)
                                                        toIndex++;

                                                    mAdapter.LastChatsList.Move(index, toIndex);
                                                    mAdapter.NotifyItemMoved(index, toIndex);
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
                                            IdChat = PageClassObject.PageId + userId,
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
                                isPin = !DataChatObject.IsPin;

                                var checkGroup = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChat?.GroupId == DataChatObject.GroupId);
                                if (checkGroup?.LastChat != null)
                                {
                                    checkGroup.LastChat.IsPin = isPin;
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

                                                mAdapter.LastChatsList.Move(index, toIndex);
                                                mAdapter.NotifyItemMoved(index, toIndex);
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
                                        IdChat = DataChatObject.GroupId,
                                        UserId = "",
                                        GroupId = DataChatObject.GroupId,
                                        PageId = "",
                                        Name = DataChatObject.GroupName
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

                    Toast.MakeText(Context, Context.GetText(Resource.String.Lbl_MessagePinned), ToastLength.Long)?.Show();
                }
                else
                {
                    var checkPin = ListUtils.PinList.FirstOrDefault(a => pinObject != null && a.IdChat == pinObject.IdChat && a.ChatType == pinObject.ChatType);
                    if (checkPin != null)
                    {
                        ListUtils.PinList.Remove(checkPin);

                        var sqLiteDatabase = new SqLiteDatabase();
                        sqLiteDatabase.InsertORDelete_Pin(checkPin);
                    }

                    Toast.MakeText(Context, Context.GetText(Resource.String.Lbl_MessageUnPinned), ToastLength.Long)?.Show();
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
                var dialog = new MaterialDialog.Builder(Context).Theme(AppSettings.SetTabDarkTheme ? AFollestad.MaterialDialogs.Theme.Dark : AFollestad.MaterialDialogs.Theme.Light);
                dialog.Title(GetText(Resource.String.Lbl_DeleteTheEntireConversation));
                dialog.Content(GetText(Resource.String.Lbl_OnceYouDeleteConversation));
                dialog.PositiveText(GetText(Resource.String.Lbl_Yes)).OnPositive((materialDialog, action) =>
                {
                    try
                    {
                        if (!Methods.CheckConnectivity())
                        {
                            Toast.MakeText(Context, Context.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                            return;
                        }

                        switch (Type)
                        {
                            case "user":
                                {
                                    if (AppSettings.LastChatSystem == SystemApiGetLastChat.New)
                                    {
                                        var mAdapter = GlobalContext?.LastChatTab?.MAdapter;
                                        var userToDelete = mAdapter?.LastChatsList?.FirstOrDefault(a => a.LastChat?.UserId == DataChatObject.UserId);
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
                                        dbDatabase.Delete_LastUsersChat(DataChatObject.UserId, "user");
                                        dbDatabase.DeleteAllMessagesUser(UserDetails.UserId, DataChatObject.UserId);

                                        Methods.Path.DeleteAll_FolderUser(DataChatObject.UserId);

                                        PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Global.Delete_Conversation(DataChatObject.UserId) });
                                        Toast.MakeText(Context, Context.GetText(Resource.String.Lbl_TheConversationHasBeenDeleted), ToastLength.Long)?.Show();
                                    }
                                    else
                                    {
                                        var mAdapter = GlobalContext?.LastChatTab?.MAdapter;
                                        var userToDelete = mAdapter?.LastChatsList?.FirstOrDefault(a => a.LastMessagesUser?.UserId == DataUserObject.UserId);
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
                                        dbDatabase.Delete_LastUsersChat(DataUserObject.UserId, "user");
                                        dbDatabase.DeleteAllMessagesUser(UserDetails.UserId, DataUserObject.UserId);

                                        Methods.Path.DeleteAll_FolderUser(DataUserObject.UserId);

                                        PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Global.Delete_Conversation(DataUserObject.UserId) });
                                        Toast.MakeText(Context, Context.GetText(Resource.String.Lbl_TheConversationHasBeenDeleted), ToastLength.Long)?.Show();
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

                                        var checkPage = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChat?.PageId == DataChatObject.PageId);
                                        if (checkPage != null)
                                        {
                                            var userAdminPage = DataChatObject.UserId;
                                            if (userAdminPage == DataChatObject.LastMessage.LastMessageClass.ToData.UserId)
                                            {
                                                userId = DataChatObject.LastMessage.LastMessageClass.UserData.UserId;
                                                var data = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChat?.LastMessage.LastMessageClass.UserData.UserId == userId);
                                                if (data != null)
                                                {
                                                    mAdapter?.LastChatsList.Remove(data);
                                                    mAdapter?.NotifyItemRemoved(mAdapter.LastChatsList.IndexOf(data));
                                                }
                                            }
                                            else
                                            {
                                                userId = DataChatObject.LastMessage.LastMessageClass.ToData.UserId;
                                                var data = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChat?.LastMessage.LastMessageClass.ToData.UserId == userId);
                                                if (data != null)
                                                {
                                                    mAdapter?.LastChatsList.Remove(data);
                                                    mAdapter?.NotifyItemRemoved(mAdapter.LastChatsList.IndexOf(data));
                                                }
                                            }

                                            var dbDatabase = new SqLiteDatabase();
                                            dbDatabase.Delete_LastUsersChat(DataChatObject.PageId, "page", userId);

                                            Methods.Path.DeleteAll_FolderUser(DataChatObject.PageId);

                                            PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.PageChat.DeletePageChat(DataChatObject.PageId, userId) });
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

                                            PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.PageChat.DeletePageChat(PageClassObject.PageId, userId) });
                                        }
                                    }

                                    Toast.MakeText(Activity, Activity.GetString(Resource.String.Lbl_TheConversationHasBeenDeleted), ToastLength.Short)?.Show();

                                    break;
                                }
                            case "group":
                                {
                                    //remove item to my Group list  
                                    if (AppSettings.LastChatSystem == SystemApiGetLastChat.New)
                                    {
                                        var mAdapter = GlobalContext?.LastChatTab?.MAdapter;
                                        var data = mAdapter?.LastChatsList?.FirstOrDefault(a => a.LastChat?.GroupId == DataChatObject.GroupId);
                                        if (data != null)
                                        {
                                            mAdapter.LastChatsList.Remove(data);
                                            mAdapter.NotifyItemRemoved(mAdapter.LastChatsList.IndexOf(data));
                                        }
                                    }
                                    else
                                    {
                                        var adapter = GlobalContext?.LastGroupChatsTab.MAdapter;
                                        var data = adapter?.LastChatsList?.FirstOrDefault(a => a.LastChat?.GroupId == DataChatObject.GroupId);
                                        if (data != null)
                                        {
                                            adapter.LastChatsList.Remove(data);
                                            adapter.NotifyItemRemoved(adapter.LastChatsList.IndexOf(data));
                                        }
                                    }

                                    var dbDatabase = new SqLiteDatabase();
                                    dbDatabase.Delete_LastUsersChat(DataChatObject.GroupId, "group");
                                    dbDatabase.DeleteAllMessagesUser(UserDetails.UserId, DataChatObject.GroupId);

                                    Methods.Path.DeleteAll_FolderUser(DataChatObject.GroupId);

                                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.GroupChat.ExitGroupChat(DataChatObject.GroupId) });

                                    Toast.MakeText(Activity, Activity.GetString(Resource.String.Lbl_GroupSuccessfullyLeaved), ToastLength.Short)?.Show();
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
                Classes.LastChatArchive archiveObject = null;

                //wael add api  
                Classes.LastChatsClass checkUser = null;

                if (AppSettings.LastChatSystem == SystemApiGetLastChat.New)
                {
                    var mAdapter = GlobalContext?.LastChatTab?.MAdapter;
                    isArchive = !DataChatObject.IsArchive;
                    DataChatObject.IsArchive = isArchive;

                    switch (Type)
                    {
                        case "user":
                            checkUser = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChat?.UserId == DataChatObject.UserId);
                            archiveObject = new Classes.LastChatArchive
                            {
                                ChatType = "user",
                                IdChat = DataChatObject.UserId,
                                UserId = DataChatObject.UserId,
                                GroupId = "",
                                PageId = "",
                                Name = DataChatObject.Name,
                                IdLastMessage = DataChatObject.LastMessage.LastMessageClass?.Id ?? "",
                                LastChat = DataChatObject
                            };
                            break;
                        case "page":
                            var checkPage = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChat?.PageId == DataChatObject.PageId && a.LastChat?.LastMessage.LastMessageClass?.ToData?.UserId == DataChatObject.LastMessage.LastMessageClass?.ToData?.UserId);
                            if (checkPage != null)
                            {
                                var userAdminPage = DataChatObject.UserId;
                                if (userAdminPage == DataChatObject.LastMessage.LastMessageClass.ToData.UserId)
                                {
                                    var userId = DataChatObject.LastMessage.LastMessageClass.UserData?.UserId;
                                    checkUser = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChat?.LastMessage.LastMessageClass.UserData?.UserId == userId);

                                    var name = DataChatObject.LastMessage.LastMessageClass.UserData?.Name + "(" + DataChatObject.PageName + ")";
                                    Console.WriteLine(name);

                                    archiveObject = new Classes.LastChatArchive
                                    {
                                        ChatType = "page",
                                        IdChat = DataChatObject.PageId + userId,
                                        UserId = userId,
                                        GroupId = "",
                                        PageId = DataChatObject.PageId,
                                        Name = name,
                                        IdLastMessage = DataChatObject.LastMessage.LastMessageClass?.Id ?? "",
                                        LastChat = DataChatObject
                                    };
                                }
                                else
                                {
                                    var userId = DataChatObject.LastMessage.LastMessageClass.ToData.UserId;
                                    checkUser = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChat?.LastMessage.LastMessageClass.ToData.UserId == userId);

                                    var name = DataChatObject.LastMessage.LastMessageClass.ToData.Name + "(" + DataChatObject.PageName + ")";
                                    Console.WriteLine(name);

                                    archiveObject = new Classes.LastChatArchive
                                    {
                                        ChatType = "page",
                                        IdChat = DataChatObject.PageId + userId,
                                        UserId = userId,
                                        GroupId = "",
                                        PageId = DataChatObject.PageId,
                                        Name = name,
                                        IdLastMessage = DataChatObject.LastMessage.LastMessageClass?.Id ?? "",
                                        LastChat = DataChatObject
                                    };
                                }
                            }
                            else
                            {
                                checkUser = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChat?.PageId == DataChatObject.PageId);
                                archiveObject = new Classes.LastChatArchive
                                {
                                    ChatType = "page",
                                    IdChat = DataChatObject.PageId,
                                    UserId = "",
                                    GroupId = "",
                                    PageId = DataChatObject.PageId,
                                    Name = DataChatObject.PageName,
                                    IdLastMessage = DataChatObject.LastMessage.LastMessageClass?.Id ?? "",
                                    LastChat = DataChatObject
                                };
                            }
                            break;
                        //break;
                        case "group":
                            {
                                checkUser = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChat?.GroupId == DataChatObject.GroupId);
                                archiveObject = new Classes.LastChatArchive
                                {
                                    ChatType = "group",
                                    IdChat = DataChatObject.GroupId,
                                    UserId = "",
                                    GroupId = DataChatObject.GroupId,
                                    PageId = "",
                                    Name = DataChatObject.GroupName,
                                    IdLastMessage = DataChatObject.LastMessage.LastMessageClass?.Id ?? "",
                                    LastChat = DataChatObject
                                };
                            }
                            break;
                    }
                    if (checkUser != null)
                    {
                        checkUser.LastChat.IsArchive = isArchive;

                        var index = mAdapter.LastChatsList.IndexOf(checkUser);
                        mAdapter.LastChatsList.Remove(checkUser);
                        mAdapter.NotifyItemRemoved(index);
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
                                isArchive = !DataUserObject.IsArchive;
                                DataUserObject.IsArchive = isArchive;

                                checkUser = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastMessagesUser?.UserId == DataUserObject.UserId);
                                if (checkUser?.LastMessagesUser != null)
                                {
                                    checkUser.LastMessagesUser.IsArchive = isArchive;
                                    var index = mAdapter.LastChatsList.IndexOf(checkUser);
                                    mAdapter.LastChatsList.Remove(checkUser);
                                    mAdapter.NotifyItemRemoved(index);

                                    archiveObject = new Classes.LastChatArchive
                                    {
                                        ChatType = "user",
                                        IdChat = DataUserObject.UserId,
                                        UserId = DataUserObject.UserId,
                                        GroupId = "",
                                        PageId = "",
                                        Name = DataUserObject.Name,
                                        IdLastMessage = DataUserObject?.LastMessage?.Id ?? "",
                                        LastMessagesUser = DataUserObject
                                    };
                                }
                                break;
                            }
                        case "page":
                            {
                                var mAdapter = GlobalContext?.LastPageChatsTab?.MAdapter;
                                isArchive = !PageClassObject.IsArchive;
                                PageClassObject.IsArchive = isArchive;

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
                                            var index = mAdapter.LastChatsList.IndexOf(checkUser);
                                            mAdapter.LastChatsList.Remove(checkUser);
                                            mAdapter.NotifyItemRemoved(index);
                                        }

                                        var name = PageClassObject.LastMessage.UserData?.Name + "(" + PageClassObject.PageName + ")";
                                        Console.WriteLine(name);

                                        archiveObject = new Classes.LastChatArchive
                                        {
                                            ChatType = "page",
                                            IdChat = PageClassObject.PageId + userId,
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
                                            var index = mAdapter.LastChatsList.IndexOf(checkUser);
                                            mAdapter.LastChatsList.Remove(checkUser);
                                            mAdapter.NotifyItemRemoved(index);
                                        }

                                        var name = PageClassObject.LastMessage.ToData.Name + "(" + PageClassObject.PageName + ")";
                                        Console.WriteLine(name);

                                        archiveObject = new Classes.LastChatArchive
                                        {
                                            ChatType = "page",
                                            IdChat = PageClassObject.PageId + userId,
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
                                isArchive = !DataChatObject.IsArchive;
                                DataChatObject.IsArchive = isArchive;

                                checkUser = mAdapter?.LastChatsList.FirstOrDefault(a => a.LastChat?.GroupId == DataChatObject.GroupId);
                                if (checkUser?.LastChat != null)
                                {
                                    checkUser.LastChat.IsArchive = isArchive;
                                    var index = mAdapter.LastChatsList.IndexOf(checkUser);
                                    mAdapter.LastChatsList.Remove(checkUser);
                                    mAdapter.NotifyItemRemoved(index);

                                    archiveObject = new Classes.LastChatArchive
                                    {
                                        ChatType = "group",
                                        IdChat = DataChatObject.GroupId,
                                        UserId = "",
                                        GroupId = DataChatObject.GroupId,
                                        PageId = "",
                                        Name = DataChatObject.GroupName,
                                        IdLastMessage = DataChatObject.LastMessage.LastMessageClass?.Id ?? "",
                                        LastChat = DataChatObject
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
                    }

                    Toast.MakeText(Context, Context.GetText(Resource.String.Lbl_Archive), ToastLength.Long)?.Show();
                }
                else
                {
                    var checkArchive = ListUtils.ArchiveList.FirstOrDefault(a => archiveObject != null && a.IdChat == archiveObject.IdChat && a.ChatType == archiveObject.ChatType);
                    if (checkArchive != null)
                    {
                        ListUtils.ArchiveList.Remove(checkArchive);

                        var sqLiteDatabase = new SqLiteDatabase();
                        sqLiteDatabase.InsertORDelete_Archive(checkArchive);

                        ArchivedActivity.GetInstance()?.GetArchivedList();
                    }

                    Toast.MakeText(Context, Context.GetText(Resource.String.Lbl_UnArchive), ToastLength.Long)?.Show();
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

        public void OnSelection(MaterialDialog p0, View p1, int itemId, ICharSequence itemString)
        {
            try
            {
                string text = itemString.ToString();

                if (text == Context.GetText(Resource.String.Lbl_Voice_call))
                {
                    string timeNow = DateTime.Now.ToString("hh:mm");
                    var unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                    string time = Convert.ToString(unixTimestamp);

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

                    switch (AppSettings.LastChatSystem)
                    {
                        case SystemApiGetLastChat.New:
                            intentVideoCall.PutExtra("UserID", DataChatObject.UserId);
                            intentVideoCall.PutExtra("avatar", DataChatObject.Avatar);
                            intentVideoCall.PutExtra("name", DataChatObject.Name);
                            break;
                        default:
                            intentVideoCall.PutExtra("UserID", DataUserObject.UserId);
                            intentVideoCall.PutExtra("avatar", DataUserObject.Avatar);
                            intentVideoCall.PutExtra("name", DataUserObject.Name);
                            break;
                    }

                    intentVideoCall.PutExtra("time", timeNow);
                    intentVideoCall.PutExtra("CallID", time);
                    Activity.StartActivity(intentVideoCall);
                }
                else if (text == Context.GetText(Resource.String.Lbl_Video_call))
                {
                    string timeNow = DateTime.Now.ToString("hh:mm");
                    var unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                    string time = Convert.ToString(unixTimestamp);

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

                    switch (AppSettings.LastChatSystem)
                    {
                        case SystemApiGetLastChat.New:
                            intentVideoCall.PutExtra("UserID", DataChatObject.UserId);
                            intentVideoCall.PutExtra("avatar", DataChatObject.Avatar);
                            intentVideoCall.PutExtra("name", DataChatObject.Name);
                            break;
                        default:
                            intentVideoCall.PutExtra("UserID", DataUserObject.UserId);
                            intentVideoCall.PutExtra("avatar", DataUserObject.Avatar);
                            intentVideoCall.PutExtra("name", DataUserObject.Name);
                            break;
                    }

                    intentVideoCall.PutExtra("time", timeNow);
                    intentVideoCall.PutExtra("CallID", time);
                    intentVideoCall.PutExtra("access_token", "YOUR_TOKEN");
                    intentVideoCall.PutExtra("access_token_2", "YOUR_TOKEN");
                    intentVideoCall.PutExtra("from_id", "0");
                    intentVideoCall.PutExtra("active", "0");
                    intentVideoCall.PutExtra("status", "0");
                    intentVideoCall.PutExtra("room_name", "TestRoom");
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
                                DataChatObject = JsonConvert.DeserializeObject<ChatObject>(Arguments.GetString("ItemObject") ?? "");
                                if (DataChatObject != null) //not read Change to read (Normal)
                                {
                                    if (DataChatObject.LastMessage.LastMessageClass?.Seen == "0")
                                    {
                                        ReadText.Text = Context.GetText(Resource.String.Lbl_MarkAsRead);
                                        FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, ReadIcon, IonIconsFonts.Mail);
                                    }
                                    else
                                    {
                                        ReadText.Text = Context.GetText(Resource.String.Lbl_MarkAsUnRead);
                                        FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, ReadIcon, IonIconsFonts.MailUnread);
                                    }

                                    if (DataChatObject.IsMute)
                                    {
                                        MuteText.Text = Context.GetText(Resource.String.Lbl_UnMuteNotification);
                                        FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, MuteIcon, IonIconsFonts.Notifications);
                                    }
                                    else
                                    {
                                        MuteText.Text = Context.GetText(Resource.String.Lbl_MuteNotification);
                                        FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, MuteIcon, IonIconsFonts.NotificationsOff);
                                    }

                                    PinText.Text = Context.GetText(DataChatObject.IsPin ? Resource.String.Lbl_UnPin : Resource.String.Lbl_Pin);
                                    ArchiveText.Text = Context.GetText(DataChatObject.IsArchive ? Resource.String.Lbl_UnArchive : Resource.String.Lbl_Archive);

                                }
                            }
                            else
                            {
                                DataUserObject = JsonConvert.DeserializeObject<GetUsersListObject.User>(Arguments.GetString("ItemObject") ?? "");
                                if (DataUserObject != null) //not read Change to read (Normal)
                                {
                                    if (DataUserObject.LastMessage?.Seen == "0")
                                    {
                                        ReadText.Text = Context.GetText(Resource.String.Lbl_MarkAsRead);
                                        FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, ReadIcon, IonIconsFonts.Mail);
                                    }
                                    else
                                    {
                                        ReadText.Text = Context.GetText(Resource.String.Lbl_MarkAsUnRead);
                                        FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, ReadIcon, IonIconsFonts.MailUnread);
                                    }

                                    if (DataUserObject.IsMute)
                                    {
                                        MuteText.Text = Context.GetText(Resource.String.Lbl_UnMuteNotification);
                                        FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, MuteIcon, IonIconsFonts.Notifications);
                                    }
                                    else
                                    {
                                        MuteText.Text = Context.GetText(Resource.String.Lbl_MuteNotification);
                                        FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, MuteIcon, IonIconsFonts.NotificationsOff);
                                    }

                                    PinText.Text = Context.GetText(DataUserObject.IsPin ? Resource.String.Lbl_UnPin : Resource.String.Lbl_Pin);
                                    ArchiveText.Text = Context.GetText(DataUserObject.IsArchive ? Resource.String.Lbl_UnArchive : Resource.String.Lbl_Archive);

                                }
                            }

                            GroupInfoLayout.Visibility = ViewStates.Gone;
                            ExitGroupLayout.Visibility = ViewStates.Gone;
                            AddMembersLayout.Visibility = ViewStates.Gone;

                            if (page == "Archived")
                                PinLayout.Visibility = ViewStates.Gone;

                            break;
                        }
                    case "page":
                        {
                            if (AppSettings.LastChatSystem == SystemApiGetLastChat.New)
                            {
                                DataChatObject = JsonConvert.DeserializeObject<ChatObject>(Arguments.GetString("ItemObject") ?? "");

                                if (DataChatObject != null) //not read Change to read (Normal)  
                                {
                                    if (DataChatObject.LastMessage.LastMessageClass?.Seen == "0")
                                    {
                                        ReadText.Text = Context.GetText(Resource.String.Lbl_MarkAsRead);
                                        FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, ReadIcon, IonIconsFonts.Mail);
                                    }
                                    else
                                    {
                                        ReadText.Text = Context.GetText(Resource.String.Lbl_MarkAsUnRead);
                                        FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, ReadIcon, IonIconsFonts.MailUnread);
                                    }

                                    if (DataChatObject.IsMute)
                                    {
                                        MuteText.Text = Context.GetText(Resource.String.Lbl_UnMuteNotification);
                                        FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, MuteIcon, IonIconsFonts.Notifications);
                                    }
                                    else
                                    {
                                        MuteText.Text = Context.GetText(Resource.String.Lbl_MuteNotification);
                                        FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, MuteIcon, IonIconsFonts.NotificationsOff);
                                    }

                                    PinText.Text = Context.GetText(DataChatObject.IsPin ? Resource.String.Lbl_UnPin : Resource.String.Lbl_Pin);
                                    ArchiveText.Text = Context.GetText(DataChatObject.IsArchive ? Resource.String.Lbl_UnArchive : Resource.String.Lbl_Archive);

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
                            ReportLayout.Visibility = ViewStates.Gone;

                            if (page == "Archived")
                                PinLayout.Visibility = ViewStates.Gone;

                            break;
                        }
                    case "group":
                        {
                            DataChatObject = JsonConvert.DeserializeObject<ChatObject>(Arguments.GetString("ItemObject") ?? "");
                            if (DataChatObject != null) //not read Change to read (Normal)  
                            {
                                if (DataChatObject.LastMessage.LastMessageClass?.Seen == "0")
                                {
                                    ReadText.Text = Context.GetText(Resource.String.Lbl_MarkAsRead);
                                    FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, ReadIcon, IonIconsFonts.Mail);
                                }
                                else
                                {
                                    ReadText.Text = Context.GetText(Resource.String.Lbl_MarkAsUnRead);
                                    FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, ReadIcon, IonIconsFonts.MailUnread);
                                }

                                if (DataChatObject.IsMute)
                                {
                                    MuteText.Text = Context.GetText(Resource.String.Lbl_UnMuteNotification);
                                    FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, MuteIcon, IonIconsFonts.Notifications);
                                }
                                else
                                {
                                    MuteText.Text = Context.GetText(Resource.String.Lbl_MuteNotification);
                                    FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, MuteIcon, IonIconsFonts.NotificationsOff);
                                }

                                PinText.Text = Context.GetText(DataChatObject.IsPin ? Resource.String.Lbl_UnPin : Resource.String.Lbl_Pin);
                                ArchiveText.Text = Context.GetText(DataChatObject.IsArchive ? Resource.String.Lbl_UnArchive : Resource.String.Lbl_Archive);

                                if (DataChatObject?.Owner != null && !DataChatObject.Owner.Value)
                                    AddMembersLayout.Visibility = ViewStates.Gone;
                            }

                            BlockLayout.Visibility = ViewStates.Gone;
                            CallLayout.Visibility = ViewStates.Gone;
                            ProfileLayout.Visibility = ViewStates.Gone;
                            ReportLayout.Visibility = ViewStates.Gone;

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