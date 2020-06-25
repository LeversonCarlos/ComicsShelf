using ComicsShelf.Observables;
using System.Collections.Generic;

namespace ComicsShelf.Settings
{
   public class SettingsVM : BaseVM
   {

      public List<LibrariesSetting> LibraryTypes { get; }

      public SettingsVM()
      {
         Title = Strings.TITLE;
         LibraryTypes = new List<LibrariesSetting> {
            new LibrariesSetting(ViewModels.enLibraryType.LocalDrive),
            new LibrariesSetting(ViewModels.enLibraryType.OneDrive)
         };
      }

   }
}
