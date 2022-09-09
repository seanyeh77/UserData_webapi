using Microsoft.Azure.CognitiveServices.Vision.Face.Models;

namespace UserData_webapi
{
    public interface IFaceRepository
    {
        /// <summary>
        /// 偵測照片並取得face_toke
        /// </summary>
        /// <param name="formFile"></param>
        /// <returns></returns>
        public Task<List<string>> DetictFace(IFormFile formFile);
        ///// <summary>
        ///// 新建群組
        ///// </summary>
        ///// <returns></returns>
        //public Task<int> CreateFaceSet();
        /// <summary>
        /// 新增照片，回傳成功數量
        /// </summary>
        /// <param name="GroupName"></param>
        /// <returns></returns>
        public Task<int> AddFaceAsync(List<string> face_tokens);
        public Task<int> RemoveFaceAsync(List<string> face_tokens);
        public Task<List<SearchUser>> SearchUser(List<string> face_tokes);
    }
}
