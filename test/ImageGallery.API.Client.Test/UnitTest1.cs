using System;
using FlickrNet;
using Xunit;

namespace ImageGallery.API.Client.Test
{
    public class UnitTest1
    {
        private readonly Flickr flickr;

        public UnitTest1()
        {
            string apiKey = "9553173ca1a819c3afcea61bd6e288cd";
            string secret = "75a03c6bc1b8811d";

            this.flickr = new Flickr(apiKey, secret);
        }

        [Theory]
        [InlineData("47222519@N07")]
        public async void Can_Get_Photos_By_UserId(string userId)
        {
            var results = await flickr.PhotosetsGetListAsync(userId, 0, 10);

            Assert.NotEmpty(results);
        }
    }
}
