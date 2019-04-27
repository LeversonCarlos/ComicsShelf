using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms.Internals;

namespace ComicsShelf.Views.Home
{
   public class HomeData : Folder.FolderData
   {

      #region New
      public HomeData() : base(new Helpers.Database.ComicFolder { Text = R.Strings.AppTitle })
      {
         this.Libraries = new Helpers.Observables.ObservableList<LibraryData>();
         this.Libraries.Add(new EmptyData(true));
         this.Libraries.ObservableCollectionChanged += this.Libraries_CollectionChanged;
      }
      #endregion


      #region EmptyCoverImage
      Xamarin.Forms.ImageSource _EmptyCoverImage = null;
      public Xamarin.Forms.ImageSource EmptyCoverImage
      {
         get { return this._EmptyCoverImage; }
         set { this.SetProperty(ref this._EmptyCoverImage, value); }
      }
      #endregion

      #region Libraries

      public Helpers.Observables.ObservableList<LibraryData> Libraries { get; set; }

      private void Libraries_CollectionChanged(object sender, System.EventArgs e)
      {
         if (this.Libraries.Count == 0)
         { this.Libraries.Add(new EmptyData(false)); }
         else {
            if (this.Libraries.Count(x => x.IsEmptyPage == false) > 0) {
               var empty = this.Libraries.Where(x => x.IsEmptyPage == true).FirstOrDefault();
               if (empty != null)
               { this.Libraries.Remove(empty); }
            }
         }
      }

      #endregion

   }
}