using System;
using System.Collections.Generic;
using System.Text;

namespace ImageGallery.FlickrService.Helpers
{
    public static class PagingUtil
    {
        public static int CalculateNumberOfPages(int totalNumberOfItems, int pageSize)
        {
            var result = totalNumberOfItems % pageSize;
            if (result == 0)
                return totalNumberOfItems / pageSize;
            else
                return totalNumberOfItems / pageSize + 1;
        }
    }
}
