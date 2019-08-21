using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using BookShareApp.API.Config;
using BookShareApp.API.DataAccess;
using BookShareApp.API.Dto;
using BookShareApp.API.Framework;
using BookShareApp.API.Models;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BookShareApp.API.Controllers
{
    [Authorize]
    [Route("api/users/{userId}/photos")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly IBookShareRepository _repository;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _cloudinarySettings;
        private readonly Cloudinary _cloudinary;

        public PhotosController(IBookShareRepository repository,
                    IMapper mapper,
                    IOptions<CloudinarySettings> cloudinarySettings)
        {
            this._cloudinarySettings = cloudinarySettings;
            this._mapper = mapper;
            this._repository = repository;

            Account acc = new Account(
                _cloudinarySettings.Value.CloudName,
                _cloudinarySettings.Value.ApiKey,
                _cloudinarySettings.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(acc);
        }

        [HttpGet("{id}", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var photoFromRepo = await _repository.GetPhoto(id);
            var photo = _mapper.Map<PhotoForReturnDto>(photoFromRepo);

            return Ok(photo);
        }

        [HttpPost("{id}/setMain")]
        public async Task<IActionResult> SetMainPhoto(int userId, int id)
        {
            if (!isUserAuthorizd(userId))
            {
                return Unauthorized();
            }

            var user = await _repository.GetUser(userId);
            if (!user.Photos.Any(a => a.Id == id))
            {
                return Unauthorized();
            }

            var photoFromRepo = await _repository.GetPhoto(id);
            if (photoFromRepo.IsMain)
            {
                return BadRequest("This is already the main photo");
            }

            var currentMainPhoto = await _repository.GetMainPhotoForUser(userId);
            currentMainPhoto.IsMain = false;

            photoFromRepo.IsMain = true;

            if (await _repository.SaveAll())
            {
                return NoContent();
            }

            return BadRequest("Could not set photo to main");
        }

        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int userId, [FromForm]PhotoForCreationDto photoForCreationDto)
        {
            if (!isUserAuthorizd(userId))
            {
                return Unauthorized();
            }
            var userFromRepo = await _repository.GetUser(userId);
            var file = photoForCreationDto.File;
            var uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill")
                    };

                    uploadResult = _cloudinary.Upload(uploadParams);
                }
            }

            photoForCreationDto.Url = uploadResult.Uri.ToString();
            photoForCreationDto.PublicId = uploadResult.PublicId;

            var photo = _mapper.Map<Photo>(photoForCreationDto);
            if (!userFromRepo.Photos.Any(x => x.IsMain))
            {
                photo.IsMain = true;
            }

            userFromRepo.Photos.Add(photo);

            if (await _repository.SaveAll())
            {
                var photoToReturn = _mapper.Map<PhotoForReturnDto>(photo);
                return CreatedAtRoute("GetPhoto", new { id = photo.Id }, photoToReturn);
            }

            return BadRequest("Could not add the photo");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto(int userId, int id)
        {
            if (!isUserAuthorizd(userId))
                return Unauthorized();

            var user = await _repository.GetUser(userId);
            if (!user.Photos.Any(a => a.Id == id))
                return Unauthorized();

            var photoFromRepo = await _repository.GetPhoto(id);
            if (photoFromRepo.IsMain)
                return BadRequest("Please make sure to have a main photo");


            var deleteParams = new DeletionParams(photoFromRepo.PublicId);
            var result = _cloudinary.Destroy(deleteParams);

            if (result.Result == Constants.CloudinarySuccessResponse)
                _repository.Delete(photoFromRepo);

            if (await _repository.SaveAll())
                return Ok();

            return BadRequest();
        }

        private bool isUserAuthorizd(int userId)
        {
            int userConnectedId;
            bool isOk = int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier).Value, out userConnectedId);
            if (!isOk) return false;
            if (isOk && userId != userConnectedId)
            {
                return false;
            }
            return true;
        }
    }
}