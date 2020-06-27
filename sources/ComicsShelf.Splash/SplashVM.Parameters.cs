namespace ComicsShelf.Splash
{
   partial class SplashVM
   {

      string _LibraryID { get; set; }
      public void SetLibraryID(string libraryID) => _LibraryID = System.Uri.UnescapeDataString(libraryID);

      string _FileID { get; set; }
      public void SetFileID(string fileID) => _FileID = fileID;

   }
}
