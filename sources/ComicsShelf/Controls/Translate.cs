using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ComicsShelf.Controls
{

   [ContentProperty("Text")]
   public class TranslateExtension : IMarkupExtension
   {

      public string Text { get; set; }

      public object ProvideValue(IServiceProvider serviceProvider)
      {
         if (this.Text == null) return "";
         var translatedText = Resources.Translations.ResourceManager.GetString(this.Text);
         // if (translatedText == null) { translatedText = string.Format("[{0}]", this.Text); }
         return translatedText;
      }

   }

}
