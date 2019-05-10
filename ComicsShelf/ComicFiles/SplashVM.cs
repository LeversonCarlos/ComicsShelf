﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.ComicFiles
{
   public class SplashVM : Helpers.Observables.ObservableObject
   {

      public List<ComicFileVM> ComicFiles { get; private set; }
      public SplashVM(ComicFileVM currentFile)
      {
         this.CurrentFile = currentFile;
         var libraryService = DependencyService.Get<Libraries.LibraryService>();
         var comicFiles = libraryService
            .ComicFiles[this.CurrentFile.ComicFile.LibraryKey]
            .Where(x => x.ComicFile.Available && x.ComicFile.FolderPath == CurrentFile.ComicFile.FolderPath)
            .OrderBy(x => x.ComicFile.FilePath)
            .ToList();
         this.ComicFiles = comicFiles;
         this.ItemSelectedCommand = new Command(async (item) => await this.ItemSelected(item));
      }

      ComicFileVM _CurrentFile;
      public ComicFileVM CurrentFile
      {
         get { return this._CurrentFile; }
         set { this.SetProperty(ref this._CurrentFile, value); }
      }


      public Command ItemSelectedCommand { get; set; }
      private async Task ItemSelected(object item)
      {
         try
         {
            this.CurrentFile = item as ComicFileVM;
         }
         catch (Exception) { throw; }
      }

   }
}
