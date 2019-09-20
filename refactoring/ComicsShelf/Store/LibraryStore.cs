using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComicsShelf.Store
{
   public class LibraryStore : ILibraryStore
   {

      #region New

      public LibraryStore()
      {
         this.Libraries = new List<LibraryModel>();
         this.LibraryFiles = new Dictionary<enLibraryFilesGroup, List<ComicFiles.ComicFileVM>>();
      }

      public List<LibraryModel> Libraries { get; set; }
      public Dictionary<enLibraryFilesGroup, List<ComicFiles.ComicFileVM>> LibraryFiles { get; set; }

      #endregion

      #region LoadAndSave
      private const string LibraryIDs = "ComicsShelf.LibraryIDs";

      public async Task LoadLibrariesAsync()
      {
         try
         {

            // LIBRARY IDs
            var libraryIDsJSON = Xamarin.Essentials.Preferences.Get(LibraryIDs, "[]");
            var libraryIDs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(libraryIDsJSON);

            // LIBRARY CONTENT
            foreach (var libraryID in libraryIDs)
            {
               var libraryJSON = Xamarin.Essentials.Preferences.Get($"{LibraryIDs}.{libraryID}", "");
               if (!string.IsNullOrEmpty(libraryJSON))
               {
                  var library = Newtonsoft.Json.JsonConvert.DeserializeObject<LibraryModel>(libraryJSON);
                  await this.AddLibraryAsync(library);
               }
            }

            // LIBRARY SERVICE
            foreach (var library in this.Libraries)
            {
               using (var services = new Services.LibraryService(library))
               { await services.InitializeLibrary(); }
            }
            foreach (var library in this.Libraries)
            { Services.LibraryService.RefreshLibrary(library); }

         }
         catch (Exception ex) { Helpers.AppCenter.TrackEvent(ex); }
      }

      private void SaveLibrary(LibraryModel library)
      {
         try
         {
            var libraryJSON = Newtonsoft.Json.JsonConvert.SerializeObject(library);
            var libraryID = $"{LibraryIDs}.{library.ID}";
            Xamarin.Essentials.Preferences.Set(libraryID, libraryJSON);
         }
         catch (Exception ex) { Helpers.AppCenter.TrackEvent(ex); }
      }

      private void RemoveLibrary(LibraryModel library)
      {
         try
         {
            var libraryID = $"{LibraryIDs}.{library.ID}";
            Xamarin.Essentials.Preferences.Remove(libraryID);
         }
         catch (Exception ex) { Helpers.AppCenter.TrackEvent(ex); }
      }

      public void SaveLibraries()
      {
         try
         {
            var libraryIDs = this.Libraries.Select(x => x.ID).ToList();
            var libraryIDsJSON = Newtonsoft.Json.JsonConvert.SerializeObject(libraryIDs);
            Xamarin.Essentials.Preferences.Set(LibraryIDs, libraryIDsJSON);
         }
         catch (Exception ex) { Helpers.AppCenter.TrackEvent(ex); }
      }

      #endregion


      #region NewLibraryAsync
      public async Task<bool> NewLibraryAsync(LibraryType libraryType)
      {
         try
         {
            App.HideNavigation();

            // ENGINE
            var engine = Engines.Engine.Get(libraryType);
            var library = await engine.NewLibrary();
            if (library == null) { return false; }

            // ADD LIBRARY
            library.ID = Guid.NewGuid().ToString();
            if (!await this.AddLibraryAsync(library)) { return false; }

            // STORE LIBRARY
            this.SaveLibrary(library);
            this.SaveLibraries();

            // LIBRARY SERVICE
            using (var services = new Services.LibraryService(library))
            { await services.InitializeLibrary(); }
            Services.LibraryService.RefreshLibrary(library);

            return true;
         }
         catch (Exception ex) { Helpers.AppCenter.TrackEvent(ex); return false; }
      }
      #endregion

      #region AddLibraryAsync
      public async Task<bool> AddLibraryAsync(LibraryModel library)
      {
         try
         {

            // VALIDATE
            if (this.Libraries.Any(x => x.Type == library.Type && x.Key == library.Key)) { return false; }

            // ADD
            this.Libraries.Add(library);
            Messaging.Send(Messaging.enMessages.LibraryAdded.ToString(), library);

            return await Task.FromResult(true);
         }
         catch (Exception ex) { Helpers.AppCenter.TrackEvent(ex); return false; }
      }
      #endregion

      #region DeleteLibraryAsync
      public async Task<bool> DeleteLibraryAsync(string id)
      {
         try
         {
            await App.Navigation().PopAsync();

            // LOCATE LIBRARY
            var library = this.GetLibrary(id);
            if (library == null) { Helpers.AppCenter.TrackEvent("Library.Delete.NotFound", $"LibraryID:{id}"); return false; }
            library.Removed = true;

            // LIBRARY SERVICE
            using (var service = new Services.LibraryService(library))
            {

               // REMOVE FILES 
               if (!await service.RemoveLibrary()) { return false; }
               Messaging.Send(Messaging.enMessages.LibraryRemoved.ToString(), library);

               // STORE LIBRARY
               this.Libraries.RemoveAll(x => x.ID == library.ID);
               this.RemoveLibrary(library);
               this.SaveLibraries();

            }

            // ENGINE
            var engine = Engines.Engine.Get(library.Type);
            await engine.DeleteLibrary(library);

            return true;
         }
         catch (Exception ex) { Helpers.AppCenter.TrackEvent(ex); return false; }
      }
      #endregion


      #region GetLibrary
      public LibraryModel GetLibrary(string id)
      { return this.Libraries.FirstOrDefault(x => x.ID == id); }
      #endregion

      #region GetLibraryFiles
      public List<ComicFiles.ComicFileVM> GetLibraryFiles(LibraryModel library)
      {
         try
         {
            return this.LibraryFiles[enLibraryFilesGroup.Libraries]
               .Where(x => x.ComicFile.LibraryKey == library.ID)
               .ToList();
         }
         catch (Exception ex) { Helpers.AppCenter.TrackEvent(ex); return null; }
      }
      #endregion

   }
}
