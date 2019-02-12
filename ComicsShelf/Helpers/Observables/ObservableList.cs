using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace ComicsShelf.Helpers.Observables
{

   public class ObservableList<T> : ObservableCollection<T>, INotifyObservableCollectionChanged
   {
      public ObservableList() : base() { }
      public ObservableList(IEnumerable<T> collection) : base(collection) { }

      public void AddRange(IEnumerable<T> collection, NotifyCollectionChangedAction notificationMode = NotifyCollectionChangedAction.Add)
      {

         if (collection == null) throw new ArgumentNullException("collection");
         this.CheckReentrancy();

         if (notificationMode == NotifyCollectionChangedAction.Reset)
         {
            foreach (var i in collection) { Items.Add(i); }
            this.OnPropertyChanged(new PropertyChangedEventArgs("Count"));
            this.OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            return;
         }

         int startIndex = Count;
         var changedItems = collection is List<T> ? (List<T>)collection : new List<T>(collection);
         foreach (var i in changedItems)
         {
            Items.Add(i);
         }

         this.OnPropertyChanged(new PropertyChangedEventArgs("Count"));
         this.OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
         this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, changedItems, startIndex));
      }

      public void RemoveRange(IEnumerable<T> collection)
      {
         if (collection == null) throw new ArgumentNullException("collection");

         foreach (var i in collection)
            Items.Remove(i);
         this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
      }

      protected override void InsertItem(int index, T item)
      {
         base.InsertItem(index, item);
         this.RefreshAnalysis(NotifyCollectionChangedAction.Add);
      }

      protected override void RemoveItem(int index)
      {
         base.RemoveItem(index);
         this.RefreshAnalysis(NotifyCollectionChangedAction.Remove);
      }

      #region Replace
      public void Replace(T item)
      { this.ReplaceRange(new T[] { item }); }
      public void ReplaceRange(IEnumerable<T> collection)
      {
         if (collection == null) throw new ArgumentNullException("collection");
         this.Items.Clear();
         this.AddRange(collection, NotifyCollectionChangedAction.Reset);
         // this.RefreshAnalysis(NotifyCollectionChangedAction.Reset);
      }
      #endregion

      #region ObservableCollectionChangedAnalysis
      public event NotifyCollectionChangedEventHandler ObservableCollectionChanged;
      private void RefreshAnalysis(NotifyCollectionChangedAction changedAction)
      {
         if (this.ObservableCollectionChanged != null)
         {
            var countBefore = this.Count;
            System.Threading.Tasks.Task.Delay(500).ContinueWith(x =>
            {
               var countAfter = this.Count;
               if (countBefore == countAfter)
               {
                  Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                  {
                     this.ObservableCollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                  });
               }
            });
         }
      }
      public void RefreshNow()
      { this.ObservableCollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset)); }
      #endregion

   }

   public interface INotifyObservableCollectionChanged
   {
      // object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e
      event NotifyCollectionChangedEventHandler ObservableCollectionChanged;
      // event EventHandler ObservableCollectionChanged;
   }

}