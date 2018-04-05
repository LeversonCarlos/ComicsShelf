using Xamarin.Forms;

namespace ComicsShelf
{
   public partial class App : Application
	{

      #region New
      public App()
      {
         InitializeComponent();
         MainPage = new NavigationPage(new InitialPage());
         /*
         ((NavigationPage)MainPage).Popped += (object sender, NavigationEventArgs e) => {

            if (e.Page.BindingContext != null && e.Page.BindingContext.GetType() == typeof(Helpers.ViewModels.BaseVM))
            {
               ((Helpers.ViewModels.BaseVM)e.Page.BindingContext).Dispose();
               e.Page.BindingContext = null;
            }
            GC.Collect();
         };
         */
      }
      #endregion

      #region Settings
      private static Helpers.Settings.Settings _Settings = null;
      internal static Helpers.Settings.Settings Settings
      {
         get
         {
            if (_Settings == null) { _Settings = new Helpers.Settings.Settings(); }
            return _Settings;
         }
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

      protected override void OnStart ()
      { Startup.StartupEngine.Start(); }

      protected override void OnSleep ()
		{ /* Handle when your app sleeps */ }

		protected override void OnResume ()
		{ /* Handle when your app resumes */ }

	}
}
