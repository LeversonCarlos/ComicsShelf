using Xamarin.Forms;

namespace ComicsShelf.ComicFiles
{
   public class ReadingVM : Helpers.BaseVM
   {

      public ReadingVM(ComicFileVM currentFile)
      {
         this.CurrentFile = currentFile;
      }

      ComicFileVM _CurrentFile;
      public ComicFileVM CurrentFile
      {
         get { return this._CurrentFile; }
         set { this.SetProperty(ref this._CurrentFile, value); }
      }

   }
}
