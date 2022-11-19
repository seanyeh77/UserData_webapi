using isRock.LineBot;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using NAudio.Wave;

namespace UserData_webapi
{
    public class LineJobRepository :ILineJobRespository
    {
        string Message = "";
        private readonly IUserDataRepository _userDataRepository;
        private readonly IHostEnvironment _environment;
        private readonly IConfiguration _configuration;
        private readonly IUserLogRepository _userLogRepository;
        private linkline _linkline;
        public LineJobRepository(IUserDataRepository userDataRepository, IHostEnvironment environment, IConfiguration configuration,IUserLogRepository userLogRepository)
        {
            _userDataRepository = userDataRepository;
            _environment = environment;
            _configuration = configuration;
            _userLogRepository = userLogRepository;
            _linkline = new linkline(_configuration);
        }
         
        public string function()
        {
            Message = "===系統管理員級命令===\n";
            Message += "[reset]-->重置所有人狀態\n";
            Message += "[lineuserid]-->列出Line Bot的朋友ID\n";
            Message += "[changerole ID 角色]-->更改成員權限(角色:admin manager tester)\n";
            Message += "===管理員級命令===\n";
            Message += "[lock 學號]-->鎖定成員\n";
            Message += "[unlock 學號]-->解除鎖定成員\n";
            Message += "===一般成員級命令===\n";
            Message += "[help]-->查詢功能選單\n";
            Message += "[url]-->查詢管理網站\n";
            Message += "[website]-->查詢資料網站\n";
            Message += "[state]-->查詢成員以簽到狀態\n";
            Message += "[data (學號)]-->查詢成員資料\n";
            return Message;
        }
        public string listdata(string ID)
        {
            if (string.IsNullOrEmpty(ID))
            {
                if (_userDataRepository.All.Count()==0)
                {
                    Message = "沒有找到任何人的個人資料\n";
                }
                else
                {
                    Message = "學號      姓名　 鎖定 狀態\n";
                    foreach (UserData data in _userDataRepository.All)
                    {
                        Message += $"{data.ID}  {data.ChineseName}  {(data.Lock?"是":"否")}   {(data.state ? "在教室":"離開")}\n";
                    }
                }
            }
            else
            {
                bool itemExistsID = _userDataRepository.DoesItemExistID(ID);
                if (!itemExistsID)
                {
                    Message = $"沒有找到\n{ID}\n用戶";
                }
                else
                {
                    UserData userData = _userDataRepository.FindID(ID);
                    Message = "學號      姓名　 鎖定 狀態\n";
                    Message += $"{userData}  {userData.ChineseName}  {(userData.Lock ? "是":"否")}   {(userData.state ? "在教室":"離開")}\n";
                }
                
            }
            return Message;
        }
        public string Lock(string ID)
        {
            var data = _userDataRepository.FindID(ID);
            if (data == null)
            {
                Message = $"沒有找到{ID}用戶";
            }
            bool itemExistslock = _userDataRepository.DoesItemExistlock(data.ID);
            if (itemExistslock)
            {
                Message = $"{ID}\n已經被鎖定";
            }
            else
            {
                _userDataRepository.DeletelockID(ID);
                Message = $"{ID}\n已被鎖定";
                _linkline.sendlinenotify(Message, "level2");
            }
            return Message;
        }
        public string unLock(string ID)
        {
            var data = _userDataRepository.FindID(ID);
            if (data == null)
            {
                Message = "沒有找到此用戶";
            }
            bool itemExistslock = _userDataRepository.DoesItemExistlockfalse(data.ID);
            if (itemExistslock)
            {
                Message = $"{ID}\n沒有被鎖定";
            }
            else
            {
                _userDataRepository.DeleteunLockID(ID);
                Message = $"{ID}\n已被解除鎖定";
                _linkline.sendlinenotify(Message, "level2");
            }
            return Message;
        }
        public async Task<string[]> audiototext(string ID)
        {
            string ChienceMessage = "";
            string EnglishtMessage = "";
            string mp3_dir = Path.Combine(_environment.ContentRootPath, "mp3");
            if (Directory.Exists(mp3_dir) == false)
                Directory.CreateDirectory(mp3_dir);
            string wav_dir = Path.Combine(_environment.ContentRootPath, "wav");
            if (Directory.Exists(wav_dir) == false)
                Directory.CreateDirectory(wav_dir);
            var filenamemp3 = Path.Combine(mp3_dir, Guid.NewGuid().ToString() + ".mp3");
            var filenamewav = Path.Combine(wav_dir, Guid.NewGuid().ToString() + ".wav");
            var fileBody = _linkline.getaudiobytes(ID);
            File.WriteAllBytes(filenamemp3, fileBody);
            //將 AAC 轉成 WAV
            using (var reader = new MediaFoundationReader(filenamemp3))
            using (var pcmStream = new WaveFormatConversionStream(
                //WAV 需為 16KHz
                new WaveFormat(16000, 1), reader))
            {
                WaveFileWriter.CreateWaveFile(filenamewav, pcmStream);
            }
            var _speechConfig = SpeechConfig.FromSubscription(_configuration.GetValue<string>("line:speechKey"), _configuration.GetValue<string>("line:speechDomain"));
            //設定語言為繁體中文
            _speechConfig.SpeechRecognitionLanguage = "zh-TW";

            //設定語音名稱
            //zh-TW-Yating-Apollo、zh-TW-HanHanRUS、zh-TW-Zhiwei-Apollo
            _speechConfig.SpeechSynthesisVoiceName = "zh-TW-Yating-Apollo";

            //將語音轉為文字
            using (var audioInput = AudioConfig.FromWavFileInput(filenamewav))
            using (var recognizer = new SpeechRecognizer(_speechConfig, audioInput))
            {
                //語音超過 15 秒需改用 StartContinuousRecognitionAsync 方法
                var result = await recognizer.RecognizeOnceAsync();

                //識別成功
                if (result.Reason == ResultReason.RecognizedSpeech)
                {
                    ChienceMessage = result.Text;
                }
                //識別失敗
                else
                {
                    ChienceMessage = null;
                }
            }
            //設定語言為英文
            _speechConfig.SpeechRecognitionLanguage = "en-US";

            //設定語音名稱
            //zh-TW-Yating-Apollo、zh-TW-HanHanRUS、zh-TW-Zhiwei-Apollo
            _speechConfig.SpeechSynthesisVoiceName = "en-US-JennyNeural";
            using (var audioInput = AudioConfig.FromWavFileInput(filenamewav))
            using (var recognizer = new SpeechRecognizer(_speechConfig, audioInput))
            {
                //語音超過 15 秒需改用 StartContinuousRecognitionAsync 方法
                var result = await recognizer.RecognizeOnceAsync();

                //識別成功
                if (result.Reason == ResultReason.RecognizedSpeech)
                {
                    EnglishtMessage = result.Text;
                }
                //識別失敗
                else
                {
                    EnglishtMessage = null;
                }
            }
            string[] message = { ChienceMessage, EnglishtMessage };
            return message;
        }
        public string state()   //  查詢狀態
        {
            int countperosn = _userDataRepository.All.Count(x => x.state);
            if (countperosn == 0)
            {
                Message = "沒有找到任何人狀態為在教室";
            }
            else
            {
                Message = $"簽到人數:{countperosn}人\n";
                Message = "學號      姓名　 鎖定 狀態\n";
                foreach (UserData data in _userDataRepository.All.Where(x => x.state))
                {
                    Message += $"{data.ID}  {data.ChineseName}  {(data.Lock ? "是" : "否")}   {(data.state ? "在教室" : "離開")}\n";
                }
            }
            return Message;
            
        }
        public void sendmessage(string Token, string notify,string bot,string level)
        {
            _linkline.sendlinenotify(notify, level);
            _linkline.sendlinebot(Token, bot);
        }
    }
}
