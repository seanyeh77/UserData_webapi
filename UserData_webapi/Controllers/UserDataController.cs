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
                    string message = $"{_userDataRepository.getchinesename(userdata.ID)}���\���U�ӤH���\n���\�[�J�F{count}�i�Ӥ�";
                    await sendEmail.sendemail_id(userdata.ID, "���\���U�q��", message);
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
                    case 0://�䤣��H�y
                        return BadRequest("face");
                        break;
                    case 1://���H�y���䤣��H
                        return BadRequest("people");
                        break;
                    case 2://���H�y�]���H
                        return Ok(userdata.Item1);
                        break;
                    case 3://�PFace++�s�u�ɥX�{���D
                        return BadRequest("connet");
                        break;
                    default:
                        return BadRequest();//�S��
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
                    string message = $"{_userDataRepository.getchinesename(userdata.ID)}���\�ק�F�ӤH���";
                    await sendEmail.sendemail_id(userdata.ID, "���\�ק�q��", message);
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
                string message = $"{_userDataRepository.getchinesename(ID)}���\�R���F�ӤH���";
                await sendEmail.sendemail_id(ID, "���\�R���q��", message);
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
                string message = $"{_userDataRepository.getchinesename(ID)}�w�Q��w";
                await sendEmail.sendemail_id(ID, "��w�q��", message);
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
                string message = $"{_userDataRepository.getchinesename(ID)}�w�Q�Ѱ���w";
                await sendEmail.sendemail_id(ID, "�Ѱ���w�q��", message);
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