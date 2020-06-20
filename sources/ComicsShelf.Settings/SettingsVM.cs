using ComicsShelf.Observables;

namespace ComicsShelf.Settings
{
   public class SettingsVM : BaseVM
   {

      public LibrariesSetting LocalDrive { get; }
      public LibrariesSetting OneDrive { get; }

      public SettingsVM()
      {
         this.LocalDrive = new LibrariesSetting(ViewModels.enLibraryType.LocalDrive);
         this.OneDrive = new LibrariesSetting(ViewModels.enLibraryType.OneDrive);
      }

   }
}
