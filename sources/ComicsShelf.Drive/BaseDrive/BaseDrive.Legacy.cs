using ComicsShelf.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.CloudDrive.Connector;

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
            var fileList = await this.CloudService.GetFiles(root);
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

               // DESERIALIZE ITS CONTENT
               using (var memoryStream = new System.IO.MemoryStream())
               {
                  await fileStream.CopyToAsync(memoryStream);
                  await memoryStream.FlushAsync();
                  memoryStream.Position = 0;
                  var byteArray = memoryStream.ToArray();
                  var serializedContent = System.Text.Encoding.Unicode.GetString(byteArray);

                  itemsList = await Helpers.Json.Deserialize<LegacyVM[]>(serializedContent);
               }

            }

            return itemsList;
         }
         catch (Exception ex) { Helpers.Insights.TrackException(ex); return null; }
      }

   }
}
