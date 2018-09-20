namespace ImageGallery.API.Client.Service.Configuration
{
    public class GeneralConfiguration
    {
        public string LocalImagesPath { get; set; }

        public int QueryRetriesCount { get; set; } = 3;

        public int QueryWaitBetweenQueries { get; set; } = 200;
    }
}
