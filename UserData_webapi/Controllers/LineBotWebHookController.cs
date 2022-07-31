using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.IO;
using System.Text;
using System.Net.Http;
using Microsoft.VisualBasic;
using Microsoft.CognitiveServices.Speech;
using NAudio.Wave;
using isRock.LineBot;
using Microsoft.CognitiveServices.Speech.Audio;

namespace UserData_webapi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LineBotWebHookController : Controller
    {
        private readonly ILogger<LineBotWebHookController> _logger;
        private readonly IUserDataRepository _userDataRepository;
        private readonly ILineJobRespository _jobRespository;
        private readonly IConfiguration _configuration;
        private readonly IUserLogRepository _userLogRepository;
        private readonly IUserCardRepository _userCardRepository;
        private linkline _linkline;
        private readonly ILineBotManageRespository _lineBotManageRespository;
        public LineBotWebHookController(ILogger<LineBotWebHookController> logger, IUserDataRepository userDataRepistory, ILineJobRespository lineJobRespository, IConfiguration configuration,ILineBotManageRespository lineBotManageRespository, IUserLogRepository userLogRepository,IUserCardRepository userCardRepository)
        {
            _configuration = configuration;
            _jobRespository = lineJobRespository;
            _configuration = configuration;
            _logger = logger;
            _userDataRepository = userDataRepistory;
            _linkline = new linkline(_configuration);
            _lineBotManageRespository = lineBotManageRespository;
            _userLogRepository = userLogRepository;
            _userCardRepository = userCardRepository;
        }
        [HttpGet("userdaylog")]
        public IActionResult getuserdaylog()
        {
            var data1 = _userLogRepository.All.Where(x => x.time.Month == DateTime.Today.Month);
            var data2 = from a in data1
                        join b in _userCardRepository.All on a.UID equals b.UID into c
                        from b in c.DefaultIfEmpty()
                        select new
                        {
                            time = a.time,
                            ID = b.ID

                        };
            var data3 = from a in data2
                        join b in _userDataRepository.All on a.ID equals b.ID
                        select new Userlogdataprint
                        {
                            Name = b.Name,
                            Time = a.time
                        };
            return Ok(data3);
        }
        [HttpGet("fivetime")]
        public IActionResult fivetime()
        {
            try
            {
                if (DateTime.Now.Hour == 21 && DateTime.Now.Minute == 0)
                {
                    timerun();
                }
                else if (DateTime.Now.Hour == 2 && DateTime.Now.Minute == 0)
                {
                    reset();
                    _linkline.sendlinenotify($"{DateTime.Now.ToString()}排程重置了所有人的狀態", "level2");
                }
            }
            catch
            {

            }
            return Ok();
        }
        [HttpGet("reset")]
        public IActionResult reset()
        {
            try
            {
                _userDataRepository.reset();
            }
            catch (Exception ex)
            {
                _linkline.sendlinenotify(ex.ToString(), "level1");
            }

            return Ok();
        }
        [HttpGet]
        public IActionResult timerun()
        {
            try
            {
                _linkline.sendlinenotify($"{DateTime.Now.ToString()}排程查詢了已簽到狀態", "level2");
                _linkline.sendlinenotify(_jobRespository.state(), "level2");
            }
            catch (Exception ex)
            {
                _linkline.sendlinenotify(ex.ToString(), "level1");
            }

            return Ok();
        }
        [HttpGet("lineuser")]
        public IActionResult getlineuser()
        {
            return Ok(_lineBotManageRespository.All);
        }
        [HttpPost("addlineuser")]
        public async Task<IActionResult> adduser([FromBody] LineUser lineUser)
        {
            _lineBotManageRespository.adduser(lineUser);
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> message()
        {
            linkline _linkline = new linkline(_configuration);

            try
            {
                LineUser lineUser = new LineUser();
                var request = "";
                using (StreamReader reader = new StreamReader(Request.Body, System.Text.Encoding.UTF8))
                {
                    request = reader.ReadToEndAsync().Result;
                }
                string postData = Convert.ToString(request);
                var receivedmessage = isRock.LineBot.Utility.Parsing(postData);
                Event item = receivedmessage.events.FirstOrDefault();
                //get JSON Body

                //判斷事件來源
                switch (item.type)
                {
                    //一般訊息
                    case "message":
                        //判斷訊息類型
                        switch (item.message.type)
                        {
                            case "text":
                                judgemessage(item.replyToken, item.message.text, item.source.userId);
                                break;
                            case "audio":
                                string[] audiotext = await _jobRespository.audiototext(item.message.id);
                                for(int i = 0; i < audiotext.Length; i++)
                                {
                                    audiotext[i] = audiotext[i].Trim(new Char[] { '。', '.' });
                                }

                                if (audiotext[0] == null && audiotext[1] == null)
                                {
                                    _jobRespository.sendmessage(
                                    item.replyToken,
                                    $"{item.source.userId}\n使用了語音",
                                    "語音辨識失敗",
                                    "level1");
                                }
                                else if (audiotext[1] != null)
                                {
                                    if (!judgemessage(item.replyToken, audiotext[1],item.source.userId)) { 
                                        if (audiotext[0] != null)
                                        {
                                            _jobRespository.sendmessage(
                                           item.replyToken,
                                           $"{item.source.userId}\n使用了語音",
                                           $"你說了\n{audiotext[0]}\n{audiotext[1]}",
                                           "level1");
                                        }
                                    }
                                }
                                else
                                {
                                    _jobRespository.sendmessage(
                                        item.replyToken,
                                        $"{item.source.userId}\n使用了語音",
                                        $"你說了\n{audiotext[0]}\n{audiotext[1]}",
                                        "level1");
                                }
                                break;
                            default:
                                Thread.Sleep(1000);
                                _linkline.sendlinebot(item.replyToken, "收到的訊息類型為: \n" + receivedmessage.events.FirstOrDefault().message.type);
                                break;
                        }
                        break;
                    //LINE Bot 被用戶家為好友
                    case "follow":
                        lineUser = new LineUser()
                        {
                            ID = item.source.userId,
                        };
                        _lineBotManageRespository.adduser(lineUser);
                        _jobRespository.sendmessage(
                            item.replyToken,
                            $"frc打卡已被 \n{item.source.userId}\n加為好友",
                            $"歡迎{item.source.userId}加入",
                            "level1");
                        break;
                    //LINE Bot 被用戶封鎖
                    case "unfollow":
                        _lineBotManageRespository.deluser(item.source.userId);
                        _linkline.sendlinenotify($"frc打卡已被\n {item.source.userId}\n封鎖", "level1");
                        break;
                    //LINE Bot 被加入聊天室
                    case "join":
                        lineUser = new LineUser()
                        {
                            ID = item.source.groupId,
                        };
                        _lineBotManageRespository.adduser(lineUser);
                        _jobRespository.sendmessage(
                            item.replyToken,
                            $"{(item.source.type == "room" ? item.source.roomId : item.source.groupId)}已被加入{item.source.type.ToString()}中",
                            $"各位好，frc打卡已被加入{item.source.type}中",
                            "level1");
                        break;
                    //LINE Bot 離開聊天室
                    case "leave":
                        _lineBotManageRespository.deluser(item.source.userId);
                        _linkline.sendlinenotify($"frc打卡已被刪除在{(item.source.type == "room" ? item.source.roomId : item.source.groupId)}中，類型是{item.source.type.ToString()}", "level1");
                        break;
                    //LINE Bot 收到 postback 訊息
                    case "postback":
                        _linkline.sendlinenotify("postback", "level1");
                        break;
                    //LINE Bot 收到 Beacon 訊號
                    case "beacon":
                        _linkline.sendlinenotify("beacon", "level1");
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                _linkline.sendlinenotify(ex.ToString(), "level1");
            }
            return Ok();

        }
        private static string ToNarrow(string input)
        {
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 12288)
                {
                    c[i] = (char)32;
                    continue;
                }
                if (c[i] > 65280 && c[i] < 65375)
                    c[i] = (char)(c[i] - 65248);
            }
            return new string(c);
        }
        private bool judgemessage(string Token, string message,string userID)
        {
            string[] textmessage = ToNarrow(message).ToLower().Split(" ");
            //判斷命令
            if (textmessage.Length == 1)    //當只輸入了一個文字時
            {
                switch (textmessage.FirstOrDefault())
                {
                    case "help":
                        _jobRespository.sendmessage(
                            Token,
                            $"{userID}查詢了功能選單",
                            _jobRespository.function(),
                            "level1");
                        break;
                    case "url":
                        _jobRespository.sendmessage(
                            Token,
                            $"{userID}查詢了網站",
                            "https://team8723.azurewebsites.net/",
                            "level1");
                        break;
                    case "data":
                        //if (_lineBotManageRespository.getuserid(userID).Role == "admin"){
                        _jobRespository.sendmessage(
                            Token,
                            $"{userID}\n查詢了個人資料",
                            _jobRespository.listdata(""),
                            "level1");
                        //}
                        //else
                        //{
                            //_jobRespository.sendmessage(
                            //Token,
                            //$"{userID}\n查詢了個人資料，但權限不足",
                            //"您的權限不足，詳情請向管理員洽詢",
                            //"level1");
                        //}
                        break;
                    case "state":
                        //if (_lineBotManageRespository.getuserid(userID).Role == "admin")
                        //{
                        _jobRespository.sendmessage(
                            Token,
                            $"{userID}\n查詢了以簽到狀態",
                            _jobRespository.state(),
                            "level1");
                        //}
                        //else
                        //{
                            //_jobRespository.sendmessage(
                            //    Token,
                            //    $"{userID}\n查詢了個人資料，但權限不足",
                            //    "您的權限不足，詳情請向管理員洽詢",
                            //    "level1");
                        //}
                        break;
                    case "reset":
                        if (_lineBotManageRespository.getuserid(userID).Role == "admin")
                        {
                            reset();
                            _linkline.sendlinenotify($"{Token}重製了狀態", "level1");
                            _jobRespository.sendmessage(
                                Token,
                                $"{userID}\n查詢了以簽到狀態",
                                "以重製了狀態",
                                "level1");
                        }
                        else
                        {
                            _jobRespository.sendmessage(
                                Token,
                                $"{userID}\n查詢了個人資料，但權限不足",
                                "您的權限不足，詳情請向管理員洽詢",
                                "level1");
                        }
                        break;
                    case "lineuserid":
                        //if (_lineBotManageRespository.getuserid(userID).Role == "admin")
                        //{
                            _jobRespository.sendmessage(
                            Token,
                            $"{userID}\n查詢了以line bot的使用者",
                            _lineBotManageRespository.listralluser(),
                            "level1");
                        //}
                        //else
                        //{
                            //_jobRespository.sendmessage(
                            //    Token,
                            //    $"{userID}\n查詢了個人資料，但權限不足",
                            //    "您的權限不足，詳情請向管理員洽詢",
                            //    "level1");
                        //}
                        break;

                    default:
                        return false;
                }
            }
            else if (textmessage.Length == 2)   //當輸入了兩個文字時
            {
                switch (textmessage.FirstOrDefault())
                {
                    case "data":
                            _jobRespository.sendmessage(
                            Token,
                            $"{userID}\n查詢了{textmessage[1]}的個人資料",
                            _jobRespository.listdata(textmessage[1]),
                            "level1");
                        break;
                    case "freeze":
                        if (_lineBotManageRespository.getuserid(userID).Role != "texter")
                        {
                            _jobRespository.sendmessage(
                            Token,
                           $"{userID}\n想要凍結\n{textmessage[1]}\n的個人資料",
                            _jobRespository.freeze(textmessage[1]),
                            "level1");
                        }
                        break;
                    case "disfreeze":
                        if (_lineBotManageRespository.getuserid(userID).Role != "texter")
                        {
                            _jobRespository.sendmessage(
                            Token,
                           $"{userID}\n想要解除凍結\n{textmessage[1]}\n的個人資料",
                            _jobRespository.disfreeze(textmessage[1]),
                            "level1");
                        }
                        else
                        {
                            _jobRespository.sendmessage(
                                Token,
                                $"{userID}\n查詢了個人資料，但權限不足",
                                "您的權限不足，詳情請向管理員洽詢",
                                "level1");
                        }
                        break;
                    default:
                        return false;
                }
            }
            else if (textmessage.Length == 3)
            {

                switch (textmessage.FirstOrDefault())
                {
                    case "changerole":
                        if (_lineBotManageRespository.getuserid(userID).Role == "admin")
                        {
                            _jobRespository.sendmessage(
                           Token,
                           $"{userID}想要更改{textmessage[1]}的角色成{textmessage[2]}\n",
                           _lineBotManageRespository.changeRole(textmessage[1], textmessage[2]),
                           "level1"
                           );
                        }
                        else
                        {
                            _jobRespository.sendmessage(
                                Token,
                                $"{userID}\n查詢了個人資料，但權限不足",
                                "您的權限不足，詳情請向管理員洽詢",
                                "level1");
                        }
                        break;
                    default:
                        return false;
                }
            }
                return true;
        }
    }

}