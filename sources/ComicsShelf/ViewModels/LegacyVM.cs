using System;

namespace ComicsShelf.ViewModels
{
   public class LegacyVM
   {

      public string Key { get; set; }
      public DateTime ReleaseDate { get; set; }
      public bool Readed { get; set; }
      public DateTime ReadingDate { get; set; }
      public short ReadingPage { get; set; }
      public double ReadingPercent { get; set; }
      public short Rating { get; set; }

      /*
         {
            "Key": "E6710A9CD086B950!60381",
            "ReleaseDate": "2019-02-16T13:25:39.763-03:00",
            "Readed": true,
            "ReadingDate": "2019-10-19T15:05:32.131073-03:00",
            "ReadingPage": 0,
            "ReadingPercent": 1,
            "Rating": 0
         },
       */

   }
}
