using AE_Xamarin.Managers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace VexIQTimer
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            //Create App Manager Instance
            AppManager.Instance.ToString();
            MainPage = new TimerPage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
