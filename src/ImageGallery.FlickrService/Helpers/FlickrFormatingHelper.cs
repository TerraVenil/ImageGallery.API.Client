using System;
using System.Collections.Generic;
using System.Linq;
using FlickrNet;

namespace ImageGallery.FlickrService.Helpers
{
    public static class FlickrFormatingHelper
    {
        public static void FormatMachineTagValue(Value value)
        {
            string formatedString =
                $"MachineTag {value.MachineTag}|Value: {value.ValueText}|Date Added {value.DateFirstAdded}|DateLastUsed {value.DateLastUsed}|Namespace: {value.NamespaceName}|Predicate: {value.PredicateName}|Usage: {value.Usage}";

            Console.WriteLine(formatedString);
        }

        public static void FormatMachineTagPair(Pair pair)
        {
            string formatedString =
                $"PairName:{pair.PairName}|Namespace Name:{pair.NamespaceName}|PredicateName:{pair.PredicateName}|Usage:{pair.Usage}";

            Console.WriteLine(formatedString);
        }

        public static void FormatSearchOptions(PhotoSearchOptions photoSearchOptions)
        {
            Console.WriteLine("---- PhotoSearch Options ----");
            Console.WriteLine("UserId:" + photoSearchOptions.UserId);
            Console.WriteLine("Machine Tags:" + photoSearchOptions.MachineTags);
            Console.WriteLine("Extras:" + photoSearchOptions.Extras);
            Console.WriteLine("HasGeo:" + photoSearchOptions.HasGeo);
            Console.WriteLine("PerPage:" + photoSearchOptions.PerPage);
            Console.WriteLine("----------------------------");
        }

        public static void PrintPhotos(PhotoCollection photos)
        {
            Console.WriteLine("------------------------------------------------------------------");
            Console.WriteLine("Total Photos: " + photos.Count);
            Console.WriteLine("------------------------------------------------------------------");

            foreach (Photo p in photos)
            {
                PrintPhoto(p);
            }
        }

        public static void PrintPhotos(IList<Photo> photos)
        {
            Console.WriteLine("------------------------------------------------------------------");
            Console.WriteLine("Total Photos: " + photos.Count);
            Console.WriteLine("------------------------------------------------------------------");

            foreach (var p in photos)
            {
                PrintPhoto(p);
            }
        }

        public static void PrintPhoto(this Photo p)
        {
            Console.WriteLine($"PhotoId: {p.PhotoId}");
            Console.WriteLine($"http://farm{p.Farm}.static.flickr.com/{p.Server}/{p.PhotoId}_{p.Secret}.jpg");
            Console.WriteLine($"Url: {p.WebUrl}");

            Console.WriteLine($"Title: {p.Title}");
            Console.WriteLine($"UserId: {p.UserId}");
            Console.WriteLine($"Date Taken: {p.DateTaken}");
            Console.WriteLine($"Views: {p.Views}");
            Console.WriteLine($"License: {p.License}");
            Console.WriteLine($"Can Download: {p.CanDownload}");

            // p.OriginalSecret
            // p.OwnerName
            //  p.GeoPermissions.IsContact
            // p.GeoPermissions.IsFamily
            // p.GeoPermissions.IsPublic
            // p.GeoPermissions.PhotoId


            Console.WriteLine($"PlaceId: {p.PlaceId}");
            Console.WriteLine($"Where On Earth/WoeId: {p.WoeId}");
            Console.WriteLine($"Lat/Lon: {p.Latitude} {p.Longitude} - {p.Accuracy} \n");

            Console.WriteLine("Tags Count: " + p.Tags.Count + "\n");

            if (p.Tags.Count > 0)
            {
                Console.WriteLine("-- Tag List --");
                Console.WriteLine(string.Join(", ", p.Tags.ToArray()));

            }
            else
                Console.WriteLine("-- No Tags Found --");

            Console.WriteLine("\n------------------------------------------------------------------");
        }

        public static void PrintPhotoInfo(PhotoInfo p)
        {
            Console.WriteLine($"PhotoId: {p.PhotoId}");
            Console.WriteLine($"Title: {p.Title}");
            Console.WriteLine($"Date Taken: {p.DateTaken}");
            Console.WriteLine($"License: {p.License}");
            Console.WriteLine($"Can Download: {p.CanDownload}");
            Console.WriteLine($"MediumUrl: {p.MediumUrl}");
            Console.WriteLine($"SmallUrl: {p.Small320Url}");
            Console.WriteLine($"OriginalUrl: {p.OriginalUrl}");
            Console.WriteLine($"SquareThumbnailUrl: {p.SquareThumbnailUrl}");
            Console.WriteLine($"Small: {p.SmallUrl}");
            Console.WriteLine($"WebUrl: {p.WebUrl}");

            if (p.Location != null)
            {
                Console.WriteLine($"Latitude: {p.Location.Latitude}");
                Console.WriteLine($"Longitude: {p.Location.Longitude}");
            }

        }

