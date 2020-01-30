using AE_Xamarin.Managers;
using Plugin.SimpleAudioPlayer;
using System;
using System.Timers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VexIQTimer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TimerPage : ContentPage
    {
        //Timer Data
        TimerState CurrentTimerState = TimerState.Start;
        DateTime TimerStartTime = DateTime.Now;
        Timer TimerBackend = new Timer(1000); //Interval Every Second
        float TotalSecondCount = 0;

        //Audio Players
        ISimpleAudioPlayer ChangeControllerSoundPlayer;
        ISimpleAudioPlayer StartGameSoundPlayer;
        ISimpleAudioPlayer EndGameSoundPlayer;

        public TimerPage()
        {
            InitializeComponent();

            //Set The Picker Value
            TimePicker.SelectedIndex = 0;

            //Get Elapsed Event
            TimerBackend.Elapsed += TimerBackend_Elapsed;

            //Update The Theme
            UpdateThemeColors(AppThemeManager.Instance.CurrentTheme);
            AppThemeManager.Instance.AppThemeChange += Instance_AppThemeChange;

            //Set Up Audio Players
            StartGameSoundPlayer = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
            StartGameSoundPlayer.Load("TimerSounds/StartGameSound.mp3");
            EndGameSoundPlayer = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
            EndGameSoundPlayer.Load("TimerSounds/EndGameSound.mp3");

            ChangeControllerSoundPlayer = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
            ChangeControllerSoundPlayer.Load("TimerSounds/ChangeControllerSound.mp3");
        }

        public void UpdateThemeColors(AppTheme CurrentTheme)
        {
            TimeLBL.TextColor = CurrentTheme.TextColor;
            TimePicker.TextColor = CurrentTheme.TextColor;
            SelectTimeLBL.TextColor = CurrentTheme.TextColor;
            StartBTN.ForegroundColor = CurrentTheme.TextColor;
            StartBTN.BackgroundColor = CurrentTheme.AccentColor;
            TimerCircle.RingBaseColor = Color.FromHex("#1C1C1C");
            TimerCircle.RingProgressColor = CurrentTheme.AccentColor;
            MainLayout.BackgroundColor = CurrentTheme.BackgroundColor;
        }
        private void Instance_AppThemeChange(object sender, ThemeChangeArgs e)
        {
            UpdateThemeColors(e.NewTheme);
        }
        private void TimerAndProgressCircle_SizeChanged(object sender, EventArgs e)
        {
            //Check If Landscape Or Portrait
            if (TimerAndProgressCircle.Width > TimerAndProgressCircle.Height)
            {
                TimerAndProgressCircle.Orientation = StackOrientation.Horizontal;
                TimerAndProgressCircle.Spacing = 0;
                TimeLBL.FontSize = 128;
            }
            else
            {
                TimerAndProgressCircle.Orientation = StackOrientation.Vertical;
                TimerAndProgressCircle.Spacing = 6;
                TimeLBL.FontSize = 64;
            }
        }

        private void StartBTN_Clicked(object sender, EventArgs e)
        {
            //Switch To The New State And Fix Label
            switch (CurrentTimerState)
            {
                case TimerState.Start:
                    CurrentTimerState = TimerState.Running;
                    StartBTN.Text = "Stop";
                    break;
                case TimerState.Running:
                    CurrentTimerState = TimerState.Stopped;
                    StartBTN.Text = "Reset";
                    break;
                case TimerState.Stopped:
                    CurrentTimerState = TimerState.Start;
                    StartBTN.Text = "Start";
                    break;
            }

            //Process The New State
            switch (CurrentTimerState)
            {
                case TimerState.Start:
                    TimeLBL.Text = TimePicker.SelectedItem.ToString();
                    TimerCircle.Progress = 1;
                    TotalSecondCount = 0;
                    break;
                case TimerState.Stopped:
                    TimerBackend.Stop();
                    EndGameSoundPlayer.Play();
                    break;
                case TimerState.Running:
                    //Get Total Seccond Count
                    string[] TimeParts = TimePicker.SelectedItem.ToString().Split(':');
                    TotalSecondCount += int.Parse(TimeParts[0]) * 60;
                    TotalSecondCount += int.Parse(TimeParts[1]);

                    //Start The Timer
                    TimerStartTime = DateTime.Now;
                    StartGameSoundPlayer.Play();
                    TimerBackend.Start();
                    break;
            }
        }
        private void TimePicker_Focused(object sender, FocusEventArgs e)
        {
            CurrentTimerState = TimerState.Start;
            StartBTN.Text = "Start";
            TotalSecondCount = 0;
            TimerBackend.Stop();
        }
        private void TimePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Reset The Time Label & Progress
            TimerCircle.Progress = 1;
            TimeLBL.Text = TimePicker.SelectedItem.ToString();
        }

        private void TimerBackend_Elapsed(object sender, ElapsedEventArgs e)
        {
            //Find The Total Progress
            int CurrentSecondCount = (int)TotalSecondCount - (e.SignalTime - TimerStartTime).Seconds;
            float CurrentProgress = CurrentSecondCount / TotalSecondCount;
            TimerCircle.Progress = CurrentProgress;

            //Check If We Reached Max
            if (CurrentProgress <= 0)
            {
                TimerBackend.Stop();
                CurrentSecondCount = 0;
                CurrentTimerState = TimerState.Stopped;
                Device.BeginInvokeOnMainThread(() =>
                {
                    EndGameSoundPlayer.Play(); StartBTN.Text = "Reset";
                });
            }

            //Check Switch Controller Times
            if (CurrentSecondCount == 35 && TotalSecondCount != 35)
            {
                Device.BeginInvokeOnMainThread(() => { ChangeControllerSoundPlayer.Play(); });
            }
            if (CurrentSecondCount == 25 && TotalSecondCount != 25)
            {
                Device.BeginInvokeOnMainThread(() => { ChangeControllerSoundPlayer.Play(); });
            }

            //Update The Label
            if (CurrentTimerState != TimerState.Stopped)
            {
                string TimeLBLText = $"{CurrentSecondCount / 60}:{CurrentSecondCount % 60:00}";
                Device.BeginInvokeOnMainThread(() => { TimeLBL.Text = TimeLBLText; });
            }
            else
            {
                Device.BeginInvokeOnMainThread(() => { TimeLBL.Text = "0:00"; });
            }
        }
    }
}
