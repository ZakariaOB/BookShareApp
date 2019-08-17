using System.Threading.Tasks;
using AutoMapper;
using BookShareApp.API.Config;
using BookShareApp.API.DataAccess;
using BookShareApp.API.Dto;
using CloudinaryDotNet;
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

        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int userId,
                    PhotoForCreationDto photoForCreationDto)
        {
            return null;
        }
    }
}