using ComicsShelf.Helpers;
using ComicsShelf.Store;
using ComicsShelf.ViewModels;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Screens.Home
{
   partial class HomeVM
   {

      public ObservableList<SectionVM> Sections { get; }

      SectionVM[] GetSections() =>
         DependencyService.Get<IStoreService>()?.GetSections();

      async Task ApplySections(SectionVM[] sections)
      {
         if (sections == null) return;
         var start = DateTime.Now;
         await Sections.ReplaceRangeAsync(sections);
         Insights.TrackDuration("Sections Updating", DateTime.Now.Subtract(start).TotalSeconds);
      }

      bool _HasSections;
      public bool HasSections
      {
         get => _HasSections;
         set => SetProperty(ref _HasSections, value);
      }

   }
}
