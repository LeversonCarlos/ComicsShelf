using ComicsShelf.ViewModels;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Xamarin.CloudDrive.Connector;
using Xamarin.Forms;

namespace ComicsShelf.Drive
{
   partial class BaseDrive<T>
   {

      public async Task<LegacyVM[]> SearchLegacySettings(LibraryVM library)
      {
         try
         {

            // DEFINE ROOT DIRECTORY
            var root = new DirectoryVM
            {
               ID = library.ID,
               Path = library.Path,
               Name = library.Description
            };

            // SEARCH FOR FILES ON THE DIRECTORY
            var fileList = await this.CloudService.SearchFiles(root, "*.library", 10);
            if (fileList == null || fileList.Length == 0) return null;

            // RETRIEVE THE ID FROM THE FIRST ONE
            var fileID = fileList?
               .Where(x => x.Name == "ComicsShelf.library")
               .OrderBy(x => x.Path)
               .Select(x => x.ID)
               .FirstOrDefault();
            if (string.IsNullOrEmpty(fileID)) { return null; }

            // DOWNLOAD ITS CONTENT
            LegacyVM[] itemsList = null;
            using (var fileStream = await this.CloudService.Download(fileID))
            {
               itemsList = await JsonSerializer.DeserializeAsync<LegacyVM[]>(fileStream);
               // var streamReader = new StreamReader(fileStream)
            }

            return itemsList;
         }
         catch (Exception ex) { Helpers.Insights.TrackException(ex); return null; }
      }

   }
}
