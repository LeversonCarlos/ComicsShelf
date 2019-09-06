using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ComicsShelf.Library
{
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class LibraryPage : ContentPage
   {

      public LibraryPage()
      {
         InitializeComponent();
      }

      private void DeleteLibrary_Clicked(object sender, System.EventArgs e)
      {
         try
         {
            var vm = this.BindingContext as LibraryVM;
            DependencyService.Get<Store.ILibraryStore>().DeleteLibraryAsync(vm.Library.ID);
         }
         catch (Exception ex) { Helpers.AppCenter.TrackEvent(ex); }
      }

   }
}