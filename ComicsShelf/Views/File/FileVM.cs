using System;
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
            if (this.Data.Pages == null) { this.Data.Pages = new Helpers.Observables.ObservableList<PageData>(); }
            if (this.Data.Pages.Count != 0) { return; }

            /*
            var fileSystem = Helpers.FileSystem.Get();

            // DEFINE PATH
            var cachePath = this.Data.FullPath
               .Replace(App.Settings.Paths.Separator, "_")
               .Replace("#", "")
               .Replace(":", "")
               .Replace(".", "_")
               .Replace(" ", "_")
               .Replace("___", "_")
               .Replace("__", "_");
            cachePath = $"{App.Settings.Paths.FilesCachePath}{App.Settings.Paths.Separator}{cachePath}";
            if (!Directory.Exists(cachePath)) { Directory.CreateDirectory(cachePath); }

            // OPEN ZIP ARCHIVE
            using (var zipArchive = await fileSystem.GetZipArchive(App.Settings, this.Data))
            {

               // LOOP THROUGH ZIP ENTRIES
               short pageIndex = 0;
               var zipEntries = zipArchive.Entries
                  .Where(x => x.Name.ToLower().EndsWith(".jpg"))
                  .OrderBy(x => x.Name)
                  .ToList();
               foreach (ZipArchiveEntry zipEntry in zipEntries)
               {

                  // PAGE DATA
                  var pageIndexText = pageIndex.ToString().PadLeft(3, "0".ToCharArray()[0]);
                  var pagePath = $"{cachePath}{App.Settings.Paths.Separator}P{pageIndexText}.jpg";
                  var pageData = new FilePageData { Page = pageIndex, Path = pagePath, IsVisible=false };
                  this.Data.Pages.Add(pageData);
                  pageIndex++;

                  // EXTRACT PAGE IMAGE
                  if (System.IO.File.Exists(pagePath)) { continue; }
                  try
                  {
                     using (System.IO.Stream zipStream = zipEntry.Open())
                     {
                        FileStream pageFile = null;
                        await Task.Run(() => pageFile = System.IO.File.Open(pagePath, FileMode.OpenOrCreate));
                        await zipStream.CopyToAsync(pageFile);
                        await pageFile.FlushAsync();
                        pageFile.Dispose();
                     }
                  }
                  catch { }

               }
            }

            this.Data.Pages.Add(new FilePageData { });
            */

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
         { GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced); }
         catch (Exception ex) { await App.ShowMessage(ex); }
      }
      #endregion

   }
}