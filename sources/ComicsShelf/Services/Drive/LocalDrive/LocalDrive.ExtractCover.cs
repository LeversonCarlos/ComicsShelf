using ComicsShelf.ViewModels;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace ComicsShelf.Drive
{
   partial class LocalDrive
   {

      public override async Task<bool> ExtractCover(LibraryVM library, ItemVM libraryItem)
      {
         try
         {

            // PARAMETERS
            var filePath = $"{libraryItem.FolderPath}/{libraryItem.FileName}";
            var coverPath = $"{Helpers.Paths.CoversCache}/{libraryItem.EscapedID}.jpg";

            // CHECK OF THE FILE EXISTS
            if (!File.Exists(filePath)) { return false; }

            // OPEN STREAM TO EXTRACT IMAGE FROM IT
            if (!File.Exists(coverPath))
            {
               using (var fileStream = await this.CloudService.Download(filePath))
               {
                  using (var fileArchive = new ZipArchive(fileStream, ZipArchiveMode.Read))
                  {

                     // FIRST ENTRY [should be the cover]
                     var coverEntry = fileArchive.Entries
                        .Where(entry => ImageExtentions.Any(ext => entry.Name.EndsWith(ext, StringComparison.CurrentCultureIgnoreCase)))
                        .OrderBy(entry => entry.Name)
                        .Take(1)
                        .FirstOrDefault();
                     if (coverEntry == null) { return false; }

                     // RETRIEVE IMAGE CONTENT
                     using (var coverStream = coverEntry.Open())
                     {
                        if (!await this.FileSystem.SaveThumbnail(coverStream, coverPath))
                           return false;
                     }

                     // RELEASE DATE
                     File.SetLastWriteTime(coverPath, coverEntry.LastWriteTime.DateTime.ToLocalTime());
                     libraryItem.ReleaseDate = coverEntry.LastWriteTime.DateTime.ToLocalTime();

                  }
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
