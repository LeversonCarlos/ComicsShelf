namespace ComicsShelf.ComicFiles
{
   public class ReadingVM : Helpers.BaseVM
   {

      public ComicFileVM ComicFile { get; set; }
      public ReadingVM(ComicFileVM comicFile)
      {
         this.ComicFile = comicFile;
      }

   }
}
