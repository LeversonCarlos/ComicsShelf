using System;

namespace ComicsShelf.ComicFiles
{
   public class ReadingVM : Helpers.BaseVM
   {

      public ComicFileVM ComicFile { get; set; }
      public ReadingVM(ComicFileVM comicFile)
      {
         this.ComicFile = comicFile;
      }

      short _ReadingPage;
      public short ReadingPage
      {
         get { return this._ReadingPage; }
         set
         {
            this.SetProperty(ref this._ReadingPage, value);
            this.ComicFile.ReadingPage = value;
            this.ComicFile.ReadingDate = DateTime.Now;
            var totalPagesIncludingTheCover = (double)(this.ComicFile.ComicFile.TotalPages + 0);
            this.ComicFile.ReadingPercent = Math.Round(((double)value / totalPagesIncludingTheCover * (double)100), 0) / 100;
            if (this.ComicFile.ReadingPercent > (double)1)
            {
               this.ComicFile.Readed = true;
               Xamarin.Forms.Device.BeginInvokeOnMainThread(async () => await AppShell.Current.Navigation.PopAsync());
            }
         }
      }

   }
}
