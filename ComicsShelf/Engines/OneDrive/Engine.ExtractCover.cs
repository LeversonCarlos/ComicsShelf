using ComicsShelf.ComicFiles;
using ComicsShelf.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.OneDrive.Files;

namespace ComicsShelf.Engines.OneDrive
{
   partial class OneDriveEngine
   {

      public async Task<bool> ExtractCover(LibraryModel library, ComicFile comicFile)
      {
         var log = new List<string>();
         try
         {
            log.Add($"Extracting Cover:true");

            // VALIDATE
            var hasNetworkAccess = Xamarin.Essentials.Connectivity.NetworkAccess == Xamarin.Essentials.NetworkAccess.Internet;
            log.Add($"Has Network Access:{hasNetworkAccess}");
            if (!hasNetworkAccess) { return false; }

            // RETRIEVE THE DOWNLOAD URL
            var downloadUrl = await this.Connector.GetDownloadUrlAsync(new FileData { id = comicFile.Key });
            log.Add($"Download Url:{downloadUrl}");
            if (string.IsNullOrEmpty(downloadUrl)) { return false; }

            // OPEN REMOTE STREAM
            using (var zipStream = new System.IO.Compression.HttpZipStream(downloadUrl))
            {

               // STREAM SIZE
               var streamSizeValue = comicFile.GetKeyValue("StreamSize");
               log.Add($"Stream Size Value:{streamSizeValue}");
               if (!string.IsNullOrEmpty(streamSizeValue))
               {
                  long streamSize;
                  if (long.TryParse(streamSizeValue, out streamSize))
                  { zipStream.SetContentLength(streamSize); }
               }

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