﻿using System;
using System.IO;
using ImageGallery.API.Client.Service.Interface;
using ImageGallery.API.Client.Service.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ImageGallery.API.Client.Test.Fixtures
{
    public class ImageServiceFixture : IDisposable
    {
        public ImageServiceFixture()
        {
            string appPath = Directory.GetCurrentDirectory();
            string photoPath = @"../../../../../data/photos";

            PhotoDirectory = Path.GetFullPath(Path.Combine(appPath, photoPath));

            var serviceProvider = new ServiceCollection()
                .AddScoped<IImageGalleryService, ImageGalleryService>()
                .BuildServiceProvider();

            ImageSearchService = serviceProvider.GetRequiredService<IImageGalleryService>();
        }

        public IImageGalleryService ImageSearchService { get; private set; }

        public string PhotoDirectory { get; private set; }

        public void Dispose()
        {
        }
    }
}
