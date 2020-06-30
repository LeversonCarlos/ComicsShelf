using ComicsShelf.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Reading
{
   partial class ReadingVM
   {

      readonly PageVM[] _PagesList;

      public ReadingVM(ItemVM item, PageVM[] pagesList)
      {
         IsBusy = true;
         Item = item;
         _PagesList = pagesList.Union(new PageVM[] { new PageVM { } }).ToArray();

         // pagesList = pagesList.Union(new PageVM[] { new PageVM { } }).ToArray();
         // ReadingPage = Item.ReadingPage.HasValue ? Item.ReadingPage.Value : (short)0;
      }

      public static string GetRoute() => "reading";
      public static Type GetPageType() => typeof(ReadingPage);
      public static ReadingVM Instance { get; internal set; }

   }

   public static class ReadingExtentions
   {
      public static Task GoToAsync(this Shell shell, ReadingVM viewModel)
      {
         ReadingVM.Instance = viewModel;
         var navState = new ShellNavigationState(ReadingVM.GetRoute());
         return shell.GoToAsync(navState);
      }
   }

}