        public static void PrintSet(Photoset photoset)
        {
            Console.WriteLine($"Title: {photoset.Title}");
            Console.WriteLine($"PhotosetId: {photoset.PhotosetId}");
            Console.WriteLine($"OwnerId: {photoset.OwnerId}");
            Console.WriteLine($"Description\n {photoset.Description}\n");

            Console.WriteLine($"PrimaryPhotoId: {photoset.PrimaryPhotoId}");
            Console.WriteLine($"{photoset.Url}");
            Console.WriteLine($"{photoset.PhotosetSmallUrl}");
            Console.WriteLine($"{photoset.PhotosetSquareThumbnailUrl}"); ;
            Console.WriteLine($"{photoset.PhotosetThumbnailUrl}\n");

            Console.WriteLine($"Farm: {photoset.Farm}");
            Console.WriteLine($"Secret: {photoset.Secret}");
            Console.WriteLine($"Server: {photoset.Server}\n");

            Console.WriteLine($"NumberOfPhotos: {photoset.NumberOfPhotos}"); ;
            Console.WriteLine($"NumberOfVideos: {photoset.NumberOfVideos}"); ;

            Console.WriteLine("\n------------------------------------------------------------------");

        }

        public static void PrintExifTags(ExifTagCollection exif)
        {
            var tags = exif.GroupBy(ex => ex.TagSpace).Select(g => new { Category = g.Key, ExifTag = g });

            foreach (var tag in tags)
            {
                Console.WriteLine($"--- {tag.Category} ---");
                foreach (var exifTag in tag.ExifTag)
                    Console.WriteLine($"{exifTag.Label} {exifTag.Raw}");

                Console.WriteLine("\n");
            }
        }

        public static void PrintPlaceInfo(PlaceInfo placeInfo)
        {
            Console.WriteLine($"Description: {placeInfo.Description}");
            Console.WriteLine($"PlaceId: {placeInfo.PlaceId}");
            Console.WriteLine($"WoeId: {placeInfo.WoeId}");
            Console.WriteLine($"Longitude: {placeInfo.Longitude}");
            Console.WriteLine($"Latitude: {placeInfo.Latitude}\n");

            Console.WriteLine($"PlaceUrl: {placeInfo.PlaceUrl}");
            Console.WriteLine($"PlaceType: {placeInfo.PlaceType}");
            Console.WriteLine($"HasShapeData: {placeInfo.HasShapeData}");
            Console.WriteLine($"TimeZone: {placeInfo.TimeZone}\n");

            Console.WriteLine("--- Neighbourhood ---");
            PrintPlace(placeInfo.Neighbourhood);

            Console.WriteLine("--- Locality ---");
            PrintPlace(placeInfo.Locality);

            Console.WriteLine("--- County ---");
            PrintPlace(placeInfo.County);

            Console.WriteLine("--- Region ---");
            PrintPlace(placeInfo.Region);

            Console.WriteLine("--- Country ---");
            PrintPlace(placeInfo.Country);

            Console.WriteLine("--- ShapeData ---");
            if (placeInfo.HasShapeData)
            {
                Console.WriteLine($"DateCreated: {placeInfo.ShapeData.DateCreated}");
                Console.WriteLine($"PointCount: {placeInfo.ShapeData.PointCount}");
                //Console.WriteLine(string.Format("Urls Count: {0}", placeInfo.ShapeData.Urls.Count));

                //for (int i = 0; i < placeInfo.ShapeData.PointCount; i++)
                //    Console.WriteLine(string.Format("Point {0}: {1} {2}", i, placeInfo.ShapeData.PolyLines[0][i].X, placeInfo.ShapeData.PolyLines[0][i].Y));     

            }
            else
                Console.WriteLine("Not Available");

        }

        public static void PrintPlace(Place place)
        {
            Console.WriteLine($"Description: {place.Description}");
            Console.WriteLine($"PlaceType: {place.PlaceType}");
            Console.WriteLine($"Lat/Lon: {place.Latitude} {place.Longitude}");
            Console.WriteLine($"PlaceUrl: {place.PlaceUrl}\n");
            Console.WriteLine($"PlaceId: {place.PlaceId}");
            Console.WriteLine($"WoeId: {place.WoeId}");
            Console.WriteLine($"TimeZone: {place.TimeZone}\n");
            Console.WriteLine($"PhotoCount: {place.PhotoCount}");
            Console.WriteLine("\n------------------------------------------------------------------");
        }

    }
}
