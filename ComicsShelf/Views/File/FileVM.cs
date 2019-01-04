using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Views.File
{
   public class FileVM : Helpers.DataVM<FileData>
   {

      #region New
      public FileVM(FileData args)
      {
         this.Title = args.FullText;
         this.ViewType = typeof(FileView);
         this.Data = args;
         this._Readed = args.Readed;
         this._Rating = args.Rating;
         this.OpenTappedCommand = new Command(async (item) => await this.OpenTapped(item));
         this.Finalize += this.OnFinalize;
         Engine.AppCenter.TrackEvent("File: Show View");
      }
      #endregion

      #region OpenTapped
      public Command OpenTappedCommand { get; set; }
      private async Task OpenTapped(object item)
      {
         try
         {
            this.IsBusy = true;
            await this.OpenFile();
            await PushAsync<PageVM>(this.Data);
         }
         catch (Exception ex) { await App.ShowMessage(ex); }
         finally { this.IsBusy = false; }
      }
      #endregion

      #region OpenFile
      private async Task OpenFile()
      {
         try
         {

            // VALIDATE
            if (this.Data.Pages == null) { this.Data.Pages = new Helpers.Observables.ObservableList<PageData>(); }
            if (this.Data.Pages.Count != 0) { return; }

            // TRACK
            var OpeningTime = DateTime.Now;
            Engine.AppCenter.TrackEvent("File: Opening");

            // EXTRACT PAGES
            var library = App.Settings.Paths.Libraries
               .Where(x => x.LibraryPath == this.Data.ComicFile.LibraryPath)
               .FirstOrDefault();
            var libraryService = Library.LibraryService.Get(library);
            await libraryService.ExtractPagesAsync(library, this.Data);
            this.Data.TotalPages = (short)this.Data.Pages.Count;
            this.Data.Pages.Add(new PageData { });

            // TRACK
            var milliseconds = (long)(DateTime.Now - OpeningTime).TotalMilliseconds;
            Engine.AppCenter.TrackEvent("File: Opened", "milliseconds", milliseconds.ToString());

         }
         catch (Exception ex) { throw; }
         finally { GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced); }
      }
      #endregion


      #region Readed
      bool _Readed;
      public bool Readed
      {
         get { return this._Readed; }
         set
         {
            this.SetProperty(ref this._Readed, value);

            this.Data.Readed = value;
            this.Data.ReadingDate = (value ? App.Database.GetDate() : null);
            this.Data.ReadingPercent = (value ? 1 : 0);
            App.Database.Update(this.Data.ComicFile);
         }
      }
      #endregion

      #region Rating
      int _Rating;
      public int Rating
      {
         get { return this._Rating; }
         set
         {
            this.SetProperty(ref this._Rating, value);

            this.Data.Rating = value;
            App.Database.Update(this.Data.ComicFile);
         }
      }
      #endregion     


      #region OnFinalize
      private async void OnFinalize()
      {
         try
         {
            GC.Collect();
            Engine.Statistics.Execute();
         }
         catch (Exception ex) { await App.ShowMessage(ex); }
      }
      #endregion

   }
}