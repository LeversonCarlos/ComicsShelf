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
      public ObservableList<ComicFile> ComicFiles { get; private set; }
      public LibraryVM(LibraryModel value)
      {
         this.Library = value;
         this.Title = value.Description;
         this.ComicFiles = new ObservableList<ComicFile>();
         this.ItemSelectCommand = new Command(() => this.ItemSelect());
      }

      public async Task OnAppearing()
      {
         this.IsBusy = true;
         var service = DependencyService.Get<LibraryService>();
         this.ComicFiles.AddRange(service.ComicFiles[this.Library.ID]);

         Messaging.Subscribe<List<ComicFile>>("OnRefreshingList", this.Library.ID, this.OnRefreshingList);
         Messaging.Subscribe<ComicFile>("OnRefreshingItem", this.Library.ID, this.OnRefreshingItem);

         this.IsBusy = false;
      }

      public async Task OnDisappearing()
      {

         Messaging.Unsubscribe("OnRefreshingList", this.Library.ID);
         Messaging.Unsubscribe("OnRefreshingItem", this.Library.ID);
         this.ComicFiles.Clear();

      }


      public void OnRefreshingList(List<ComicFile> comicFiles)
      {
         this.ComicFiles.ReplaceRange(comicFiles);
      }

      public void OnRefreshingItem(ComicFile comicFile)
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
