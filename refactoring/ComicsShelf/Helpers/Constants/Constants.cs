namespace ComicsShelf.Helpers
{
   internal class Constants
   {

      public static string DefaultCover
      {
         get { return $"{Xamarin.Essentials.FileSystem.AppDataDirectory}/DefaultCover.png"; }
      }

      public static string DatabaseFile
      {
         get { return $"{Xamarin.Essentials.FileSystem.AppDataDirectory}/Database.json"; }
      }

      public static string FilesCachePath
      {
         get { return $"{Xamarin.Essentials.FileSystem.CacheDirectory}/FilesCache/"; }
      }

      public static string CoversCachePath
      {
         get { return $"{Xamarin.Essentials.FileSystem.CacheDirectory}/CoversCachePath/"; }
      }

   }
}
