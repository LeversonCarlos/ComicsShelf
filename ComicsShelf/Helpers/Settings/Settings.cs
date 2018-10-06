using System;
using System.Threading.Tasks;

namespace ComicsShelf.Helpers.Settings
{
   public class Settings
   {

      internal async Task Initialize()
      {
         try
         {
            this.Paths = new Paths();
            await this.Paths.Initialize();
         }
         catch (Exception ex) { throw; }
      }

      public Paths Paths { get; set; }

   }
}