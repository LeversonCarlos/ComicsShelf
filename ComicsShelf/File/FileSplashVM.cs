using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.File
{
   public class FileSplashVM : Helpers.ViewModels.DataVM<FileData>
   {

      #region New
      public FileSplashVM(FileData args)
      {
         this.Title = args.Text;
         this.ViewType = typeof(FileSplashPage);
         this.Data = args;
         this.OpenTappedCommand = new Command(async (item) => await this.OpenTapped(item));
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
            await PushAsync<FileReadVM>(this.Data);
         }
         catch (Exception ex) { await App.Message.Show(ex.ToString()); }
         finally { this.IsBusy = false; }
      }
      #endregion

      #region OpenFile
      private async Task OpenFile()
      {
         try
         {
            if (this.Data.Pages == null) { this.Data.Pages = new Helpers.Observables.ObservableList<FilePageData>(); }
            if (this.Data.Pages.Count != 0) { return; }
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
         }
         catch (Exception ex) { throw; }
         finally { GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced); }
      }
      #endregion

   }
}