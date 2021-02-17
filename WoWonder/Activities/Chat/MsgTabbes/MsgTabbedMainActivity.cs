using System;
using System.Collections.Generic;
using System.Linq;
using AFollestad.MaterialDialogs;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;
using AndroidX.ViewPager.Widget;
using Google.Android.Material.Tabs;
using Java.Lang;
using Newtonsoft.Json;
using WoWonder.Activities.BlockedUsers;
using WoWonder.Activities.Chat.Call;
using WoWonder.Activities.Chat.ChatWindow;
using WoWonder.Activities.Chat.Floating;
using WoWonder.Activities.Chat.GroupChat;
using WoWonder.Activities.Chat.MsgTabbes.Fragment;
using WoWonder.Activities.Chat.MsgTabbes.Services;
using WoWonder.Activities.Contacts;
using WoWonder.Activities.NearBy;
using WoWonder.Activities.Search;
using WoWonder.Activities.SettingsPreferences;
using WoWonder.Activities.SettingsPreferences.General;
using WoWonder.Activities.Tabbes;
using WoWonder.Adapters;
using WoWonder.Helpers.Ads;
using WoWonder.Helpers.Controller;
using WoWonder.Helpers.Model;
using WoWonder.Helpers.Utils;
using WoWonder.SQLite;
using WoWonderClient.Classes.Message;
using Exception = System.Exception;

namespace WoWonder.Activities.Chat.MsgTabbes
{
    //[Activity(Icon = "@mipmap/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.UiMode)]
    public class MsgTabbedMainActivity : AndroidX.Fragment.App.Fragment, MaterialDialog.IListCallback
    {
        #region Variables

        public TabLayout Tabs;

        private ViewPager ViewPager;

        //private FloatingActionButton FloatingActionFilter;
        //private FrameLayout FloatingActionButtonView;
        //private ImageView FloatingActionImageView;
        private TextView TxtAppName;
        private string FloatingActionTag;
        public LastChatFragment LastChatTab; 
        public LastCallsFragment LastCallsTab;
        private static MsgTabbedMainActivity Instance;
        //private string ImageType;
        //public static ServiceResultReceiver Receiver;
        public static bool RunCall = false;
        private PowerManager.WakeLock Wl;
        //private Handler ExitHandler;
        //private bool RecentlyBackPressed;
        public LastGroupChatsFragment LastGroupChatsTab;
        public LastPageChatsFragment LastPageChatsTab;
        //ToolBar 
        private ImageView DiscoverImageView, SearchImageView, MoreImageView;

        public InitFloating Floating;
        private TabbedMainActivity GlobalContext;
        #endregion

        #region General

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your fragment here
            GlobalContext = (TabbedMainActivity)Activity ?? TabbedMainActivity.GetInstance();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                View view = inflater.Inflate(Resource.Layout.MsgTabbedMainPage, container, false);

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

                Instance = this;
                Floating = new InitFloating();
                RunCall = false;

                //Get Value And Set Toolbar
                InitComponent(view);
                //InitToolbar();

