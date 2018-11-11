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
    }
}