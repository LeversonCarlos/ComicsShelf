using ComicsShelf.Helpers;
using ComicsShelf.Screens.Splash;
using ComicsShelf.Store;
using ComicsShelf.ViewModels;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Screens.Home
{
   public class HomeVM : BaseVM
   {

      public HomeVM()
      {
         Title = Resources.Translations.HOME_MAIN_TITLE;
         Sections = new ObservableList<SectionVM>(GetSections());
         Sections.CollectionChanged += (sender, e) => { HasSections = Sections?.Count > 0; };
         Notify.SectionsUpdate(async sections => await ApplySections(sections));
         OpenCommand = new Command(async folder => await OpenAsync(folder));
      }

      public string NO_LIBRARY_TITLE => Resources.Translations.HOME_NO_LIBRARY_WARNING_TITLE;
      public string NO_LIBRARY_MESSAGE => Resources.Translations.HOME_NO_LIBRARY_WARNING_MESSAGE;

      SectionVM[] GetSections() =>
         DependencyService.Get<IStoreService>()?.GetSections();

      public ObservableList<SectionVM> Sections { get; }
      async Task ApplySections(SectionVM[] sections)
      {
         if (sections == null) return;
         var start = DateTime.Now;
         await Sections.ReplaceRangeAsync(sections);
         Insights.TrackMetric("Sections Updating", DateTime.Now.Subtract(start).TotalSeconds);
      }

      bool _HasSections;
      public bool HasSections
      {
         get => _HasSections;
         set => SetProperty(ref _HasSections, value);
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
