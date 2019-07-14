﻿using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ComicsShelf.Home
{
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class HomePage : ContentPage
   {

      readonly HomeVM vm;
      public HomePage()
      {
         InitializeComponent();
         this.BindingContext = vm = new HomeVM();
      }

      /*
      async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
      {
         var item = args.SelectedItem as Item;
         if (item == null)
            return;

         await Navigation.PushAsync(new ItemDetailPage(new ItemDetailViewModel(item)));

         // Manually deselect item.
         ItemsListView.SelectedItem = null;
      }

      async void AddItem_Clicked(object sender, EventArgs e)
      {
         await Navigation.PushModalAsync(new NavigationPage(new NewItemPage()));
      }

      protected override void OnAppearing()
      {
         base.OnAppearing();

         if (viewModel.Items.Count == 0)
            viewModel.LoadItemsCommand.Execute(null);
      }
       */

   }
}