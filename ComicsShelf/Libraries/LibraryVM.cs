using ComicsShelf.ComicFiles;
using ComicsShelf.Helpers.Observables;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Libraries
{
   public class LibraryVM : Helpers.BaseVM
   {

      internal readonly LibraryModel Library;
      public ObservableList<ComicFileVM> ComicFiles { get; private set; }
      public LibraryVM(LibraryModel value)
      {
         this.Library = value;
         this.Title = value.Description;
         this.ComicFiles = new ObservableList<ComicFileVM>();
         this.ItemSelectCommand = new Command(() => this.ItemSelect());
      }

      public void OnAppearing()
      {
         this.IsBusy = true;
         var service = DependencyService.Get<LibraryService>();
         this.ComicFiles.AddRange(service.ComicFiles[this.Library.ID]);

         Messaging.Subscribe<List<ComicFileVM>>("OnRefreshingList", this.Library.ID, this.OnRefreshingList);
         Messaging.Subscribe<ComicFileVM>("OnRefreshingItem", this.Library.ID, this.OnRefreshingItem);

         this.IsBusy = false;
      }

      public void OnDisappearing()
      {
         Messaging.Unsubscribe("OnRefreshingList", this.Library.ID);
         Messaging.Unsubscribe("OnRefreshingItem", this.Library.ID);
         this.ComicFiles.Clear();
      }


      public void OnRefreshingList(List<ComicFileVM> comicFiles)
      {
         this.ComicFiles.ReplaceRange(comicFiles);
      }

      public void OnRefreshingItem(ComicFileVM comicFile)
      {
         this.ComicFiles.Replace(comicFile);
      }

      public ComicFile SelectedItem { get; set; }

      public Command ItemSelectCommand { get; set; }
      void ItemSelect()
      {
         var service = DependencyService.Get<LibraryService>();
         service.Test(this.Library.ID, this.SelectedItem.Key);
      }

   }
}
