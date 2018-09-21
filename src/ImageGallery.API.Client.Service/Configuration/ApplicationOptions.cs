namespace ImageGallery.API.Client.Service.Configuration
{
    public class ApplicationOptions
    {
        /// <summary>
        ///
        /// </summary>
        public OpenIdConnectConfiguration OpenIdConnectConfiguration { get; set; }

        public FlickrConfiguration FlickrConfiguration { get; set; }

        public GeneralConfiguration GeneralConfiguration { get; set; }

    }
}
