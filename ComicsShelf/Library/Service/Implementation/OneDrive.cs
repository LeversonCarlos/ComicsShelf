using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.OneDrive.Files;
using Xamarin.OneDrive.Profile;

namespace ComicsShelf.Library.Implementation
{
   internal class OneDrive : ILibraryService
   {

      Xamarin.OneDrive.Connector Connector { get; set; }
      public OneDrive()
      {
         var clientID = System.Environment.GetEnvironmentVariable("ComicsShelfApplicationID");
         this.Connector = new Xamarin.OneDrive.Connector(clientID, "User.Read", "Files.Read");
      }

      public async Task<bool> Validate(Helpers.Database.Library library)
      {
         library.Available = await this.Connector.ConnectAsync();
         return library.Available;
      }

      public async Task<bool> AddLibrary(Helpers.Database.Library library)
      {
         if (!await this.Connector.ConnectAsync()) { return false; }
         var profile = await this.Connector.GetProfileAsync();
         if (profile == null) { return false; }
         library.LibraryPath = profile.id;
         library.LibraryText = profile.Name;
         library.Available = true;
         return true;
      }

      public async Task<List<Helpers.Database.ComicFile>> SearchFilesAsync(Helpers.Database.Library library)
      {
         try
         {

            // LOCATE FILES
            var fileList = await this.Connector.SearchFilesAsync("*.cbz");
            /*
            fileList = fileList
               .Where(x => x.FilePath.StartsWith("/Media"))
               .Take(50)
               .ToList();
            */

            // CONVERT
            var comicFiles = fileList
               .Select(file => new Helpers.Database.ComicFile
               {
                  LibraryPath = library.LibraryPath,
                  Key = file.id,
                  FullPath = $"{file.FilePath}/{file.FileName}",
                  ParentPath = file.FilePath,
                  ReleaseDate = (!file.CreatedDateTime.HasValue ? "" : App.Database.GetDate(file.CreatedDateTime.Value.ToLocalTime())),
                  StreamSize = (file.Size.HasValue? file.Size.Value:0),
                  Available = true
               })
               .ToList();

            // TEXT
            foreach (var comicFile in comicFiles)
            {
               comicFile.FullText = System.IO.Path.GetFileNameWithoutExtension(comicFile.FullPath).Trim();
               var folderText = System.IO.Path.GetFileNameWithoutExtension(comicFile.ParentPath);
               if (!string.IsNullOrEmpty(folderText))
               { comicFile.SmallText = comicFile.FullText.Replace(folderText, ""); }
               if (string.IsNullOrEmpty(comicFile.SmallText))
               { comicFile.SmallText = comicFile.FullText; }
            }

            // RESULT
            return comicFiles;

         }
         catch (Exception) { throw; }
      }

      public async Task ExtractCoverAsync(Helpers.Database.Library library, Helpers.Database.ComicFile comicFile)
      {
         try
         {
            var downloadUrl = await this.Connector.GetDownloadUrlAsync(new FileData { id = comicFile.Key });
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
               await zipStream.ExtractAsync(entry, async (entryStream) => {

                  using (var thumbnailFile = new System.IO.FileStream(comicFile.CoverPath, System.IO.FileMode.CreateNew, System.IO.FileAccess.Write))
                  {
                     await entryStream.CopyToAsync(thumbnailFile);
                     await thumbnailFile.FlushAsync();
                     thumbnailFile.Close();
                     thumbnailFile.Dispose();
                  }

               });
            }
         }
         catch (Exception) { throw; }
      }

      public async Task ExtractPagesAsync(Helpers.Database.Library library, Views.File.FileData fileData)
      {
      }

   }
}