using ComicsShelf.Helpers.Observables;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace ComicsShelf.Library
{
   public class LibraryVM : Helpers.DataVM<ObservableList<LibraryData>>
   {

      #region New
      public LibraryVM()
      {
         this.Title = R.Strings.LIBRARY_MAIN_TITLE; ;
         this.ViewType = typeof(LibraryView);

         this.Data = new Helpers.Observables.ObservableList<LibraryData>();
         this.Data.CollectionChanged += this.Libraries_CollectionChanged;
         var libraries = App.Settings.Libraries
            .Select(x => new LibraryData(x))
            .AsEnumerable();
         libraries.ForEach(library => this.Data.Add(library));

         this.AddLibraryTappedCommand = new Command(async (item) => await this.AddLibraryTapped(item));
         this.RemoveLibraryTappedCommand = new Command(async (item) => await this.RemoveLibraryTapped(item));
         this.AddOneDriveLibraryCommand = new Command(async (item) => await this.AddOneDriveLibrary(item));
      }
      #endregion

      #region Libraries

      private void Libraries_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
      {
         this.HasLibraries = this.Data.Count != 0;
         // this.HasntLibraries = this.Libraries.Count == 0;
         this.HasOneDriveLibrary = this.Data.Count(x => x.LibraryType == vTwo.Libraries.TypeEnum.OneDrive) != 0;
      }

      #endregion


      #region AddLibrary 

      public Command AddLibraryTappedCommand { get; set; }
      private async Task AddLibraryTapped(object item)
      {
         this.IsBusy = true;
         await this.AddLibrary(vTwo.Libraries.TypeEnum.FileSystem);
         this.IsBusy = false;
      }

      public Command AddOneDriveLibraryCommand { get; set; }
      private async Task AddOneDriveLibrary(object item)
      {
         this.IsBusy = true;
         await this.AddLibrary(vTwo.Libraries.TypeEnum.OneDrive);
         this.IsBusy = false;
      }

      private async Task AddLibrary(vTwo.Libraries.TypeEnum libraryType)
      {
         try
         {

            // INITIALIZE
            var library = new vTwo.Libraries.Library { Type = libraryType };

            // USE SERVICE IMPLEMENTATION
            var libraryService = LibraryService.Get(library);
            if (!await libraryService.AddLibrary(library))
            { await App.ShowMessage(R.Strings.LIBRARY_INVALID_FOLDER_MESSAGE); return; }

            // SAVE LIBRARY CONFIG
            App.Settings.Libraries.Add(library);
            await App.Settings.SaveLibraries();
            this.Data.Add(new LibraryData(library));

            // SCHEDULE LIBRARY REFRESH
            Engine.Search.Refresh(library);

         }
         catch (Exception ex) { Engine.AppCenter.TrackEvent("AddLibrary", ex); await App.ShowMessage(ex); }
      }

      #endregion

      #region RemoveLibrary

      public Command RemoveLibraryTappedCommand { get; set; }
      private async Task RemoveLibraryTapped(object item)
      {
         this.IsBusy = true;
         await this.RemoveLibrary(item as LibraryData);
         this.IsBusy = false;
      }

      internal async Task RemoveLibrary(LibraryData item)
      {
         try
         {

            // USE SERVICE IMPLEMENTATION
            var library = item.Library;
            var libraryService = LibraryService.Get(library);
            if (!await libraryService.RemoveLibrary(library)) { return; }

            // REMOVE FILES AND THE LIBRARY ITSELF
            var comicFiles = App.Database.Table<Helpers.Database.ComicFile>()
               .Where(x => x.LibraryPath == library.LibraryID)
               .ToList();
            await Task.Run(() => {
               foreach (var comicFile in comicFiles)
               { App.Database.Delete(comicFile); }
            });

            // REMOVE LIBRARY DATA
            var libraryData = App.HomeData.Libraries
               .Where(x => x.ComicFolder.LibraryPath == library.LibraryID)
               .FirstOrDefault();
            foreach (var librarySection in libraryData.Sections)
            {
               foreach (var libraryFolder in librarySection.Folders)
               { libraryFolder.Files.Clear(); }
               librarySection.Folders.Clear();
            }
            libraryData.Sections.Clear();
            libraryData.Folders.Clear();
            libraryData.Files.Clear();
            App.HomeData.Libraries.Remove(libraryData);

            // REMOVE LIBRARY ITSELF
            App.Settings.Libraries.Remove(library);
            await App.Settings.SaveLibraries();
            this.Data.Remove(item);

         }
         catch (Exception ex) { Engine.AppCenter.TrackEvent("RemoveLibrary", ex); await App.ShowMessage(ex); }
      }

      #endregion


      #region HasLibraries
      bool _HasLibraries;
      public bool HasLibraries
      {
         get { return this._HasLibraries; }
         set { this.SetProperty(ref this._HasLibraries, value); }
      }
      #endregion

      #region HasOneDriveLibrary
      bool _HasOneDriveLibrary;
      public bool HasOneDriveLibrary
      {
         get { return this._HasOneDriveLibrary; }
         set { this.SetProperty(ref this._HasOneDriveLibrary, value); }
      }
      #endregion

   }
}