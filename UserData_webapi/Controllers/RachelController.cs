using Microsoft.AspNetCore.Mvc;

namespace UserData_webapi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RachelController : Controller
    {
        private readonly ILogger<UserCardController> _logger;
        private readonly IUserCardRepository _userCardRepository;
        private readonly IUserDataRepository _userDataRepository;
        private readonly IConfiguration _configuration;
        private readonly IRechalRepository _rechalRepository;
        public RachelController(ILogger<UserCardController> logger, IUserCardRepository userCardRepository, IUserDataRepository userDataRepository, IConfiguration configuration, IRechalRepository rechalRepository)
        {
            _logger = logger;
            _userCardRepository = userCardRepository;
            _userDataRepository = userDataRepository;
            _configuration = configuration;
            _rechalRepository = rechalRepository;
        }
        [HttpGet]
        public IActionResult getstate()
        {
            return Ok(_rechalRepository.All.First().state);
        }
        [HttpPost("{UID}")]
        public IActionResult Updata(string UID)
        {
            linkline linkline = new linkline(_configuration);
            Rachelstate rachelstate = new Rachelstate();
            if (UID == "null")
            {
                rachelstate.ID = null;
                rachelstate.state = false;
                _rechalRepository.Update(rachelstate);
                linkline.sendlinenotify($"雷切機已關閉", "level2");
                return Ok("close");
            }
            else
            {
                int ID = _userCardRepository.GetID(UID);
                if (ID == 0)    //沒有找到此UID
                {
                    return BadRequest("UID");
                }
                else if (_rechalRepository.getstate())  //雷切機以開啟
                {
                    return BadRequest("isopen");
                }
                else if (!_rechalRepository.getstate())
                {
                    if (_userDataRepository.DoesItemExistfreeze(ID))
                    {
                        linkline.sendlinenotify($"{_userDataRepository.getchinesename(ID)}想開啟雷切機，但已被凍結", "level2");
                        return BadRequest("freeze");
                    }
                    else if (_userDataRepository.getstate(ID)) //判別此人簽到狀態
                    {
                        rachelstate.ID = ID;
                        rachelstate.state = true;
                        _rechalRepository.Update(rachelstate);
                        linkline.sendlinenotify($"雷切機已被{_userDataRepository.getchinesename(ID)}開啟", "level2");
                        return Ok("open");
                    }
                    else
                    {
                        linkline.sendlinenotify($"{_userDataRepository.getchinesename(ID)}尚未遷到想開啟雷切機", "level2");
                        return BadRequest("state");
                    }
                }
                else
                {
                    
                    return BadRequest("error");
                }
            }
        }
    }
}
