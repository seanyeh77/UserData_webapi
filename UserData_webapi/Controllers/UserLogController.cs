using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UserData_webapi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserLogController : ControllerBase
    {
        private readonly ILogger<UserLogController> _logger;
        private readonly IUserLogRepository _userLogRepistory;
        private readonly IUserCardRepository _userCardRepistory;
        private readonly IUserDataRepository _userDataRepository;
        private readonly IConfiguration _configuration;
        public UserLogController(ILogger<UserLogController> logger, IUserLogRepository userLogRepistory, IUserCardRepository userCardRepistory, IUserDataRepository userDataRepistory, IConfiguration configuration)
        {
            _configuration = configuration;
            _logger = logger;
            _userLogRepistory = userLogRepistory;
            _userCardRepistory = userCardRepistory;
            _userDataRepository = userDataRepistory;
        }

        [HttpGet]
        public IActionResult List()
        {
            return Ok(_userLogRepistory.All);
        }
        [HttpGet("table/")]
        public IActionResult Listtable()
        {
            return Ok(_userLogRepistory.usertable(_userCardRepistory, _userDataRepository));
        }
        [HttpPost]
        public IActionResult Create([FromBody] UserLog item)
        {
            try
            {
                linkline linkline = new linkline(_configuration);

                if (item == null || !ModelState.IsValid)
                {
                    return BadRequest("null");
                }
                if (_userCardRepistory.DoesItemExistfreezefalse(item.UID))
                {
                    if (_userDataRepository.DoesItemExistfreezefalse(_userCardRepistory.GetID(item.UID)))
                    {
                        _userLogRepistory.Insert(item);
                        int id = _userCardRepistory.GetID(item.UID);
                        _userDataRepository.changestate(_userCardRepistory.GetID(item.UID));
                        linkline.sendlinenotify($"{_userDataRepository.getname(id)}已{(_userDataRepository.getIDstate(_userCardRepistory.GetID(item.UID)) ? "簽到" : "簽退")}({id.ToString("D6")})", "level2");
                    }
                    else
                    {
                        int id = _userCardRepistory.GetID(item.UID);
                        linkline.sendlinenotify($"{_userDataRepository.getname(id)}已{(_userDataRepository.getIDstate(_userCardRepistory.GetID(item.UID)) ? "簽到" : "簽退")}({id.ToString("D6")})\n但被結凍", "level2");
                        return BadRequest("freeze");
                    }
                }
                else
                {
                    return BadRequest("UID");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(item);
        }
        [HttpDelete("{name}")]
        public IActionResult Delete(string name)
        {
            try
            {
                var item = _userLogRepistory.FindUID(name);
                if (item == null)
                {
                    return BadRequest("null");
                }
                _userLogRepistory.DeleteUIDAll(name);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return NoContent();
        }
    }
}
