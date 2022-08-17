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
        private readonly IFaceRepository _faceRepository;
        public UserDataController(ILogger<UserDataController> logger, IUserDataRepository userDataRepistory, IUserCardRepository userCardRepistory , IConfiguration configuration, IFaceRepository faceRepository)
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
        [HttpGet("{ID}")]
        public IActionResult List(int ID)
        {
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
            return Ok(_userDataRepository.Find(ID));
        }
        [HttpGet("GetPersonGroupsListAsync")]
        public async Task<IActionResult> GetPersonGroupsListAsync()
        {
            try
            {

                return Ok(await _faceRepository.GetPersonGroupsListAsync());

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("persongroup/{GroupName}")]
        public async Task<IActionResult> CreatepersongroupID(string GroupName)
        {
            try
            {
                string personGroupId = await _faceRepository.CreatePersonGroupAsync(GroupName);
                return Ok(personGroupId);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserData item)
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
                    foreach (var stream in item.Image)
                    {
                        if (await _faceRepository.GetFaceDetectAsync(stream.OpenReadStream()) != 1)
                        {
                            return BadRequest("image");
                        }
                    }
                    await _userDataRepository.Insert(item);
                    linkline.sendlinenotify($"{_userDataRepository.getchinesename(item.ID)}已註冊個人資料", "level2");
                }
                ;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(item);
        }
        [HttpPut]
        public IActionResult Edit([FromBody] UserData item)
        {
            try
            {
                bool itemExistsID = _userDataRepository.DoesItemExistID(item.ID);
                if (itemExistsID)
                {
                    _userDataRepository.Update(item);
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
            return NoContent();
        }
        [HttpDelete("{ID}")]
        public IActionResult Delete(int ID)
        {
            try
            {
                bool itemExistsID = _userDataRepository.DoesItemExistID(ID);
                if (itemExistsID)
                {
                    _userDataRepository.Delete(ID);
                    _userCardRepistory.DeleteID(ID);
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
            return NoContent();
        }
        [HttpDelete("freeze/{ID}")]
        public IActionResult freeze(int ID)
        {
            try
            {
                var item = _userDataRepository.FindID(ID);
                if (item == null)
                {
                    return BadRequest("ID");
                }
                bool itemExistsfreeze = _userDataRepository.DoesItemExistfreeze(item.ID);
                if( itemExistsfreeze)
                {
                    return BadRequest("freeze");
                }
                _userDataRepository.DeletefreezeID(ID);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return NoContent();
        }
        [HttpDelete("disfreeze/{ID}")]
        public IActionResult disfreeze(int ID)
        {
            try
            {
                var item = _userDataRepository.FindID(ID);
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
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return NoContent();
        }
    }
}