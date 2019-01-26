namespace ComicsShelf.Libraries
{
   public enum TypeEnum : short { FileSystem = 0, OneDrive = 1 }

   public class Library
   {
      public string LibraryID { get; set; }
      public string Description { get; set; }
      public TypeEnum Type { get; set; }
      public bool Available { get; set; }
   }

}