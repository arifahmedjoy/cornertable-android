using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using AndroidX.SwipeRefreshLayout.Widget;
using Bumptech.Glide.Util;
using Newtonsoft.Json;
using WoWonder.Activities.Chat.Call;
using WoWonder.Activities.Chat.ChatWindow;
using WoWonder.Activities.Chat.Floating;
using WoWonder.Activities.Chat.GroupChat;
using WoWonder.Activities.Chat.MsgTabbes.Adapter;
using WoWonder.Activities.Chat.PageChat;
using WoWonder.Activities.FriendRequest;
using WoWonder.Helpers.Controller;
using WoWonder.Helpers.Model;
using WoWonder.Helpers.Utils;
using WoWonder.Library.Anjo.IntegrationRecyclerView;
using WoWonder.SQLite;
using WoWonderClient.Classes.Global;
using WoWonderClient.Classes.Message;
using WoWonderClient.Requests;
using Exception = System.Exception;

namespace WoWonder.Activities.Chat.MsgTabbes.Fragment
{
    public class LastChatFragment : AndroidX.Fragment.App.Fragment
    {
        #region Variables Basic

        public LastChatsAdapter MAdapter;
        private SwipeRefreshLayout SwipeRefreshLayout;
        private RecyclerView MRecycler;
        private LinearLayoutManager LayoutManager;
        private RecyclerViewOnScrollListener MainScrollEvent;
        private MsgTabbedMainActivity GlobalContext;
        public static bool ApiRun;

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
                View view = inflater.Inflate(Resource.Layout.TLastMessagesLayout, container, false);
                return view;
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
                return null;
            }
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            try
            {
                base.OnViewCreated(view, savedInstanceState);

                InitComponent(view);
                SetRecyclerViewAdapters();

                LoadChat();

                MsgTabbedMainActivity.GetInstance()?.GetOneSignalNotification();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public override void OnLowMemory()
        {
            try
            {
                GC.Collect(GC.MaxGeneration);
                base.OnLowMemory();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion

        #region Functions

        private void InitComponent(View view)
        {
            try
            {
                MRecycler = (RecyclerView)view.FindViewById(Resource.Id.recyler);

                SwipeRefreshLayout = (SwipeRefreshLayout)view.FindViewById(Resource.Id.swipeRefreshLayout);
                SwipeRefreshLayout.SetColorSchemeResources(Android.Resource.Color.HoloBlueLight, Android.Resource.Color.HoloGreenLight, Android.Resource.Color.HoloOrangeLight, Android.Resource.Color.HoloRedLight);
                SwipeRefreshLayout.Refreshing = false;
                SwipeRefreshLayout.Enabled = true;
                SwipeRefreshLayout.SetProgressBackgroundColorSchemeColor(AppSettings.SetTabDarkTheme ? Color.ParseColor("#424242") : Color.ParseColor("#f7f7f7"));
                SwipeRefreshLayout.Refresh += SwipeRefreshLayoutOnRefresh;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void SetRecyclerViewAdapters()
        {
            try
            {
                MAdapter = new LastChatsAdapter(Activity, "user") { LastChatsList = new ObservableCollection<Classes.LastChatsClass>() };
                MAdapter.ItemClick += MAdapterOnItemClick;
                MAdapter.ItemLongClick += MAdapterOnItemLongClick;

                LayoutManager = new LinearLayoutManager(Activity);
                MRecycler.SetLayoutManager(LayoutManager);
                MRecycler.HasFixedSize = true;
                MRecycler.SetItemViewCacheSize(50);
                MRecycler.GetLayoutManager().ItemPrefetchEnabled = true;
                MRecycler.SetItemAnimator(null);

                var sizeProvider = new ViewPreloadSizeProvider();
                var preLoader = new RecyclerViewPreloader<ChatObject>(Activity, MAdapter, sizeProvider, 10);
                MRecycler.AddOnScrollListener(preLoader);
                MRecycler.SetAdapter(MAdapter);

                RecyclerViewOnScrollListener xamarinRecyclerViewOnScrollListener = new RecyclerViewOnScrollListener(LayoutManager);
                MainScrollEvent = xamarinRecyclerViewOnScrollListener;
                MainScrollEvent.LoadMoreEvent += MainScrollEventOnLoadMoreEvent;
                MRecycler.AddOnScrollListener(xamarinRecyclerViewOnScrollListener);
                MainScrollEvent.IsLoading = false;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Events 

        //Scroll
        private void MainScrollEventOnLoadMoreEvent(object sender, EventArgs e)
        {
            try
            {
                //Code get last id where LoadMore >>
                if (AppSettings.LastChatSystem == SystemApiGetLastChat.New)
                {
                    var item = MAdapter.LastChatsList.LastOrDefault(a => a.Type == Classes.ItemType.LastChatNewV);
                    if (item?.LastChat != null && !string.IsNullOrEmpty(item?.LastChat?.ChatTime) && !MainScrollEvent.IsLoading)
                        LoadChatAsync(item?.LastChat?.ChatTime).ConfigureAwait(false);

                    //PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => LoadChatAsync(item?.LastChat?.ChatTime) });
                }
                else
                {
                    var item = MAdapter.LastChatsList.LastOrDefault(a => a.Type == Classes.ItemType.LastChatOldV);
                    if (item?.LastMessagesUser != null && !string.IsNullOrEmpty(item?.LastMessagesUser?.UserId) && !MainScrollEvent.IsLoading)
                        LoadChatAsync(item?.LastMessagesUser?.UserId).ConfigureAwait(false);

                    //PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => LoadChatAsync(item?.LastMessagesUser?.UserId) });
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Refresh
        private void SwipeRefreshLayoutOnRefresh(object sender, EventArgs e)
        {
            try
            {
                MainScrollEvent.IsLoading = false;
                ApiRun = false;

                if (!Methods.CheckConnectivity())
                {
                    Toast.MakeText(Context, Context.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();

                    if (SwipeRefreshLayout.Refreshing)
                        SwipeRefreshLayout.Refreshing = false;
                }
                else
                {
                    if (AppSettings.LastChatSystem == SystemApiGetLastChat.New)
                    {
                        MAdapter.LastChatsList.Clear();
                        MAdapter.NotifyDataSetChanged();
                        ListUtils.UserList.Clear();
                    }
                    else
                    {
                        MAdapter.LastChatsList.Clear();
                        MAdapter.NotifyDataSetChanged();
                        ListUtils.UserChatList.Clear();
                    }

                    SqLiteDatabase dbDatabase = new SqLiteDatabase();
                    dbDatabase.ClearAll_LastUsersChat();
                    //dbDatabase.ClearAll_Messages();

                    StartApiService();
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void MAdapterOnItemClick(object sender, LastChatsAdapterClickEventArgs e)
        {
            try
            {
                var position = e.Position;
                if (position >= 0)
                {
                    var item = MAdapter.GetItem(position);
                    if (item != null)
                    {
                        switch (item.Type)
                        {
                            case Classes.ItemType.LastChatOldV:
                                {
                                    Activity?.RunOnUiThread(() =>
                                    {
                                        try
                                        {
                                            if (item.LastMessagesUser.LastMessage != null && item.LastMessagesUser.LastMessage.Seen == "0" && item.LastMessagesUser.LastMessage.ToId == UserDetails.UserId && item.LastMessagesUser.LastMessage.FromId != UserDetails.UserId)
                                            {
                                                item.LastMessagesUser.LastMessage.Seen = "1";
                                                MAdapter.NotifyItemChanged(position);
                                            }
                                        }
                                        catch (Exception exception)
                                        {
                                            Methods.DisplayReportResultTrack(exception);
                                        }
                                    });

                                    item.LastMessagesUser.ChatColor ??= AppSettings.MainColor;

                                    var mainChatColor = item.LastMessagesUser.ChatColor.Contains("rgb") ? Methods.FunString.ConvertColorRgBtoHex(item.LastMessagesUser.ChatColor) : item.LastMessagesUser.ChatColor ?? AppSettings.MainColor;

                                    Intent intent = new Intent(Context, typeof(ChatWindowActivity));
                                    intent.PutExtra("UserID", item.LastMessagesUser.UserId);
                                    intent.PutExtra("TypeChat", "LastMessenger");
                                    intent.PutExtra("ShowEmpty", "no");
                                    intent.PutExtra("ColorChat", mainChatColor);
                                    intent.PutExtra("UserItem", JsonConvert.SerializeObject(item.LastMessagesUser));
                                    StartActivity(intent);
                                    break;
                                }
                            case Classes.ItemType.LastChatNewV:
                                {
                                    Activity?.RunOnUiThread(() =>
                                    {
                                        try
                                        {
                                            if (item.LastChat.LastMessage.LastMessageClass != null && item.LastChat.LastMessage.LastMessageClass.Seen == "0" && item.LastChat.LastMessage.LastMessageClass.ToId == UserDetails.UserId && item.LastChat.LastMessage.LastMessageClass.FromId != UserDetails.UserId)
                                            {
                                                item.LastChat.LastMessage.LastMessageClass.Seen = "1";
                                                MAdapter.NotifyItemChanged(position);
                                            }
                                        }
                                        catch (Exception exception)
                                        {
                                            Methods.DisplayReportResultTrack(exception);
                                        }
                                    });

                                    Intent intent = null;
                                    switch (item.LastChat.ChatType)
                                    {
                                        case "user":
                                            item.LastChat.LastMessage.LastMessageClass.ChatColor ??= AppSettings.MainColor;

                                            var mainChatColor = item.LastChat.LastMessage.LastMessageClass.ChatColor.Contains("rgb") ? Methods.FunString.ConvertColorRgBtoHex(item.LastChat.LastMessage.LastMessageClass.ChatColor) : item.LastChat.LastMessage.LastMessageClass.ChatColor ?? AppSettings.MainColor;

                                            intent = new Intent(Context, typeof(ChatWindowActivity));
                                            intent.PutExtra("UserID", item.LastChat.UserId);
                                            intent.PutExtra("TypeChat", "LastMessenger");
                                            intent.PutExtra("ShowEmpty", "no");
                                            intent.PutExtra("ColorChat", mainChatColor);
                                            intent.PutExtra("UserItem", JsonConvert.SerializeObject(item.LastChat));
                                            break;
                                        case "page":
                                            intent = new Intent(Context, typeof(PageChatWindowActivity));
                                            intent.PutExtra("PageId", item.LastChat.PageId);
                                            intent.PutExtra("ShowEmpty", "no");
                                            intent.PutExtra("TypeChat", "");
                                            intent.PutExtra("PageObject", JsonConvert.SerializeObject(item.LastChat));
                                            break;
                                        case "group":
                                            intent = new Intent(Context, typeof(GroupChatWindowActivity));
                                            intent.PutExtra("GroupObject", JsonConvert.SerializeObject(item.LastChat));
                                            intent.PutExtra("ShowEmpty", "no");
                                            intent.PutExtra("GroupId", item.LastChat.GroupId);
                                            break;
                                    }
                                    StartActivity(intent);
                                    break;
                                }
                            case Classes.ItemType.FriendRequest:
                                {
                                    if (item.UserRequestList.Count > 0)
                                    {
                                        var intent = new Intent(Context, typeof(FriendRequestActivity));
                                        Context.StartActivity(intent);
                                    }

                                    break;
                                }
                            case Classes.ItemType.GroupRequest:
                                {
                                    if (item.GroupRequestList.Count > 0)
                                    {
                                        var intent = new Intent(Context, typeof(GroupRequestActivity));
                                        Context.StartActivity(intent);
                                    }

                                    break;
                                }
                            case Classes.ItemType.Archive:
                                {
                                    var intent = new Intent(Context, typeof(ArchivedActivity));
                                    Context.StartActivity(intent);
                                    break;
                                }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void MAdapterOnItemLongClick(object sender, LastChatsAdapterClickEventArgs e)
        {
            try
            {
                var position = e.Position;
                if (position >= 0)
                {
                    var item = MAdapter.GetItem(position);
                    if (item != null)
                    {
                        switch (item.Type)
                        {
                            case Classes.ItemType.LastChatOldV:
                                {
                                    OptionsLastMessagesBottomSheet bottomSheet = new OptionsLastMessagesBottomSheet();
                                    Bundle bundle = new Bundle();
                                    bundle.PutString("Type", "user");
                                    bundle.PutString("ItemObject", JsonConvert.SerializeObject(item.LastMessagesUser));
                                    bottomSheet.Arguments = bundle;
                                    bottomSheet.Show(ChildFragmentManager, bottomSheet.Tag);
                                    break;
                                }
                            case Classes.ItemType.LastChatNewV:
                                {
                                    OptionsLastMessagesBottomSheet bottomSheet = new OptionsLastMessagesBottomSheet();
                                    Bundle bundle = new Bundle();
                                    switch (item.LastChat.ChatType)
                                    {
                                        case "user":
                                            bundle.PutString("Type", "user");
                                            bundle.PutString("ItemObject", JsonConvert.SerializeObject(item.LastChat));
                                            break;
                                        case "page":
                                            bundle.PutString("Type", "page");
                                            bundle.PutString("ItemObject", JsonConvert.SerializeObject(item.LastChat));
                                            break;
                                        case "group":
                                            bundle.PutString("Type", "group");
                                            bundle.PutString("ItemObject", JsonConvert.SerializeObject(item.LastChat));
                                            break;
                                    }
                                    bottomSheet.Arguments = bundle;
                                    bottomSheet.Show(ChildFragmentManager, bottomSheet.Tag);
                                    break;
                                }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion

        #region Load Chat

        private void LoadChat()
        {
            try
            {
                if (AppSettings.LastChatSystem == SystemApiGetLastChat.New)
                {
                    if (ListUtils.UserList.Count > 0)
                    {
                        foreach (var chatObject in ListUtils.UserList)
                        {
                            if (!chatObject.IsArchive)
                            {
                                var item = WoWonderTools.FilterDataLastChatNewV(chatObject);

                                MAdapter.LastChatsList.Add(new Classes.LastChatsClass
                                {
                                    LastChat = item,
                                    Type = Classes.ItemType.LastChatNewV
                                });
                            }
                        }

                        MAdapter.NotifyDataSetChanged();
                        Activity?.RunOnUiThread(ShowEmptyPage);
                    }
                }
                else
                {
                    if (ListUtils.UserChatList.Count > 0)
                    {
                        foreach (var chatObject in ListUtils.UserChatList)
                        {
                            if (!chatObject.IsArchive)
                            {
                                var item = WoWonderTools.FilterDataLastChatOldV(chatObject);

                                MAdapter.LastChatsList.Add(new Classes.LastChatsClass
                                {
                                    LastMessagesUser = item,
                                    Type = Classes.ItemType.LastChatOldV
                                });
                            }
                        }

                        MAdapter.NotifyDataSetChanged();
                        Activity?.RunOnUiThread(ShowEmptyPage);
                    }
                }

                if (MAdapter.LastChatsList.Count == 0)
                {
                    SwipeRefreshLayout.Refreshing = true;
                    StartApiService();
                }
                else
                {
                    if (Methods.CheckConnectivity())
                        PollyController.RunRetryPolicyFunction(new List<Func<Task>> { LoadGeneralData });
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void StartApiService(string offset = "0")
        {
            if (Methods.CheckConnectivity())
                PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => LoadChatAsync(offset), LoadGeneralData });
            else
                Toast.MakeText(Context, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Long)?.Show();
        }

        private async Task LoadChatAsync(string offset = "0")
        {
            if (MainScrollEvent.IsLoading)
                return;

            if (Methods.CheckConnectivity())
            {
                ApiRun = true;
                MainScrollEvent.IsLoading = true;
                var countList = MAdapter.LastChatsList.Count;

                if (AppSettings.LastChatSystem == SystemApiGetLastChat.New)
                {
                    var fetch = "users";
                    if (AppSettings.EnableChatGroup)
                        fetch += ",groups";

                    if (AppSettings.EnableChatPage)
                        fetch += ",pages";

                    var (apiStatus, respond) = await RequestsAsync.Message.GetChatAsync(fetch, "", offset, "20", offset, "20", offset, "20");
                    if (apiStatus != 200 || respond is not LastChatObject result || result.Data == null)
                    {
                        ApiRun = false;
                        MainScrollEvent.IsLoading = false;
                        Methods.DisplayReportResult(Activity, respond);
                    }
                    else
                    {
                        var respondList = result.Data.Count;
                        if (respondList > 0)
                        {
                            if (countList > 0)
                            {
                                LoadMoreLastChatNewV(result);
                            }
                            else
                            {
                                LoadDataLastChatNewV(result);
                            }
                        }
                        else
                        {
                            Activity?.RunOnUiThread(() =>
                            {
                                if (MAdapter.LastChatsList.Count > 10 && !MRecycler.CanScrollVertically(1))
                                    Toast.MakeText(Context, GetText(Resource.String.Lbl_No_more_users), ToastLength.Short)?.Show();
                            });

                        }

                        Activity?.RunOnUiThread(() => { LoadCall(result); });
                    }
                }
                else
                {
                    var (apiStatus, respond) = await RequestsAsync.Message.GetusersListAsync(UserDetails.UserId, UserDetails.UserId, "20", offset, UserDetails.OnlineUsers);
                    if (apiStatus != 200 || respond is not GetUsersListObject result || result.Users == null)
                    {
                        MainScrollEvent.IsLoading = false;
                        Methods.DisplayReportResult(Activity, respond);
                    }
                    else
                    {
                        var respondList = result.Users.Count;
                        if (respondList > 0)
                        {
                            if (countList > 0)
                            {
                                LoadMoreLastChatOldV(result);
                            }
                            else
                            {
                                LoadDataLastChatOldV(result);
                            }
                        }
                        else
                        {
                            Activity?.RunOnUiThread(() =>
                            {
                                if (MAdapter.LastChatsList.Count > 10 && !MRecycler.CanScrollVertically(1))
                                    Toast.MakeText(Context, GetText(Resource.String.Lbl_No_more_users), ToastLength.Short)?.Show();
                            });
                        }

                        Activity?.RunOnUiThread(() => { LoadCall(result); });
                    }
                }

                Activity?.RunOnUiThread(ShowEmptyPage);
            }
        }

        public void LoadDataLastChatNewV(LastChatObject result)
        {
            try
            {
                var countList = MAdapter.LastChatsList.Count;

                var respondList = result.Data?.Count;
                if (respondList > 0)
                {
                    if (countList > 0)
                    {
                        result.Data = result.Data.OrderBy(o => o.ChatTime).ToList();

                        foreach (var itemChatObject in result.Data)
                        {
                            var item = WoWonderTools.FilterDataLastChatNewV(itemChatObject);

                            Classes.LastChatsClass checkUser = null;
                            Classes.LastChatArchive archiveObject = null;
                            int index = -1;
                            switch (item.ChatType)
                            {
                                case "user":
                                    {
                                        checkUser = MAdapter.LastChatsList.FirstOrDefault(a => a.LastChat?.UserId == item.UserId);
                                        if (checkUser != null)
                                            index = MAdapter.LastChatsList.IndexOf(checkUser);

                                        //wael change after add in api 
                                        item.IsMute = WoWonderTools.CheckMute(item.UserId, "user");
                                        item.IsPin = WoWonderTools.CheckPin(item.UserId, "user");
                                        archiveObject = WoWonderTools.CheckArchive(item.UserId, "user");
                                        item.IsArchive = archiveObject != null;
                                    }
                                    break;
                                case "page":
                                    var checkPage = MAdapter.LastChatsList.FirstOrDefault(a => a.LastChat?.PageId == item.PageId && a.LastChat?.LastMessage.LastMessageClass?.ToData?.UserId == item.LastMessage.LastMessageClass?.ToData?.UserId);
                                    if (checkPage != null)
                                    {
                                        var userAdminPage = item.UserId;
                                        if (userAdminPage == item.LastMessage.LastMessageClass?.ToData?.UserId)
                                        {
                                            var userId = item.LastMessage.LastMessageClass?.UserData?.UserId;
                                            checkUser = MAdapter.LastChatsList.FirstOrDefault(a => a.LastChat?.LastMessage.LastMessageClass?.UserData?.UserId == userId);

                                            if (checkUser != null)
                                                index = MAdapter.LastChatsList.IndexOf(checkUser);

                                            var name = item.LastMessage.LastMessageClass?.UserData?.Name + "(" + item.PageName + ")";
                                            Console.WriteLine(name);

                                            //wael change after add in api 
                                            item.IsMute = WoWonderTools.CheckMute(item.PageId + userId, "page");
                                            item.IsPin = WoWonderTools.CheckPin(item.PageId + userId, "page");
                                            archiveObject = WoWonderTools.CheckArchive(item.PageId + userId, "page");
                                            item.IsArchive = archiveObject != null;
                                        }
                                        else
                                        {
                                            var userId = item.LastMessage.LastMessageClass?.ToData?.UserId;
                                            checkUser = MAdapter.LastChatsList.FirstOrDefault(a => a.LastChat?.LastMessage.LastMessageClass?.ToData?.UserId == userId);

                                            if (checkUser != null)
                                                index = MAdapter.LastChatsList.IndexOf(checkUser);

                                            var name = item.LastMessage.LastMessageClass?.ToData?.Name + "(" + item.PageName + ")";
                                            Console.WriteLine(name);

                                            //wael change after add in api 
                                            item.IsMute = WoWonderTools.CheckMute(item.PageId + userId, "page");
                                            item.IsPin = WoWonderTools.CheckPin(item.PageId + userId, "page");
                                            archiveObject = WoWonderTools.CheckArchive(item.PageId + userId, "page");
                                            item.IsArchive = archiveObject != null;
                                        }
                                    }
                                    else
                                    {
                                        checkUser = MAdapter.LastChatsList.FirstOrDefault(a => a.LastChat?.PageId == item.PageId);

                                        if (checkUser != null)
                                            index = MAdapter.LastChatsList.IndexOf(checkUser);

                                        //wael change after add in api 
                                        item.IsMute = WoWonderTools.CheckMute(item.PageId, "page");
                                        item.IsPin = WoWonderTools.CheckPin(item.PageId, "page");
                                        archiveObject = WoWonderTools.CheckArchive(item.PageId, "page");
                                        item.IsArchive = archiveObject != null;
                                    }
                                    break;
                                case "group":
                                    {
                                        checkUser = MAdapter.LastChatsList.FirstOrDefault(a => a.LastChat?.GroupId == item.GroupId);

                                        if (checkUser != null)
                                            index = MAdapter.LastChatsList.IndexOf(checkUser);

                                        //wael change after add in api 
                                        item.IsMute = WoWonderTools.CheckMute(item.GroupId, "group");
                                        item.IsPin = WoWonderTools.CheckPin(item.GroupId, "group");
                                        archiveObject = WoWonderTools.CheckArchive(item.GroupId, "group");
                                        item.IsArchive = archiveObject != null;
                                    }
                                    break;
                            }

                            if (checkUser == null)
                            {
                                Activity?.RunOnUiThread(() =>
                                {
                                    try
                                    {
                                        if (!item.IsArchive)
                                        {
                                            var checkPin = MAdapter.LastChatsList.LastOrDefault(o => o.LastChat != null && o.LastChat.IsPin);
                                            if (checkPin != null)
                                            {
                                                var toIndex = MAdapter.LastChatsList.IndexOf(checkPin) + 1;

                                                MAdapter.LastChatsList.Insert(toIndex, new Classes.LastChatsClass
                                                {
                                                    LastChat = item,
                                                    Type = Classes.ItemType.LastChatNewV
                                                });
                                                MAdapter.NotifyItemInserted(toIndex);
                                                MRecycler.ScrollToPosition(toIndex);
                                            }
                                            else
                                            {
                                                if (ListUtils.FriendRequestsList.Count > 0)
                                                {
                                                    MAdapter.LastChatsList.Insert(1, new Classes.LastChatsClass
                                                    {
                                                        LastChat = item,
                                                        Type = Classes.ItemType.LastChatNewV
                                                    });
                                                    MAdapter.NotifyItemInserted(1);
                                                    MRecycler.ScrollToPosition(1);
                                                }
                                                else
                                                {
                                                    MAdapter.LastChatsList.Insert(0, new Classes.LastChatsClass
                                                    {
                                                        LastChat = item,
                                                        Type = Classes.ItemType.LastChatNewV
                                                    });
                                                    MAdapter.NotifyItemInserted(0);
                                                    MRecycler.ScrollToPosition(0);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (archiveObject != null)
                                            {
                                                if (archiveObject.LastChat.LastMessage.LastMessageClass?.Id != item.LastMessage.LastMessageClass?.Id)
                                                {
                                                    var checkPin = MAdapter.LastChatsList.LastOrDefault(o => o.LastChat != null && o.LastChat.IsPin);
                                                    if (checkPin != null)
                                                    {
                                                        var toIndex = MAdapter.LastChatsList.IndexOf(checkPin) + 1;

                                                        MAdapter.LastChatsList.Insert(toIndex, new Classes.LastChatsClass
                                                        {
                                                            LastChat = item,
                                                            Type = Classes.ItemType.LastChatNewV
                                                        });
                                                        MAdapter.NotifyItemInserted(toIndex);
                                                        MRecycler.ScrollToPosition(toIndex);
                                                    }
                                                    else
                                                    {
                                                        if (ListUtils.FriendRequestsList.Count > 0)
                                                        {
                                                            MAdapter.LastChatsList.Insert(1, new Classes.LastChatsClass
                                                            {
                                                                LastChat = item,
                                                                Type = Classes.ItemType.LastChatNewV
                                                            });
                                                            MAdapter.NotifyItemInserted(1);
                                                            MRecycler.ScrollToPosition(1);
                                                        }
                                                        else
                                                        {
                                                            MAdapter.LastChatsList.Insert(0, new Classes.LastChatsClass
                                                            {
                                                                LastChat = item,
                                                                Type = Classes.ItemType.LastChatNewV
                                                            });
                                                            MAdapter.NotifyItemInserted(0);
                                                            MRecycler.ScrollToPosition(0);
                                                        }
                                                    }

                                                    ListUtils.ArchiveList.Remove(archiveObject);

                                                    var sqLiteDatabase = new SqLiteDatabase();
                                                    sqLiteDatabase.InsertORDelete_Archive(archiveObject);
                                                }
                                            }
                                        }

                                        if (item.LastMessage.LastMessageClass?.FromId != UserDetails.UserId && !item.IsMute)
                                        {
                                            var floating = new FloatingObject
                                            {
                                                ChatType = item.ChatType,
                                                UserId = item.UserId,
                                                PageId = item.PageId,
                                                GroupId = item.GroupId,
                                                Avatar = item.Avatar,
                                                ChatColor = "",
                                                LastSeen = item.LastseenStatus,
                                                LastSeenUnixTime = item.LastseenUnixTime,
                                                Name = item.Name,
                                                MessageCount = item.LastMessage.LastMessageClass?.MessageCount ?? "1"
                                            };

                                            switch (item.ChatType)
                                            {
                                                case "user":
                                                    floating.Name = item.Name;
                                                    break;
                                                case "page":
                                                    var userAdminPage = item.UserId;
                                                    if (userAdminPage == item.LastMessage.LastMessageClass?.ToData?.UserId)
                                                    {
                                                        floating.Name = item.LastMessage.LastMessageClass?.UserData.Name + "(" + item.PageName + ")";
                                                    }
                                                    else
                                                    {
                                                        floating.Name = item.LastMessage.LastMessageClass?.ToData?.Name + "(" + item.PageName + ")";
                                                    }
                                                    break;
                                                case "group":
                                                    floating.Name = item.GroupName;
                                                    break;
                                            }

                                            if (UserDetails.ChatHead && InitFloating.CanDrawOverlays(Context) && Methods.AppLifecycleObserver.AppState == "Background")
                                                GlobalContext?.Floating?.FloatingShow(floating);
                                            //else if (!InitFloating.CanDrawOverlays(this))
                                            //    DisplayChatHeadDialog();
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        Methods.DisplayReportResultTrack(e);
                                    }
                                });
                            }
                            else
                            {
                                Activity?.RunOnUiThread(() =>
                                {
                                    try
                                    {
                                        if (item.LastMessage.LastMessageClass == null)
                                            return;

                                        if (checkUser.LastChat.LastMessage.LastMessageClass?.Text != item.LastMessage.LastMessageClass?.Text || checkUser.LastChat.LastMessage.LastMessageClass?.Media != item.LastMessage.LastMessageClass?.Media)
                                        {
                                            checkUser.LastChat.LastMessage = item.LastMessage;

                                            switch (index)
                                            {
                                                case > 0 when checkUser.LastChat.ChatType == item.ChatType:
                                                    {
                                                        if (!item.IsPin)
                                                        {
                                                            var checkPin = MAdapter.LastChatsList.LastOrDefault(o => o.LastChat != null && o.LastChat.IsPin);
                                                            if (checkPin != null)
                                                            {
                                                                var toIndex = MAdapter.LastChatsList.IndexOf(checkPin) + 1;

                                                                MAdapter.LastChatsList.Move(index, toIndex);
                                                                MAdapter.NotifyItemMoved(index, toIndex);
                                                                MAdapter.NotifyItemChanged(toIndex, "WithoutBlobText");
                                                            }
                                                            else
                                                            {
                                                                if (ListUtils.FriendRequestsList.Count > 0)
                                                                {
                                                                    MAdapter.LastChatsList.Move(index, 1);
                                                                    MAdapter.NotifyItemMoved(index, 1);
                                                                    MAdapter.NotifyItemChanged(1, "WithoutBlobText");
                                                                }
                                                                else
                                                                {
                                                                    MAdapter.LastChatsList.Move(index, 0);
                                                                    MAdapter.NotifyItemMoved(index, 0);
                                                                    MAdapter.NotifyItemChanged(0, "WithoutBlobText");
                                                                }
                                                            }
                                                        }

                                                        if (item.LastMessage.LastMessageClass.FromId != UserDetails.UserId && !item.IsMute)
                                                        {
                                                            var floating = new FloatingObject
                                                            {
                                                                ChatType = item.ChatType,
                                                                UserId = item.UserId,
                                                                PageId = item.PageId,
                                                                GroupId = item.GroupId,
                                                                Avatar = item.Avatar,
                                                                ChatColor = "",
                                                                LastSeen = item.Lastseen,
                                                                LastSeenUnixTime = item.LastseenUnixTime,
                                                                Name = item.Name,
                                                                MessageCount = item.LastMessage.LastMessageClass.MessageCount ?? "1"
                                                            };

                                                            switch (item.ChatType)
                                                            {
                                                                case "user":
                                                                    floating.Name = item.Name;
                                                                    break;
                                                                case "page":
                                                                    var userAdminPage = item.UserId;
                                                                    if (userAdminPage == item.LastMessage.LastMessageClass.ToData?.UserId)
                                                                    {
                                                                        floating.Name = item.LastMessage.LastMessageClass.UserData?.Name + "(" + item.PageName + ")";
                                                                    }
                                                                    else
                                                                    {
                                                                        floating.Name = item.LastMessage.LastMessageClass.ToData?.Name + "(" + item.PageName + ")";
                                                                    }
                                                                    break;
                                                                case "group":
                                                                    floating.Name = item.GroupName;
                                                                    break;
                                                            }

                                                            if (UserDetails.ChatHead && InitFloating.CanDrawOverlays(Context) && Methods.AppLifecycleObserver.AppState == "Background")
                                                                GlobalContext?.Floating?.FloatingShow(floating);
                                                            //else if (!InitFloating.CanDrawOverlays(this))
                                                            //    DisplayChatHeadDialog();
                                                        }

                                                        break;
                                                    }
                                                case 0 when checkUser.LastChat.ChatType == item.ChatType:
                                                    MAdapter.NotifyItemChanged(index, "WithoutBlobText");
                                                    break;
                                            }
                                        }
                                        else if (checkUser.LastChat.LastseenStatus?.ToLower() != item.LastseenStatus?.ToLower())
                                        {
                                            checkUser.LastChat.ChatTime = item.ChatTime;
                                            checkUser.LastChat.LastseenTimeText = item.LastseenTimeText;
                                            checkUser.LastChat.LastseenStatus = item.LastseenStatus;

                                            if (index > -1 && checkUser.LastChat.ChatType == item.ChatType || checkUser.LastChat.ChatTime != item.ChatTime)
                                                MAdapter.NotifyItemChanged(index, "WithoutBlobLastSeen");
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        Methods.DisplayReportResultTrack(e);
                                    }
                                });
                            }
                        }
                    }
                    else
                    {
                        foreach (var itemChatObject in result.Data.Where(chatObject => !chatObject.IsArchive))
                        {
                            var item = WoWonderTools.FilterDataLastChatNewV(itemChatObject);

                            MAdapter.LastChatsList.Add(new Classes.LastChatsClass
                            {
                                LastChat = item,
                                Type = Classes.ItemType.LastChatNewV
                            });
                        }

                        Activity?.RunOnUiThread(() => { MAdapter.NotifyDataSetChanged(); });
                    }
                }

                Activity?.RunOnUiThread(ShowEmptyPage);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                Activity?.RunOnUiThread(ShowEmptyPage);
            }
        }

        private void LoadMoreLastChatNewV(LastChatObject result)
        {
            try
            {
                var countList = MAdapter.LastChatsList.Count;

                var respondList = result.Data?.Count;
                if (respondList > 0)
                {
                    bool add = false;

                    foreach (var itemChatObject in result.Data)
                    {
                        Classes.LastChatsClass checkUser = null;
                        Classes.LastChatArchive archiveObject = null;

                        var item = WoWonderTools.FilterDataLastChatNewV(itemChatObject);

                        switch (item.ChatType)
                        {
                            case "user":
                                checkUser = MAdapter.LastChatsList.FirstOrDefault(a => a.LastChat?.UserId == item.UserId);
                                //wael change after add in api 
                                item.IsMute = WoWonderTools.CheckMute(item.UserId, "user");
                                item.IsPin = WoWonderTools.CheckPin(item.UserId, "user");
                                archiveObject = WoWonderTools.CheckArchive(item.UserId, "user");
                                item.IsArchive = archiveObject != null;
                                break;
                            case "page":
                                var checkPage = MAdapter.LastChatsList.FirstOrDefault(a => a.LastChat?.PageId == item.PageId && a.LastChat?.LastMessage.LastMessageClass?.ToData?.UserId == item.LastMessage.LastMessageClass?.ToData?.UserId);
                                if (checkPage != null)
                                {
                                    var userAdminPage = item.UserId;
                                    if (userAdminPage == item.LastMessage.LastMessageClass?.ToData?.UserId)
                                    {
                                        var userId = item.LastMessage.LastMessageClass?.UserData?.UserId;
                                        checkUser = MAdapter.LastChatsList.FirstOrDefault(a => a.LastChat?.LastMessage.LastMessageClass?.UserData?.UserId == userId);

                                        var name = item.LastMessage.LastMessageClass?.UserData?.Name + "(" + item.PageName + ")";
                                        Console.WriteLine(name);

                                        //wael change after add in api 
                                        item.IsMute = WoWonderTools.CheckMute(item.PageId + userId, "page");
                                        item.IsPin = WoWonderTools.CheckPin(item.PageId + userId, "page");
                                        archiveObject = WoWonderTools.CheckArchive(item.PageId + userId, "page");
                                        item.IsArchive = archiveObject != null;
                                    }
                                    else
                                    {
                                        var userId = item.LastMessage.LastMessageClass?.ToData?.UserId;
                                        checkUser = MAdapter.LastChatsList.FirstOrDefault(a => a.LastChat?.LastMessage.LastMessageClass?.ToData?.UserId == userId);

                                        var name = item.LastMessage.LastMessageClass?.ToData?.Name + "(" + item.PageName + ")";
                                        Console.WriteLine(name);

                                        //wael change after add in api 
                                        item.IsMute = WoWonderTools.CheckMute(item.PageId + userId, "page");
                                        item.IsPin = WoWonderTools.CheckPin(item.PageId + userId, "page");
                                        archiveObject = WoWonderTools.CheckArchive(item.PageId + userId, "page");
                                        item.IsArchive = archiveObject != null;
                                    }
                                }
                                else
                                {
                                    checkUser = MAdapter.LastChatsList.FirstOrDefault(a => a.LastChat?.PageId == item.PageId);
                                    //wael change after add in api 
                                    item.IsMute = WoWonderTools.CheckMute(item.PageId, "page");
                                    item.IsPin = WoWonderTools.CheckPin(item.PageId, "page");
                                    archiveObject = WoWonderTools.CheckArchive(item.PageId, "page");
                                    item.IsArchive = archiveObject != null;
                                }
                                break;
                            case "group":
                                {
                                    checkUser = MAdapter.LastChatsList.FirstOrDefault(a => a.LastChat?.GroupId == item.GroupId);
                                    //wael change after add in api 
                                    item.IsMute = WoWonderTools.CheckMute(item.GroupId, "group");
                                    item.IsPin = WoWonderTools.CheckPin(item.GroupId, "group");
                                    archiveObject = WoWonderTools.CheckArchive(item.GroupId, "group");
                                    item.IsArchive = archiveObject != null;
                                }
                                break;
                        }

                        if (checkUser != null)
                            continue;

                        if (!item.IsArchive)
                        {
                            add = true;
                            MAdapter.LastChatsList.Add(new Classes.LastChatsClass
                            {
                                Type = Classes.ItemType.LastChatNewV,
                                LastChat = item
                            });
                        }
                        else
                        {
                            if (archiveObject != null)
                            {
                                if (archiveObject.LastChat.LastMessage.LastMessageClass?.Id != item.LastMessage.LastMessageClass?.Id)
                                {
                                    add = true;
                                    MAdapter.LastChatsList.Add(new Classes.LastChatsClass
                                    {
                                        Type = Classes.ItemType.LastChatNewV,
                                        LastChat = item
                                    });

                                    ListUtils.ArchiveList.Remove(archiveObject);

                                    var sqLiteDatabase = new SqLiteDatabase();
                                    sqLiteDatabase.InsertORDelete_Archive(archiveObject);
                                }
                            }
                            else
                            {
                                add = true;
                                MAdapter.LastChatsList.Add(new Classes.LastChatsClass
                                {
                                    Type = Classes.ItemType.LastChatNewV,
                                    LastChat = item
                                });
                            }
                        }

                        if (add)
                            Activity?.RunOnUiThread(() => { MAdapter.NotifyItemRangeInserted(countList, MAdapter.LastChatsList.Count - countList); });
                    }
                }

                Activity?.RunOnUiThread(ShowEmptyPage);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                Activity?.RunOnUiThread(ShowEmptyPage);
            }
        }

        //===============================================================

        public void LoadDataLastChatOldV(GetUsersListObject result)
        {
            try
            {
                var countList = MAdapter.LastChatsList.Count;

                var respondList = result.Users?.Count;
                if (respondList > 0)
                {
                    if (countList > 0)
                    {
                        foreach (var itemChatObject in result.Users)
                        {
                            var item = WoWonderTools.FilterDataLastChatOldV(itemChatObject);
                            var archiveObject = WoWonderTools.CheckArchive(item.UserId, "user");

                            var checkUser = MAdapter.LastChatsList.FirstOrDefault(a => a.LastMessagesUser?.UserId == item.UserId);
                            if (checkUser == null)
                            {
                                //checkUser.ChatTime = item.ChatTime;

                                Activity?.RunOnUiThread(() =>
                                {
                                    try
                                    {
                                        if (!item.IsArchive)
                                        {
                                            var checkPin = MAdapter.LastChatsList.LastOrDefault(o => o.LastMessagesUser != null && o.LastMessagesUser.IsPin);
                                            if (checkPin != null)
                                            {
                                                var toIndex = MAdapter.LastChatsList.IndexOf(checkPin) + 1;

                                                MAdapter.LastChatsList.Insert(toIndex, new Classes.LastChatsClass
                                                {
                                                    LastMessagesUser = item,
                                                    Type = Classes.ItemType.LastChatOldV
                                                });
                                                MAdapter.NotifyItemInserted(toIndex);
                                                MRecycler.ScrollToPosition(toIndex);
                                            }
                                            else
                                            {
                                                if (ListUtils.FriendRequestsList.Count > 0)
                                                {
                                                    MAdapter.LastChatsList.Insert(1, new Classes.LastChatsClass
                                                    {
                                                        LastMessagesUser = item,
                                                        Type = Classes.ItemType.LastChatOldV
                                                    });
                                                    MAdapter.NotifyItemInserted(1);
                                                    MRecycler.ScrollToPosition(1);
                                                }
                                                else
                                                {
                                                    MAdapter.LastChatsList.Insert(0, new Classes.LastChatsClass
                                                    {
                                                        LastMessagesUser = item,
                                                        Type = Classes.ItemType.LastChatOldV
                                                    });
                                                    MAdapter.NotifyItemInserted(0);
                                                    MRecycler.ScrollToPosition(0);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (archiveObject != null)
                                            {
                                                if (archiveObject.LastMessagesUser.LastMessage?.Id != item.LastMessage.Id)
                                                {
                                                    var checkPin = MAdapter.LastChatsList.LastOrDefault(o => o.LastMessagesUser != null && o.LastMessagesUser.IsPin);
                                                    if (checkPin != null)
                                                    {
                                                        var toIndex = MAdapter.LastChatsList.IndexOf(checkPin) + 1;

                                                        MAdapter.LastChatsList.Insert(toIndex, new Classes.LastChatsClass
                                                        {
                                                            LastMessagesUser = item,
                                                            Type = Classes.ItemType.LastChatOldV
                                                        });
                                                        MAdapter.NotifyItemInserted(toIndex);
                                                        MRecycler.ScrollToPosition(toIndex);
                                                    }
                                                    else
                                                    {
                                                        if (ListUtils.FriendRequestsList.Count > 0)
                                                        {
                                                            MAdapter.LastChatsList.Insert(1, new Classes.LastChatsClass
                                                            {
                                                                LastMessagesUser = item,
                                                                Type = Classes.ItemType.LastChatOldV
                                                            });
                                                            MAdapter.NotifyItemInserted(1);
                                                            MRecycler.ScrollToPosition(1);
                                                        }
                                                        else
                                                        {
                                                            MAdapter.LastChatsList.Insert(0, new Classes.LastChatsClass
                                                            {
                                                                LastMessagesUser = item,
                                                                Type = Classes.ItemType.LastChatOldV
                                                            });
                                                            MAdapter.NotifyItemInserted(0);
                                                            MRecycler.ScrollToPosition(0);
                                                        }
                                                    }

                                                    ListUtils.ArchiveList.Remove(archiveObject);

                                                    var sqLiteDatabase = new SqLiteDatabase();
                                                    sqLiteDatabase.InsertORDelete_Archive(archiveObject);
                                                }
                                            }
                                        }

                                        if (item.LastMessage.FromId != UserDetails.UserId && !item.IsMute)
                                        {
                                            var floating = new FloatingObject
                                            {
                                                ChatType = "user",
                                                UserId = item.UserId,
                                                PageId = "",
                                                GroupId = "",
                                                Avatar = item.Avatar,
                                                ChatColor = item.ChatColor,
                                                LastSeen = item.Lastseen,
                                                LastSeenUnixTime = item.LastseenUnixTime,
                                                Name = item.Name,
                                                MessageCount = "1"
                                            };

                                            if (UserDetails.ChatHead && InitFloating.CanDrawOverlays(Context) && Methods.AppLifecycleObserver.AppState == "Background")
                                                GlobalContext?.Floating?.FloatingShow(floating);
                                            //else if (!InitFloating.CanDrawOverlays(this))
                                            //    DisplayChatHeadDialog();
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        Methods.DisplayReportResultTrack(e);
                                    }
                                });
                            }
                            else
                            {
                                var index = MAdapter.LastChatsList.IndexOf(checkUser);
                                if (checkUser.LastMessagesUser.LastMessage.Text != item.LastMessage.Text || checkUser.LastMessagesUser.LastMessage.Media != item.LastMessage.Media)
                                {
                                    checkUser.LastMessagesUser = item;
                                    if (index > -1)
                                    {
                                        Activity?.RunOnUiThread(() =>
                                        {
                                            try
                                            {
                                                if (!item.IsPin)
                                                {
                                                    var checkPin = MAdapter.LastChatsList.LastOrDefault(o => o.LastMessagesUser != null && o.LastMessagesUser.IsPin);
                                                    if (checkPin != null)
                                                    {
                                                        var toIndex = MAdapter.LastChatsList.IndexOf(checkPin) + 1;

                                                        MAdapter.LastChatsList.Move(index, toIndex);
                                                        MAdapter.NotifyItemMoved(index, toIndex);
                                                        MAdapter.NotifyItemChanged(toIndex, "WithoutBlobText");
                                                    }
                                                    else
                                                    {
                                                        if (ListUtils.FriendRequestsList.Count > 0)
                                                        {
                                                            MAdapter.LastChatsList.Move(index, 1);
                                                            MAdapter.NotifyItemMoved(index, 1);
                                                            MAdapter.NotifyItemChanged(1, "WithoutBlobText");
                                                        }
                                                        else
                                                        {
                                                            MAdapter.LastChatsList.Move(index, 0);
                                                            MAdapter.NotifyItemMoved(index, 0);
                                                            MAdapter.NotifyItemChanged(0, "WithoutBlobText");
                                                        }
                                                    }
                                                }

                                                if (item.LastMessage.FromId != UserDetails.UserId && !item.IsMute)
                                                {
                                                    var floating = new FloatingObject
                                                    {
                                                        ChatType = "user",
                                                        UserId = item.UserId,
                                                        PageId = "",
                                                        GroupId = "",
                                                        Avatar = item.Avatar,
                                                        ChatColor = item.ChatColor ?? AppSettings.MainColor,
                                                        LastSeen = item.Lastseen,
                                                        LastSeenUnixTime = item.LastseenUnixTime,
                                                        Name = item.Name,
                                                        MessageCount = "1"
                                                    };

                                                    if (UserDetails.ChatHead && InitFloating.CanDrawOverlays(Context) && Methods.AppLifecycleObserver.AppState == "Background")
                                                        GlobalContext?.Floating?.FloatingShow(floating);
                                                    //else if (!InitFloating.CanDrawOverlays(this))
                                                    //    DisplayChatHeadDialog();
                                                }
                                            }
                                            catch (Exception e)
                                            {
                                                Methods.DisplayReportResultTrack(e);
                                            }
                                        });
                                    }
                                }
                                else if (checkUser.LastMessagesUser.Lastseen?.ToLower() != item.Lastseen?.ToLower() || checkUser.LastMessagesUser.LastseenUnixTime != item.LastseenUnixTime)
                                {
                                    checkUser.LastMessagesUser.Lastseen = item.Lastseen;
                                    checkUser.LastMessagesUser.LastseenTimeText = item.LastseenTimeText;

                                    MAdapter.NotifyItemChanged(index, "WithoutBlobLastSeen");
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var itemChatObject in result.Users.Where(chatObject => !chatObject.IsArchive))
                        {
                            var item = WoWonderTools.FilterDataLastChatOldV(itemChatObject);

                            MAdapter.LastChatsList.Add(new Classes.LastChatsClass
                            {
                                LastMessagesUser = item,
                                Type = Classes.ItemType.LastChatOldV
                            });
                        }

                        Activity?.RunOnUiThread(() => { MAdapter.NotifyDataSetChanged(); });
                    }
                }

                Activity?.RunOnUiThread(ShowEmptyPage);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                Activity?.RunOnUiThread(ShowEmptyPage);
            }
        }

        private void LoadMoreLastChatOldV(GetUsersListObject result)
        {
            try
            {
                var countList = MAdapter.LastChatsList.Count;
                var respondList = result.Users?.Count;
                if (respondList > 0)
                {
                    foreach (var itemChatObject in result.Users)
                    {
                        var item = WoWonderTools.FilterDataLastChatOldV(itemChatObject);
                        var archiveObject = WoWonderTools.CheckArchive(item.UserId, "user");

                        var user = MAdapter.LastChatsList.FirstOrDefault(a => a.LastMessagesUser != null && a.LastMessagesUser?.UserId == item.UserId);
                        if (user == null)
                        {
                            if (!item.IsArchive)
                            {
                                MAdapter.LastChatsList.Add(new Classes.LastChatsClass
                                {
                                    LastMessagesUser = item,
                                    Type = Classes.ItemType.LastChatOldV
                                });
                            }
                            else
                            {
                                if (archiveObject != null)
                                {
                                    if (archiveObject.LastMessagesUser.LastMessage?.Id != item.LastMessage.Id)
                                    {
                                        MAdapter.LastChatsList.Add(new Classes.LastChatsClass
                                        {
                                            LastMessagesUser = item,
                                            Type = Classes.ItemType.LastChatOldV
                                        });

                                        ListUtils.ArchiveList.Remove(archiveObject);

                                        var sqLiteDatabase = new SqLiteDatabase();
                                        sqLiteDatabase.InsertORDelete_Archive(archiveObject);
                                    }
                                }
                                else
                                {
                                    MAdapter.LastChatsList.Add(new Classes.LastChatsClass
                                    {
                                        LastMessagesUser = item,
                                        Type = Classes.ItemType.LastChatOldV
                                    });
                                }
                            }
                        }
                    }
                    Activity?.RunOnUiThread(() => { MAdapter.NotifyItemRangeInserted(countList, MAdapter.LastChatsList.Count - countList); });
                }

                Activity?.RunOnUiThread(ShowEmptyPage);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                Activity?.RunOnUiThread(ShowEmptyPage);
            }
        }

        public static void LoadCall(dynamic respond)
        {
            try
            {
                if (respond == null || !AppSettings.EnableAudioVideoCall || MsgTabbedMainActivity.RunCall || VideoAudioComingCallActivity.IsActive)
                    return;

                bool twilioVideoCall, twilioAudioCall, agoraCall;
                string typeCalling = "";
                CallUserObject callUser = null;

                switch (respond)
                {
                    case GetUsersListObject usersObject:
                        switch (AppSettings.UseLibrary)
                        {
                            case SystemCall.Twilio:
                                {
                                    twilioVideoCall = usersObject.VideoCall ?? false;
                                    twilioAudioCall = usersObject.AudioCall ?? false;

                                    if (twilioVideoCall && AppSettings.EnableVideoCall)
                                    {
                                        typeCalling = "Twilio_video_call";
                                        callUser = usersObject.VideoCallUser?.CallUserClass;
                                    }
                                    else if (twilioAudioCall && AppSettings.EnableAudioCall)
                                    {
                                        typeCalling = "Twilio_audio_call";
                                        callUser = usersObject.AudioCallUser?.CallUserClass;
                                    }

                                    break;
                                }
                            case SystemCall.Agora:
                                {
                                    agoraCall = usersObject.AgoraCall ?? false;
                                    if (agoraCall)
                                    {
                                        callUser = usersObject.AgoraCallData?.CallUserClass;
                                        if (callUser != null)
                                        {
                                            typeCalling = callUser.Data.Type switch
                                            {
                                                "video" when AppSettings.EnableVideoCall => "Agora_video_call_recieve",
                                                "audio" when AppSettings.EnableAudioCall => "Agora_audio_call_recieve",
                                                _ => typeCalling
                                            };
                                        }
                                    }

                                    break;
                                }
                        }

                        break;
                    case LastChatObject chatObject:
                        switch (AppSettings.UseLibrary)
                        {
                            case SystemCall.Twilio:
                                {
                                    twilioVideoCall = chatObject.VideoCall ?? false;
                                    twilioAudioCall = chatObject.AudioCall ?? false;

                                    if (twilioVideoCall)
                                    {
                                        typeCalling = "Twilio_video_call";
                                        callUser = chatObject.VideoCallUser?.CallUserClass;
                                    }
                                    else if (twilioAudioCall)
                                    {
                                        typeCalling = "Twilio_audio_call";
                                        callUser = chatObject.AudioCallUser?.CallUserClass;
                                    }

                                    break;
                                }
                            case SystemCall.Agora:
                                {
                                    agoraCall = chatObject.AgoraCall ?? false;
                                    if (agoraCall)
                                    {
                                        callUser = chatObject.AgoraCallData?.CallUserClass;
                                        if (callUser != null)
                                        {
                                            typeCalling = callUser.Data.Type switch
                                            {
                                                "video" => "Agora_video_call_recieve",
                                                "audio" => "Agora_audio_call_recieve",
                                                _ => typeCalling
                                            };
                                        }
                                    }

                                    break;
                                }
                        }

                        break;

                }

                if (callUser != null)
                {
                    MsgTabbedMainActivity.RunCall = true;

                    var userId = callUser.UserId;
                    var avatar = callUser.Avatar;
                    var name = callUser.Name;

                    var id = callUser.Data.Id; //call_id
                    var accessToken = callUser.Data.AccessToken;
                    var accessToken2 = callUser.Data.AccessToken2;
                    var fromId = callUser.Data.FromId;
                    var active = callUser.Data.Active;
                    var time = callUser.Data.Called;
                    var declined = callUser.Data.Declined;
                    var roomName = callUser.Data.RoomName;
                    var status = callUser.Data.Status;

                    Intent intent = null;
                    switch (typeCalling)
                    {
                        case "Twilio_video_call":
                            intent = new Intent(Application.Context, typeof(VideoAudioComingCallActivity));
                            intent.PutExtra("UserID", userId);
                            intent.PutExtra("avatar", avatar);
                            intent.PutExtra("name", name);
                            intent.PutExtra("access_token", accessToken);
                            intent.PutExtra("access_token_2", accessToken2);
                            intent.PutExtra("from_id", fromId);
                            intent.PutExtra("active", active);
                            intent.PutExtra("time", time);
                            intent.PutExtra("CallID", id);
                            intent.PutExtra("status", status);
                            intent.PutExtra("room_name", roomName);
                            intent.PutExtra("declined", declined);
                            intent.PutExtra("type", "Twilio_video_call");
                            break;
                        case "Twilio_audio_call":
                            intent = new Intent(Application.Context, typeof(VideoAudioComingCallActivity));
                            intent.PutExtra("UserID", userId);
                            intent.PutExtra("avatar", avatar);
                            intent.PutExtra("name", name);
                            intent.PutExtra("access_token", accessToken);
                            intent.PutExtra("access_token_2", accessToken2);
                            intent.PutExtra("from_id", fromId);
                            intent.PutExtra("active", active);
                            intent.PutExtra("time", time);
                            intent.PutExtra("CallID", id);
                            intent.PutExtra("status", status);
                            intent.PutExtra("room_name", roomName);
                            intent.PutExtra("declined", declined);
                            intent.PutExtra("type", "Twilio_audio_call");
                            break;
                        case "Agora_video_call_recieve":
                            intent = new Intent(Application.Context, typeof(VideoAudioComingCallActivity));
                            intent.PutExtra("UserID", userId);
                            intent.PutExtra("avatar", avatar);
                            intent.PutExtra("name", name);
                            intent.PutExtra("from_id", fromId);
                            intent.PutExtra("status", status);
                            intent.PutExtra("time", time);
                            intent.PutExtra("CallID", id);
                            intent.PutExtra("room_name", roomName);
                            intent.PutExtra("type", "Agora_video_call_recieve");
                            intent.PutExtra("declined", "0");
                            break;
                        case "Agora_audio_call_recieve":
                            intent = new Intent(Application.Context, typeof(VideoAudioComingCallActivity));
                            intent.PutExtra("UserID", userId);
                            intent.PutExtra("avatar", avatar);
                            intent.PutExtra("name", name);
                            intent.PutExtra("from_id", fromId);
                            intent.PutExtra("status", status);
                            intent.PutExtra("time", time);
                            intent.PutExtra("CallID", id);
                            intent.PutExtra("room_name", roomName);
                            intent.PutExtra("type", "Agora_audio_call_recieve");
                            intent.PutExtra("declined", "0");
                            break;
                    }

                    if (intent != null && !VideoAudioComingCallActivity.IsActive)
                    {
                        intent.AddFlags(ActivityFlags.NewTask);
                        Application.Context.StartActivity(intent);
                    }
                }
                else
                {
                    if (VideoAudioComingCallActivity.IsActive)
                        VideoAudioComingCallActivity.CallActivity?.FinishVideoAudio();
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);

                MsgTabbedMainActivity.RunCall = false;

                if (VideoAudioComingCallActivity.IsActive)
                    VideoAudioComingCallActivity.CallActivity?.FinishVideoAudio();

            }
        }

        public void ShowEmptyPage()
        {
            try
            {
                if (SwipeRefreshLayout.Refreshing)
                    SwipeRefreshLayout.Refreshing = false;

                if (MainScrollEvent != null)
                    MainScrollEvent.IsLoading = false;

                if (MAdapter.LastChatsList.Count > 0)
                {
                    var emptyStateChecker = MAdapter.LastChatsList.FirstOrDefault(a => a.Type == Classes.ItemType.EmptyPage);
                    if (emptyStateChecker != null)
                    {
                        var index = MAdapter.LastChatsList.IndexOf(emptyStateChecker);

                        MAdapter.LastChatsList.Remove(emptyStateChecker);
                        MAdapter.NotifyItemRemoved(index);
                    }

                    if (ListUtils.ArchiveList.Count > 0)
                    {
                        var archive = MAdapter.LastChatsList.FirstOrDefault(a => a.Type == Classes.ItemType.Archive);
                        if (archive != null)
                        {
                            archive.CountArchive = ListUtils.ArchiveList.Count.ToString();

                            var index = MAdapter.LastChatsList.IndexOf(archive);

                            MAdapter.LastChatsList.Move(index, MAdapter.LastChatsList.Count - 1);
                            MAdapter.NotifyItemMoved(index, MAdapter.LastChatsList.Count - 1);
                        }
                        else
                        {
                            MAdapter.LastChatsList.Add(new Classes.LastChatsClass
                            {
                                CountArchive = ListUtils.ArchiveList.Count.ToString(),
                                Type = Classes.ItemType.Archive,
                            });
                            MAdapter.NotifyItemInserted(MAdapter.LastChatsList.Count - 1);
                        }
                    }
                    else
                    {
                        var archive = MAdapter.LastChatsList.FirstOrDefault(a => a.Type == Classes.ItemType.Archive);
                        if (archive != null)
                        {
                            var index = MAdapter.LastChatsList.IndexOf(archive);

                            MAdapter.LastChatsList.Remove(archive);
                            MAdapter.NotifyItemRemoved(index);
                        }
                    }

                    //add insert dbDatabase 
                    if (AppSettings.LastChatSystem == SystemApiGetLastChat.New)
                    {
                        var list = MAdapter.LastChatsList.Where(a => a.LastChat != null && a.Type == Classes.ItemType.LastChatNewV).ToList();
                        ListUtils.UserList = new ObservableCollection<ChatObject>(list.Select(lastChatsClass => lastChatsClass.LastChat).ToList());

                        SqLiteDatabase dbDatabase = new SqLiteDatabase();
                        dbDatabase.Insert_Or_Update_LastUsersChat(Context, ListUtils.UserList, UserDetails.ChatHead);
                    }
                    else
                    {
                        var list = MAdapter.LastChatsList.Where(a => a.LastMessagesUser != null && a.Type == Classes.ItemType.LastChatOldV).ToList();
                        ListUtils.UserChatList = new ObservableCollection<GetUsersListObject.User>(list.Select(lastChatsClass => lastChatsClass.LastMessagesUser).ToList());

                        SqLiteDatabase dbDatabase = new SqLiteDatabase();
                        dbDatabase.Insert_Or_Update_LastUsersChat(Context, ListUtils.UserChatList, UserDetails.ChatHead);
                    }
                }
                else
                {
                    var emptyStateChecker = MAdapter.LastChatsList.FirstOrDefault(q => q.Type == Classes.ItemType.EmptyPage);
                    if (emptyStateChecker == null)
                    {
                        MAdapter.LastChatsList.Add(new Classes.LastChatsClass
                        {
                            Type = Classes.ItemType.EmptyPage
                        });
                        MAdapter.NotifyDataSetChanged();
                    }
                }
                ApiRun = false;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);

                if (SwipeRefreshLayout.Refreshing)
                    SwipeRefreshLayout.Refreshing = false;

                if (MainScrollEvent != null)
                    MainScrollEvent.IsLoading = false;
                ApiRun = false;
            }
        }

        #endregion

        //Get General Data Using Api >> Friend Requests and Group Chat Requests
        private async Task LoadGeneralData()
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    //var fetch = "friend_requests";
                    //if (AppSettings.EnableChatGroup)
                    //    fetch += ",group_chat_requests";

                    var (apiStatus, respond) = await RequestsAsync.Global.GetGeneralDataAsync(false, UserDetails.OnlineUsers, UserDetails.DeviceId, UserDetails.DeviceMsgId, "1", "group_chat_requests");
                    if (apiStatus == 200)
                    {
                        if (respond is GetGeneralDataObject result)
                        {
                            // FriendRequests
                            //var respondListFriendRequests = result?.FriendRequests?.Count;
                            //if (respondListFriendRequests > 0)
                            //{
                            //    ListUtils.FriendRequestsList = new ObservableCollection<UserDataObject>(result.FriendRequests);

                            //    var checkList = MAdapter.LastChatsList.FirstOrDefault(q => q.Type == Classes.ItemType.FriendRequest);
                            //    if (checkList == null)
                            //    {
                            //        var friendRequests = new Classes.LastChatsClass
                            //        {
                            //            UserRequestList = new List<UserDataObject>(),
                            //            Type = Classes.ItemType.FriendRequest
                            //        };

                            //        var list = result.FriendRequests.TakeLast(4).ToList();
                            //        if (list.Count > 0)
                            //            friendRequests.UserRequestList.AddRange(list);

                            //        MAdapter.LastChatsList.Insert(0, friendRequests);
                            //    }
                            //    else
                            //    {
                            //        if (checkList.UserRequestList.Count < 3)
                            //        {
                            //            var list = result.FriendRequests.TakeLast(4).ToList();
                            //            if (list.Count > 0)
                            //                checkList.UserRequestList.AddRange(list);
                            //        }
                            //    }

                            //    Activity?.RunOnUiThread(() => { MAdapter.NotifyItemInserted(0); });
                            //}
                            //else
                            //{
                            //    var checkList = MAdapter?.LastChatsList?.FirstOrDefault(q => q.Type == Classes.ItemType.FriendRequest);
                            //    if (checkList != null)
                            //    {
                            //        MAdapter.LastChatsList.Remove(checkList);
                            //        Activity?.RunOnUiThread(() => { MAdapter.NotifyItemRemoved(0); });
                            //    }
                            //}

                            // Group Requests
                            if (AppSettings.EnableChatGroup && AppSettings.LastChatSystem == SystemApiGetLastChat.New)
                            {
                                var respondListGroupRequests = result?.GroupChatRequests?.Count;
                                if (respondListGroupRequests > 0)
                                {
                                    ListUtils.GroupRequestsList = new ObservableCollection<GroupChatRequest>(result.GroupChatRequests);
                                }
                            }
                        }
                    }
                    else Methods.DisplayReportResult(Activity, respond);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
    }
}