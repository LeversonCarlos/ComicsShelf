﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace ComicsShelf.Helpers.Controls
{
   public class ListView : Grid
   {

      #region New
      public ListView()
      {
         this.RowSpacing = 0;
         this.ColumnSpacing = 0;         
      }
      #endregion

      #region Properties
      public DataTemplate ItemTemplate { get; set; }
      #endregion

      #region Columns
      public static readonly BindableProperty ColumnsProperty =
         BindableProperty.Create("Columns", typeof(int), typeof(ListView), 3, propertyChanged: OnColumnsChanged);
      public int Columns
      {
         get { return (int)GetValue(ColumnsProperty); }
         set { SetValue(ColumnsProperty, value); }
      }
      private static void OnColumnsChanged(BindableObject bindable, object oldValue, object newValue)
      {
         try
         {
            var VIEW = bindable as ListView;
            var COLUMNS = (int)newValue;
            if (VIEW.ColumnDefinitions.Count == COLUMNS) { return; }

            VIEW.ColumnDefinitions.Clear();
            for (var i = 0; i < COLUMNS; i++)
            { VIEW.ColumnDefinitions.Add(new ColumnDefinition()); }

            VIEW.Children?.Clear();
            VIEW.RefreshTiles();         

         }
         catch { }
      }
      #endregion

      #region ItemsSource
      public static readonly BindableProperty ItemsSourceProperty =
         BindableProperty.Create(nameof(ItemsSource),
            typeof(IList), typeof(ListView), null,
            BindingMode.TwoWay,
            propertyChanged: OnItemsSourceChanged);
      public IList ItemsSource
      {
         get { return (IList)GetValue(ItemsSourceProperty); }
         set { SetValue(ItemsSourceProperty, value); }
      }
      private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
      {
         try
         {
            var VIEW = bindable as ListView;
            var VALUES = newValue as INotifyCollectionChanged;
            if (VALUES != null)
            {
               VIEW.RefreshTiles();
               VALUES.CollectionChanged += 
                  (object sender, NotifyCollectionChangedEventArgs e) => 
                  { VIEW.RefreshTiles(); };
            }
            
         }
         catch { }
      }
      #endregion

      #region ItemTappedCommand

      public static readonly BindableProperty ItemTappedCommandProperty =
         BindableProperty.Create("ItemTappedCommand", typeof(ICommand), typeof(ListView), null);

      public ICommand ItemTappedCommand
      {
         get { return (ICommand)GetValue(ItemTappedCommandProperty); }
         set { SetValue(ItemTappedCommandProperty, value); }
      }

      #endregion


      #region RefreshTiles

      private void RefreshTiles()
      {
         try
         {

            // INITIALIZE
            var ITEMS = ((IEnumerable<object>)this.ItemsSource).ToList();
            if (ITEMS == null || ITEMS.Count == 0) { this.Children?.Clear(); return; }
            this.RowDefinitions?.Clear();

            // ROWS
            var ROWS = Math.Ceiling(ITEMS.Count / (float)this.Columns);
            this.RowDefinitions.Clear();
            for (var i = 0; i < ROWS; i++)
            { this.RowDefinitions?.Add(new RowDefinition { }); }

            // ITEM
            for (var index = 0; index < ITEMS.Count; index++)
            {
               var columnIndex = index % this.Columns;
               var rowIndex = (int)Math.Floor(index / (float)this.Columns);
               var itemTile = this.RefreshTiles_GetItemTile(ITEMS[index]);
               this.Children?.Add(itemTile, columnIndex, rowIndex);
            }

         }
         catch { }
      }

      private View RefreshTiles_GetItemTile(object item)
      {

         var tapGestureRecognizer = new TapGestureRecognizer
         {
            Command = this.ItemTappedCommand,
            CommandParameter = item,
            NumberOfTapsRequired = 1
         };

         // var itemTile = (Layout)Activator.CreateInstance(this.ItemTemplate, item);
         var itemTile = (View)this.ItemTemplate.CreateContent();
         var bindableObject = itemTile as BindableObject;
         if (bindableObject != null)
            bindableObject.BindingContext = item;
         itemTile.InputTransparent = false;
         itemTile?.GestureRecognizers.Add(tapGestureRecognizer);
         // this.Children.Add(itemTile);

         return itemTile;
      }

      #endregion

   }
}