                //new Handler(Looper.MainLooper).Post(new Runnable(GetGeneralAppData));
                SetService();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public override void OnResume()
        {
            try
            {

                base.OnResume();
                AddOrRemoveEvent(true);
                SetWakeLock(); 
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public override void OnPause()
        {
            try
            {
                base.OnPause();
                AddOrRemoveEvent(false);
                OffWakeLock(); 
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        //public override void OnTrimMemory(TrimMemory level)
        //{
        //    try
        //    {
        //        GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
        //        base.OnTrimMemory(level);
        //    }
        //    catch (Exception e)
        //    {
        //        Methods.DisplayReportResultTrack(e);
        //    }
        //}

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

        public override void OnDestroy()
        {
            try
            {
               
                base.OnDestroy();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Menu

        //public override void OnBackPressed()
        //{
        //    try
        //    {
        //        ExitHandler ??= new Handler(Looper.MainLooper);
        //        if (RecentlyBackPressed)
        //        {
        //            ExitHandler.RemoveCallbacks(() => { RecentlyBackPressed = false; });
        //            RecentlyBackPressed = false;
        //            MoveTaskToBack(true);
        //            Finish();
        //        }
        //        else
        //        {
        //            RecentlyBackPressed = true;
        //            Toast.MakeText(this, GetString(Resource.String.press_again_exit), ToastLength.Long)?.Show();
        //            ExitHandler.PostDelayed(() => { RecentlyBackPressed = false; }, 2000L);
        //        }
        //    }
        //    catch (Exception exception)
        //    {
        //        Methods.DisplayReportResultTrack(exception);
        //    }
        //}

        #endregion

        #region Functions

        private void InitComponent(View view)
        {
            try
            {
                DiscoverImageView = view.FindViewById<ImageView>(Resource.Id.discoverButton);
                SearchImageView = view.FindViewById<ImageView>(Resource.Id.searchButton);
                MoreImageView = view.FindViewById<ImageView>(Resource.Id.moreButton);
                TxtAppName = view.FindViewById<TextView>(Resource.Id.appName);
                //FloatingActionFilter = FindViewById<FloatingActionButton>(Resource.Id.floatingActionFilter);

                TxtAppName.Text = GetText(Resource.String.Lbl_Tab_Chats);

                //FloatingActionButtonView = view.FindViewById<FrameLayout>(Resource.Id.FloatingAction);
                //FloatingActionImageView = view.FindViewById<ImageView>(Resource.Id.Image);
                //FloatingActionTag = "lastMessages";
                //FloatingActionButtonView.Visibility = ViewStates.Visible;
                Tabs = view.FindViewById<TabLayout>(Resource.Id.tabsLayout);
                ViewPager = view.FindViewById<ViewPager>(Resource.Id.viewpager);

                SetUpViewPager(ViewPager);
                if (ViewPager != null) Tabs.SetupWithViewPager(ViewPager);

                //var tab = Tabs.GetTabAt(0); //Lbl_Tab_Chats 
                ////set custom view
                //tab.SetCustomView(Resource.Layout.IconBadgeLayout);
                //TextView textView = (TextView)tab.CustomView.FindViewById(Resource.Id.text);
                //textView.Visibility = ViewStates.Gone;
                //var d = Color.ParseColor("");

                Tabs.SetTabTextColors(AppSettings.SetTabDarkTheme ? Color.White : Color.DimGray,
                    Color.ParseColor(AppSettings.MainColor));
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void AddOrRemoveEvent(bool addEvent)
        {
            try
            {
                // true +=  // false -=
                if (addEvent)
                {
                    //FloatingActionButtonView.Click += FloatingActionButtonView_Click;
                    //FloatingActionFilter.Click += FloatingActionFilterOnClick;
                    DiscoverImageView.Click += DiscoverImageView_Click;
                    SearchImageView.Click += SearchImageView_Click;
                    MoreImageView.Click += MoreImageView_Click;
                }
                else
                {
                    //FloatingActionButtonView.Click -= FloatingActionButtonView_Click;
                    //FloatingActionFilter.Click -= FloatingActionFilterOnClick;
                    DiscoverImageView.Click -= DiscoverImageView_Click;
                    SearchImageView.Click -= SearchImageView_Click;
                    MoreImageView.Click -= MoreImageView_Click;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public static MsgTabbedMainActivity GetInstance()
        {
            try
            {
                return Instance;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return null;
            }
        }

        #endregion

        #region Events

        private void MoreImageView_Click(object sender, EventArgs e)
        {
            try
            {
                var arrayAdapter = new List<string>();
                var dialogList = new MaterialDialog.Builder(Activity).Theme(AppSettings.SetTabDarkTheme
                    ? Theme.Dark
                    : Theme.Light);

                switch (FloatingActionTag)
                {
                    case "lastMessages" when AppSettings.LastChatSystem == SystemApiGetLastChat.Old:
                        arrayAdapter.Add(GetText(Resource.String.Lbl_Blocked_User_List));
                        arrayAdapter.Add(GetText(Resource.String.Lbl_Settings));
                        break;
                    case "lastMessages":
                        arrayAdapter.Add(GetText(Resource.String.Lbl_CreateNewGroup));
                        arrayAdapter.Add(GetText(Resource.String.Lbl_GroupRequest));
                        arrayAdapter.Add(GetText(Resource.String.Lbl_Blocked_User_List));
                        arrayAdapter.Add(GetText(Resource.String.Lbl_Settings));
                        break;
                    case "GroupChats":
                        arrayAdapter.Add(GetText(Resource.String.Lbl_CreateNewGroup));
                        arrayAdapter.Add(GetText(Resource.String.Lbl_GroupRequest));
                        arrayAdapter.Add(GetText(Resource.String.Lbl_Settings));
                        break;
                    case "PageChats":
                    case "Story":
                        arrayAdapter.Add(GetText(Resource.String.Lbl_Settings));
                        break;
                    case "Call":
                        arrayAdapter.Add(GetText(Resource.String.Lbl_Clear_call_log));
                        arrayAdapter.Add(GetText(Resource.String.Lbl_Settings));
                        break;
                }

                dialogList.Title(GetString(Resource.String.Lbl_Menu_More));
                dialogList.Items(arrayAdapter);
                dialogList.NegativeText(GetText(Resource.String.Lbl_Close)).OnNegative(new WoWonderTools.MyMaterialDialog());
                dialogList.AlwaysCallSingleChoiceCallback();
                dialogList.ItemsCallback(this).Build().Show();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void SearchImageView_Click(object sender, EventArgs e)
        {
            try
            {
                var intent = new Intent(Activity, typeof(SearchTabbedActivity));
                intent.PutExtra("Key", "");
                Activity.StartActivity(intent);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void DiscoverImageView_Click(object sender, EventArgs e)
        {
            try
            {
                Activity.StartActivity(new Intent(Activity, typeof(PeopleNearByActivity)));
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        public void FloatingActionButtonView_Click()
        {
            try
            {
                switch (FloatingActionTag)
                {
                    case "lastMessages":
                    {
                        var intent = new Intent(Activity, typeof(MyContactsActivity));
                        intent.PutExtra("ContactsType", "Following");
                        intent.PutExtra("UserId", UserDetails.UserId);
                        Activity.StartActivity(intent);
                        break;
                    }
                    case "GroupChats":
                    {
                        var intent = new Intent(Activity, typeof(CreateGroupChatActivity));
                        Activity.StartActivity(intent);
                        break;
                    } 
                    case "Call":
                        Activity.StartActivity(new Intent(Activity, typeof(AddNewCallActivity)));
                        break;
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        public void FloatingActionButtonView_Tag()
        {
            try
            {
                if (FloatingActionTag == "lastMessages")
                {
                    FloatingActionTag = "lastMessages";
                    GlobalContext.FloatingActionButton.Tag = "lastMessages";
                    GlobalContext.FloatingActionButton.SetImageResource(Resource.Drawable.ic_add_user);
                    GlobalContext.FloatingActionButton.Visibility = ViewStates.Visible;
                }
                else if (FloatingActionTag == "GroupChats")
                {
                    FloatingActionTag = "GroupChats";
                    GlobalContext.FloatingActionButton.Tag = "GroupChats";
                    GlobalContext.FloatingActionButton.SetImageResource(Resource.Drawable.ic_add);
                    GlobalContext.FloatingActionButton.Visibility = ViewStates.Visible;
                }
                else if (FloatingActionTag == "Call")
                {
                    FloatingActionTag = "Call";
                    GlobalContext.FloatingActionButton.Tag = "Call";
                    GlobalContext.FloatingActionButton.SetImageResource(Resource.Drawable.ic_phone_user);
                    GlobalContext.FloatingActionButton.Visibility = ViewStates.Visible;
                }
                else
                {
                    FloatingActionTag = "lastMessages";
                    GlobalContext.FloatingActionButton.Tag = "lastMessages";
                    GlobalContext.FloatingActionButton.SetImageResource(Resource.Drawable.ic_add_user);
                    GlobalContext.FloatingActionButton.Visibility = ViewStates.Visible;
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion

        #region Permissions && Result

        //Result
        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            try
            {
                base.OnActivityResult(requestCode, resultCode, data);

                if (requestCode == InitFloating.ChatHeadDataRequestCode && InitFloating.CanDrawOverlays(Activity))
                {
                    Floating.FloatingShow(InitFloating.FloatingObject);

                    UserDetails.ChatHead = true;
                    MainSettings.SharedData?.Edit()?.PutBoolean("chatheads_key", UserDetails.ChatHead)?.Commit();
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        //Permissions
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            try
            {
                Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

                if (requestCode == 110)
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        Activity?.Window?.AddFlags(WindowManagerFlags.KeepScreenOn);
                    }
                    else
                    {
                        Toast.MakeText(Activity, Activity.GetText(Resource.String.Lbl_Permission_is_denied), ToastLength.Long)?.Show();
                    }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Set Tab

        private void ViewPagerOnPageSelected(object sender, ViewPager.PageSelectedEventArgs e)
        {
            try
            {
                var position = e.Position;
                if (AppSettings.LastChatSystem == SystemApiGetLastChat.New)
                {
                    switch (position)
                    {
                        // lastMessages
                        case 0:
                            AdsGoogle.Ad_AppOpenManager(Activity);
                            break;
                        // Story
                        case 1:
                            AdsGoogle.Ad_RewardedVideo(Activity);
                            //LastStoriesTab.StartApiService(); 
                            break;
                        // Call
                        case 2:
                            AdsGoogle.Ad_Interstitial(Activity);
                            //LastCallsTab.Get_CallUser();
                            break;
                    }
                }
                else
                {
                    switch (position)
                    {
                        // lastMessages
                        case 0:
                            AdsGoogle.Ad_AppOpenManager(Activity);
                            break;
                        // GroupChats
                        case 1:
                            AdsGoogle.Ad_RewardedVideo(Activity);
                            //if (AppSettings.EnableChatGroup)
                            //{
                            //    LastGroupChatsTab.StartApiService();
                            //}
                            //else if (AppSettings.EnableChatPage)
                            //{
                            //    LastPageChatsTab.StartApiService();
                            //}
                            //else
                            //{
                            //    LastStoriesTab.StartApiService();
                            //}
                            break;
                        // PageChats
                        case 2:
                            AdsGoogle.Ad_Interstitial(Activity);
                            //if (AppSettings.EnableChatPage)
                            //{
                            //    LastPageChatsTab.StartApiService();
                            //}
                            //else
                            //{
                            //    LastStoriesTab.StartApiService();
                            //}
                            break;
                        // Story
                        case 3:
                            AdsGoogle.Ad_AppOpenManager(Activity);
                            //if (AppSettings.EnableChatGroup)
                            //{
                            //    LastStoriesTab.StartApiService();
                            //}
                            //else
                            //{
                            //    LastCallsTab.Get_CallUser();
                            //} 
                            break;
                        // Call
                        case 4:
                            AdsGoogle.Ad_Interstitial(Activity);
                            //LastCallsTab.Get_CallUser();
                            break;
                    }
                } 
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void ViewPagerOnPageScrolled(object sender, ViewPager.PageScrolledEventArgs e)
        {
            try
            {
                if (AppSettings.LastChatSystem == SystemApiGetLastChat.New)
                {
                    switch (e.Position)
                    {
                        // lastMessages
                        case 0:
                        {
                            if (GlobalContext.FloatingActionButton.Tag?.ToString() != "lastMessages")
                            {
                                GlobalContext.FloatingActionButton.Tag = "lastMessages";
                                GlobalContext.FloatingActionButton.SetImageResource(Resource.Drawable.icon_profile_vector);
                                GlobalContext.FloatingActionButton.Visibility = ViewStates.Visible;
                                //FloatingActionFilter.Visibility = ViewStates.Visible;
                            }

                            break;
                        } 
                        // Call
                        case 1:
                        {
                            if (GlobalContext.FloatingActionButton.Tag?.ToString() != "Call")
                            {
                                GlobalContext.FloatingActionButton.Tag = "Call";
                                GlobalContext.FloatingActionButton.SetImageResource(Resource.Drawable.ic_phone_user);
                                GlobalContext.FloatingActionButton.Visibility = ViewStates.Visible;
                                //FloatingActionFilter.Visibility = ViewStates.Invisible;

                                //if (Tabs != null)
                                //{
                                //    var tab = Tabs.GetTabAt(0); //Lbl_Tab_Chats

                                //    var textView = (TextView)tab.CustomView.FindViewById(Resource.Id.text);
                                //    textView.Visibility = ViewStates.Gone;
                                //}
                            }

                            break;
                        }
                    }
                }
                else
                {
                    switch (e.Position)
                    {
                        //FloatingActionFilter.Visibility = ViewStates.Invisible;
                        // lastMessages
                        case 0:
                        {
                            if (GlobalContext.FloatingActionButton.Tag?.ToString() != "lastMessages")
                            {
                                FloatingActionTag = "lastMessages";
                                GlobalContext.FloatingActionButton.Tag = "lastMessages";
                                GlobalContext.FloatingActionButton.SetImageResource(Resource.Drawable.ic_add_user);
                                GlobalContext.FloatingActionButton.Visibility = ViewStates.Visible;
                            }

                            break;
                        }
                        // GroupChats
                        case 1 when AppSettings.EnableChatGroup:
                        {
                            if (GlobalContext.FloatingActionButton.Tag?.ToString() != "GroupChats")
                            {
                                FloatingActionTag = "GroupChats";
                                GlobalContext.FloatingActionButton.Tag = "GroupChats";
                                GlobalContext.FloatingActionButton.SetImageResource(Resource.Drawable.ic_add);
                                GlobalContext.FloatingActionButton.Visibility = ViewStates.Visible;
                            }

                            break;
                        }
                        case 1 when AppSettings.EnableChatPage:
                        {
                            if (GlobalContext.FloatingActionButton.Tag?.ToString() != "PageChats")
                            {
                                FloatingActionTag = "PageChats";
                                GlobalContext.FloatingActionButton.Tag = "PageChats";
                                GlobalContext.FloatingActionButton.Visibility = ViewStates.Invisible;
                            }

                            break;
                        }
                        case 1:
                        {
                            if (GlobalContext.FloatingActionButton.Tag?.ToString() != "Call")
                            {
                                FloatingActionTag = "Call";
                                GlobalContext.FloatingActionButton.Tag = "Call";
                                GlobalContext.FloatingActionButton.SetImageResource(Resource.Drawable.ic_phone_user);
                                GlobalContext.FloatingActionButton.Visibility = ViewStates.Visible;
                            } 
                            break;
                        }
                        // PageChats
                        case 2 when AppSettings.EnableChatPage:
                        {
                            if (GlobalContext.FloatingActionButton.Tag?.ToString() != "PageChats")
                            {
                                FloatingActionTag = "PageChats";
                                GlobalContext.FloatingActionButton.Tag = "PageChats";
                                GlobalContext.FloatingActionButton.Visibility = ViewStates.Invisible;
                            }

                                break;
                        }
                        case 2:
                        {
                            if (GlobalContext.FloatingActionButton.Tag?.ToString() != "Call")
                            {
                                FloatingActionTag = "Call";
                                GlobalContext.FloatingActionButton.Tag = "Call";
                                GlobalContext.FloatingActionButton.SetImageResource(Resource.Drawable.ic_phone_user);
                                GlobalContext.FloatingActionButton.Visibility = ViewStates.Visible;
                            }

                                break;
                        }
                        // Story
                        case 3 when AppSettings.EnableChatPage:
                        {
                            if (GlobalContext.FloatingActionButton.Tag?.ToString() != "Call")
                            {
                                FloatingActionTag = "Call";
                                GlobalContext.FloatingActionButton.Tag = "Call";
                                GlobalContext.FloatingActionButton.SetImageResource(Resource.Drawable.ic_phone_user);
                                GlobalContext.FloatingActionButton.Visibility = ViewStates.Visible;
                            }

                                break;
                        }
                        case 3:
                        {
                            if (GlobalContext.FloatingActionButton.Tag?.ToString() != "Call")
                            {
                                FloatingActionTag = "Call";
                                GlobalContext.FloatingActionButton.Tag = "Call";
                                GlobalContext.FloatingActionButton.SetImageResource(Resource.Drawable.ic_phone_user);
                                GlobalContext.FloatingActionButton.Visibility = ViewStates.Visible;
                            }

                                break;
                        }
                        // Call
                        case 4:
                        {
                            if (GlobalContext.FloatingActionButton.Tag?.ToString() != "Call")
                            {
                                FloatingActionTag = "Call";
                                GlobalContext.FloatingActionButton.Tag = "Call";
                                GlobalContext.FloatingActionButton.SetImageResource(Resource.Drawable.ic_phone_user);
                                GlobalContext.FloatingActionButton.Visibility = ViewStates.Visible;
                            }

                                break;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void SetUpViewPager(ViewPager viewPager)
        {
            try
            {
                LastChatTab = new LastChatFragment(); 
                LastCallsTab = new LastCallsFragment();

                MainTabAdapter adapter = new MainTabAdapter(ChildFragmentManager, FragmentStatePagerAdapter.BehaviorResumeOnlyCurrentFragment);
                switch (AppSettings.LastChatSystem)
                {
                    case SystemApiGetLastChat.New:
                        adapter.AddFragment(LastChatTab, GetText(Resource.String.Lbl_Tab_Chats));
                        break;
                    case SystemApiGetLastChat.Old:
                    {
                        LastGroupChatsTab = new LastGroupChatsFragment();
                        LastPageChatsTab = new LastPageChatsFragment();

                        adapter.AddFragment(LastChatTab, GetText(Resource.String.Lbl_Tab_Chats));
                        if (AppSettings.EnableChatGroup)
                            adapter.AddFragment(LastGroupChatsTab, GetText(Resource.String.Lbl_Tab_GroupChats));

                        if (AppSettings.EnableChatPage)
                            adapter.AddFragment(LastPageChatsTab, GetText(Resource.String.Lbl_Tab_PageChats));

                        break;
                    }
                }
                 
                if (AppSettings.EnableAudioVideoCall)
                    adapter.AddFragment(LastCallsTab, GetText(Resource.String.Lbl_Tab_Calls));

                viewPager.CurrentItem = adapter.Count;
                viewPager.OffscreenPageLimit = adapter.Count;
                viewPager.Adapter = adapter;
                viewPager.Adapter.NotifyDataSetChanged();

                ViewPager.PageScrolled += ViewPagerOnPageScrolled;
                ViewPager.PageSelected += ViewPagerOnPageSelected;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region WakeLock System

        public void AddFlagsWakeLock()
        {
            try
            {
                if ((int) Build.VERSION.SdkInt < 23)
                {
                    Activity.Window.AddFlags(WindowManagerFlags.KeepScreenOn);
                }
                else
                {
                    if (Activity.CheckSelfPermission(Manifest.Permission.WakeLock) == Permission.Granted)
                    {
                        Activity.Window.AddFlags(WindowManagerFlags.KeepScreenOn);
                    }
                    else
                    {
                        //request Code 110
                        new PermissionsController(Activity).RequestPermission(110);
                    }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void SetWakeLock()
        {
            try
            {
                if (Wl == null)
                {
                    PowerManager pm = (PowerManager)Activity.GetSystemService(Context.PowerService);
                    Wl = pm?.NewWakeLock(WakeLockFlags.ScreenBright, "My Tag");
                    Wl?.Acquire();
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void SetOnWakeLock()
        {
            try
            {
                PowerManager pm = (PowerManager)Activity.GetSystemService(Context.PowerService);
                Wl = pm?.NewWakeLock(WakeLockFlags.ScreenBright, "My Tag");
                Wl?.Acquire();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void SetOffWakeLock()
        {
            try
            {
                PowerManager pm = (PowerManager)Activity.GetSystemService(Context.PowerService);
                Wl = pm?.NewWakeLock(WakeLockFlags.ProximityScreenOff, "My Tag");
                Wl?.Acquire();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void OffWakeLock()
        {
            try
            {
                // ..screen will stay on during this section..
                Wl?.Release();
                Wl = null;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Service

        private void SetService()
        {
            try
            {
                var intent = new Intent(Activity, typeof(ChatApiService));
                Activity.StartService(intent);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void OnReceiveResult(string resultData)
        {
            try
            {
                //Toast.MakeText(Application.Context, "Result got ", ToastLength.Short)?.Show();

                if (AppSettings.LastChatSystem == SystemApiGetLastChat.New)
                {
                    var result = JsonConvert.DeserializeObject<LastChatObject>(resultData);
                    if (result != null)
                    {
                        LastChatTab?.LoadDataLastChatNewV(result);
                        Activity?.RunOnUiThread(() => { LastChatFragment.LoadCall(result); });
                    }
                }
                else
                {
                    var result = JsonConvert.DeserializeObject<GetUsersListObject>(resultData);
                    if (result != null)
                    {
                        LastChatTab?.LoadDataLastChatOldV(result);
                        Activity?.RunOnUiThread(() => { LastChatFragment.LoadCall(result); });
                    }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion
          
        #region General App Data

        public void GetOneSignalNotification()
        {
            try
            {
                if (AppSettings.LastChatSystem == SystemApiGetLastChat.New)
                {
                    string userId = Activity.Intent?.GetStringExtra("UserID") ?? "Don't have type";
                    if (!string.IsNullOrEmpty(userId) && userId != "Don't have type")
                    {
                        var dataUser = LastChatTab?.MAdapter?.LastChatsList?.FirstOrDefault(a =>
                            a.LastChat?.UserId == userId && a.LastChat?.ChatType == "user");

                        Intent intent = new Intent(Activity, typeof(ChatWindowActivity));
                        intent.PutExtra("UserID", userId);

                        if (dataUser?.LastChat != null)
                        {
                            intent.PutExtra("TypeChat", "LastMessenger");
                            intent.PutExtra("ColorChat", dataUser.LastChat.ChatColor);
                            intent.PutExtra("UserItem", JsonConvert.SerializeObject(dataUser.LastChat));
                        }
                        else
                        {
                            intent.PutExtra("TypeChat", "OneSignalNotification");
                            intent.PutExtra("ColorChat", AppSettings.MainColor);
                        }

                        StartActivity(intent);
                    }
                }
                else
                {
                    string userId = Activity.Intent?.GetStringExtra("UserID") ?? "Don't have type";
                    if (!string.IsNullOrEmpty(userId) && userId != "Don't have type")
                    {
                        var dataUser =
                            LastChatTab?.MAdapter?.LastChatsList?.FirstOrDefault(a =>
                                a.LastMessagesUser.UserId == userId);

                        Intent intent = new Intent(Activity, typeof(ChatWindowActivity));
                        intent.PutExtra("UserID", userId);

                        if (dataUser?.LastMessagesUser != null)
                        {
                            intent.PutExtra("TypeChat", "LastMessenger");
                            intent.PutExtra("ColorChat", dataUser.LastMessagesUser.ChatColor);
                            intent.PutExtra("UserItem", JsonConvert.SerializeObject(dataUser.LastMessagesUser));
                        }
                        else
                        {
                            intent.PutExtra("TypeChat", "OneSignalNotification");
                            intent.PutExtra("ColorChat", AppSettings.MainColor);
                        }

                        StartActivity(intent);
                    }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
          
        #endregion

        #region MaterialDialog

        public void OnSelection(MaterialDialog p0, View p1, int itemId, ICharSequence itemString)
        {
            try
            {
                if (itemString.ToString() == GetText(Resource.String.Lbl_CreateNewGroup))
                {
                    StartActivity(new Intent(Activity, typeof(CreateGroupChatActivity)));
                }
                else if (itemString.ToString() == GetText(Resource.String.Lbl_GroupRequest))
                {
                    StartActivity(new Intent(Activity, typeof(GroupRequestActivity)));
                }
                else if (itemString.ToString() == GetText(Resource.String.Lbl_Blocked_User_List))
                {
                    StartActivity(new Intent(Activity, typeof(BlockedUsersActivity)));
                }
                else if (itemString.ToString() == GetText(Resource.String.Lbl_Settings))
                {
                    StartActivity(new Intent(Activity, typeof(GeneralAccountActivity)));
                }
                else if (itemString.ToString() == GetText(Resource.String.Lbl_Clear_call_log))
                {
                    var dialog = new MaterialDialog.Builder(Activity).Theme(AppSettings.SetTabDarkTheme
                        ? Theme.Dark
                        : Theme.Light);
                    dialog.Title(GetText(Resource.String.Lbl_Warning));
                    dialog.Content(GetText(Resource.String.Lbl_Clear_call_log));
                    dialog.PositiveText(GetText(Resource.String.Lbl_Yes)).OnPositive((materialDialog, action) =>
                    {
                        try
                        {
                            LastCallsTab?.MAdapter?.MCallUser?.Clear();
                            LastCallsTab?.MAdapter?.NotifyDataSetChanged();
                            LastCallsTab?.ShowEmptyPage();

                            //Toast.MakeText(this, GetText(Resource.String.Lbl_Done), ToastLength.Long)?.Show();

                            SqLiteDatabase dbDatabase = new SqLiteDatabase();
                            dbDatabase.Clear_CallUser_List();

                        }
                        catch (Exception e)
                        {
                            Methods.DisplayReportResultTrack(e);
                        }
                    });
                    dialog.NegativeText(GetText(Resource.String.Lbl_No)).OnNegative(new WoWonderTools.MyMaterialDialog());
                    dialog.AlwaysCallSingleChoiceCallback();
                    dialog.Build().Show();
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Chat Head

        private Dialog ChatHeadWindow;
        private static bool OpenDialog;

        public void DisplayChatHeadDialog()
        { 
            try
            {
                Activity.RunOnUiThread(() =>
                {
                    try
                    {
                        if (OpenDialog && InitFloating.CanDrawOverlays(Activity))
                            return;

                        ChatHeadWindow = new Dialog(Activity, AppSettings.SetTabDarkTheme ? Resource.Style.MyTheme_Dark_Base : Resource.Style.MyTheme_Base);
                        ChatHeadWindow.SetContentView(Resource.Layout.ChatHeadDialogLayout);

                        var subTitle1 = ChatHeadWindow.FindViewById<TextView>(Resource.Id.subTitle1);
                        var btnNotNow = ChatHeadWindow.FindViewById<TextView>(Resource.Id.notNowButton);
                        var btnGoToSettings = ChatHeadWindow.FindViewById<Button>(Resource.Id.goToSettingsButton);

                        subTitle1.Text = GetText(Resource.String.Lbl_EnableChatHead_SubTitle1) + " " + AppSettings.ApplicationName + ", " + GetText(Resource.String.Lbl_EnableChatHead_SubTitle2);

                        btnNotNow.Click += BtnNotNowOnClick;
                        btnGoToSettings.Click += BtnGoToSettingsOnClick;

                        ChatHeadWindow.Show();

                        OpenDialog = true;
                    }
                    catch (Exception e)
                    {
                        Methods.DisplayReportResultTrack(e);
                    }
                });
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void BtnGoToSettingsOnClick(object sender, EventArgs e)
        {
            try
            {
                if (!Floating.CheckPermission())
                {
                    Floating.OpenManagePermission();
                }

                if (ChatHeadWindow != null)
                {
                    ChatHeadWindow.Hide();
                    ChatHeadWindow.Dispose();
                    ChatHeadWindow = null;
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void BtnNotNowOnClick(object sender, EventArgs e)
        {
            try
            {
                if (ChatHeadWindow != null)
                {
                    ChatHeadWindow.Hide();
                    ChatHeadWindow.Dispose();
                    ChatHeadWindow = null;
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion

    }
}