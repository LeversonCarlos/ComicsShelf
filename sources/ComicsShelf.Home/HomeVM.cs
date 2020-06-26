using ComicsShelf.Observables;
using ComicsShelf.Store;
using ComicsShelf.ViewModels;
using System.Threading.Tasks;
using System.Timers;
using Xamarin.Forms;

namespace ComicsShelf.Home
{
   public class HomeVM : BaseVM
   {

      public ObservableList<SectionVM> Sections { get; }

      public HomeVM()
      {
         Title = Strings.TITLE;
         Sections = new ObservableList<SectionVM>();
         Helpers.Notify.SectionsUpdate(sections => Sections.ReplaceRange(sections));
         OpenCommand = new Command(async folder => await Open(folder));
      }

      public override Task OnAppearing()
      {
         var sections = DependencyService.Get<IStoreService>()?.GetSections();
         if (sections != null) Sections.ReplaceRange(sections);
         return base.OnAppearing();
      }

      public override Task OnDisappearing()
      {
         return base.OnDisappearing();
      }

      public Command OpenCommand { get; set; }
      Task Open(object folder) => Shell.Current
         .GoToAsync(GetNavigationState(folder as FolderVM));

      ShellNavigationState GetNavigationState(FolderVM folder) =>
         new ShellNavigationState($"splash?libraryID={folder?.FirstItem?.LibraryID}&fileID={folder?.FirstItem?.EscapedID}");

   }
}
