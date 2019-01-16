using System.Linq;
using Xamarin.Forms.Internals;

namespace ComicsShelf.Library
{
   public class LibraryData : Helpers.Observables.ObservableObject
   {

      public LibraryData()
      { this.RefreshLibraries(); }

      public Helpers.Observables.ObservableList<LibraryDataItem> Libraries { get; set; }
      private void Libraries_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
      {
         this.HasLibraries = this.Libraries.Count != 0;
         this.HasntLibraries = this.Libraries.Count == 0;
         this.HasntOneDriveLibrary = this.Libraries.Count(x => x.LibraryType == vTwo.Libraries.TypeEnum.OneDrive) == 0;
      }

      internal void RefreshLibraries()
      {
         try
         {

            if (this.Libraries != null)
            { this.Libraries.CollectionChanged -= this.Libraries_CollectionChanged; }
            this.Libraries = new Helpers.Observables.ObservableList<LibraryDataItem>();
            this.Libraries.CollectionChanged += this.Libraries_CollectionChanged;

            var libraries = App.Settings .Libraries 
               .Select(x => new LibraryDataItem(x))
               .AsEnumerable();
            libraries.ForEach(library => this.Libraries.Add(library));
         }
         catch { }
      }


      #region HasLibraries
      bool _HasLibraries;
      public bool HasLibraries
      {
         get { return this._HasLibraries; }
         set { this.SetProperty(ref this._HasLibraries, value); }
      }
      #endregion

      #region HasntLibraries
      bool _HasntLibraries;
      public bool HasntLibraries
      {
         get { return this._HasntLibraries; }
         set { this.SetProperty(ref this._HasntLibraries, value); }
      }
      #endregion

      #region HasntOneDriveLibrary
      bool _HasntOneDriveLibrary;
      public bool HasntOneDriveLibrary
      {
         get { return this._HasntOneDriveLibrary; }
         set { this.SetProperty(ref this._HasntOneDriveLibrary, value); }
      }
      #endregion

   }
}