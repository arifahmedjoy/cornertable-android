﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Android.App;

using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Bumptech.Glide;
using Java.Util;
using Refractored.Controls;
using WoWonder.Helpers.CacheLoaders;
using WoWonder.Helpers.Fonts;
using WoWonder.Helpers.Model;
using WoWonder.Helpers.Utils;
using WoWonderClient.Classes.Global;
using Exception = System.Exception;
using IList = System.Collections.IList;
using Object = Java.Lang.Object;

namespace WoWonder.Activities.Communities.Adapters
{
    public class MembersAdapter : RecyclerView.Adapter, ListPreloader.IPreloadModelProvider
    {
        public event EventHandler<MembersAdapterClickEventArgs> MoreItemClick;
        public event EventHandler<MembersAdapterClickEventArgs> ItemClick;
        public event EventHandler<MembersAdapterClickEventArgs> ItemLongClick;

        private readonly Activity ActivityContext;
        public ObservableCollection<UserDataObject> UserList = new ObservableCollection<UserDataObject>();

        public MembersAdapter(Activity activity)
        {
            try
            {
                HasStableIds = true;
                ActivityContext = activity;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public override int ItemCount => UserList?.Count ?? 0;

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Style_HContactMore_view
                var itemView = LayoutInflater.From(parent.Context)?.Inflate(Resource.Layout.Style_HContactMore_view, parent, false);
                var vh = new MembersAdapterViewHolder(itemView, MoreClick, Click, LongClick);
                return vh;
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
                return null!;
            }
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            try
            {

                if (viewHolder is MembersAdapterViewHolder holder)
                {
                    var item = UserList[position];
                    if (item != null)
                    {
                        Initialize(holder, item);
                    }
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void Initialize(MembersAdapterViewHolder holder, UserDataObject users)
        {
            try
            {
                GlideImageLoader.LoadImage(ActivityContext, users.Avatar, holder.Image, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);

                holder.Name.Text = Methods.FunString.SubStringCutOf(WoWonderTools.GetNameFinal(users), 25);

                if (users.Verified == "1")
                    holder.Name.SetCompoundDrawablesWithIntrinsicBounds(0, 0, Resource.Drawable.icon_checkmark_small_vector, 0);
                   
                holder.About.Text = users.UserId == UserDetails.UserId ? ActivityContext.GetString(Resource.String.Lbl_Online): ActivityContext.GetString(Resource.String.Lbl_Last_seen) + " " +Methods.Time.TimeAgo(Convert.ToInt32(users.LastseenUnixTime), false);

                //Online Or offline
                var online = WoWonderTools.GetStatusOnline(Convert.ToInt32(users.LastseenUnixTime), users.LastseenStatus);
                holder.ImageLastSeen.SetImageResource(online ? Resource.Drawable.Green_Color : Resource.Drawable.Grey_Offline);


                if (users.UserId == UserDetails.UserId)
                    holder.ButtonMore.Visibility = ViewStates.Gone; 
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public override void OnViewRecycled(Object holder)
        {
            try
            {
                 if (ActivityContext?.IsDestroyed != false)
                        return;

                 if (holder is MembersAdapterViewHolder viewHolder)
                {
                    Glide.With(ActivityContext).Clear(viewHolder.Image);
                }
                base.OnViewRecycled(holder);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
        public UserDataObject GetItem(int position)
        {
            return UserList[position];
        }

        public override long GetItemId(int position)
        {
            try
            {
                return position;
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
                return 0;
            }
        }

        public override int GetItemViewType(int position)
        {
            try
            {
                return position;
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
                return 0;
            }
        }

        private void MoreClick(MembersAdapterClickEventArgs args)
        {
            MoreItemClick?.Invoke(this, args);
        }

        private void Click(MembersAdapterClickEventArgs args)
        {
            ItemClick?.Invoke(this, args);
        }

        private void LongClick(MembersAdapterClickEventArgs args)
        {
            ItemLongClick?.Invoke(this, args);
        }

        public IList GetPreloadItems(int p0)
        {
            try
            {
                var d = new List<string>();
                var item = UserList[p0];
                if (item == null)
                    return Collections.SingletonList(p0);

                if (item.Avatar != "")
                {
                    d.Add(item.Avatar);
                    return d;
                }

                return d;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return Collections.SingletonList(p0);
            }
        }

        public RequestBuilder GetPreloadRequestBuilder(Object p0)
        {
            return GlideImageLoader.GetPreLoadRequestBuilder(ActivityContext, p0.ToString(), ImageStyle.CircleCrop);
        }
    }

    public class MembersAdapterViewHolder : RecyclerView.ViewHolder
    {
        public MembersAdapterViewHolder(View itemView, Action<MembersAdapterClickEventArgs> moreClickListener, Action<MembersAdapterClickEventArgs> clickListener,Action<MembersAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                Image = MainView.FindViewById<ImageView>(Resource.Id.card_pro_pic);
                Name = MainView.FindViewById<TextView>(Resource.Id.card_name);
                About = MainView.FindViewById<TextView>(Resource.Id.card_dist);
                ImageLastSeen = (CircleImageView)MainView.FindViewById(Resource.Id.ImageLastseen);
                ButtonMore = MainView.FindViewById<TextView>(Resource.Id.more);

                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, ButtonMore, FontAwesomeIcon.EllipsisH);

                //Event
                ButtonMore.Click += (sender, e) => moreClickListener(new MembersAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
                itemView.Click += (sender, e) => clickListener(new MembersAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
                itemView.LongClick += (sender, e) => longClickListener(new MembersAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #region Variables Basic

        public View MainView { get; }

        public ImageView Image { get; private set; }
        public TextView Name { get; private set; }
        public TextView About { get; private set; }
        public TextView ButtonMore { get; private set; }
        public CircleImageView ImageLastSeen { get; private set; }

        #endregion
    }

    public class MembersAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}