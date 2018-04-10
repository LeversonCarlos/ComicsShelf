namespace ComicsShelf.File
{
   internal interface iFileData
   {

      bool Readed { get; set; }
      double ReadingPercent { get; set; }
      short ReadingPage { get; set; }
      string ReadingDate { get; set; }

      // short? Rate { get; set; }

   }
}