using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Views.Library
{
   public class LibraryVM : Helpers.DataVM<LibraryData>
   {

      #region New
      public LibraryVM()
      {
         this.Title = R.Strings.LIBRARY_MAIN_TITLE; ;
         this.ViewType = typeof(LibraryView);
         this.Data = new LibraryData { LibraryPath = App.Settings.Paths.LibraryPath };
         this.LibraryTappedCommand = new Command(async (item) => await this.LibraryTapped(item));
         this.LinkTappedCommand = new Command(async (item) => await this.LinkTapped(item));
      }
      #endregion

      #region LibraryTapped
      public Command LibraryTappedCommand { get; set; }
      private async Task LibraryTapped(object item)
      {
         try
         {
            if (await Engine.Library.Execute())
            {             
               await Helpers.NavVM.PushAsync<Views.Home.HomeVM>(true, App.HomeData);
            }
         }
         catch (Exception ex) { await App.ShowMessage(ex); }
      }
      #endregion

      #region LinkTapped
      public Command LinkTappedCommand { get; set; }
      private async Task LinkTapped(object item)
      {
         try
         { Device.OpenUri(new Uri("http://www.comicbookplus.com")); }
         catch (Exception ex) { await App.ShowMessage(ex); }
      }
      #endregion

   }
}