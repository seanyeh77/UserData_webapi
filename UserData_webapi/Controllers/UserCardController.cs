using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.IO;
using System.Text;
namespace UserData_webapi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserCardController : ControllerBase
    {
        private readonly ILogger<UserCardController> _logger;
        private readonly IUserCardRepository _userCardRepistory;
        private readonly IUserDataRepository _userDataRepository;
        private readonly IConfiguration _configuration;
        public UserCardController(ILogger<UserCardController> logger, IUserCardRepository userCardRepistory, IUserDataRepository userDataRepistory, IConfiguration configuration)
        {
            _logger = logger;
            _userCardRepistory = userCardRepistory;
            _userDataRepository = userDataRepistory;
            _configuration = configuration;
        }
        [HttpGet]
        public IActionResult List()
        {
            return Ok(_userCardRepistory.All);
        }
        [HttpGet("cardcount/{ID}")]
        public IActionResult count(string ID)
        {
            return Ok(_userCardRepistory.GetCount(ID));
        }

        [HttpGet("{UID}")]
        public IActionResult List(string UID)
        {
            UserCard userCard = _userCardRepistory.FindUIDlockfalse(UID);
            if (userCard == null)
            {
                return BadRequest("UID");
            }
            string ID = _userCardRepistory.GetID(UID);
            bool itemExistsID = _userDataRepository.DoesItemExistID(ID);
            bool itemExistslock = _userDataRepository.DoesItemExistlock(ID);
            if (!itemExistsID)
            {
                return BadRequest("ID");
            }
            if (itemExistslock)
            {
                return BadRequest("lock");
            }
            return Ok(_userDataRepository.Find(userCard.ID));
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserCard item)
        {
            try
            {
                linkline linkline = new linkline(_configuration);
                SendEmail sendEmail = new SendEmail(_configuration, _userDataRepository);
                if (item == null || !ModelState.IsValid)
                {
                    return BadRequest("null");
                }
                bool itemExistsID = _userDataRepository.DoesItemExistID(item.ID);
                bool itemExistslock = _userDataRepository.DoesItemExistlock(item.ID);
                if (!itemExistsID)
                {
                    return BadRequest("ID");
                }
                if (itemExistslock)
                {
                    return BadRequest("lock");
                }
                
                if (_userCardRepistory.DoesItemExistlockfalse(item.UID))
                {
                    return BadRequest("UID");
                }
                else if (_userCardRepistory.DoesItemExistUID(item.UID))
                {
                    _userCardRepistory.Update(item);
                }
                else
                {
                    _userCardRepistory.Insert(item);
                }
                string message = $"{_userDataRepository.getchinesename(item.ID)}已新增卡片";
                await sendEmail.sendemail_id(item.ID, "卡片新增成功通知", message);
                linkline.sendlinenotify(message, "level1");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(item);
        }
        [HttpDelete("{UID}")]
        public async Task<IActionResult> Delete(string UID)
        {
            try
            {
                linkline linkline = new linkline(_configuration);
                SendEmail sendEmail = new SendEmail(_configuration, _userDataRepository);
                UserCard item = _userCardRepistory.FindUIDlockfalse(UID);
                if (item == null)
                {
                    return BadRequest("null");
                }
                string ID = item.ID;
                bool itemExistsID = _userDataRepository.DoesItemExistID(ID);
                bool itemExistslock = _userDataRepository.DoesItemExistlock(ID);
                if (!itemExistsID)
                {
                    return BadRequest("ID");
                }
                if (itemExistslock)
                {
                    return BadRequest("lock");
                }
                _userCardRepistory.DeletelockUID(UID);
                string message = $"{_userDataRepository.getchinesename(item.ID)}已刪除卡片";
                await sendEmail.sendemail_id(ID, "卡片成功刪除通知", message);
                linkline.sendlinenotify(message, "level1");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok();
        }
    }
}