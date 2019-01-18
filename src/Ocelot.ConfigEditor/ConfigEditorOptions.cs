namespace Ocelot.ConfigEditor
{
    public class ConfigEditorOptions
    {
        public string Path { get; set; } = "cfgedt";

        public bool AllowDownload { get; set; } = true;

        public bool AllowUpload { get; set; } = true;
    }
}