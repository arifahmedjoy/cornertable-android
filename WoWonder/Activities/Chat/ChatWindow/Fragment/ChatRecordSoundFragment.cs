using System;
using System.Timers;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using AndroidX.Interpolator.View.Animation;
using AT.Markushi.UI;
using WoWonder.Activities.Story;
using WoWonder.Helpers.Model;
using WoWonder.Helpers.Utils;
using Uri = Android.Net.Uri;

namespace WoWonder.Activities.Chat.ChatWindow.Fragment
{
    public class ChatRecordSoundFragment : AndroidX.Fragment.App.Fragment
    {

        private CircleButton RecordPlayButton;
        private CircleButton RecordCloseButton;
        private SeekBar VoiceSeekBar;
        private string RecordFilePath;
        private readonly string Type;
        private Methods.AudioRecorderAndPlayer AudioPlayerClass;
        private ChatWindowActivity MainActivity;
        private StoryReplyActivity MainStoryActivity;
        private Timer TimerSound;
        private MediaPlayer MediaPlayer;

        public ChatRecordSoundFragment(string type = "ChatWindowActivity")
        {
            Type = type;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                View view = inflater.Inflate(Resource.Layout.ChatRecourdSoundFragment, container, false);
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
                
                RecordFilePath = Arguments.GetString("FilePath");
                 
                VoiceSeekBar = view.FindViewById<SeekBar>(Resource.Id.voiceseekbar);
                VoiceSeekBar.Max = 10000;
                VoiceSeekBar.Progress = 0;

                RecordPlayButton = view.FindViewById<CircleButton>(Resource.Id.playButton);
                RecordPlayButton.Click += RecordPlayButton_Click;

                RecordCloseButton = view.FindViewById<CircleButton>(Resource.Id.closeRecourdButton);
                RecordCloseButton.Click += RecordCloseButton_Click;

                switch (Type)
                {
                    case "ChatWindowActivity":
                        MainActivity = (ChatWindowActivity)Activity;
                        MainActivity.RecordButton.SetImageResource(Resource.Drawable.SendLetter);
                        MainActivity.RecordButton.Tag = "Audio";
                        MainActivity.RecordButton.SetListenForRecord(false);

                        AudioPlayerClass = new Methods.AudioRecorderAndPlayer(MainActivity.UserId);
                        break;
                    case "StoryReplyActivity":
                        MainStoryActivity = (StoryReplyActivity)Activity;
                        MainStoryActivity.RecordButton.SetImageResource(Resource.Drawable.SendLetter);
                        MainStoryActivity.RecordButton.Tag = "Audio";
                        MainStoryActivity.RecordButton.SetListenForRecord(false);

                        AudioPlayerClass = new Methods.AudioRecorderAndPlayer(MainStoryActivity.UserId);
                        break;
                }
                TimerSound = new Timer();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
         
        private void RecordCloseButton_Click(object sender, EventArgs e)
        {
            try
            {
                StopAudioPlay();

                switch (UserDetails.SoundControl)
                {
                    case true:
                        Methods.AudioRecorderAndPlayer.PlayAudioFromAsset("Error.mp3");
                        break;
                }

                AudioPlayerClass.Delete_Sound_Path(RecordFilePath);
                 
                RecordFilePath = "";

                if (!string.IsNullOrEmpty(RecordFilePath))
                    Methods.AudioRecorderAndPlayer.StopAudioFromAsset();

                if (UserDetails.SoundControl)
                {
                    Methods.AudioRecorderAndPlayer.PlayAudioFromAsset("Error.mp3");
                }

                var fragmentHolder = Activity.FindViewById<FrameLayout>(Resource.Id.TopFragmentHolder);

                AudioPlayerClass.Delete_Sound_Path(RecordFilePath);
                var interplator = new FastOutSlowInInterpolator();
                fragmentHolder.Animate().SetInterpolator(interplator).TranslationY(1200).SetDuration(300);
                Activity.SupportFragmentManager.BeginTransaction().Remove(this)?.Commit();

                switch (Type)
                {
                    case "ChatWindowActivity":
                        MainActivity.RecordButton.Tag = "Free";
                        MainActivity.RecordButton.SetTheImageResource(Resource.Drawable.icon_mic_vector);
                        MainActivity.RecordButton.SetListenForRecord(true);
                        break;
                    case "StoryReplyActivity":
                        MainStoryActivity.RecordButton.Tag = "Free";
                        MainStoryActivity.RecordButton.SetTheImageResource(Resource.Drawable.icon_mic_vector);
                        MainStoryActivity.RecordButton.SetListenForRecord(true);
                        break;
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void RecordPlayButton_Click(object sender, EventArgs e)
        {
            try
            {
                switch (MediaPlayer)
                {
                    case null:
                        {
                            MediaPlayer = new MediaPlayer();
                            MediaPlayer.SetAudioAttributes(new AudioAttributes.Builder()?.SetUsage(AudioUsageKind.Media)?.SetContentType(AudioContentType.Music)?.Build());

                            MediaPlayer.Completion += (sender, e) =>
                            {
                                try
                                {
                                    RecordPlayButton.Tag = "Play";
                                    RecordPlayButton.SetImageResource(Resource.Drawable.ic_play_dark_arrow);

                                    MediaPlayer.Stop();
                                    MediaPlayer.Reset();
                                    MediaPlayer = null!;

                                    TimerSound.Enabled = false;
                                    TimerSound.Stop();
                                    TimerSound = null!;

                                    VoiceSeekBar.Progress = 0;
                                }
                                catch (Exception exception)
                                {
                                    Methods.DisplayReportResultTrack(exception);
                                }
                            };

                            MediaPlayer.Prepared += (s, ee) =>
                            {
                                try
                                {
                                    RecordPlayButton.Tag = "Pause";
                                    RecordPlayButton.SetImageResource(AppSettings.SetTabDarkTheme ? Resource.Drawable.ic_media_pause_light : Resource.Drawable.ic_media_pause_dark);

                                    TimerSound ??= new Timer { Interval = 1000 };

                                    MediaPlayer.Start();

                                    TimerSound.Elapsed += (sender, eventArgs) =>
                                    {
                                        Activity?.RunOnUiThread(() =>
                                        {
                                            try
                                            {
                                                if (TimerSound != null && TimerSound.Enabled)
                                                {
                                                    if (MediaPlayer != null)
                                                    {
                                                        int totalDuration = MediaPlayer.Duration;
                                                        int currentDuration = MediaPlayer.CurrentPosition;

                                                        // Updating progress bar
                                                        int progress = WoWonderTools.GetProgressSeekBar(currentDuration, totalDuration);

                                                        switch (Build.VERSION.SdkInt)
                                                        {
                                                            case >= BuildVersionCodes.N:
                                                                VoiceSeekBar.SetProgress(progress, true);
                                                                break;
                                                            default:
                                                                // For API < 24 
                                                                VoiceSeekBar.Progress = progress;
                                                                break;
                                                        }
                                                    }
                                                }
                                            }
                                            catch (Exception e)
                                            {
                                                Methods.DisplayReportResultTrack(e);
                                                RecordPlayButton.Tag = "Play";
                                            }
                                        });
                                    };
                                    TimerSound.Start();
                                }
                                catch (Exception e)
                                {
                                    Methods.DisplayReportResultTrack(e);
                                }
                            };

                            if (RecordFilePath.Contains("http"))
                            {
                                MediaPlayer.SetDataSource(Activity, Uri.Parse(RecordFilePath));
                                MediaPlayer.PrepareAsync();
                            }
                            else
                            {
                                Java.IO.File file2 = new Java.IO.File(RecordFilePath);
                                var photoUri = FileProvider.GetUriForFile(Activity, Activity.PackageName + ".fileprovider", file2);

                                MediaPlayer.SetDataSource(Activity, photoUri);
                                MediaPlayer.PrepareAsync();
                            }

                            break;
                        }
                    default:
                        switch (RecordPlayButton?.Tag?.ToString())
                        {
                            case "Play":
                                {
                                    RecordPlayButton.Tag = "Pause";
                                    RecordPlayButton.SetImageResource(AppSettings.SetTabDarkTheme ? Resource.Drawable.ic_media_pause_light : Resource.Drawable.ic_media_pause_dark);

                                    MediaPlayer?.Start();

                                    if (TimerSound != null)
                                    {
                                        TimerSound.Enabled = true;
                                        TimerSound.Start();
                                    }

                                    break;
                                }
                            case "Pause":
                                {
                                    RecordPlayButton.Tag = "Play";
                                    RecordPlayButton.SetImageResource(Resource.Drawable.ic_play_dark_arrow);

                                    MediaPlayer?.Pause();

                                    if (TimerSound != null)
                                    {
                                        TimerSound.Enabled = false;
                                        TimerSound.Stop();
                                    }

                                    break;
                                }
                        }

                        break;
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void StopAudioPlay()
        {
            try
            {
                RecordPlayButton.Tag = "Play";
                RecordPlayButton.SetColor(Color.White);
                RecordPlayButton.SetImageResource(Resource.Drawable.ic_play_dark_arrow);

                if (MediaPlayer != null)
                {
                    MediaPlayer.Stop();
                    MediaPlayer.Reset();
                }
                MediaPlayer = null!;


                if (TimerSound != null)
                {
                    TimerSound.Enabled = false;
                    TimerSound.Stop();
                }

                TimerSound = null!;

                VoiceSeekBar.Progress = 0;
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

        public override void OnDestroy()
        {
            try
            {
                StopAudioPlay();
                base.OnDestroy();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
    }
}