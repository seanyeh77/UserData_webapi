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
            _faceRepository = faceRepository;
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
                bool itemExistsID = _userDataRepository.DoesItemExistID(userdata.ID);
                bool itemExistsfreeze = _userDataRepository.DoesItemExistfreeze(userdata.ID);
                if (itemExistsID)
                {
                    if (itemExistsfreeze)
                    {
                        return BadRequest("freeze");
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
                bool itemExistsfreeze = _userDataRepository.DoesItemExistfreeze(userdata.ID);
                if (itemExistsID)
                {
                    if (itemExistsfreeze)
                    {
                        return BadRequest("freeze");
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
        [HttpDelete("freeze/{ID}")]
        public async Task<IActionResult> freeze(string ID)
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
                bool itemExistsfreeze = _userDataRepository.DoesItemExistfreeze(item.ID);
                if (itemExistsfreeze)
                {
                    return BadRequest("freeze");
                }
                _userDataRepository.DeletefreezeID(ID);
                string message = $"{_userDataRepository.getchinesename(ID)}�w�Q�ᵲ";
                await sendEmail.sendemail_id(ID, "�ᵲ�q��", message);
                linkline.sendlinenotify(message, "level2");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok();
        }
        [HttpDelete("disfreeze/{ID}")]
        public async Task<IActionResult> disfreeze(string ID)
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
                bool itemExistsfreeze = _userDataRepository.DoesItemExistfreezefalse(item.ID);
                if (itemExistsfreeze)
                {
                    return BadRequest("disfreeze");
                }
                _userDataRepository.DeletedisfreezeID(ID);
                string message = $"{_userDataRepository.getchinesename(ID)}�w�Q�Ѱ��ᵲ";
                await sendEmail.sendemail_id(ID, "�Ѱ��ᵲ�q��", message);
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