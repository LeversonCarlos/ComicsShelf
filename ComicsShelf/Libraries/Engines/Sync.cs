using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComicsShelf.Engine
{

   internal class SyncLibrary 
   {

      public static async void Save(Libraries.Library library)
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
               .Select(x => new SyncData
               {
                  Key = x.Key,
                  ReleaseDate = x.ReleaseDate,
                  Readed = x.Readed,
                  ReadingDate = x.ReadingDate,
                  ReadingPage = x.ReadingPage,
                  ReadingPercent = x.ReadingPercent,
                  Rating = x.Rating
               })
               .ToList();
            var serializedData = vTwo.Helpers.FileStream.Serialize(libraryData);

            // SAVE DATA
            var libraryService = Libraries.LibraryService.Get(library);
            await libraryService.SaveDataAsync(library, serializedData);

         }
         catch (Exception ex) { Engine.AppCenter.TrackEvent("SyncLibrary.Save", ex); }
      }

      public static async Task<List<SyncData>> Load(Libraries.Library library)      
      {
         try
         {
            var libraryService = Libraries.LibraryService.Get(library);
            var serializedData = await libraryService.LoadDataAsync(library);
            if (serializedData == null || serializedData.Length == 0) { return null; }
            var libraryData = vTwo.Helpers.FileStream.Deserialize<List<SyncData>>(serializedData);
            return libraryData;
         }
         catch (Exception ex) { Engine.AppCenter.TrackEvent("SyncLibrary.Load", ex); return null; }
      }

   }

   public class SyncData
   {
      public string Key { get; set; }
      public string ReleaseDate { get; set; }
      public bool Readed { get; set; }
      public string ReadingDate { get; set; }
      public short ReadingPage { get; set; }
      public double ReadingPercent { get; set; }
      public int Rating { get; set; }
   }

}