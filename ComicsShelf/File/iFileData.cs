namespace ComicsShelf.File
{
   internal interface iFileData
   {

      bool Readed { get; set; }
      short ReadingPercent { get; set; }
      short ReadingPage { get; set; }
      string ReadingDate { get; set; }

      // short? Rate { get; set; }

   }
}