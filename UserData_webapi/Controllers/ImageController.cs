
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
        public IActionResult getuserdaylog()
        {
            HtmlNodeCollection strJson = _imageRepository.crawler();
            return Ok(strJson);
        }
    }
}
