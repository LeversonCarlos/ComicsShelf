using ComicsShelf.Helpers;
using ComicsShelf.ViewModels;
using System;
using System.Threading.Tasks;

namespace ComicsShelf.Drive
{
   partial class LocalDrive
   {

      public override Task<PageVM[]> ExtractPages(LibraryVM library, ItemVM libraryItem)
      {
         try
         {

            return Task.Delay(TimeSpan.FromSeconds(10)).ContinueWith(t => new PageVM[] { });

         }
         catch (Exception ex) { Insights.TrackException(ex); return null; }
      }

   }
}
