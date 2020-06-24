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

      public Command OpenCommand { get; set; }
      Task Open(object folder) => Shell.Current
         .GoToAsync(GetNavigationState(folder as FolderVM));

      ShellNavigationState GetNavigationState(FolderVM folder) =>
         new ShellNavigationState($"splash?folderPath={System.Uri.EscapeUriString(folder?.Path)}");

   }
}
