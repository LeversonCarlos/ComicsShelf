using ComicsShelf.Helpers;
using ComicsShelf.Observables;
using ComicsShelf.Splash;
using ComicsShelf.Store;
using ComicsShelf.ViewModels;
using System;
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
         Helpers.Notify.SectionsUpdate(sections => ApplySections(sections));
         OpenCommand = new Command(async folder => await OpenAsync(folder));
      }

      public string NO_LIBRARY_MESSAGE_TITLE => Translations.NO_LIBRARY_MESSAGE_TITLE;
      public string NO_LIBRARY_MESSAGE_DETAILS => Translations.NO_LIBRARY_MESSAGE_DETAILS;

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

   }
}
