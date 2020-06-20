using ComicsShelf.Observables;

namespace ComicsShelf.Settings
{
   public class SettingsVM : BaseVM
   {

      public LibrariesSetting LocalDrive { get; }
      public LibrariesSetting OneDrive { get; }

      public SettingsVM()
      {
         Title = Strings.TITLE;
         LocalDrive = new LibrariesSetting(ViewModels.enLibraryType.LocalDrive);
         OneDrive = new LibrariesSetting(ViewModels.enLibraryType.OneDrive);
      }

   }
}
