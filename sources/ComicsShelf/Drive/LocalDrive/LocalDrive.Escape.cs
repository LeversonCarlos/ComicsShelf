namespace ComicsShelf.Drive
{
   partial class LocalDrive
   {

      public override string EscapeFileID(string fileID) => fileID
         .Replace("#", "")
         .Replace(".", "")
         .Replace("[", "")
         .Replace("]", "")
         .Replace("(", "")
         .Replace(")", "")
         .Replace(" ", "")
         .Replace("/", "_")
         .Replace("___", "_")
         .Replace("__", "_");

   }
}
