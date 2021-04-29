using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Android.Animation;
using Android.Content;
using Android.Database;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Views.Accessibility;
using Android.Views.Animations;
using Android.Widget;
using AndroidX.AppCompat.Content.Res;
using AndroidX.ViewPager.Widget;
using Java.Lang;
using WoWonder.Helpers.Utils;
using Exception = System.Exception;
using Math = System.Math;
using Object = Java.Lang.Object;

namespace WoWonder.Library.Anjo.Stories
{
    public class CustomViewPager : ViewGroup
    {
        #region Variables Basic

        private new const string Tag = "CustomViewPager";
		//private new const bool Debug = false;

        private const bool UseCache = true;

		private const int DefaultOffscreenPages = 0; // The default loading page, ViewPager is 1, so two Fragments will be loaded
        private const int MaxSettleDuration = 600; // ms

        public class ItemInfo
		{
			internal Object Object;
			internal int Position;
			internal bool Scrolling;
		}

		private static readonly IComparer<ItemInfo> Comparator = new ComparatorAnonymousInnerClass();

		private class ComparatorAnonymousInnerClass : IComparer<ItemInfo>
		{
			public int Compare(ItemInfo x, ItemInfo y)
			{
				if (ReferenceEquals(x, y)) return 0;
				if (ReferenceEquals(null, y)) return 1;
				if (ReferenceEquals(null, x)) return -1;
				var positionComparison = x.Position.CompareTo(y.Position);
				if (positionComparison != 0) return positionComparison;

				return x.Position - y.Position;
			}
		}

		private static readonly IInterpolator SInterpolator = new InterpolatorAnonymousInnerClass();

		private class InterpolatorAnonymousInnerClass : Object, IInterpolator
		{ 
            float ITimeInterpolator.GetInterpolation(float input)
            {
                try
                {
					// _o(t) = t * t * ((tension + 1) * t + tension)
					// o(t) = _o(t - 1) + 1
                    input -= 1.0f;
                    return input * input * input + 1.0f;
				}
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e);
                    return input;
                }
            }

