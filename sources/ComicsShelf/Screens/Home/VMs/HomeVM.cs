using ComicsShelf.Helpers;
using ComicsShelf.ViewModels;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Screens.Home
{
   public partial class HomeVM : BaseVM
   {

      public HomeVM()
      {
         Title = Resources.Translations.SCREEN_HOME_MAIN_TITLE;
         Sections = new ObservableList<SectionVM>();
         Sections.CollectionChanged += (sender, e) => { HasSections = Sections?.Count > 0; };
         Notify.SectionsUpdate(this, async sections => await ApplySections(sections));
         OpenCommand = new Command(async folder => await OpenAsync(folder));
      }

      public override Task OnAppearing() =>
         Sections.ReplaceRangeAsync(GetSections());

      public override void Dispose()
      {
         Notify.SectionsUpdateUnsubscribe(this);
         base.Dispose();
      }

   }
}
