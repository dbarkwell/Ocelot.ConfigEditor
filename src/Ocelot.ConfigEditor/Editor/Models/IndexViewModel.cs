using Ocelot.Configuration.File;

namespace Ocelot.ConfigEditor.Editor.Models
{
    public class IndexViewModel
    {
        public IndexViewModel()
        {
            Error = new Error();    
        }
        
        public FileConfiguration FileConfiguration { get; set; }
        
        public Error Error { get; set; }
    }

    public class Error
    {
        public bool HasError => !string.IsNullOrEmpty(ErrorMessage);
        
        public string ErrorMessage { get; set; }
    }
}
