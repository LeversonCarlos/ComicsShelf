using System;
using System.Linq;
using System.Threading.Tasks;

namespace ComicsShelf.Libraries.Implementation
{
   partial class OneDrive
   {

      public async Task<bool> SaveDataAsync(Library library)
      {
         try
         {
            
            // LOAD COMIC FILES
            var libraryComics = App.HomeData.Libraries
               .Where(x => x.ComicFolder.LibraryPath == library.LibraryID)
               .Select(x => x.Files)
               .SelectMany(x => x)
               .Select(x => x.ComicFile)
               .ToList();

            // CONVERT AND SERIALIZE
            var libraryData = libraryComics
               .Select(x => new
               {
                  x.Key,
                  x.ReleaseDate,
                  x.Readed,
                  x.ReadingDate,
                  x.ReadingPage,
                  x.ReadingPercent,
                  x.Rating
               })
               .ToList();
            var librarySerializedData = vTwo.Helpers.FileStream.Serialize(libraryData);

            // SAVE DATA
            // if (!await Helpers.Permissions.HasStoragePermission()) { return false; }
            // if (!await this.FileSystem.SaveDataAsync(library, librarySerializedData)) { return false; }

            // RESULT
            return true;
         }
         catch (Exception ex) { Engine.AppCenter.TrackEvent("OneDrive.SaveDataAsync", ex); await App.ShowMessage(ex); return false; }
      }

   }
}