using ComicsShelf.Observables;
using ComicsShelf.Store;
using System;
using Xamarin.Forms;

namespace ComicsShelf.Reading
{
   public partial class ReadingVM : BaseVM
   {

      IStoreService Store { get => DependencyService.Get<IStoreService>(); }

      short _ReadingPage;
      public short ReadingPage
      {
         get => _ReadingPage;
         set
         {
            SetProperty(ref _ReadingPage, value);
            this.Item.ReadingPage = value;
            this.Item.ReadingDate = DateTime.UtcNow;
            this.Item.ReadingPercent = (double)value / (double)this.ItemPages.Count;
            foreach (var page in ItemPages)
               page.IsVisible = page.Index >= (value - 1) && page.Index <= (value + 1);
         }
      }

   }
}
