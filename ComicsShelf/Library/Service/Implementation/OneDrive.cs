using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.OneDrive.Profile;
using Xamarin.OneDrive.Files;
using System.Linq;
using System;

namespace ComicsShelf.Library.Implementation
{
   internal class OneDrive : ILibraryService
   {

      Xamarin.OneDrive.Connector Connector { get; set; }
      public OneDrive()
      {
         var clientID = System.Environment.GetEnvironmentVariable("clientID");
         this.Connector = new Xamarin.OneDrive.Connector(clientID, "User.Read", "Files.Read");
      }

      public async Task<bool> Validate(Helpers.Database.Library library)
      { return await this.Connector.ConnectAsync(); }

      public async Task<bool> AddLibrary(Helpers.Database.Library library)
      {
         if (!await this.Connector.ConnectAsync()) { return false; }
         var profile = await this.Connector.GetProfileAsync();
         if (profile == null) { return false; }
         library.LibraryPath = profile.id;
         library.LibraryText = profile.Name;
         return true;
      }

      public async Task<List<Helpers.Database.ComicFile>> SearchFilesAsync(Helpers.Database.Library library)
      {
         try
         {

            // LOCATE FILES
            var fileList = await this.Connector.SearchFilesAsync("*.cbz");

            // CONVERT
            var comicFiles = fileList
               .Select(file => new Helpers.Database.ComicFile
               {
                  LibraryPath = library.LibraryPath,
                  Key = file.id,
                  FullPath = $"{file.FilePath}/{file.FileName}",
                  ParentPath = file.FilePath,
                  ReleaseDate = (!file.CreatedDateTime.HasValue ? "" : App.Database.GetDate(file.CreatedDateTime.Value.ToLocalTime())),
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
      }

      public async Task ExtractPagesAsync(Helpers.Database.Library library, Views.File.FileData fileData)
      {
      }

   }
}