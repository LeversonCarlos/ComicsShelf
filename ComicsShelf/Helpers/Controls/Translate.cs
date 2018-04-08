using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ComicsShelf.Helpers.Controls
{

   [ContentProperty("Text")]
   public class TranslateExtension : Observables.ObservableObject, IMarkupExtension
   {

      #region Properties

      public string Text { get; set; }

      /*
      Type _Type;
      public Type Type
      {
         get { return this._Type; }
         set { this.SetProperty(ref this._Type, value); }
      }
      */

      #endregion

      #region ProvideValue
      public object ProvideValue(IServiceProvider serviceProvider)
      {

         if (this.Text == null) { return ""; }

         // var assembly = this.Type.GetTypeInfo().Assembly;
         // var resourceManager = this.GetResourceManager(assembly);

         var translatedText = R.Strings.ResourceManager.GetString(this.Text);
         if (translatedText == null) { translatedText = string.Format("[{0}]", this.Text); }
         return translatedText;
      }
      #endregion

   }

}