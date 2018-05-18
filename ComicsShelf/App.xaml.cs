using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Xamarin.Forms;

namespace ComicsShelf
{
   public partial class App : Application
	{

      #region New
      public App()
      {
         InitializeComponent();
         MainPage = new NavigationPage(new ContentPage
         {
            Title = R.Strings.AppTitle,
            Content = new Label
            {
               Text = R.Strings.BASE_WAIT_MESSAGE,
               HorizontalOptions = LayoutOptions.CenterAndExpand,
               VerticalOptions = LayoutOptions.CenterAndExpand
            }
         });
      }
      #endregion

      #region Settings
      private Helpers.Settings.Settings _Settings = null;
      internal static Helpers.Settings.Settings Settings
      {
         get { return ((App)Application.Current)._Settings; }
         set { ((App)Application.Current)._Settings = value; }
      }
      #endregion

      #region Database
      private Database.Connector _Database = null;
      internal static Database.Connector Database
      {
         get { return ((App)Application.Current)._Database; }
         set { ((App)Application.Current)._Database = value; }
      }
      #endregion

      #region Message
      private static Helpers.Controls.Messages _Message = null;
      public static Helpers.Controls.Messages Message
      {
         get
         {
            if (_Message == null) { _Message = new Helpers.Controls.Messages(); }
            return _Message;
         }
      }
      #endregion

      #region RootFolder
      private Home.HomeData _RootFolder = null;
      public static Home.HomeData RootFolder
      {
         get { return ((App)Application.Current)._RootFolder; }
         set { ((App)Application.Current)._RootFolder = value; }
      }
      #endregion

      protected override void OnStart()
      {
         Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
         {
            AppCenter.Start("android=4ebe7891-1962-4e2a-96c4-c37a7c06c104;" +
                            "uwp=21539a63-8335-46ef-8771-c9c001371f87;" +
                            "ios={Your iOS App secret here}",
                              typeof(Analytics), typeof(Crashes));
         });
         Engine.Startup.Execute();
      }

      protected override void OnSleep ()
		{ /* Handle when your app sleeps */ }

		protected override void OnResume ()
		{ /* Handle when your app resumes */ }

	}
}
