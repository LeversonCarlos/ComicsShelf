using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace ComicsShelf.ViewModels
{
   public class ObservableList<T> : ObservableCollection<T>, INotifyCollectionChanged
   {

      SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
      public ObservableList() : base() { }
      public ObservableList(IEnumerable<T> collection) : base(collection) { }

      public async Task AddRangeAsync(IEnumerable<T> collection, NotifyCollectionChangedAction notificationMode = NotifyCollectionChangedAction.Add)
      {
         try
         {
            await semaphore.WaitAsync();
            AddRange(collection, notificationMode);
         }
         catch (Exception ex) { Helpers.Insights.TrackException(ex); }
         finally { semaphore.Release(); }
      }

      void AddRange(IEnumerable<T> collection, NotifyCollectionChangedAction notificationMode = NotifyCollectionChangedAction.Add)
      {
         try
         {
            if (collection == null) throw new ArgumentNullException("collection");

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
            foreach (var i in changedItems) { Items.Add(i); }

            this.OnPropertyChanged(new PropertyChangedEventArgs("Count"));
            this.OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, changedItems, startIndex));

         }
         catch (Exception ex) { Helpers.Insights.TrackException(ex); }
      }

      /*
      public void RemoveRange(IEnumerable<T> collection)
      {
         try
         {
            semaphore.Wait();
            if (collection == null) throw new ArgumentNullException("collection");
            foreach (var i in collection) { Items.Remove(i); }
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
         }
         catch (Exception ex) { Helpers.Insights.TrackException(ex); }
         finally { semaphore.Release(); }
      }
      */

      public void Replace(T item)
      {
         try
         {
            var index = this.Items.IndexOf(item);
            if (index == -1)
            {
               this.Items.Add(item);
               this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
            }
            else
            {
               var oldItem = this.Items[index];
               this.SetItem(index, item);
               this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, item, oldItem, index));
            }
         }
         catch { }
      }

      public async Task ReplaceRangeAsync(IEnumerable<T> collection)
      {
         try
         {
            await semaphore.WaitAsync();
            if (collection == null) throw new ArgumentNullException("collection");
            Items.Clear();
            AddRange(collection, NotifyCollectionChangedAction.Reset);
         }
         catch (Exception ex) { Helpers.Insights.TrackException(ex); }
         finally { semaphore.Release(); }
      }

   }
}