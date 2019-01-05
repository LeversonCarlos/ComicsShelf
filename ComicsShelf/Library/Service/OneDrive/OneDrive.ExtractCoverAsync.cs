using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.OneDrive.Files;

namespace ComicsShelf.Library.Implementation
{
   partial class OneDrive
   {

      public async Task ExtractCoverAsync(Helpers.Database.Library library, Helpers.Database.ComicFile comicFile)
      {
         try
         {
            var downloadUrl = await this.Connector.GetDownloadUrlAsync(new FileData { id = comicFile.Key });
            if (string.IsNullOrEmpty(downloadUrl)) { return; }

            using (var zipStream = new System.IO.Compression.HttpZipStream(downloadUrl))
            {
               if (comicFile.StreamSize > 0) { zipStream.SetContentLength(comicFile.StreamSize); }
               var entryList = await zipStream.GetEntriesAsync();
               var entry = entryList
                  .Where(x =>
                     x.FileName.ToLower().EndsWith(".jpg") ||
                     x.FileName.ToLower().EndsWith(".jpeg") ||
                     x.FileName.ToLower().EndsWith(".png"))
                  .OrderBy(x => x.FileName)
                  .FirstOrDefault();
               await zipStream.ExtractAsync(entry, async (entryStream) =>
               {
                  using (var imageStream = new System.IO.MemoryStream())
                  {
                     await entryStream.CopyToAsync(imageStream);
                     await imageStream.FlushAsync();
                     imageStream.Position = 0;
                     await this.FileSystem.SaveThumbnail(imageStream, comicFile.CoverPath);
                  }
               });
            }

         }
         catch (Exception ex) { throw new Exception(comicFile.FullPath, ex); }
      }

   }
}