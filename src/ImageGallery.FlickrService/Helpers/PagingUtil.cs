namespace ImageGallery.FlickrService.Helpers
{
    public static class PagingUtil
    {
        public static int CalculateNumberOfPages(int totalNumberOfItems, int pageSize)
        {
            var result = totalNumberOfItems % pageSize;
            if (result == 0)
                return totalNumberOfItems / pageSize;
            return totalNumberOfItems / pageSize + 1;
        }
    }
}
