using System.ComponentModel.DataAnnotations;

namespace ImageGallery.API.Client.Service.Models
{
    public class ImageForCreation
    {
        /// <summary>
        ///
        /// </summary>
        [Required]
        [MaxLength(150)]
        public string Title { get; set; }

        /// <summary>
        ///
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Category { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long? PhotoId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string DataSource { get; set; }

        /// <summary>
        ///
        /// </summary>
        [Required]
        public byte[] Bytes { get; set; }

        public override string ToString()
        {
            return $"PhotoId:{PhotoId}|Title:{Title}";
        }

    }
}
