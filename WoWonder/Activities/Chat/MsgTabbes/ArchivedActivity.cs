﻿using System;
using System.Collections.ObjectModel;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Ads;
using Android.Graphics;
using Android.OS;
using Android.Views;
using AndroidX.AppCompat.Content.Res;
using AndroidX.RecyclerView.Widget;
using AndroidX.SwipeRefreshLayout.Widget;
using Newtonsoft.Json;
using WoWonder.Activities.Base;
using WoWonder.Activities.Chat.ChatWindow;
using WoWonder.Activities.Chat.GroupChat;
using WoWonder.Activities.Chat.MsgTabbes.Adapter;
using WoWonder.Activities.Chat.PageChat;
using WoWonder.Helpers.Ads;
using WoWonder.Helpers.Model;
using WoWonder.Helpers.Utils;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace WoWonder.Activities.Chat.MsgTabbes
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.UiMode | ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class ArchivedActivity : BaseActivity
    {
        #region Variables Basic

        private LastChatsAdapter MAdapter;
        private SwipeRefreshLayout SwipeRefreshLayout;
        private RecyclerView MRecycler;
        private LinearLayoutManager LayoutManager;
        private ViewStub EmptyStateLayout;
        private View Inflated;
        private AdView MAdView;
        private static ArchivedActivity Instance;

        #endregion

        #region General

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                SetTheme(AppSettings.SetTabDarkTheme ? Resource.Style.MyTheme_Dark_Base : Resource.Style.MyTheme_Base);
                Methods.App.FullScreenApp(this);

                // Create your application here
                SetContentView(Resource.Layout.RecyclerDefaultLayout);

                Instance = this;

                //Get Value And Set Toolbar
                InitComponent();
                InitToolbar();
                SetRecyclerViewAdapters();

                GetArchivedList();
                AdsGoogle.Ad_Interstitial(this);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        protected override void OnResume()
        {
            try
            {
                MAdView?.Resume();
                base.OnResume();
                AddOrRemoveEvent(true);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        protected override void OnPause()
        {
            try
            {
                MAdView?.Pause();
                base.OnPause();
                AddOrRemoveEvent(false);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        protected override void OnDestroy()
        {
            try
            {
                MAdView?.Destroy();
                base.OnDestroy();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion

        #region Functions

        private void InitComponent()
        {
            try
            {
                MRecycler = (RecyclerView)FindViewById(Resource.Id.recyler);
                EmptyStateLayout = FindViewById<ViewStub>(Resource.Id.viewStub);

                SwipeRefreshLayout = (SwipeRefreshLayout)FindViewById(Resource.Id.swipeRefreshLayout);
                SwipeRefreshLayout.SetColorSchemeResources(Android.Resource.Color.HoloBlueLight, Android.Resource.Color.HoloGreenLight, Android.Resource.Color.HoloOrangeLight, Android.Resource.Color.HoloRedLight);
                SwipeRefreshLayout.Refreshing = false;
                SwipeRefreshLayout.Enabled = false;
                SwipeRefreshLayout.SetProgressBackgroundColorSchemeColor(AppSettings.SetTabDarkTheme ? Color.ParseColor("#424242") : Color.ParseColor("#f7f7f7"));

                MAdView = FindViewById<AdView>(Resource.Id.adView);
                AdsGoogle.InitAdView(MAdView, MRecycler);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void InitToolbar()
        {
            try
            {
                var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
                if (toolbar != null)
                {
                    toolbar.Title = GetText(Resource.String.Lbl_Archived);
                    toolbar.SetTitleTextColor(Color.ParseColor(AppSettings.MainColor));
                    SetSupportActionBar(toolbar);
                    SupportActionBar.SetDisplayShowCustomEnabled(true);
                    SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                    SupportActionBar.SetHomeButtonEnabled(true);
                    SupportActionBar.SetDisplayShowHomeEnabled(true);
                    SupportActionBar.SetHomeAsUpIndicator(AppCompatResources.GetDrawable(this, AppSettings.FlowDirectionRightToLeft ? Resource.Drawable.ic_action_right_arrow_color : Resource.Drawable.ic_action_left_arrow_color));

                }
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
                MAdapter = new LastChatsAdapter(this, "Archived")
                {
                    LastChatsList = new ObservableCollection<Classes.LastChatsClass>()
                };
                LayoutManager = new LinearLayoutManager(this);
                MRecycler.SetLayoutManager(LayoutManager);
                MRecycler.HasFixedSize = true;
                MRecycler.SetItemViewCacheSize(10);
                MRecycler.GetLayoutManager().ItemPrefetchEnabled = true;
                MRecycler.SetAdapter(MAdapter);
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
                    MAdapter.ItemClick += MAdapterOnItemClick;
                    MAdapter.ItemLongClick += MAdapterOnItemLongClick;
                }
                else
                {
                    MAdapter.ItemClick += MAdapterOnItemClick;
                    MAdapter.ItemLongClick += MAdapterOnItemLongClick;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public static ArchivedActivity GetInstance()
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
                                    this?.RunOnUiThread(() =>
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

                                    Intent intent = new Intent(this, typeof(ChatWindowActivity));
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
                                    this?.RunOnUiThread(() =>
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

                                            intent = new Intent(this, typeof(ChatWindowActivity));
                                            intent.PutExtra("UserID", item.LastChat.UserId);
                                            intent.PutExtra("TypeChat", "LastMessenger");
                                            intent.PutExtra("ShowEmpty", "no");
                                            intent.PutExtra("ColorChat", mainChatColor);
                                            intent.PutExtra("UserItem", JsonConvert.SerializeObject(item.LastChat));
                                            break;
                                        case "page":
                                            intent = new Intent(this, typeof(PageChatWindowActivity));
                                            intent.PutExtra("PageId", item.LastChat.PageId);
                                            intent.PutExtra("ShowEmpty", "no");
                                            intent.PutExtra("TypeChat", "");
                                            intent.PutExtra("PageObject", JsonConvert.SerializeObject(item.LastChat));
                                            break;
                                        case "group":
                                            intent = new Intent(this, typeof(GroupChatWindowActivity));
                                            intent.PutExtra("GroupObject", JsonConvert.SerializeObject(item.LastChat));
                                            intent.PutExtra("ShowEmpty", "no");
                                            intent.PutExtra("GroupId", item.LastChat.GroupId);
                                            break;
                                    }
                                    StartActivity(intent);
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
                                    bundle.PutString("Page", "Archived");
                                    bundle.PutString("ItemObject", JsonConvert.SerializeObject(item.LastMessagesUser));
                                    bottomSheet.Arguments = bundle;
                                    bottomSheet.Show(SupportFragmentManager, bottomSheet.Tag);
                                    break;
                                }
                            case Classes.ItemType.LastChatNewV:
                                {
                                    OptionsLastMessagesBottomSheet bottomSheet = new OptionsLastMessagesBottomSheet();
                                    Bundle bundle = new Bundle();
                                    bundle.PutString("Page", "Archived");
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
                                    bottomSheet.Show(SupportFragmentManager, bottomSheet.Tag);
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

        public void GetArchivedList()
        {
            try
            {
                if (ListUtils.ArchiveList.Count > 0)
                {
                    MAdapter.LastChatsList = new ObservableCollection<Classes.LastChatsClass>();
                    foreach (var archive in ListUtils.ArchiveList)
                    {
                        if (archive.LastChatPage != null)
                        {
                            MAdapter.LastChatsList.Add(new Classes.LastChatsClass()
                            {
                                LastChatPage = archive.LastChatPage,
                                Type = Classes.ItemType.LastChatPage
                            });
                        }
                        else if (archive.LastMessagesUser != null)
                        {
                            MAdapter.LastChatsList.Add(new Classes.LastChatsClass()
                            {
                                LastMessagesUser = archive.LastMessagesUser,
                                Type = Classes.ItemType.LastChatOldV
                            });
                        }
                        else if (archive.LastChat != null)
                        {
                            MAdapter.LastChatsList.Add(new Classes.LastChatsClass()
                            {
                                LastChat = archive.LastChat,
                                Type = Classes.ItemType.LastChatNewV
                            });
                        }
                    }

                    MAdapter.NotifyDataSetChanged();

                    MRecycler.Visibility = ViewStates.Visible;
                    SwipeRefreshLayout.Refreshing = false;
                }
                else
                {
                    MRecycler.Visibility = ViewStates.Gone;

                    Inflated ??= EmptyStateLayout.Inflate();

                    EmptyStateInflater x = new EmptyStateInflater();
                    x.InflateLayout(Inflated, EmptyStateInflater.Type.NoArchive);
                    if (!x.EmptyStateButton.HasOnClickListeners)
                    {
                        x.EmptyStateButton.Click += null;
                    }
                    EmptyStateLayout.Visibility = ViewStates.Visible;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

    }
}