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
            Item.ReadingPage = value;
            Item.ReadingDate = DateTime.UtcNow;
            Item.ReadingPercent = (double)value / (double)(ItemPages.Count - 1);

            foreach (var page in ItemPages)
               page.IsVisible = page.Index >= (value - 1) && page.Index <= (value + 1);

            if (Item.ReadingPercent >= (double)1)
            {
               Item.Readed = true;
               Item.ReadingPage = 0;
               Helpers.Modal.Pop();
               return;
            }

         }
      }

   }
}
