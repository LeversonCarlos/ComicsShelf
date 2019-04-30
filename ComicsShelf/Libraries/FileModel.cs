using ComicsShelf.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace ComicsShelf.Libraries
{
   public class ComicModel : Helpers.Observables.ObservableObject
   {

      internal readonly File File;
      public ComicModel(File file)
      {
         this.File = file;
      }

      string _CoverPath;
      public string CoverPath
      {
         get { return this._CoverPath; }
         set { this.SetProperty(ref this._CoverPath, value); }
      }


   }
}
