using ComicsShelf.ViewModels;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ComicsShelf.Drive
{
   partial class OneDrive
   {

      public override async Task<bool> ExtractCover(LibraryVM library, ItemVM libraryItem)
      {
         try
         {

            // CHECK SERVICE
            if (!await this.CloudService.CheckConnectionAsync()) return false;
            var coverPath = $"{Helpers.Paths.CoversCache}/{libraryItem.EscapedID}.jpg";
            
            if (!File.Exists(coverPath))
            {

               // REFRESH FILE DETAILS
               var fileVM = await this.CloudService.GetDetails(libraryItem.ID);
               if (!fileVM.KeyValues.TryGetValue("downloadUrl", out string downloadUrl)) return false;
               if (string.IsNullOrEmpty(downloadUrl)) return false;

               // OPEN REMOTE STREAM
               using (var fileArchive = new System.IO.Compression.HttpZipStream(downloadUrl))
               {

                  // STREAM SIZE
                  if (fileVM.SizeInBytes.HasValue)
                     fileArchive.SetContentLength(fileVM.SizeInBytes.Value);

                  // FIRST ENTRY [should be the cover]
                  var entryList = await fileArchive.GetEntriesAsync();
                  var coverEntry = entryList
                     .Where(entry => ImageExtentions.Any(ext => entry.FileName.EndsWith(ext, StringComparison.CurrentCultureIgnoreCase)))
                     .OrderBy(entry => entry.FileName)
                     .Take(1)
                     .FirstOrDefault();
                  if (coverEntry == null) { return false; }

                  // RETRIEVE REMOTE IMAGE CONTENT
                  var coverByteArray = await fileArchive.ExtractAsync(coverEntry);
                  if (coverByteArray == null || coverByteArray.Length == 0) { return false; }

                  // SAVE CACHE FILE
                  using (var coverStream = new MemoryStream(coverByteArray))
                  {
                     await coverStream.FlushAsync();
                     coverStream.Position = 0;
                     await this.FileSystem.SaveThumbnail(coverStream, coverPath);
                     coverStream.Close();
                  }

                  // RELEASE DATE
                  File.SetLastWriteTime(coverPath, fileVM.CreatedDateTime.GetValueOrDefault());
                  libraryItem.ReleaseDate = fileVM.CreatedDateTime.GetValueOrDefault();

               }

            }

            // RETURN
            if (!File.Exists(coverPath)) { return false; }
            libraryItem.CoverPath = coverPath;
            return true;
         }
         catch (Exception ex) { Helpers.Insights.TrackException(new Exception($"Stop extracting covers at the item [{libraryItem.FullText}]", ex)); return false; }
      }

   }
}
