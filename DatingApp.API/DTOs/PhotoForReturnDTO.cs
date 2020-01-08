using System;

namespace DatingApp.API.DTOs
{
    internal class PhotoForReturnDTO
    {
        public string Id { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public DateTime DateAdded { get; set; } = DateTime.Now;
        public bool IsProfilePic { get; set; }
        public string PublicId { get; set; }
    }
}