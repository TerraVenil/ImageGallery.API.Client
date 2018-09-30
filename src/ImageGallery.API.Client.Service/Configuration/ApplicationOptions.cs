namespace ImageGallery.API.Client.Service.Configuration
{
    public class ApplicationOptions
    {
        public string ApplicationName { get; set; }

        /// <summary>
        ///
        /// </summary>
        public OpenIdConnectConfiguration OpenIdConnectConfiguration { get; set; }

        /// <summary>
        ///
        /// </summary>
        public ImagegalleryApiConfiguration ImagegalleryApiConfiguration { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public FlickrConfiguration FlickrConfiguration { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public GeneralConfiguration GeneralConfiguration { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ZipkinConfiguration ZipkinConfiguration { get; set; }
    }
}
