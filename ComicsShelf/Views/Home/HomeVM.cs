using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Views.Home
{
   public class HomeVM : Folder.FolderVM<HomeData>
   {

      #region New
      public HomeVM() : this(new HomeData { Text = R.Strings.AppTitle }) { }
      public HomeVM(HomeData args) : base(args)
      {
         this.Title = args.Text;
#if DEBUG
         try
         {
            var appVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            this.Title += $" - v{appVersion.Major}.{appVersion.Minor}.{appVersion.Build}";
         }
         catch { }
#endif
         this.ViewType = typeof(HomeView);

         this.Data = args;
         this.OpenLibraryCommand = new Command(async (item) => await this.OpenLibrary(item));

         Engine.AppCenter.TrackEvent("Home: Show View");
      }
      #endregion

      #region OpenLibrary
      public Command OpenLibraryCommand { get; set; }
      private async Task OpenLibrary(object item)
      {
         try
         { await Helpers.NavVM.PushAsync<Views.Library.LibraryVM>(false); }
         catch (Exception ex) { await App.ShowMessage(ex); }
      }
      #endregion

   }
}