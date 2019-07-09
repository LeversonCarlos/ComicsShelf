using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComicsShelf.Libraries
{

   internal class LibrarySync
   {

      public static async Task<bool> SaveSyncData(LibraryModel library, List<ComicFiles.ComicFile> comicFiles)
      {
         try
         {

            if (comicFiles == null) { return true; }
            var syncData = comicFiles
               .Select(comicFile => new LibrarySyncVM(comicFile))
               .ToList();

            var byteArray = Helpers.FileStream.Serialize(syncData);
            if (byteArray == null) { return true; }

            var engine = Engines.Engine.Get(library.Type);
            if (engine == null) { return false; }

            if (!await engine.SaveSyncData(library, byteArray)) { return false; }
            return true;

         }
         catch (Exception) { throw; }
      }

      public static async Task<List<LibrarySyncVM>> LoadSyncData(LibraryModel library)
      {
         try
         {
            var engine = Engines.Engine.Get(library.Type);
            if (engine == null) { return null; }

            var byteArray = await engine.LoadSyncData(library);
            if (byteArray == null) { return null; }

            List<LibrarySyncVM> syncData = null;
            try
            { syncData = Helpers.FileStream.Deserialize<List<LibrarySyncVM>>(byteArray); }
            catch
            {
               var dynamicData = Helpers.FileStream.Deserialize<List<dynamic>>(byteArray);
               if (dynamicData != null)
               {
                  syncData = dynamicData.Select(x => new LibrarySyncVM(x)).ToList();
               }
            }

            syncData = syncData
               .Where(x => x.Readed || x.ReadingPage > 0 || x.Rating > 0)
               .ToList();

            return syncData;
         }
         catch (Exception ex) { throw; }
      }

   }

   public class LibrarySyncVM
   {

      public LibrarySyncVM(ComicFiles.ComicFile comicFile)
      {
         this.Key = comicFile.Key;
         this.ReleaseDate = comicFile.ReleaseDate;
         this.Readed = comicFile.Readed;
         this.ReadingDate = comicFile.ReadingDate;
         this.ReadingPage = comicFile.ReadingPage;
         this.ReadingPercent = comicFile.ReadingPercent;
         this.Rating = comicFile.Rating;
      }

      public LibrarySyncVM(dynamic comicFile)
      {
         this.Key = comicFile.Key;
         try { this.ReleaseDate = DateTime.Parse((string)comicFile.ReleaseDate); } catch { this.ReleaseDate = DateTime.MinValue; }
         try { this.Readed = comicFile.Readed; } catch { }
         try { this.ReadingDate = DateTime.Parse((string)comicFile.ReadingDate); } catch { this.ReadingDate = DateTime.MinValue; }
         try { this.ReadingPage = comicFile.ReadingPage; } catch { }
         try { this.ReadingPercent = comicFile.ReadingPercent; } catch { }
         try { this.Rating = comicFile.Rating; } catch { }
      }

      public string Key { get; set; }
      public DateTime ReleaseDate { get; set; }
      public bool Readed { get; set; }
      public DateTime ReadingDate { get; set; }
      public short ReadingPage { get; set; }
      public double ReadingPercent { get; set; }
      public short Rating { get; set; }

      public ComicFiles.ComicFile ToComicFile()
      {
         return new ComicFiles.ComicFile
         {
            Key = this.Key,
            ReleaseDate = this.ReleaseDate,
            Readed = this.Readed,
            ReadingDate = this.ReadingDate,
            ReadingPage = this.ReadingPage,
            ReadingPercent = this.ReadingPercent,
            Rating = this.Rating
         };
      }

   }

}
