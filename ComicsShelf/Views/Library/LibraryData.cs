using Xamarin.Forms.Internals;

namespace ComicsShelf.Views.Library
{
   public class LibraryData : Helpers.Observables.ObservableObject
   {

      #region New
      public LibraryData()
      {
         this.Libraries = new Helpers.Observables.ObservableList<LibraryDataItem>();
         this.Libraries.CollectionChanged += this.Libraries_CollectionChanged;

         var libraries = App.Database.Table<Helpers.Database.Library>();
         libraries.ForEach(library => this.Libraries.Add(new LibraryDataItem(library)));
      }
      #endregion

      #region Libraries

      public Helpers.Observables.ObservableList<LibraryDataItem> Libraries { get; set; }

      private void Libraries_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
      {
         this.HasLibraries = this.Libraries.Count != 0;
         this.HasntLibraries = (this.Libraries.Count == 0);
      }

      #endregion

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

   }
}