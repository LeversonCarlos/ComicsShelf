namespace ComicsShelf.Drive.FolderDialog
{
   internal enum enFolderType : short { Drive, Folder, File }
   internal class FolderVM
   {
      public string ID { get; set; }
      public string Name { get; set; }
      public string Path { get; set; }
      public FolderVM Parent { get; set; }
      public enFolderType Type { get; set; }
   }
}
