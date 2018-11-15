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

      bool _HasLibraries;
      public bool HasLibraries
      {
         get { return this._HasLibraries; }
         set { this.SetProperty(ref this._HasLibraries, value); }
      }

      private void Libraries_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
      { this.HasLibraries = this.Libraries.Count != 0; }

      #endregion


      #region LibraryPath
      string _LibraryPath;
      public string LibraryPath
      {
         get { return this._LibraryPath; }
         set
         {
            this.SetProperty(ref this._LibraryPath, value);
            this.IsLibraryEmpty = string.IsNullOrEmpty(value);
            this.IsLibraryDefined = !string.IsNullOrEmpty(value);
         }
      }
      #endregion

      #region IsLibraryEmpty
      bool _IsLibraryEmpty;
      public bool IsLibraryEmpty
      {
         get { return this._IsLibraryEmpty; }
         set { this.SetProperty(ref this._IsLibraryEmpty, value); }
      }
      #endregion

      #region IsLibraryDefined
      bool _IsLibraryDefined;
      public bool IsLibraryDefined
      {
         get { return this._IsLibraryDefined; }
         set { this.SetProperty(ref this._IsLibraryDefined, value); }
      }
      #endregion


   }
}