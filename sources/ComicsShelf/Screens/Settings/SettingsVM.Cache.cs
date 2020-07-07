using ComicsShelf.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Screens.Settings
{
   partial class SettingsVM
   {

      long _CacheSizeInMegas;
      public long CacheSizeInMegas
      {
         get => _CacheSizeInMegas;
         set => SetProperty(ref _CacheSizeInMegas, value);
      }

      Task LoadCacheSize()
      {
         try
         {
            var filesList = System.IO.Directory.GetFiles(Helpers.Paths.FilesCache, "*.*", System.IO.SearchOption.AllDirectories);

            var filesInfo = filesList
               .Select(file => new System.IO.FileInfo(file))
               .Select(file => new { file.FullName, SizeInBytes = file.Length })
               .ToArray();

            var cacheSizeInBytes = filesInfo.Select(file => file.SizeInBytes).Sum();

            CacheSizeInMegas = (long)Math.Round((double)cacheSizeInBytes / (double)1024 / (double)1024, 0);
         }
         catch (Exception ex) { Insights.TrackException(ex); }
         return Task.CompletedTask;
      }

      public Command ClearCacheCommand { get; set; }
      async Task ClearCache()
      {
         try
         {
            if (!await Helpers.Message.Confirm(Resources.Translations.SETTINGS_LOCAL_CACHE_CLEAR_CONFIRMATION)) { return; }
            IsBusy = true;

            var filesList = System.IO.Directory.GetFiles(Helpers.Paths.FilesCache, "*.*", System.IO.SearchOption.AllDirectories);

            foreach (var file in filesList)
               System.IO.File.Delete(file);

         }
         catch (Exception ex) { Insights.TrackException(ex); }
         finally { await LoadCacheSize(); IsBusy = false; }
      }

   }
}
