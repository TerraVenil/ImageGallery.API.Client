using System;
using ImageGallery.API.Client.Service.Interface;
using ImageGallery.API.Client.Service.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ImageGallery.API.Client.Test.Fixtures
{
    public class ImageServiceFixture : IDisposable
    {
        public ImageServiceFixture()
        {
            var serviceProvider = new ServiceCollection()
                .AddScoped<IImageService, ImageService>()
                .BuildServiceProvider();

            ImageService = serviceProvider.GetRequiredService<IImageService>();
        }

        public IImageService ImageService { get; private set; }

        public void Dispose()
        {
        }
    }
}
