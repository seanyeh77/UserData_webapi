using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CognitiveServices.Speech.Transcription;

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
        private readonly IFaceRepository _faceRepository;
        public UserLogController(ILogger<UserLogController> logger, IUserLogRepository userLogRepistory, IUserCardRepository userCardRepistory, IUserDataRepository userDataRepistory, IConfiguration configuration, IFaceRepository faceRepository)
        {
            _configuration = configuration;
            _logger = logger;
            _userLogRepistory = userLogRepistory;
            _userCardRepistory = userCardRepistory;
            _userDataRepository = userDataRepistory;
            _faceRepository = faceRepository;
        }

        [HttpGet]
        public IActionResult List()
        {
            return Ok(_userLogRepistory.UserLogs_All);
        }
        [HttpGet("table")]
        public IActionResult Listtable()
        {
            return Ok(_userLogRepistory.usertable(_userCardRepistory, _userDataRepository));
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserLog item)
        {
            try
            {
                linkline linkline = new linkline(_configuration);
                SendEmail sendEmail = new SendEmail(_configuration, _userDataRepository);
                if (item == null || !ModelState.IsValid)
                {
                    return BadRequest("null");
                }
                if (_userCardRepistory.DoesItemExistfreezefalse(item.UID))
                {
                    string ID = _userCardRepistory.GetID(item.UID);
                    if (_userDataRepository.DoesItemExistfreezefalse(_userCardRepistory.GetID(item.UID)))
                    {
                        item.state = !_userDataRepository.getstate(ID);
                        _userLogRepistory.UserLog_Insert(item);
                        _userDataRepository.changestate(_userCardRepistory.GetID(item.UID));
                        string message = $"{_userDataRepository.getchinesename(ID)}已{(_userDataRepository.getstate(_userCardRepistory.GetID(item.UID)) ? "簽到" : "簽退")}({ID})";
                        await sendEmail.sendemail_id(ID, "成功打卡通知", message);
                        linkline.sendlinenotify(message, "level1");
                    }
                    else
                    {
                        
                        string message = $"{_userDataRepository.getchinesename(ID)}想要{(_userDataRepository.getstate(_userCardRepistory.GetID(item.UID)) ? "簽到" : "簽退")}({ID})\n但被結凍";
                        await sendEmail.sendemail_id(ID, "打卡失敗通知", message);
                        linkline.sendlinenotify(message, "level1");
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
        [HttpPost("face")]
        public async Task<IActionResult> Createbyface([FromForm(Name = "userdata")] IFormFile userdata, [FromForm(Name = "state")]string state)
        {
            try
            {
                linkline linkline = new linkline(_configuration);
                SendEmail sendEmail = new SendEmail(_configuration, _userDataRepository);
                (List<UserData>,int) userDatas = await _userLogRepistory.detect_face(userdata);
                switch (userDatas.Item2)
                {
                    case 0://找不到人臉
                        return Ok("face");
                        break;
                    case 1://找到人臉但找不到人
                        return Ok("people");
                        break;
                    case 2://找到人臉也找到人
                        foreach (string userID in userDatas.Item1.Select(x => x.ID))
                        {
                            UserLogByFace userLogByFace = new UserLogByFace();
                            if (_userDataRepository.DoesItemExistfreezefalse(userID))
                            {
                                userLogByFace.ID = userID;
                                userLogByFace.time = DateTime.Now;
                                userLogByFace.state = (state == "in" ? true : false);
                                _userLogRepistory.UserLogByFace_Insert(userLogByFace);
                                _userDataRepository.All.FirstOrDefault(x => x.ID == userID).state = userLogByFace.state;
                                string message = $"{_userDataRepository.getchinesename(userID)}已{(userLogByFace.state ? "簽到" : "簽退")}({userID})";
                                await sendEmail.sendemail_id(userID, "成功打卡通知", message);
                                linkline.sendlinenotify(message, "level1");
                            }
                            else
                            {
                                string message = $"{_userDataRepository.getchinesename(userID)}想要{(userLogByFace.state ? "簽到" : "簽退")}({userID})\n但被結凍";
                                await sendEmail.sendemail_id(userID, "打卡失敗通知", message);
                                linkline.sendlinenotify(message, "level1");
                                return BadRequest("freeze");
                            }
                        }
                        return Ok(userDatas.Item1.Select(x => x.ChineseName).ToList());
                        break;
                    case 3://與Face++連線時出現問題
                        return Ok("connet");
                        break;
                    default:
                        return Ok();//沒用
                        break;
                }
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
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
            return Ok();
        }
    }
}
