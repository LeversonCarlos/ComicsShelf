using ComicsShelf.ComicFiles;
using ComicsShelf.Helpers.Observables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Featured
{
   public class FeaturedVM : BaseVM
   {

      public ObservableList<ComicFolderVM> ComicFolders { get; private set; }
      public Notify.NotifyVM Notify { get; private set; }

      public FeaturedVM()
      {
         this.Title = R.Strings.HOME_MAIN_TITLE;
         this.Notify = new Notify.NotifyVM("General");

         this.ComicFolders = new ObservableList<ComicFolderVM>();
         this.ComicFolders.Add(new ComicFolderVM { FolderPath = R.Strings.HOME_READING_FILES_SECTION_TITLE });
         this.ComicFolders.Add(new ComicFolderVM { FolderPath = R.Strings.HOME_RECENT_FILES_SECTION_TITLE });

         this.OpenCommand = new Command(async (item) => await this.Open(item as ComicFileVM));
         Messaging.Subscribe<List<ComicFileVM>>("OnRefreshingReadingFilesList", this.OnRefreshingReadingFilesList);
         Messaging.Subscribe<List<ComicFileVM>>("OnRefreshingRecentFilesList", this.OnRefreshingRecentFilesList);
      }

      bool _HasComicFiles;
      public bool HasComicFiles
      {
         get { return this._HasComicFiles; }
         set { this.SetProperty(ref this._HasComicFiles, value); }
      }

      public Command OpenCommand { get; set; }
      private async Task Open(ComicFileVM item)
      {
         try
         {
            var vm = new Splash.SplashVM(item);
            var page = new Splash.SplashPage { BindingContext = vm };
            await App.Navigation().PushAsync(page);
         }
         catch (Exception ex) { await App.ShowMessage(ex); }
      }

      private void OnRefreshingReadingFilesList(List<ComicFileVM> comicFiles)
      { this.OnRefreshingList(this.ComicFolders[0].ComicFiles, comicFiles); }

      private void OnRefreshingRecentFilesList(List<ComicFileVM> comicFiles)
      { this.OnRefreshingList(this.ComicFolders[1].ComicFiles, comicFiles); }

      private void OnRefreshingList(ObservableList<ComicFileVM> oldItems, List<ComicFileVM> newItems)
      {
         var oldHash = string.Join("|", oldItems.Select(x => x.ComicFile.Key));
         var newHash = string.Join("|", newItems.Select(x => x.ComicFile.Key));
         if (oldHash == newHash) { return; }
         oldItems.ReplaceRange(newItems);
         this.HasComicFiles = this.ComicFolders.SelectMany(x => x.ComicFiles).Count() > 0;
      }

   }
}
