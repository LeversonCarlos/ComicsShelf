using ComicsShelf.Observables;
using ComicsShelf.ViewModels;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Home
{
   public class HomeVM : BaseVM
   {

      public ObservableList<FolderVM> Sections { get; }

      public HomeVM()
      {
         Title = Strings.TITLE;
         OpenCommand = new Command(async folder => await Open(folder));
         Sections = new ObservableList<FolderVM>();
         Notifyers.Notify.SectionsUpdate(sections => Sections.ReplaceRange(sections));
      }

      public Command OpenCommand;
      Task Open(object folder) => Shell.Current.GoToAsync(new ShellNavigationState($"//splash?folderPath={(folder as FolderVM)?.Path}"));

   }
}
