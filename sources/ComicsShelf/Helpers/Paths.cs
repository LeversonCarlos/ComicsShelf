namespace ComicsShelf.Helpers
{
   public class Paths
   {

      public static string CoversCache =>
         $"{Xamarin.Essentials.FileSystem.CacheDirectory}/CoversCache";

      public static string FilesCache =>
         $"{Xamarin.Essentials.FileSystem.CacheDirectory}/FilesCache";

   }
}
