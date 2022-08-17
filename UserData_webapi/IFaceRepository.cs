using Microsoft.Azure.CognitiveServices.Vision.Face.Models;

namespace UserData_webapi
{
    public interface IFaceRepository
    {
        /// <summary>
        /// 取得照片中臉的數量
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public Task<int> GetFaceDetectAsync(Stream stream);
        /// <summary>
        /// 新建人臉群組
        /// </summary>
        /// <param name="GroupName"></param>
        /// <returns></returns>
        public Task<string> CreatePersonGroupAsync(string GroupName);
        public Task<IList<PersonGroup>> GetPersonGroupsListAsync();
        /// <summary>
        /// 新增群組成員
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public Task<Person> CreatePersonGroupPersonAsync(int ID);
        /// <summary>
        /// 新增成員臉部照片
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="name"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        public Task<PersistedFace> AddPersonGroupPersonFaceAsync(Guid Id,string name, Stream stream);
        /// <summary>
        /// 刪除群組成員
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        public Task DeletePersonGroupPersonAsync(Guid personId);
        /// <summary>
        /// 訓練群組模型
        /// </summary>
        /// <returns></returns>
        public Task TrainingPersonGroupAsync();
    }
}
