using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;
using WoWonder.Helpers.Utils;
using WoWonder.NiceArt.Models;

namespace WoWonder.NiceArt
{
    public class BrushDrawingView : View
    {
        static readonly float DEFAULT_BRUSH_SIZE = 25.0f;
        static readonly float DEFAULT_ERASER_SIZE = 50.0f;
        static readonly int DEFAULT_OPACITY = 255;

        private float MBrushSize = DEFAULT_BRUSH_SIZE;
        private float MBrushEraserSize = DEFAULT_ERASER_SIZE;
        private int MOpacity = DEFAULT_OPACITY;

        public List<LinePath> MDrawnPaths = new List<LinePath>();
        public List<LinePath> MRedoPaths = new List<LinePath>();
        private Paint MDrawPaint;

        private Canvas MDrawCanvas;
        private bool MBrushDrawMode;

        private Path MPath;
        private float MTouchX, MTouchY;
        private static readonly float TouchTolerance = 4;

        private INiceArt.IBrushViewChangeListener MBrushViewChangeListener;

        public class LinePath
        {
            private readonly Paint MDrawPaint;
            private readonly Path MDrawPath;

            public LinePath(Path drawPath, Paint drawPaints)
            {
                MDrawPaint = new Paint(drawPaints);
                MDrawPath = new Path(drawPath);
            }

            public Paint GetDrawPaint()
            {
                return MDrawPaint;
            }

            public Path GetDrawPath()
            {
                return MDrawPath;
            }
        }

        public BrushDrawingView(Context context) : base(context)
        {
            try
            {
                SetupBrushDrawing();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e); 
            }
        }

