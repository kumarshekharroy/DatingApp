using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using DatingApp.API.Data;
using DatingApp.API.DTOs;
using DatingApp.API.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using CloudinaryDotNet.Actions;
using DatingApp.API.Models;
using System.Linq;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [Route("api/users/{userId:long}/photos")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly IDatingRepository _datingRepository;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _cloudinarySettings;
        private readonly Cloudinary _cloudinary;
        public PhotosController(IDatingRepository datingRepository, IMapper mapper, IOptions<CloudinarySettings> cloudinarySettings)
        {
            _datingRepository = datingRepository;
            _mapper = mapper;
            _cloudinarySettings = cloudinarySettings;

            Account account = new Account(_cloudinarySettings.Value.CloudName, _cloudinarySettings.Value.ApiKey, _cloudinarySettings.Value.SecretKey);

            _cloudinary = new Cloudinary(account);
        }
        [HttpGet("{id:int}", Name = nameof(GetPhoto))]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var photoFromRepo = await this._datingRepository.GetPhoto(id);

            var photo = _mapper.Map<PhotoForReturnDTO>(photoFromRepo);
            return StatusCode(StatusCodes.Status200OK, photo);

        }


        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(long userId, [FromForm] PhotoForCreationDTO photoForCreationDTO)
        {
            if (userId != long.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return StatusCode(StatusCodes.Status401Unauthorized);
            var user = await _datingRepository.GetUser(userId);
            var file = photoForCreationDTO.File;
            var uploadResult = new ImageUploadResult();
            if (file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.Name, stream),
                    Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                };
                uploadResult = await _cloudinary.UploadAsync(uploadParams);
            }

            photoForCreationDTO.Url = uploadResult.Uri.ToString();
            photoForCreationDTO.PublicId = uploadResult.PublicId;
            var photo = _mapper.Map<Photo>(photoForCreationDTO);
            photo.IsProfilePic = !user.Photos.Any(x => x.IsProfilePic);

            user.Photos.Add(photo);
            if (await _datingRepository.SaveAll())
            {
                var photoToReturn = _mapper.Map<PhotoForReturnDTO>(photo);
                return CreatedAtRoute(nameof(GetPhoto), new { userId, id = photo.Id }, photoToReturn);
            }

            return StatusCode(StatusCodes.Status400BadRequest, "Could not add the photo");

        }
        [HttpPost("{id:int}/setProfilePic")]
        public async Task<IActionResult> SetProfilePic(long userId, int id)
        {
            if (userId != long.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return StatusCode(StatusCodes.Status401Unauthorized);
            var user = await _datingRepository.GetUser(userId);
            if (!user.Photos.Any(x => x.Id == id))
                return StatusCode(StatusCodes.Status401Unauthorized);
            var photo = await this._datingRepository.GetPhoto(id);

            if (photo.IsProfilePic)
                return StatusCode(StatusCodes.Status400BadRequest, "This is already the main photo");
            var currentProfilePhoto = await this._datingRepository.GetProfilePhotoForUser(userId);
            currentProfilePhoto.IsProfilePic = false;
            photo.IsProfilePic = true;

            if (await this._datingRepository.SaveAll())
                return StatusCode(StatusCodes.Status204NoContent);

            return StatusCode(StatusCodes.Status400BadRequest, "Could not set photo to main");
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto(long userId, int id)
        {
            if (userId != long.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return StatusCode(StatusCodes.Status401Unauthorized);
            var user = await _datingRepository.GetUser(userId);
            if (!user.Photos.Any(x => x.Id == id))
                return StatusCode(StatusCodes.Status401Unauthorized);
            var photo = await this._datingRepository.GetPhoto(id);

            if (photo.IsProfilePic)
                return StatusCode(StatusCodes.Status400BadRequest, "You cannot delete your profile photo.");


            if (string.IsNullOrWhiteSpace(photo.PublicId))
            {

                _datingRepository.Delete(photo);
            }
            else
            {
                var deleteParams = new DeletionParams(photo.PublicId);
                var result = await _cloudinary.DestroyAsync(deleteParams);
                if (result.Result == "ok")
                {
                    _datingRepository.Delete(photo);
                }

            }
            if (await _datingRepository.SaveAll())
            {
                return StatusCode(StatusCodes.Status200OK);
            }
            
            return StatusCode(StatusCodes.Status400BadRequest, "Failed to delete photo");
       
        }
    }

}