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
        public IActionResult count(int ID)
        {
            return Ok(_userCardRepistory.GetCount(ID));
        }

        [HttpGet("{UID}")]
        public IActionResult List(string UID)
        {
            UserCard userCard = _userCardRepistory.FindUIDfreezefalse(UID);
            if (userCard == null)
            {
                return BadRequest("UID");
            }
            int ID = _userCardRepistory.GetID(UID);
            bool itemExistsID = _userDataRepository.DoesItemExistID(ID);
            bool itemExistsfreeze = _userDataRepository.DoesItemExistfreeze(ID);
            if (!itemExistsID)
            {
                return BadRequest("ID");
            }
            if (itemExistsfreeze)
            {
                return BadRequest("freeze");
            }
            return Ok(_userDataRepository.Find(userCard.ID));
        }
        [HttpPost]
        public IActionResult Create([FromBody] UserCard item)
        {
            try
            {
                linkline linkline = new linkline(_configuration);
                if (item == null || !ModelState.IsValid)
                {
                    return BadRequest("null");
                }
                bool itemExistsID = _userDataRepository.DoesItemExistID(item.ID);
                bool itemExistsfreeze = _userDataRepository.DoesItemExistfreeze(item.ID);
                if (!itemExistsID)
                {
                    return BadRequest("ID");
                }
                if (itemExistsfreeze)
                {
                    return BadRequest("freeze");
                }
                
                if (_userCardRepistory.DoesItemExistfreezefalse(item.UID))
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
                linkline.sendlinenotify($"{_userDataRepository.getname(item.ID)}已新增卡片", "level2");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(item);
        }
        [HttpDelete("{UID}")]
        public IActionResult Delete(string UID)
        {
            try
            {
                var item = _userCardRepistory.FindUIDfreezefalse(UID);
                if (item == null)
                {
                    return BadRequest("null");
                }
                int ID = _userCardRepistory.GetID(UID);
                bool itemExistsID = _userDataRepository.DoesItemExistID(ID);
                bool itemExistsfreeze = _userDataRepository.DoesItemExistfreeze(ID);
                if (!itemExistsID)
                {
                    return BadRequest("ID");
                }
                if (itemExistsfreeze)
                {
                    return BadRequest("freeze");
                }
                _userCardRepistory.DeletefreezeUID(UID);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return NoContent();
        }
    }
}