            float IInterpolator.GetInterpolation(float input)
            {
				try
				{
                    // _o(t) = t * t * ((tension + 1) * t + tension)
                    // o(t) = _o(t - 1) + 1
                    input -= 1.0f;
                    return input * input * input + 1.0f;
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e);
                    return input;
                }
			}
        }

		private readonly List<ItemInfo> MItems = new List<ItemInfo>();

		private PagerAdapter Adapter;
		private int MCurItem; // Index of currently displayed page.
		private int MRestoredCurItem = -1;
		private IParcelable MRestoredAdapterState = null;
		private ClassLoader MRestoredClassLoader = null;
		private Scroller MScroller;
		private PagerObserver MObserver;

		private int PageMargin;
		private Drawable MMarginDrawable;

		private int MChildWidthMeasureSpec;
		private int MChildHeightMeasureSpec;
		private bool MInLayout;

		private bool MScrollingCacheEnabled;

		private bool MPopulatePending;
		private bool MScrolling;
		private int OffscreenPageLimit = DefaultOffscreenPages;
       
        private bool MIsBeingDragged;
		private bool MIsUnableToDrag;
		private int MTouchSlop;
		private float MInitialMotionX;
		/// <summary>
		/// Position of the last motion event.
		/// </summary>
		private float MLastMotionX;
		private float MLastMotionY;
		/// <summary>
		/// ID of the active pointer. This is used to retain consistency during
		/// drags/flings if multiple pointers are used.
		/// </summary>
		private int MActivePointerId = InvalidPointer;
		/// <summary>
		/// Sentinel value for no current active pointer. Used by
		/// <seealso cref="MActivePointerId"/>.
		/// </summary>
		private const int InvalidPointer = -1;

		/// <summary>
		/// Determines speed during touch scrolling
		/// </summary>
		private VelocityTracker MVelocityTracker;
		private int MMinimumVelocity;
		private int MMaximumVelocity;
		private float MBaseLineFlingVelocity;
		private float MFlingVelocityInfluence;

		private bool MFakeDragging;
		private long MFakeDragBeginTime;

		private EdgeEffect MLeftEdge;
		private EdgeEffect MRightEdge;

		private bool MFirstLayout = true;

		private IOnPageChangeListener MOnPageChangeListener;

        /// <summary>
        /// Indicates that the pager is in an idle, settled state. The current page
        /// is fully in view and no animation is in progress.
        /// </summary>
        public const int ScrollStateIdle = 0;

        /// <summary>
        /// Indicates that the pager is currently being dragged by the user.
        /// </summary>
        public const int ScrollStateDragging = 1;

        /// <summary>
        /// Indicates that the pager is in the process of settling to a 
        /// position.
        /// </summary>
        public const int ScrollStateSettling = 2;

		private int MScrollState = ScrollStateIdle;

        /// <summary>
        /// Callback interface for responding to changing state of the selected page.
        /// </summary>
        public interface IOnPageChangeListener
		{

			/// <summary>
			/// This method will be invoked when the current page is scrolled, either
			/// as part of a programmatically initiated smooth scroll or a user
			/// initiated touch scroll.
			/// </summary>
			/// <param name="position">             Position index of the first page currently being
			///                             displayed. Page position+1 will be visible if
			///                             positionOffset is nonzero. </param>
			/// <param name="positionOffset">       Value from [0, 1) indicating the offset from the page at
			///                             position. </param>
			/// <param name="positionOffsetPixels"> Value in pixels indicating the offset from position. </param>
			void OnPageScrolled(int position, float positionOffset, int positionOffsetPixels);

			/// <summary>
			/// This method will be invoked when a new page becomes selected.
			/// Animation is not necessarily complete.
			/// </summary>
			/// <param name="position"> Position index of the new selected page. </param>
			void OnPageSelected(int position);

            /// <summary>
            /// Called when the scroll state changes. Useful for discovering when the
            /// user begins dragging, when the pager is automatically settling to the
            /// current page, or when it is fully stopped/idle.
            /// </summary>
            /// <param name="state"> The new scroll state. </param> 
            void OnPageScrollStateChanged(int state);
		}

        /// <summary>
        /// Simple implementation of the
        /// interface with stub implementations of each method. Extend this if you do
        /// not intend to override every method of
        /// </summary>
        public class SimpleOnPageChangeListener : IOnPageChangeListener
		{
			public virtual void OnPageScrolled(int position, float positionOffset, int positionOffsetPixels)
			{
				// This space for rent
			}

			public virtual void OnPageSelected(int position)
			{
				// This space for rent
			}

			public virtual void OnPageScrollStateChanged(int state)
			{
				// This space for rent
			}
		}

        private class PagerObserver : DataSetObserver
        {
            private readonly CustomViewPager CustomViewPager;

			public PagerObserver(CustomViewPager customViewPager)
            {
                CustomViewPager = customViewPager;

            }
			public override void OnChanged()
            {
                base.OnChanged();
                CustomViewPager.DataSetChanged();
            }

            public override void OnInvalidated()
            {
                base.OnInvalidated();
                CustomViewPager.DataSetChanged();
			}
        }

        #endregion
         
        public CustomViewPager(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public CustomViewPager(Context context) : base(context)
        {
            InitViewPager();
        }

        public CustomViewPager(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            InitViewPager();
        }

        public CustomViewPager(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            InitViewPager();
        }

        public CustomViewPager(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
            InitViewPager();
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            try
            {
                MInLayout = true;
                Populate();
                MInLayout = false;

                int count = ChildCount;
                int width = r - l;

                for (int i = 0; i < count; i++)
                {
                    View child = GetChildAt(i);
                    ItemInfo ii;
                    if (child.Visibility != ViewStates.Gone && (ii = InfoForChild(child)) != null)
                    {
                        int loff = (width + PageMargin) * ii.Position;
                        int childLeft = PaddingLeft + loff;
                        int childTop = PaddingTop;
                        //if (Debug)
                        //    Console.WriteLine("Positioning #" + i + " " + child + " f=" + ii.Object + ":" + childLeft + "," + childTop + " " + child.MeasuredWidth + "x" + child.MeasuredHeight);
                      
                        child.Layout(childLeft, childTop, childLeft + child.MeasuredWidth, childTop + child.MeasuredHeight);
                    }
                }
                MFirstLayout = false;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e); 
            }
        }

        private void InitViewPager()
        {
            try
            {
                SetWillNotDraw(false);
                DescendantFocusability = DescendantFocusability.AfterDescendants;
                Focusable = true;
                Context context = Context;
                MScroller = new Scroller(context, SInterpolator);
                ViewConfiguration configuration = ViewConfiguration.Get(context);
                MTouchSlop = configuration.ScaledPagingTouchSlop;
                MMinimumVelocity = configuration.ScaledMinimumFlingVelocity;
                MMaximumVelocity = configuration.ScaledMaximumFlingVelocity;
                MLeftEdge = new EdgeEffect(context);
                MRightEdge = new EdgeEffect(context);

                float density = context.Resources.DisplayMetrics.Density;
                MBaseLineFlingVelocity = 2500.0f * density;
                MFlingVelocityInfluence = 0.4f;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e); 
            }
        }

		private void SetScrollState(int newState)
        {
            try
            {
                if (MScrollState == newState)
                {
                    return;
                }

                MScrollState = newState;
                MOnPageChangeListener?.OnPageScrollStateChanged(newState);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e); 
            }
        }

        public void SetAdapter(PagerAdapter adapter)
        {
            try
            {
                if (Adapter != null)
                {
                    Adapter.UnregisterDataSetObserver(MObserver);
                    Adapter.StartUpdate(this);
                    foreach (var ii in MItems)
                    {
                        Adapter.DestroyItem(this, ii.Position, ii.Object);
                    }
                    Adapter.FinishUpdate(this);
                    MItems.Clear();
                    RemoveAllViews();
                    MCurItem = 0;
                    ScrollTo(0, 0);
                }

                Adapter = adapter; 
                 
                if (Adapter != null)
                {
                    MObserver ??= new PagerObserver(this);
                    
                    Adapter.RegisterDataSetObserver(MObserver);
                    MPopulatePending = false;
                    if (MRestoredCurItem >= 0)
                    {
                        Adapter.RestoreState(MRestoredAdapterState, MRestoredClassLoader);
                        SetCurrentItemInternal(MRestoredCurItem, false, true);
                        MRestoredCurItem = -1;
                        MRestoredAdapterState = null;
                        MRestoredClassLoader = null; 
                    }
                    else
                    {
                        Populate();
                    } 
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e); 
            }
        }

        public PagerAdapter GetAdapter()
        {
            return Adapter;
        }

		/// <summary>
		/// Set the currently selected page. If the ViewPager has already been
		/// through its first layout there will be a smooth animated transition
		/// between the current item and the specified item.
		/// </summary>
		/// <param name="item">index to select</param>
		public void SetCurrentItem(int item)
        {
            try
            {
                MPopulatePending = false;
                SetCurrentItemInternal(item, !MFirstLayout, false);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e); 
            }
        }

        /// <summary>
        /// Set the currently selected page.
        /// </summary>
        /// <param name="item"> Item index to select</param>
        /// <param name="smoothScroll">True to smoothly scroll to the new item, false to transition immediately</param>
        public void SetCurrentItem(int item, bool smoothScroll)
        {
            try
            {
                MPopulatePending = false;
                SetCurrentItemInternal(item, smoothScroll, false);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e); 
            }
        }

        public int GetCurrentItem()
        {
            return MCurItem;
        }

		public void SetCurrentItemInternal(int item, bool smoothScroll, bool always)
        {
            try
            {
                SetCurrentItemInternal(item, smoothScroll, always, 0);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e); 
            }
        }

        public void SetCurrentItemInternal(int item, bool smoothScroll, bool always, int velocity)
        {
            try
            {
                if (Adapter == null || Adapter.Count <= 0)
                {
                    SetScrollingCacheEnabled(false);
                    return;
                }
                if (!always && MCurItem == item && MItems.Count != 0)
                {
                    SetScrollingCacheEnabled(false);
                    return;
                }
                if (item < 0)
                {
                    item = 0;
                }
                else if (item >= Adapter.Count)
                {
                    item = Adapter.Count - 1;
                }
                int pageLimit = OffscreenPageLimit;
                if (item > MCurItem + pageLimit || item < MCurItem - pageLimit)
                {
                    // We are doing a jump by more than one page. To avoid
                    // glitches, we want to keep all current pages in the view
                    // until the scroll ends.
                    foreach (var t in MItems)
                    {
                        t.Scrolling = true;
                    }
                }

                bool dispatchSelected = MCurItem != item;
                MCurItem = item;
                Populate();
                int destX = (Width + PageMargin) * item;
                if (smoothScroll)
                {
                    SmoothScrollTo(destX, 0, velocity);
                    if (dispatchSelected)
                    {
                        MOnPageChangeListener?.OnPageSelected(item);
                    }
                }
                else
                {
                    if (dispatchSelected)
                    {
                        MOnPageChangeListener?.OnPageSelected(item);
                    }
                    CompleteScroll();
                    ScrollTo(destX, 0);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e); 
            }
        }

        public void SetOnPageChangeListener(IOnPageChangeListener listener)
        {
            try
            {
                MOnPageChangeListener = listener;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);  
            }
        }

        /// <summary>
        /// Returns the number of pages that will be retained to either side of the
        /// current page in the view hierarchy in an idle state. Defaults to 1.
        /// </summary>
        /// <returns>How many pages will be kept offscreen on either side </returns>
        /// <see cref="SetOffscreenPageLimit"/>
        public int GetOffscreenPageLimit()
        {
            return OffscreenPageLimit;
        }
         
        /// <summary>
        /// Set the number of pages that should be retained to either side of the
        /// current page in the view hierarchy in an idle state. Pages beyond this
        /// limit will be recreated from the adapter when needed.
        /// This is offered as an optimization. If you know in advance the number of
        /// pages you will need to support or have lazy-loading mechanisms in place
        /// on your pages, tweaking this setting can have benefits in perceived
        /// smoothness of paging animations and interaction. If you have a small
        /// number of pages (3-4) that you can keep active all at once, less time
        /// will be spent in layout for newly created view subtrees as the user pages 
        /// back and forth.
        /// You should keep this limit low, especially if your pages have complex
        /// layouts. This setting defaults to 1.
        /// 
        /// </summary>
        /// <param name="limit">How many pages will be kept offscreen in an idle state.</param>
        public void SetOffscreenPageLimit(int limit)
        {
            try
            {
                if (limit < DefaultOffscreenPages)
                {
                    Console.WriteLine("Requested offscreen page limit " + limit + " too small; defaulting to " + DefaultOffscreenPages);
                    limit = DefaultOffscreenPages;
                }
                if (limit != OffscreenPageLimit)
                {
                    OffscreenPageLimit = limit;
                    Populate();
                } 
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e); 
            }
        }

        /// <summary>
        /// Set the margin between pages.
        /// </summary>
        /// <param name="marginPixels">Distance between adjacent pages in pixels</param>
        /// <see cref="GetPageMargin"/>
        /// <see cref="SetPageMarginDrawable(Drawable)"/>
        /// <see cref="SetPageMarginDrawable(int)"/>
        public void SetPageMargin(int marginPixels)
        {
            try
            {
                int oldMargin = PageMargin;
                PageMargin = marginPixels;

                int width = Width;
                RecomputeScrollPosition(width, width, marginPixels, oldMargin);

                RequestLayout();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e); 
            }
        }

        /// <summary>
        /// Return the margin between pages.
        /// </summary>
        /// <returns>The size of the margin in pixels</returns>
        public int GetPageMargin()
        {
            return PageMargin;
        }

        /// <summary>
        /// Set a drawable that will be used to fill the margin between pages.
        /// </summary>
        /// <param name="d">Drawable to display between pages</param>
        public void SetPageMarginDrawable(Drawable d)
        {
            try
            {
                MMarginDrawable = d;
                if (d != null)
                    RefreshDrawableState();
                SetWillNotDraw(d == null);
                Invalidate();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e); 
            }
        }

        /// <summary>
        /// Set a drawable that will be used to fill the margin between pages.
        /// </summary>
        /// <param name="resId">Resource ID of a drawable to display between pages</param>
        public void SetPageMarginDrawable(int resId)
        {
            SetPageMarginDrawable(AppCompatResources.GetDrawable(Context ,resId));
        }


        protected override bool VerifyDrawable(Drawable who)
        {
            return base.VerifyDrawable(who) || who == MMarginDrawable;
        }

        protected override void DrawableStateChanged()
        {
            try
            {
                base.DrawableStateChanged();
                Drawable d = MMarginDrawable;
                if (d != null && d.IsStateful)
                {
                    d.SetState(GetDrawableState());
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e); 
            }
        }

        /// <summary>
        /// We want the duration of the page snap animation to be influenced by the
        /// distance that
        /// the screen has to travel, however, we don't want this duration to be
        /// effected in a
        /// purely linear fashion. Instead, we use this method to moderate the effect
        /// that the distance
        /// of travel has on the overall snap duration. 
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public float DistanceInfluenceForSnapDuration(float f)
        {
            try
            {
                f -= 0.5f; // center the values about 0.
                f *= (float)(0.3f * Math.PI / 2.0f);
                return (float)Math.Sin(f);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return f;
            }
        }
         
        /// <summary>
        /// Like {@link View#scrollBy}, but scroll smoothly instead of immediately.
        /// </summary>
        /// <param name="x">the number of pixels to scroll by on the X axis</param>
        /// <param name="y">the number of pixels to scroll by on the Y axis</param>
        public void SmoothScrollTo(int x, int y)
        {
            try
            {
                SmoothScrollTo(x, y, 0);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e); 
            }
        }

        /// <summary>
        /// Like {@link View#scrollBy}, but scroll smoothly instead of immediately
        /// </summary>
        /// <param name="x">the number of pixels to scroll by on the X axis</param>
        /// <param name="y">the number of pixels to scroll by on the Y axis</param>
        /// <param name="velocity">the velocity associated with a fling, if applicable. (0 otherwise) </param>
        public void SmoothScrollTo(int x, int y, int velocity)
        {
            try
            {
                if (ChildCount == 0)
                {
                    // Nothing to do.
                    SetScrollingCacheEnabled(false);
                    return;
                }
                int sx = ScrollX;
                int sy = ScrollY;
                int dx = x - sx;
                int dy = y - sy;
                if (dx == 0 && dy == 0)
                {
                    CompleteScroll();
                    SetScrollState(ScrollStateIdle);
                    return;
                }

                SetScrollingCacheEnabled(true);
                MScrolling = true;
                SetScrollState(ScrollStateSettling);
             
                float pageDelta = (float)Math.Abs(dx) / (Width + PageMargin);
                int duration = (int)(pageDelta * 100);

                velocity = Math.Abs(velocity);
                if (velocity > 0)
                {
                    duration += (int)(duration / (velocity / MBaseLineFlingVelocity)) * (int)MFlingVelocityInfluence; 
                }
                else
                {
                    duration += 100;
                }
                duration = Math.Min(duration, MaxSettleDuration);

                MScroller.StartScroll(sx, sy, dx, dy, duration);
                Invalidate();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e); 
            }
        }

        public void AddNewItem(int position, int index)
        {
            try
            {
                ItemInfo ii = new ItemInfo
                {
                    Position = position,
                    Object = Adapter.InstantiateItem(this, position)
                };
                if (index < 0)
                {
                    MItems.Add(ii);
                }
                else
                {
                    MItems.Insert(index, ii);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e); 
            }
        }

        public void DataSetChanged()
        {
            // This method only gets called if our observer is attached, so Adapter
            // is non-null.

            try
            {
                bool needPopulate = MItems.Count < 3 && MItems.Count < Adapter.Count;
                int newCurrItem = -1;

                for (int i = 0; i < MItems.Count; i++)
                {
                    ItemInfo ii = MItems[i];
                    int newPos = Adapter.GetItemPosition(ii.Object);

                    switch (newPos)
                    {
                        case PagerAdapter.PositionUnchanged:
                            continue;
                        case PagerAdapter.PositionNone:
                        {
                            MItems.RemoveAt(i);
                            i--;
                            Adapter.DestroyItem(this, ii.Position, ii.Object);
                            needPopulate = true;

                            if (MCurItem == ii.Position)
                            {
                                // Keep the current item in the valid range
                                newCurrItem = Math.Max(0, Math.Min(MCurItem, Adapter.Count - 1));
                            }
                            continue;
                        }
                    }

                    if (ii.Position != newPos)
                    {
                        if (ii.Position == MCurItem)
                        {
                            // Our current item changed position. Follow it.
                            newCurrItem = newPos;
                        }

                        ii.Position = newPos;
                        needPopulate = true;
                    }
                }

                MItems.Sort(Comparator);

                if (newCurrItem >= 0)
                {
                    // TODO This currently causes a jump.
                    SetCurrentItemInternal(newCurrItem, false, true);
                    needPopulate = true;
                }
                if (needPopulate)
                {
                    Populate();
                    RequestLayout();
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
         
        public void Populate() 
        {
            try
            {
                if (Adapter == null)
                {
                    return;
                }

                // Bail now if we are waiting to populate. This is to hold off
                // on creating views from the time the user releases their finger to
                // fling to a new position until we have finished the scroll to
                // that position, avoiding glitches from happening at that point.
                if (MPopulatePending)
                {
                    //if (Debug)
                    //    Log.Info(Tag, "populate is pending, skipping for now...");
                    return;
                }

                // Also, don't populate until we are attached to a window. This is to
                // avoid trying to populate before we have restored our view hierarchy
                // state and conflicting with what is restored.
                if (WindowToken == null)
                {
                    return;
                }

                Adapter.StartUpdate(this);

                int pageLimit = OffscreenPageLimit;
                int startPos = Math.Max(0, MCurItem - pageLimit);
                int n = Adapter.Count;
                int endPos = Math.Min(n - 1, MCurItem + pageLimit);

                //if (Debug)
                //    Log.Verbose(Tag, "populating: startPos=" + startPos + " endPos=" + endPos);

                // Add and remove pages in the existing list.
                int lastPos = -1;
                for (int i = 0; i < MItems.Count; i++)
                {
                    ItemInfo ii = MItems[i];
                    if ((ii.Position < startPos || ii.Position > endPos) && !ii.Scrolling)
                    {
                        //if (Debug)
                        //    Log.Info(Tag, "removing: " + ii.Position + " @ " + i);
                        MItems.RemoveAt(i);
                        i--;
                        Adapter.DestroyItem(this, ii.Position, ii.Object);
                    }
                    else if (lastPos < endPos && ii.Position > startPos)
                    {
                        // The next item is outside of our range, but we have a gap
                        // between it and the last item where we want to have a page
                        // shown. Fill in the gap.
                        lastPos++;
                        if (lastPos < startPos)
                        {
                            lastPos = startPos;
                        }
                        while (lastPos <= endPos && lastPos < ii.Position)
                        {
                            //if (Debug)
                            //    Log.Info(Tag, "inserting: " + lastPos + " @ " + i);
                            AddNewItem(lastPos, i);
                            lastPos++;
                            i++;
                        }
                    }
                    lastPos = ii.Position;
                }

                // Add any new pages we need at the end.
                lastPos = MItems.Count > 0 ? MItems[MItems.Count - 1].Position : -1;
                if (lastPos < endPos)
                {
                    lastPos++;
                    lastPos = lastPos > startPos ? lastPos : startPos;
                    while (lastPos <= endPos)
                    {
                        //if (Debug)
                        //    Log.Info(Tag, "appending: " + lastPos);
                        AddNewItem(lastPos, -1);
                        lastPos++;
                    }
                }

                //if (Debug)
                //{
                //    Log.Info(Tag, "Current page list:");
                //    for (int i = 0; i < MItems.Count; i++)
                //    {
                //        Log.Info(Tag, "#" + i + ": page " + MItems[i].Position);
                //    }
                //}

                ItemInfo curItem = MItems.FirstOrDefault(t => t.Position == MCurItem);
                Adapter.SetPrimaryItem(this, MCurItem, curItem?.Object);

                Adapter.FinishUpdate(this);

                if (HasFocus)
                {
                    View currentFocused = FindFocus();
                    ItemInfo ii = currentFocused != null ? InfoForAnyChild(currentFocused) : null;
                    if (ii == null || ii.Position != MCurItem)
                    {
                        for (int i = 0; i < ChildCount; i++)
                        {
                            View child = GetChildAt(i);
                            ii = InfoForChild(child);
                            if (ii != null && ii.Position == MCurItem)
                            {
                                if (child != null && child.RequestFocus(FocusSearchDirection.Forward))
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public class SavedState : BaseSavedState
        {
            public int Position;
            public IParcelable AdapterState;
            public ClassLoader Loader;
            
            protected SavedState(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
            {
            }

            public SavedState(Parcel source) : base(source)
            {
                try
                {
                    if (Loader == null)
                    {
                        Loader = Class.ClassLoader;
                    }
                    Position = source.ReadInt();
                    AdapterState = (IParcelable)source.ReadParcelable(Loader);
                    //this.loader = loader;
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e); 
                }
            }

            public SavedState(Parcel source, ClassLoader loader) : base(source, loader)
            {
                try
                {
                    loader ??= Class.ClassLoader;
                    
                    Position = source.ReadInt();
                    AdapterState = (IParcelable) source.ReadParcelable(loader);
                    Loader = loader;
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e); 
                }
            }

            public SavedState(IParcelable superState) : base(superState)
            {
            }


            public override void WriteToParcel(Parcel dest, ParcelableWriteFlags flags)
            {
                try
                {
                    base.WriteToParcel(dest, flags);
                    dest.WriteInt(Position);
                    dest.WriteParcelable(AdapterState, flags);
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e); 
                } 
            }

            public override string ToString()
            { 
                try
                {
                    
                    return "FragmentPager.SavedState{" + Integer.ToHexString(RuntimeHelpers.GetHashCode(this)) + " position=" + Position + "}";
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e);
                    return base.ToString();
                }
            }
             
            public new static readonly IParcelableCreator Creator = new ParcelableCompatCreatorCallbacksAnonymousInnerClass();
            private class ParcelableCompatCreatorCallbacksAnonymousInnerClass : Object, IParcelableClassLoaderCreator
            { 
                public Object CreateFromParcel(Parcel @in, ClassLoader loader)
                {
                    return new SavedState(@in, loader);
                }

                public Object CreateFromParcel(Parcel source)
                {
                    return new SavedState(source);
                }

                public Object[] NewArray(int size)
                {
                    return new SavedState[size]; 
                }
            } 
        }

        protected override IParcelable OnSaveInstanceState()
        {
            try
            {
                IParcelable superState = base.OnSaveInstanceState();
                SavedState ss = new SavedState(superState) { Position = MCurItem };
                if (Adapter != null)
                {
                    ss.AdapterState = Adapter.SaveState();
                }
                return ss;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return base.OnSaveInstanceState();
            }
        }

        protected override void OnRestoreInstanceState(IParcelable state)
        {
            try
            {
                if (!(state is SavedState ss)) {
                    base.OnRestoreInstanceState(state);
                    return;
                }
                  
                base.OnRestoreInstanceState(ss.SuperState);

                if (Adapter != null)
                {
                    Adapter.RestoreState(ss.AdapterState, ss.Loader);
                    SetCurrentItemInternal(ss.Position, false, true);
                }
                else
                {
                    MRestoredCurItem = ss.Position;
                    MRestoredAdapterState = ss.AdapterState;
                    MRestoredClassLoader = ss.Loader;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                base.OnRestoreInstanceState(state);
            } 
        }
         
        public override void AddView(View child, int index, LayoutParams @params)
        {
            try
            {
                if (MInLayout)
                {
                    AddViewInLayout(child, index, @params);
                    child.Measure(MChildWidthMeasureSpec, MChildHeightMeasureSpec);
                }
                else
                {
                    base.AddView(child, index, @params);
                }

                if (UseCache)
                {
#pragma warning disable 618
                    child.DrawingCacheEnabled = child.Visibility != ViewStates.Gone && MScrollingCacheEnabled;
#pragma warning restore 618
                }

            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                base.AddView(child, index, @params);
            }  
        }

        public ItemInfo InfoForChild(View child)
        {
            try
            {
                return MItems.FirstOrDefault(ii => Adapter.IsViewFromObject(child, ii.Object));
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return null;
            }
        }

        public ItemInfo InfoForAnyChild(View child)
        {
            try
            {
                IViewParent parent;
                while ((parent = child.Parent) != this)
                {
                    if (parent is not View view) {
                        return null;
                    }
                    child = (View)parent;
                }
                return InfoForChild(child);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return null;
            }
        }

        protected override void OnAttachedToWindow()
        {
            try
            {
                base.OnAttachedToWindow();
                MFirstLayout = true;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e); 
            }
        }

        // For simple implementation, or internal size is always 0.
        // We depend on the container to specify the layout size of
        // our view. We can't really know what it is since we will be
        // adding and removing different arbitrary views and do not
        // want the layout to change as this happens. 
        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
            try
            {
                SetMeasuredDimension(GetDefaultSize(0, widthMeasureSpec), GetDefaultSize(0, heightMeasureSpec));

                // Children are just made to fill our space.
                MChildWidthMeasureSpec = MeasureSpec.MakeMeasureSpec (MeasuredWidth-   PaddingLeft - PaddingRight, MeasureSpecMode.Exactly);
                MChildHeightMeasureSpec = MeasureSpec.MakeMeasureSpec(MeasuredHeight - PaddingTop -  PaddingBottom, MeasureSpecMode.Exactly);

                // Make sure we have created all fragments that we need to have shown.
                MInLayout = true;
                Populate();
                MInLayout = false;

                // Make sure all children have been properly measured.
                int size = ChildCount;
                for (int i = 0; i < size; ++i)
                {
                    View child = GetChildAt(i);
                    if (child.Visibility != ViewStates.Gone)
                    {
                        //if (Debug)
                        //    Log.Verbose(Tag, "Measuring #" + i + " " + child + ": " + MChildWidthMeasureSpec);
                        child.Measure(MChildWidthMeasureSpec, MChildHeightMeasureSpec);
                    }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e); 
            }
        }

        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            try
            {
                base.OnSizeChanged(w, h, oldw, oldh);
                // Make sure scroll position is set correctly.
                if (w != oldw)
                {
                    RecomputeScrollPosition(w, oldw, PageMargin, PageMargin);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e); 
            }
        }

        private void RecomputeScrollPosition(int width, int oldWidth, int margin, int oldMargin)
        {
            try
            {
                int widthWithMargin = width + margin;
                if (oldWidth > 0)
                {
                    int oldScrollPos = ScrollX;
                    int oldwwm = oldWidth + oldMargin;
                    int oldScrollItem = oldScrollPos / oldwwm;
                    float scrollOffset = (float)(oldScrollPos % oldwwm) / oldwwm;
                    int scrollPos = (int)((oldScrollItem + scrollOffset) * widthWithMargin);
                    ScrollTo(scrollPos, ScrollY);
                    if (!MScroller.IsFinished)
                    {
                        // We now return to your regularly scheduled scroll, already in
                        // progress.
                        int newDuration = MScroller.Duration - MScroller.TimePassed();
                        MScroller.StartScroll(scrollPos, 0, MCurItem * widthWithMargin, 0, newDuration);
                    }
                }
                else
                {
                    int scrollPos = MCurItem * widthWithMargin;
                    if (scrollPos != ScrollX)
                    {
                        CompleteScroll();
                        ScrollTo(scrollPos, ScrollY);
                    }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e); 
            }
        }

        public override void ComputeScroll()
        {
            base.ComputeScroll();
            try
            {
                //if (Debug)
                //    Log.Info(Tag, "computeScroll: finished=" + MScroller.IsFinished);
                if (!MScroller.IsFinished)
                {
                    if (MScroller.ComputeScrollOffset())
                    {
                        //if (Debug)
                        //    Log.Info(Tag, "computeScroll: still scrolling");
                        
                        int oldX = ScrollX;
                        int oldY = ScrollY;
                        int x = MScroller.CurrX;
                        int y = MScroller.CurrY;

                        if (oldX != x || oldY != y)
                        {
                            ScrollTo(x, y);
                        }

                        if (MOnPageChangeListener != null)
                        {
                            int widthWithMargin = Width + PageMargin;
                            int position = x / widthWithMargin;
                            int offsetPixels = x % widthWithMargin;
                            float offset = (float)offsetPixels / widthWithMargin;
                            MOnPageChangeListener.OnPageScrolled(position, offset,
                                offsetPixels);
                        }

                        // Keep on drawing until the animation has finished.
                        Invalidate();
                        return;
                    }
                }

                // Done with scroll, clean up state.
                CompleteScroll();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e); 
            }
        }

        private void CompleteScroll()
        {
            try
            {
                bool needPopulate = MScrolling;
                if (needPopulate)
                {
                    // Done with scroll, no longer want to cache view drawing.
                    SetScrollingCacheEnabled(false);
                    MScroller.AbortAnimation();
                    int oldX =ScrollX;
                    int oldY =ScrollY;
                    int x = MScroller.CurrX;
                    int y = MScroller.CurrY;
                    if (oldX != x || oldY != y)
                    {
                        ScrollTo(x, y);
                    }
                    SetScrollState(ScrollStateIdle);
                }
                MPopulatePending = false;
                MScrolling = false;
                foreach (var ii in MItems.Where(ii => ii.Scrolling))
                {
                    needPopulate = true;
                    ii.Scrolling = false;
                }
                if (needPopulate)
                {
                    Populate();
                }

            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        /// <summary>
        /// This method JUST determines whether we want to intercept the motion.
        /// If we return true, onMotionEvent will be called and we do the actual scrolling there. 
        /// </summary>
        /// <param name="ev"></param>
        /// <returns></returns>
        public override bool OnInterceptTouchEvent(MotionEvent ev)
        {
            try
            {
                var action = ev.Action & MotionEventActions.Mask;

                // Always take care of the touch gesture being complete.
                if (action == MotionEventActions.Cancel || action == MotionEventActions.Up)
                {
                    // Release the drag.
                    //if (Debug)
                    //    Log.Verbose(Tag, "Intercept done!");

                    MIsBeingDragged = false;
                    MIsUnableToDrag = false;
                    MActivePointerId = InvalidPointer;
                    return false;
                }

                // Nothing more to do here if we have decided whether or not we
                // are dragging.
                if (action != MotionEventActions.Down)
                {
                    if (MIsBeingDragged)
                    {
                        //if (Debug)
                        //    Log.Verbose(Tag, "Intercept returning true!");
                        return true;
                    }
                    if (MIsUnableToDrag)
                    {
                        //if (Debug)
                        //    Log.Verbose(Tag, "Intercept returning false!");
                        return false;
                    }
                }

                switch (action)
                {
                    case MotionEventActions.Move:
                        {
                            //mIsBeingDragged == false, otherwise the shortcut would have caught it. Check whether the user has moved far enough from his original down touch.
                            //Locally do absolute value. mLastMotionY is set to the y value of the down event.
                            int activePointerId = MActivePointerId;
                            if (activePointerId == InvalidPointer)
                            {
                                // If we don't have a valid id, the touch down wasn't on
                                // content.
                                break;
                            }

                            int pointerIndex = ev.FindPointerIndex(activePointerId);
                            float x = ev.GetX(pointerIndex);
                            float dx = x - MLastMotionX;
                            float xDiff = Math.Abs(dx);
                            float y = ev.GetY(pointerIndex);
                            float yDiff = Math.Abs(y - MLastMotionY);
                            int scrollX = ScrollX;
                            bool atEdge = dx > 0 && scrollX == 0 || dx < 0 && Adapter != null && scrollX >= (Adapter.Count - 1) * Width - 1;
                            //if (Debug)
                            //    Log.Verbose(Tag, "Moved x to " + x + "," + y + " diff=" + xDiff + "," + yDiff);

                            if (CanScroll(this, false, (int)dx, (int)x, (int)y))
                            {
                                // Nested view has scrollable area under this point. Let it be
                                // handled there.
                                MInitialMotionX = MLastMotionX = x;
                                MLastMotionY = y;
                                return false;
                            }

                            if (xDiff > MTouchSlop && xDiff > yDiff)
                            {
                                //if (Debug)
                                //    Log.Verbose(Tag, "Starting drag!");
                                MIsBeingDragged = true;
                                SetScrollState(ScrollStateDragging);
                                MLastMotionX = x;
                                SetScrollingCacheEnabled(true);
                            }
                            else
                            {
                                if (yDiff > MTouchSlop)
                                {
                                    // The finger has moved enough in the vertical
                                    // direction to be counted as a drag... abort
                                    // any attempt to drag horizontally, to work correctly
                                    // with children that have scrolling containers.
                                    //if (Debug)
                                    //    Log.Verbose(Tag, "Starting unable to drag!");
                                    MIsUnableToDrag = true;
                                }
                            }

                            break;
                        }

                    case MotionEventActions.Down:
                        {
                            /*
                             * Remember location of down touch. ACTION_DOWN always refers to
                             * pointer index 0.
                             */
                            MLastMotionX = MInitialMotionX = ev.GetX();
                            MLastMotionY = ev.GetY();
                            MActivePointerId = ev.GetPointerId(0);

                            if (MScrollState == ScrollStateSettling)
                            {
                                // Let the user 'catch' the pager as it animates.
                                MIsBeingDragged = true;
                                MIsUnableToDrag = false;
                                SetScrollState(ScrollStateDragging);
                            }
                            else
                            {
                                CompleteScroll();
                                MIsBeingDragged = false;
                                MIsUnableToDrag = false;
                            }

                            //if (Debug)
                            //    Log.Verbose(Tag, "Down at " + MLastMotionX + "," + MLastMotionY + " mIsBeingDragged=" + MIsBeingDragged + "mIsUnableToDrag=" + MIsUnableToDrag);
                            break;
                        }

                    case MotionEventActions.PointerUp:
                        OnSecondaryPointerUp(ev);
                        break;
                }
                
                /*
                 * The only time we want to intercept motion events is if we are in the
                 * drag mode.
                 */
                return MIsBeingDragged;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return base.OnInterceptTouchEvent(ev);
            }
        }

        public override bool OnTouchEvent(MotionEvent ev)
        {
            try
            {
                if (MFakeDragging)
                {
                    // A fake drag is in progress already, ignore this real one
                    // but still eat the touch events.
                    // (It is likely that the user is multi-touching the screen.)
                    return true;
                }

                if (ev.Action == MotionEventActions.Down && ev.EdgeFlags != 0)
                {
                    // Don't handle edge touches immediately -- they may actually belong
                    // to one of our
                    // descendants.
                    return false;
                }

                if (Adapter == null || Adapter.Count == 0)
                {
                    // Nothing to present or scroll; nothing to touch.
                    return false;
                }

                if (MVelocityTracker == null)
                {
                    MVelocityTracker = VelocityTracker.Obtain();
                }
                MVelocityTracker.AddMovement(ev);

                var action = ev.Action;
                //bool  needsInvalidate = false;

                switch (action & MotionEventActions.Mask)
                {
                    case MotionEventActions.Down:
                        {
                            /*
                             * If being flinged and user touches, stop the fling. isFinished
                             * will be false if being flinged.
                             */
                            CompleteScroll();

                            // Remember where the motion event started
                            MLastMotionX = MInitialMotionX = ev.GetX();
                            MActivePointerId = ev.GetPointerId(0);
                            break;
                        }
                    case MotionEventActions.Move:
                        {
                            if (!MIsBeingDragged)
                            {
                                int pointerIndex = ev.FindPointerIndex(MActivePointerId);
                                float x = ev.GetX(pointerIndex);
                                float xDiff = Math.Abs(x - MLastMotionX);
                                float y = ev.GetY(pointerIndex);
                                float yDiff = Math.Abs(y - MLastMotionY);
                                //if (Debug)
                                //    Log.Verbose(Tag, "Moved x to " + x + "," + y + " diff=" + xDiff + "," + yDiff);
                                if (xDiff > MTouchSlop && xDiff > yDiff)
                                {
                                    //if (Debug)
                                    //    Log.Verbose(Tag, "Starting drag!");
                                    MIsBeingDragged = true;
                                    MLastMotionX = x;
                                    SetScrollState(ScrollStateDragging);
                                    SetScrollingCacheEnabled(true);
                                }
                            }

                            if (MIsBeingDragged)
                            {
                                // Scroll to follow the motion event
                                int activePointerIndex = ev.FindPointerIndex(MActivePointerId);
                                float x = ev.GetX(activePointerIndex);
                                float deltaX = MLastMotionX - x;
                                MLastMotionX = x;
                                float oldScrollX = ScrollX;
                                float scrollX = oldScrollX + deltaX;
                                int width = Width;
                                int widthWithMargin = width + PageMargin;

                                int lastItemIndex = Adapter.Count - 1;
                                float leftBound = Math.Max(0, (MCurItem - 1) * widthWithMargin);
                                float rightBound = Math.Min(MCurItem + 1, lastItemIndex) * widthWithMargin;
                                if (scrollX < leftBound)
                                {
                                    if (leftBound == 0)
                                    {
                                        float over = -scrollX;
                                        MLeftEdge.OnPull(over / width);
                                    }
                                    scrollX = leftBound;
                                }
                                else if (scrollX > rightBound)
                                {
                                    if (rightBound == lastItemIndex * widthWithMargin)
                                    {
                                        float over = scrollX - rightBound;
                                        MRightEdge.OnPull(over / width);
                                    }
                                    scrollX = rightBound;
                                }
                                // Don't lose the rounded component
                                MLastMotionX += scrollX - (int)scrollX;
                                ScrollTo((int)scrollX, ScrollY);
                                if (MOnPageChangeListener != null)
                                {
                                    int position = (int)scrollX / widthWithMargin;
                                    int positionOffsetPixels = (int)scrollX % widthWithMargin;
                                    float positionOffset = (float)positionOffsetPixels / widthWithMargin;
                                    MOnPageChangeListener.OnPageScrolled(position, positionOffset, positionOffsetPixels);
                                }
                            } 
                        }
                        break;
                    case MotionEventActions.Up:
                    {
                        if (MIsBeingDragged)
                        {
                            VelocityTracker velocityTracker = MVelocityTracker;
                            velocityTracker.ComputeCurrentVelocity(1000, MMaximumVelocity);
                            int initialVelocity = (int)velocityTracker.GetXVelocity(MActivePointerId);
                            MPopulatePending = true;
                            int widthWithMargin = Width + PageMargin;
                            int scrollX = ScrollX;
                            int currentPage = scrollX / widthWithMargin;
                            int nextPage = initialVelocity > 0 ? currentPage : currentPage + 1;
                            SetCurrentItemInternal(nextPage, true, true, initialVelocity);

                            MActivePointerId = InvalidPointer;
                            EndDrag();
                            MLeftEdge.OnRelease();
                            MRightEdge.OnRelease();
                        } 
                    }
                        break;
                    case MotionEventActions.Cancel:
                        if (MIsBeingDragged)
                        {
                            SetCurrentItemInternal(MCurItem, true, true);
                            MActivePointerId = InvalidPointer;
                            EndDrag();
                            MLeftEdge.OnRelease();
                            MRightEdge.OnRelease();
                        }
                        break;
                    case MotionEventActions.PointerDown:
                        {
                            int index = ev.ActionIndex;
                            float x = ev.GetX(index);
                            MLastMotionX = x;
                            MActivePointerId = ev.GetPointerId(index);
                            break;
                        }
                    case MotionEventActions.PointerUp:
                        OnSecondaryPointerUp(ev);
                        MLastMotionX = ev.GetX(ev.FindPointerIndex(MActivePointerId));
                        break;
                }

                Invalidate();
                return true;
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
                return base.OnTouchEvent(ev);
            }
        }
         
        public override void Draw(Canvas canvas)
        {
            try
            {
                base.Draw(canvas);

                bool needsInvalidate = false;

                var overScrollMode = OverScrollMode;
                if (overScrollMode == OverScrollMode.Always || overScrollMode == OverScrollMode.IfContentScrolls && Adapter != null && Adapter.Count > 1)
                {
                    if (!MLeftEdge.IsFinished)
                    {
                        int restoreCount = canvas.Save();
                        int height = Height - PaddingTop - PaddingBottom;

                        canvas.Rotate(270);
                        canvas.Translate(-height + PaddingTop, 0);
                        MLeftEdge.SetSize(height, Width);
                        needsInvalidate |= MLeftEdge.Draw(canvas);
                        canvas.RestoreToCount(restoreCount);
                    }
                    if (!MRightEdge.IsFinished)
                    {
                         int restoreCount = canvas.Save();
                         int width = Width;
                         int height = Height - PaddingTop - PaddingBottom;
                         int itemCount = Adapter?.Count ?? 1;

                        canvas.Rotate(90);
                        canvas.Translate(-PaddingTop, -itemCount * (width + PageMargin) + PageMargin);
                        MRightEdge.SetSize(height, width);
                        needsInvalidate |= MRightEdge.Draw(canvas);
                        canvas.RestoreToCount(restoreCount);
                    }
                }
                else
                {
                    MLeftEdge.Finish();
                    MRightEdge.Finish();
                }

                if (needsInvalidate)
                {
                    // Keep animating
                   Invalidate();
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e); 
            }
        }

        protected override void OnDraw(Canvas canvas)
        {
            try
            {
                base.OnDraw(canvas);

                // Draw the margin drawable if needed.
                if (PageMargin > 0 && MMarginDrawable != null)
                {
                    int scrollX = ScrollX;
                    int width = Width;
                    int offset = scrollX % (width + PageMargin);
                    if (offset != 0)
                    {
                        // Pages fit completely when settled; we only need to draw when
                        // in between
                        int left = scrollX - offset + width;
                        MMarginDrawable.SetBounds(left, 0, left + PageMargin, Height);
                        MMarginDrawable.Draw(canvas);
                    }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e); 
            }
        }


        /// <summary>
        /// Start a fake drag of the pager.
        ///
        /// A fake drag can be useful if you want to synchronize the motion of the
        /// ViewPager with the touch scrolling of another view, while still letting
        /// the ViewPager control the snapping motion and fling behavior. (e.g.
        /// parallax-scrolling tabs.) Call {@link #fakeDragBy(float)} to simulate the
        /// actual drag motion. Call {@link #endFakeDrag()} to complete the fake drag and fling as necessary.
        ///
        /// During a fake drag the ViewPager will ignore all touch events. If a real
        /// drag is already in progress, this method will return false.
        /// 
        /// </summary>
        /// <returns>true if the fake drag began successfully, false if it could not be started.</returns>
        /// <see cref="FakeDragBy"/>
        /// <see cref="EndFakeDrag"/>
        public bool BeginFakeDrag()
        {
            try
            {
                if (MIsBeingDragged)
                {
                    return false;
                }
                MFakeDragging = true;
                SetScrollState(ScrollStateDragging);
                MInitialMotionX = MLastMotionX = 0;
                if (MVelocityTracker == null)
                {
                    MVelocityTracker = VelocityTracker.Obtain();
                }
                else
                {
                    MVelocityTracker.Clear();
                }
                long time = SystemClock.UptimeMillis();
                MotionEvent ev = MotionEvent.Obtain(time, time, MotionEventActions.Down, 0, 0, 0);
                MVelocityTracker.AddMovement(ev);
                ev.Recycle();
                MFakeDragBeginTime = time;
                return true;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return false;
            }
        }

        /// <summary>
        /// End a fake drag of the pager.
        /// </summary>
        /// <see cref="BeginFakeDrag"/>
        /// <see cref="FakeDragBy"/>
        public void EndFakeDrag()
        {
            try
            {
                if (!MFakeDragging)
                {
                    throw new IllegalStateException("No fake drag in progress. Call beginFakeDrag first.");
                }

                VelocityTracker velocityTracker = MVelocityTracker;
                velocityTracker.ComputeCurrentVelocity(1000, MMaximumVelocity);
                int initialVelocity = (int)velocityTracker.GetYVelocity(MActivePointerId);
                MPopulatePending = true;
                if (Math.Abs(initialVelocity) > MMinimumVelocity
                    || Math.Abs(MInitialMotionX - MLastMotionX) >= Width / 3)
                {
                    if (MLastMotionX > MInitialMotionX)
                    {
                        SetCurrentItemInternal(MCurItem - 1, true, true);
                    }
                    else
                    {
                        SetCurrentItemInternal(MCurItem + 1, true, true);
                    }
                }
                else
                {
                    SetCurrentItemInternal(MCurItem, true, true);
                }
                EndDrag();

                MFakeDragging = false;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e); 
            }
        }

        /// <summary>
        /// Fake drag by an offset in pixels. You must have called {@link #beginFakeDrag()} first.
        /// 
        /// </summary>
        /// <param name="xOffset">Offset in pixels to drag by.</param>
        /// <see cref="BeginFakeDrag"/>
        /// <see cref="EndFakeDrag"/>
        public void FakeDragBy(float xOffset)
        {
            try
            {
                if (!MFakeDragging)
                {
                    throw new IllegalStateException("No fake drag in progress. Call beginFakeDrag first.");
                }

                MLastMotionX += xOffset;
                float scrollX = ScrollX - xOffset;
                int width = Width;
                int widthWithMargin = width + PageMargin;

                float leftBound = Math.Max(0, (MCurItem - 1) * widthWithMargin);
                float rightBound = Math.Min(MCurItem + 1, Adapter.Count - 1) * widthWithMargin;
                if (scrollX < leftBound)
                {
                    scrollX = leftBound;
                }
                else if (scrollX > rightBound)
                {
                    scrollX = rightBound;
                }
                // Don't lose the rounded component
                MLastMotionX += scrollX - (int)scrollX;
                ScrollTo((int)scrollX, ScrollY);
                if (MOnPageChangeListener != null)
                {
                    int position = (int)scrollX / widthWithMargin;
                    int positionOffsetPixels = (int)scrollX % widthWithMargin;
                    float positionOffset = (float)positionOffsetPixels / widthWithMargin;
                    MOnPageChangeListener.OnPageScrolled(position, positionOffset, positionOffsetPixels);
                }

                // Synthesize an event for the VelocityTracker.
                long time = SystemClock.UptimeMillis();
                MotionEvent ev = MotionEvent.Obtain(MFakeDragBeginTime, time, MotionEventActions.Move, MLastMotionX, 0, 0);
                MVelocityTracker.AddMovement(ev);
                ev.Recycle();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e); 
            }
        }

        public bool IsFakeDragging()
        {
            return MFakeDragging;
        }

        private void OnSecondaryPointerUp(MotionEvent ev)
        {
            try
            {
                int pointerIndex = ev.ActionIndex;
                int pointerId = ev.GetPointerId(pointerIndex);
                if (pointerId == MActivePointerId)
                {
                    // This was our active pointer going up. Choose a new
                    // active pointer and adjust accordingly.
                    int newPointerIndex = pointerIndex == 0 ? 1 : 0;
                    MLastMotionX = ev.GetX( newPointerIndex);
                    MActivePointerId = ev.GetPointerId(newPointerIndex);
                    MVelocityTracker?.Clear();
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }    
        }

        private void EndDrag()
        {
            try
            {
                MIsBeingDragged = false;
                MIsUnableToDrag = false;

                if (MVelocityTracker != null)
                {
                    MVelocityTracker.Recycle();
                    MVelocityTracker = null;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e); 
            }
        }

        public void SetScrollingCacheEnabled(bool enabled)
        {
            try
            {
                if (MScrollingCacheEnabled != enabled)
                {
                    MScrollingCacheEnabled = enabled;
                    if (UseCache)
                    {
                        int size = ChildCount;
                        for (int i = 0; i < size; ++i)
                        {
                            View child = GetChildAt(i);
                            if (child.Visibility != ViewStates.Gone)
                            {
#pragma warning disable 618
                                child.DrawingCacheEnabled = enabled;
#pragma warning restore 618
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e); 
            }
        }

        public bool CanScroll(View v, bool checkV, int dx, int x, int y)
        {
            try
            {
                if (v is ViewGroup group)
                {

                    int scrollX = v.ScrollX;
                    int scrollY = v.ScrollY;
                    int count = group.ChildCount;
                    // Count backwards - let topmost views consume scroll distance
                    // first.
                    for (int i = count - 1; i >= 0; i--)
                    {
                        // TODO: Add versioned support here for transformed views.
                        // This will not work for transformed views in Honeycomb+
                        View child = group.GetChildAt(i);
                        if (x + scrollX >= child.Left && x + scrollX < child.Right && y + scrollY >= child.Top &&
                            y + scrollY < child.Bottom && CanScroll(child, true, dx, x + scrollX - child.Left, y + scrollY - child.Top))
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e); 
            }

            return checkV && v.CanScrollHorizontally(-dx);
        }


        public override bool DispatchKeyEvent(KeyEvent e)
        { 
            return base.DispatchKeyEvent(e) || ExecuteKeyEvent(e);
        }

        /// <summary>
        /// You can call this function yourself to have the scroll view perform
        /// scrolling from a key event, just as if the event had been dispatched to it by the view hierarchy.
        /// </summary>
        /// <param name="e">The key event to execute.</param>
        /// <returns>Return true if the event was handled, else false.</returns>
        public bool ExecuteKeyEvent(KeyEvent e)
        {
            try
            {
                bool handled = false;
                if (e.Action == KeyEventActions.Down)
                {
                    switch (e.KeyCode)
                    {
                        case Keycode.DpadLeft:
                            handled = ArrowScroll(FocusSearchDirection.Left);
                            break;
                        case Keycode.DpadRight:
                            handled = ArrowScroll(FocusSearchDirection.Right);
                            break;
                        case Keycode.Tab:
                            if (e.HasNoModifiers)
                            {
                                handled = ArrowScroll(FocusSearchDirection.Forward);
                            }
                            else if (e.HasModifiers(MetaKeyStates.ShiftOn))
                            {
                                handled = ArrowScroll(FocusSearchDirection.Backward);
                            }

                            break;
                    }
                }

                return handled;
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
                return false;
            }
        }
         
        public bool ArrowScroll(FocusSearchDirection direction)
        {
            try
            {
                View currentFocused = FindFocus();
                if (currentFocused == this)
                    currentFocused = null;

                bool  handled = false;

                View nextFocused = FocusFinder.Instance.FindNextFocus(this, currentFocused, direction);
                if (nextFocused != null && nextFocused != currentFocused)
                {
                    handled = direction switch
                    {
                        // If there is nothing to the left, or this is causing us to
                        // jump to the right, then what we really want to do is page left.
                        FocusSearchDirection.Left when currentFocused != null && nextFocused.Left >= currentFocused.Left => PageLeft(),
                        FocusSearchDirection.Left => nextFocused.RequestFocus(),
                        // If there is nothing to the right, or this is causing us to
                        // jump to the left, then what we really want to do is page right.
                        FocusSearchDirection.Right when currentFocused != null && nextFocused.Left <= currentFocused.Left => PageRight(),
                        FocusSearchDirection.Right => nextFocused.RequestFocus(),
                        _ => handled
                    };
                }
                else switch (direction)
                {
                    case FocusSearchDirection.Left:
                    case FocusSearchDirection.Backward:
                        // Trying to move left and nothing there; try to page.
                        handled = PageLeft();
                        break;
                    case FocusSearchDirection.Right:
                    case FocusSearchDirection.Forward:
                        // Trying to move right and nothing there; try to page.
                        handled = PageRight();
                        break;
                }
                if (handled)
                {
                    PlaySoundEffect(SoundEffectConstants.GetContantForFocusDirection(direction));
                }
                return handled;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return false;
            }
        }


        public bool  PageLeft()
        {
            try
            {
                if (MCurItem > 0)
                {
                    SetCurrentItem(MCurItem - 1, true);
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return false;
            }
        }

        public bool  PageRight()
        {
            try
            {
                if (Adapter != null && MCurItem < Adapter.Count - 1)
                {
                    SetCurrentItem(MCurItem + 1, true);
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return false;
            }
        }

        /// <summary> 
        /// We only want the current page that is being shown to be focusable.
        /// </summary>
        /// <param name="views"></param>
        /// <param name="direction"></param>
        /// <param name="focusableMode"></param>
        public override void AddFocusables(IList<View> views, FocusSearchDirection direction, FocusablesFlags focusableMode)
        {
            base.AddFocusables(views, direction, focusableMode);
            try
            { 
                int focusableCount = views.Count;

                var descendantFocusability = DescendantFocusability;

                if (descendantFocusability != DescendantFocusability.BlockDescendants)
                {
                    for (int i = 0; i < ChildCount; i++)
                    {
                        View child = GetChildAt(i);
                        if (child.Visibility == ViewStates.Visible)
                        {
                            ItemInfo ii = InfoForChild(child);
                            if (ii != null && ii.Position == MCurItem)
                            {
                                child.AddFocusables(views, direction, focusableMode);
                            }
                        }
                    }
                }

                // we add ourselves (if focusable) in all cases except for when we are
                // FOCUS_AFTER_DESCENDANTS and there are some descendants focusable.
                // this is
                // to avoid the focus search finding layouts when a more precise search
                // among the focusable children would be more interesting.
                if (descendantFocusability != DescendantFocusability.AfterDescendants ||
                    // No focusable descendants
                    focusableCount == views.Count)
                {
                    // Note that we can't call the superclass here, because it will
                    // add all views in. So we need to do the same thing View does.
                    if (!Focusable)
                    {
                        return;
                    }
                    if ((focusableMode & FocusablesFlags.TouchMode) == FocusablesFlags.TouchMode && IsInTouchMode && !FocusableInTouchMode)
                    {
                        return;
                    }

                    views?.Add(this);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        /// <summary>
        /// We only want the current page that is being shown to be touchable.
        /// </summary>
        /// <param name="views"></param>
        public override void AddTouchables(IList<View> views)
        {
            base.AddTouchables(views);
            try
            {
                // Note that we don't call super.addTouchables(), which means that
                // we don't call View.addTouchables(). This is okay because a ViewPager
                // is itself not touchable.
                for (int i = 0; i < ChildCount; i++)
                {
                     View child = GetChildAt(i);
                    if (child.Visibility == ViewStates.Visible)
                    {
                        ItemInfo ii = InfoForChild(child);
                        if (ii != null && ii.Position == MCurItem)
                        {
                            child.AddTouchables(views);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e); 
            } 
        }


        protected override bool OnRequestFocusInDescendants(int direction, Rect previouslyFocusedRect)
        { 
            try
            {
                int index;
                int increment;
                int end;
                int count = ChildCount;
                if ((direction & (int)FocusSearchDirection.Forward) != 0)
                {
                    index = 0;
                    increment = 1;
                    end = count;
                }
                else
                {
                    index = count - 1;
                    increment = -1;
                    end = -1;
                }
                for (int i = index; i != end; i += increment)
                {
                    View child = GetChildAt(i);
                    if (child.Visibility == ViewStates.Visible)
                    {
                        ItemInfo ii = InfoForChild(child);
                        if (ii != null && ii.Position == MCurItem)
                        {
                            if (child.RequestFocus((FocusSearchDirection)direction, previouslyFocusedRect))
                            {
                                return true;
                            }
                        }
                    }
                }
                return false;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return base.OnRequestFocusInDescendants(direction, previouslyFocusedRect);
            }
        }

        public override bool DispatchPopulateAccessibilityEvent(AccessibilityEvent e)
        {
            try
            {
                int childCount = ChildCount;
                for (int i = 0; i < childCount; i++)
                {
                    View child = GetChildAt(i);
                    if (child.Visibility == ViewStates.Visible)
                    {
                        ItemInfo ii = InfoForChild(child);
                        if (ii != null && ii.Position == MCurItem && child.DispatchPopulateAccessibilityEvent(e))
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
                return base.DispatchPopulateAccessibilityEvent(e);
            } 
        }

    }
}