using ComicsShelf.ComicFiles;
using ComicsShelf.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComicsShelf.Engines.OneDrive
{
   partial class OneDriveEngine
   {

      public async override Task<bool> ExtractCover(LibraryModel library, ComicFile comicFile)
      {
         var log = new List<string>();
         try
         {
            log.Add($"Extracting Cover:true");

            // VALIDATE
            var hasNetworkAccess = Xamarin.Essentials.Connectivity.NetworkAccess == Xamarin.Essentials.NetworkAccess.Internet;
            log.Add($"Has Network Access:{hasNetworkAccess}");
            if (!hasNetworkAccess) { return false; }

            // REFRESH FILE DETAILS
            var fileVM = await this.Service.GetDetails(comicFile.Key);
            if (!fileVM.KeyValues.TryGetValue("downloadUrl", out string downloadUrl)) { return false; }
            if (string.IsNullOrEmpty(downloadUrl)) { return false; }

            // OPEN REMOTE STREAM
            using (var zipStream = new System.IO.Compression.HttpZipStream(downloadUrl))
            {

               // STREAM SIZE
               if (fileVM.SizeInBytes.HasValue)
                  zipStream.SetContentLength(fileVM.SizeInBytes.Value);

               // FIRST ENTRY
               var entryList = await zipStream.GetEntriesAsync();
               log.Add($"Entry List Count:{entryList?.Count ?? 0}");
               var entry = entryList
                  .Where(x =>
                     x.FileName.ToLower().EndsWith(".jpg") ||
                     x.FileName.ToLower().EndsWith(".jpeg") ||
                     x.FileName.ToLower().EndsWith(".png"))
                  .OrderBy(x => x.FileName)
                  .FirstOrDefault();
               log.Add($"Has Cover Entry:{entry != null}");
               if (entry == null) { return false; }

               // RETRIEVE REMOTE IMAGE CONTENT
               var imageByteArray = await zipStream.ExtractAsync(entry);
               log.Add($"Cover Byte Array:{imageByteArray?.Length ?? 0}");
               if (imageByteArray == null || imageByteArray.Length == 0) { return false; }

               // SAVE CACHE FILE
               using (var imageStream = new System.IO.MemoryStream(imageByteArray))
               {
                  log.Add($"Saving Cover Image:true");
                  await imageStream.FlushAsync();
                  imageStream.Position = 0;
                  await this.FileSystem.SaveThumbnail(imageStream, comicFile.CoverPath);
                  imageStream.Close();
                  System.IO.File.SetLastWriteTime(comicFile.CoverPath, comicFile.ReleaseDate);
                  log.Add($"Saved Cover Image:true");
               }

            }

            var extractedCover = System.IO.File.Exists(comicFile.CoverPath);
            log.Add($"Extracted Cover:{extractedCover}");
            return extractedCover;
         }
         catch (Exception ex) { log.Add($"Exception:{ex.Message}"); Helpers.AppCenter.TrackEvent(ex); return false; }
         finally { Helpers.AppCenter.TrackEvent("Extract Cover Flow", log.ToArray()); }
      }

   }
}