using ComicsShelf.Observables;
using ComicsShelf.Store;
using ComicsShelf.ViewModels;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Home
{
   public class HomeVM : BaseVM
   {

      public HomeVM()
      {
         Title = Translations.TITLE;
         Sections = new ObservableList<SectionVM>();
         Sections.CollectionChanged += (sender, e) => { this.HasSections = this.Sections?.Count > 0; };
         Helpers.Notify.SectionsUpdate(sections => Sections.ReplaceRange(sections));
         OpenCommand = new Command(async folder => await Open(folder));
      }

      public string NO_LIBRARY_MESSAGE_TITLE => Translations.NO_LIBRARY_MESSAGE_TITLE;
      public string NO_LIBRARY_MESSAGE_DETAILS => Translations.NO_LIBRARY_MESSAGE_DETAILS;

      public ObservableList<SectionVM> Sections { get; }

      bool _HasSections;
      public bool HasSections
      {
         get => _HasSections;
         set => SetProperty(ref _HasSections, value);
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
