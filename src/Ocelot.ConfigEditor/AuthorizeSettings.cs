using System.Collections.Generic;

namespace Ocelot.ConfigEditor
{
    internal class AuthorizeSettings
    {
        public AuthorizeSettings()
        {    
        }
        
        public AuthorizeSettings(string name, bool hasAuthentication)
        {
            Name = name;
            HasAuthentication = hasAuthentication;
        }
        
        public string Name { get; set; }
        
        public bool HasAuthentication { get; set; }
        
        public IEnumerable<string> SignOutSchemes { get; set; }
    }
}