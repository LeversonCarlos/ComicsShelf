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

      readonly Timer CoverSliderTimer;
      public ObservableList<SectionVM> Sections { get; }

      public HomeVM()
      {
         Title = Strings.TITLE;
         Sections = new ObservableList<SectionVM>();
         Helpers.Notify.SectionsUpdate(sections => Sections.ReplaceRange(sections));
         CoverSliderTimer = new Timer(5000);
         CoverSliderTimer.Elapsed += this.CoverSliderTimerElapsed;
         OpenCommand = new Command(async folder => await Open(folder));
      }

      public override Task OnAppearing()
      {
         Sections.ReplaceRange(DependencyService.Get<IStoreService>().GetSections());
         CoverSliderTimer.Start();
         return base.OnAppearing();
      }

      public override Task OnDisappearing()
      {
         CoverSliderTimer.Stop();
         return base.OnDisappearing();
      }

      public Command OpenCommand { get; set; }
      Task Open(object folder) => Shell.Current
         .GoToAsync(GetNavigationState(folder as FolderVM));

      ShellNavigationState GetNavigationState(FolderVM folder) =>
         new ShellNavigationState($"splash?libraryID={folder?.FirstItem?.LibraryID}&fileID={folder?.FirstItem?.EscapedID}");

      private void CoverSliderTimerElapsed(object sender, ElapsedEventArgs e) =>
         Helpers.Notify.CoverSliderTimer();

   }
}
