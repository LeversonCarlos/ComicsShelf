using ComicsShelf.Helpers;
using ComicsShelf.Screens.Splash;
using ComicsShelf.ViewModels;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Screens.Home
{
   public partial class HomeVM : BaseVM
   {

      public HomeVM()
      {
         Title = Resources.Translations.HOME_MAIN_TITLE;
         Sections = new ObservableList<SectionVM>(GetSections());
         Sections.CollectionChanged += (sender, e) => { HasSections = Sections?.Count > 0; };
         Notify.SectionsUpdate(async sections => await ApplySections(sections));
         OpenCommand = new Command(async folder => await OpenAsync(folder));
      }

      public Command OpenCommand { get; set; }
      async Task OpenAsync(object folder)
      {
         if (folder == null) { return; }
         var viewModel = await GetSplashVM(folder as FolderVM);
         await Shell.Current.GoToAsync(viewModel);
      }

      Task<SplashVM> GetSplashVM(FolderVM folder) =>
         Task.FromResult(SplashVM.Create(folder?.FirstItem));

      public override void Dispose()
      {
         Notify.SectionsUpdateUnsubscribe();
         base.Dispose();
      }

   }
}
