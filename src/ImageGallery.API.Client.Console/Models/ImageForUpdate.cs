using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ImageGallery.API.Client.Console.Models
{
    public class ImageForUpdate
    {
        /// <summary>
        ///
        /// </summary>
        [Required]
        [MaxLength(150)]
        public string Title { get; set; }

        [Required]
        [MaxLength(50)]
        public string Category { get; set; }

    }
}
