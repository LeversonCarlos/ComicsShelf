using ComicsShelf.ComicFiles;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.OneDrive.Files;

namespace ComicsShelf.Engines.OneDrive
{
   partial class OneDriveEngine
   {

      public async Task<bool> ExtractCover(Libraries.LibraryModel library, ComicFile comicFile)
      {
         try
         {

            var downloadUrl = await this.Connector.GetDownloadUrlAsync(new FileData { id = comicFile.Key });
            if (string.IsNullOrEmpty(downloadUrl)) { return false; }

            using (var zipStream = new System.IO.Compression.HttpZipStream(downloadUrl))
            {

               // STREAM SIZE
               var streamSizeValue = comicFile.GetKeyValue("StreamSize");
               if (!string.IsNullOrEmpty(streamSizeValue))
               {
                  long streamSize;
                  if (long.TryParse(streamSizeValue, out streamSize))
                  { zipStream.SetContentLength(streamSize); }
               }

               // FIRST ENTRY
               var entryList = await zipStream.GetEntriesAsync();
               var entry = entryList
                  .Where(x =>
                     x.FileName.ToLower().EndsWith(".jpg") ||
                     x.FileName.ToLower().EndsWith(".jpeg") ||
                     x.FileName.ToLower().EndsWith(".png"))
                  .OrderBy(x => x.FileName)
                  .FirstOrDefault();
               if (entry == null) { return false; }

               // RETRIEVE REMOTE IMAGE CONTENT
               var imageByteArray = await zipStream.ExtractAsync(entry);
               if (imageByteArray == null || imageByteArray.Length == 0) { return false; }

               // SAVE CACHE FILE
               using (var imageStream = new System.IO.MemoryStream(imageByteArray))
               {
                  await imageStream.FlushAsync();
                  imageStream.Position = 0;
                  await this.FileSystem.SaveThumbnail(imageStream, comicFile.CoverPath);
                  imageStream.Close();
                  System.IO.File.SetLastWriteTime(comicFile.CoverPath, comicFile.ReleaseDate);
               }

            }

            return (System.IO.File.Exists(comicFile.CoverPath));
         }
         catch (Exception) { throw; }
      }

   }
}