        public BrushDrawingView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            try
            {
                SetupBrushDrawing();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e); 
            }
        }

        public BrushDrawingView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            try
            {
                SetupBrushDrawing();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);  
            }
        }

        public void SetupBrushDrawing()
        {
            try
            {
                //Caution: This line is to disable hardware acceleration to make eraser feature work properly
                SetLayerType(LayerType.Hardware, null);
                MDrawPaint = new Paint();
                MPath = new Path();
                MDrawPaint.AntiAlias = true;
                MDrawPaint.Dither = true;
                MDrawPaint.Color = Color.Black;
                MDrawPaint.SetStyle(Paint.Style.Stroke);
                MDrawPaint.StrokeJoin = Paint.Join.Round;
                MDrawPaint.StrokeCap = Paint.Cap.Round;
                MDrawPaint.StrokeWidth = MBrushSize;
                MDrawPaint.Alpha = MOpacity;
                MDrawPaint.SetXfermode(new PorterDuffXfermode(PorterDuff.Mode.SrcOver));
                Visibility = ViewStates.Gone;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);

            }
        }


        public void RefreshBrushDrawing()
        {
            try
            {
                MBrushDrawMode = true;
                MPath = new Path();
                MDrawPaint.AntiAlias = true;
                MDrawPaint.Dither = true;
                MDrawPaint.SetStyle(Paint.Style.Stroke);
                MDrawPaint.StrokeJoin = Paint.Join.Round;
                MDrawPaint.StrokeCap = Paint.Cap.Round;
                MDrawPaint.StrokeWidth = MBrushSize;
                MDrawPaint.Alpha = MOpacity;
                MDrawPaint.SetXfermode(new PorterDuffXfermode(PorterDuff.Mode.SrcOver));

            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);

            }
        }

        public void BrushEraser()
        {
            try
            {
                MBrushDrawMode = true;
                MDrawPaint.StrokeWidth = MBrushEraserSize;
                MDrawPaint.SetXfermode(new PorterDuffXfermode(PorterDuff.Mode.Clear));
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);

            }
        }

        public void SetBrushDrawingMode(bool brushDrawMode)
        {
            try
            {
                MBrushDrawMode = brushDrawMode;
                if (brushDrawMode)
                {
                    Visibility = ViewStates.Visible;
                    RefreshBrushDrawing();
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);

            }
        }

        public void SetOpacity(int opacity)
        {
            try
            {
                // if (Enumerable.Range(0, 255).Contains(opacity))

                if (opacity >= 0 && opacity <= 255)
                {
                    //true
                    MOpacity = opacity;
                    SetBrushDrawingMode(true);
                }
                else
                {
                    throw new Exception("The amount of transparency should be between 0 and 255");
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);

            }

        }

        public int getOpacity()
        {
            return MOpacity;
        }

        public bool GetBrushDrawingMode()
        {
            try
            {
                return MBrushDrawMode;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return false;

            }
        }

        public void SetBrushSize(float size)
        {
            try
            {
                MBrushSize = size;
                SetBrushDrawingMode(true);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);

            }
        }


        public void SetBrushColor(Color color)
        {
            try
            {
                MDrawPaint.Color = color;
                SetBrushDrawingMode(true);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);

            }
        }

        public void SetBrushEraserSize(float brushEraserSize)
        {
            try
            {
                MBrushEraserSize = brushEraserSize;
                SetBrushDrawingMode(true);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);

            }
        }

        public void SetBrushEraserColor(Color color)
        {
            try
            {
                MDrawPaint.Color = color;
                SetBrushDrawingMode(true);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);

            }
        }

        public float GetEraserSize()
        {
            try
            {
                return MBrushEraserSize;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return 0;

            }
        }


        public float GetBrushSize()
        {
            try
            {

            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);

            }
            return MBrushSize;
        }

        public int GetBrushColor()
        {
            try
            {
                return MDrawPaint.Color;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return 0;

            }
        }

        public void ClearAll()
        {
            try
            {
                MDrawnPaths.Clear();
                MRedoPaths.Clear();
                MDrawCanvas?.DrawColor(Color.Black, PorterDuff.Mode.Clear);
                Invalidate();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e); 
            }
        }

        public void SetBrushViewChangeListener(INiceArt.IBrushViewChangeListener brushViewChangeListener)
        {
            try
            {
                MBrushViewChangeListener = brushViewChangeListener;
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
                Bitmap canvasBitmap = Bitmap.CreateBitmap(w, h, Bitmap.Config.Argb8888);
                MDrawCanvas = new Canvas(canvasBitmap);
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

                foreach (var linePath in MDrawnPaths)
                {
                    canvas.DrawPath(linePath.GetDrawPath(), linePath.GetDrawPaint());
                }

                canvas.DrawPath(MPath, MDrawPaint);

            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);

            }
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            try
            {
                base.OnTouchEvent(e);
                if (MBrushDrawMode)
                {
                    float touchX = e.GetX();
                    float touchY = e.GetY();


                    switch (e.Action)
                    {
                        case MotionEventActions.Down:
                            TouchStart(touchX, touchY);
                            break;
                        case MotionEventActions.Move:
                            TouchMove(touchX, touchY);
                            break;
                        case MotionEventActions.Up:
                            TouchUp();
                            break;
                    }
                    Invalidate();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
                throw new Exception(exception.Message);
            }
        }

        public bool Undo()
        {
            try
            {
                if (MDrawnPaths.Count > 0)
                {
                    var last = MDrawnPaths.LastOrDefault();
                    if (last != null)
                    {
                        var lastLinePaths = MDrawnPaths.LastOrDefault();
                        if (lastLinePaths != null)
                        {
                            MRedoPaths.Add(lastLinePaths);
                        }
                        MDrawnPaths.Remove(last);
                    }
                    Invalidate();
                }

                MBrushViewChangeListener?.OnViewRemoved(this);

                return MDrawnPaths?.Count != 0;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return false;

            }
        }

        public bool Redo()
        {
            try
            {
                if (MRedoPaths.Count > 0)
                {
                    var last = MRedoPaths.LastOrDefault();
                    if (last != null)
                    {
                        var lastLinePaths = MRedoPaths.LastOrDefault();
                        if (lastLinePaths != null)
                        {
                            MDrawnPaths.Add(lastLinePaths);
                        }
                        MRedoPaths.Remove(last);
                    }
                    Invalidate();
                }

                MBrushViewChangeListener?.OnViewAdd(this);

                return MRedoPaths.Count != 0;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return false;

            }
        }

        public void TouchStart(float x, float y)
        {
            try
            {
                MRedoPaths.Clear();
                MPath.Reset();
                MPath.MoveTo(x, y);
                MTouchX = x;
                MTouchY = y;
                MBrushViewChangeListener?.OnStartDrawing();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);

            }
        }

        public void TouchMove(float x, float y)
        {
            try
            {
                float dx = Math.Abs(x - MTouchX);
                float dy = Math.Abs(y - MTouchY);
                if (dx >= TouchTolerance || dy >= TouchTolerance)
                {
                    MPath.QuadTo(MTouchX, MTouchY, (x + MTouchX) / 2, (y + MTouchY) / 2);
                    MTouchX = x;
                    MTouchY = y;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);

            }
        }

        public void TouchUp()
        {
            try
            {
                MPath.LineTo(MTouchX, MTouchY);
                // Commit the path to our offscreen
                MDrawCanvas.DrawPath(MPath, MDrawPaint);
                // kill this so we don't double draw
                MDrawnPaths.Add(new LinePath(MPath, MDrawPaint));

                MPath = new Path();
                if (MBrushViewChangeListener != null)
                {
                    MBrushViewChangeListener.OnStopDrawing();
                    MBrushViewChangeListener.OnViewAdd(this);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);  
            }
        }

        public Paint getDrawingPaint()
        {
            return MDrawPaint;
        }

        public Dictionary<List<LinePath>, List<LinePath>> getDrawingPath()
        {  
            return new Dictionary<List<LinePath>, List<LinePath>> { { MDrawnPaths, MRedoPaths } };
        }
    }
}