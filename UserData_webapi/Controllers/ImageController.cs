
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace UserData_webapi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ImageController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IImageRepository _imageRepository;
        public ImageController(IConfiguration configuration,IImageRepository iimageRepository)
        {
            _configuration = configuration; 
            _imageRepository = iimageRepository;
        }
        [HttpGet]
        public async Task<IActionResult> getuserdaylog()
        {
            return Ok(await _imageRepository.crawler());
        }
        [HttpGet("icon")]
        public async Task<IActionResult> geticon()
        {
            return Ok(await _imageRepository.geticonurl());
        }
        [HttpGet("teamimg")]
        public async Task<IActionResult> getteamimg()
        {
            return Ok(await _imageRepository.getteamimg());
        }
        [HttpGet("frcview/{language}")]
        public IActionResult frcview(string language)
        {
            return Ok(_imageRepository.frcview(language));
        }
        [HttpGet("teamview/{language}")]
        public async Task<IActionResult> teamview(string language)
        {
            return Ok(await _imageRepository.teamview(language));
        }
    }
}
