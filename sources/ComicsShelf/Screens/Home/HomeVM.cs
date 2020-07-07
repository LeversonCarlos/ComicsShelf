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
         Sections = new ObservableList<SectionVM>();
         Sections.CollectionChanged += (sender, e) => { this.HasSections = this.Sections?.Count > 0; };
         Helpers.Notify.SectionsUpdate(sections => ApplySections(sections));
         OpenCommand = new Command(async folder => await OpenAsync(folder));
      }

      public string NO_LIBRARY_TITLE => Resources.Translations.HOME_NO_LIBRARY_WARNING_TITLE;
      public string NO_LIBRARY_MESSAGE => Resources.Translations.HOME_NO_LIBRARY_WARNING_MESSAGE;

      public ObservableList<SectionVM> Sections { get; }
      void ApplySections(SectionVM[] sections)
      {
         var start = DateTime.Now;
         Sections.ReplaceRange(sections);
         Insights.TrackMetric("Sections Updating", DateTime.Now.Subtract(start).TotalSeconds);
      }

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
