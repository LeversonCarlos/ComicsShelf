using System.Collections.Generic;
using System.Threading.Tasks;

namespace ComicsShelf.vTwo.Settings
{
   partial class Engine
   {
      public List<Library.Library> Libraries { get; set; }

      private async void InitializeLibraries()
      {
         this.Libraries = await Helpers.FileStream.ReadFile<List<Library.Library>>(this.Paths.SettingsPath);
         if (this.Libraries == null) { this.Libraries = new List<Library.Library>(); }
      }

      internal async Task<bool> SaveLibraries()
      {
         return await Helpers.FileStream.SaveFile(this.Paths.SettingsPath, this.Libraries);
      }

   }
}