using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.IO;
using System.Text;
using System.Configuration;

namespace UserData_webapi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserDataController : ControllerBase
    {
        private readonly ILogger<UserDataController> _logger;
        private readonly IUserDataRepository _userDataRepository;
        private readonly IUserCardRepository _userCardRepistory;
        private readonly IConfiguration _configuration;

        public UserDataController(ILogger<UserDataController> logger, IUserDataRepository userDataRepistory, IUserCardRepository userCardRepistory, IConfiguration configuration)
        {
            _logger = logger;
            _userDataRepository = userDataRepistory;
            _userCardRepistory = userCardRepistory;
            _configuration = configuration;
        }
        [HttpGet]
        public IActionResult List()
        {
            return Ok(_userDataRepository.All);
        }
        [HttpGet("ID")]
        public IActionResult ListID()
        {
            return Ok(_userDataRepository.All.Select(p => p.ID).ToList());
        }
        [HttpGet("{ID}")]
        public IActionResult List(string ID)
        {
            return Ok(_userDataRepository.Find(ID));
        }
        [HttpGet("position/{position}")]
        public async Task<IActionResult> ListPosition(string position)
        {
            var item = await _userDataRepository.GetUserData_poistion(position);
            return Ok(item);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromForm(Name = "userdata")] UserData userdata)
        {
            try
            {
                linkline linkline = new linkline(_configuration);
                SendEmail sendEmail = new SendEmail(_configuration, _userDataRepository);
                if (userdata == null || !ModelState.IsValid)
                {
                    return BadRequest("null");
                }
                else if(userdata.Image == null)
                {
                    return BadRequest("nullimg");
                }
                bool itemExistsID = _userDataRepository.DoesItemExistID(userdata.ID);
                bool itemExistslock = _userDataRepository.DoesItemExistlock(userdata.ID);
                if (itemExistsID)
                {
                    if (itemExistslock)
                    {
                        return BadRequest("lock");
                    }
                    return BadRequest("isID");
                }
                else
                {
                    int count = await _userDataRepository.Insert(userdata);
                    if (count == 0)
                    {
                        return BadRequest("facenull");
                    }
                    string message = $"{_userDataRepository.getchinesename(userdata.ID)}成功註冊個人資料\n成功加入了{count}張照片";
                    await sendEmail.sendemail_id(userdata.ID, "成功註冊通知", message);
                    linkline.sendlinenotify(message, "level2");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(userdata);
        }
        [HttpPost("detectimg")]
        public async Task<IActionResult> UserDataDetectImage([FromForm(Name = "image")] IFormFile image)
        {
            try
            {
                if (image == null)
                {
                    return NoContent();
                }
                (UserData, int) userdata = await _userDataRepository.SearchUser(image);
                switch (userdata.Item2)
                {
                    case 0://找不到人臉
                        return BadRequest("face");
                        break;
                    case 1://找到人臉但找不到人
                        return BadRequest("people");
                        break;
                    case 2://找到人臉也找到人
                        return Ok(userdata.Item1);
                        break;
                    case 3://與Face++連線時出現問題
                        return BadRequest("connet");
                        break;
                    default:
                        return BadRequest();//沒用
                        break;
                }
                
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut]
        public async Task<IActionResult> Edit([FromForm(Name = "userdata")] UserData userdata)
        {
            try
            {
                linkline linkline = new linkline(_configuration);
                SendEmail sendEmail = new SendEmail(_configuration, _userDataRepository);
                bool itemExistsID = _userDataRepository.DoesItemExistID(userdata.ID);
                bool itemExistslock = _userDataRepository.DoesItemExistlock(userdata.ID);
                if (itemExistsID)
                {
                    if (itemExistslock)
                    {
                        return BadRequest("lock");
                    }
                    _userDataRepository.Update(userdata);
                    string message = $"{_userDataRepository.getchinesename(userdata.ID)}成功修改了個人資料";
                    await sendEmail.sendemail_id(userdata.ID, "成功修改通知", message);
                    linkline.sendlinenotify(message, "level2");
                }
                else
                {
                    return BadRequest("ID");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok();
        }
        [HttpDelete("{ID}")]
        public async Task<IActionResult> Delete(string ID)
        {
            try
            {
                linkline linkline = new linkline(_configuration);
                SendEmail sendEmail = new SendEmail(_configuration, _userDataRepository);
                var item = _userDataRepository.FindID(ID);
                if (item == null)
                {
                    return BadRequest("ID");
                }
                string message = $"{_userDataRepository.getchinesename(ID)}成功刪除了個人資料";
                await sendEmail.sendemail_id(ID, "成功刪除通知", message);
                await _userDataRepository.Delete(ID);
                linkline.sendlinenotify(message, "level2");
                _userCardRepistory.DeleteID(ID);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok();
        }
        [HttpDelete("lock/{ID}")]
        public async Task<IActionResult> Lock(string ID)
        {
            try
            {
                linkline linkline = new linkline(_configuration);
                SendEmail sendEmail = new SendEmail(_configuration, _userDataRepository);
                var item = _userDataRepository.FindID(ID);
                if (item == null)
                {
                    return BadRequest("ID");
                }
                bool itemExistslock = _userDataRepository.DoesItemExistlock(item.ID);
                if (itemExistslock)
                {
                    return BadRequest("lock");
                }
                _userDataRepository.DeletelockID(ID);
                string message = $"{_userDataRepository.getchinesename(ID)}已被鎖定";
                await sendEmail.sendemail_id(ID, "鎖定通知", message);
                linkline.sendlinenotify(message, "level2");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok();
        }
        [HttpDelete("unlock/{ID}")]
        public async Task<IActionResult> unLock(string ID)
        {
            try
            {
                linkline linkline = new linkline(_configuration);
                SendEmail sendEmail = new SendEmail(_configuration, _userDataRepository);
                UserData item = _userDataRepository.FindID(ID);
                if (item == null)
                {
                    return BadRequest("ID");
                }
                bool itemExistslock = _userDataRepository.DoesItemExistlockfalse(item.ID);
                if (itemExistslock)
                {
                    return BadRequest("unluck");
                }
                _userDataRepository.DeleteunLockID(ID);
                string message = $"{_userDataRepository.getchinesename(ID)}已被解除鎖定";
                await sendEmail.sendemail_id(ID, "解除鎖定通知", message);
                linkline.sendlinenotify(message, "level2");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok();
        }
    }
}