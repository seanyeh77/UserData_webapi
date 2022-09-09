using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Xml.Linq;

namespace UserData_webapi
{
    public class FaceRespository :IFaceRepository
    {
        private static string personGroupId = "";
        private readonly IConfiguration _configuration;
        IFaceClient client;
        public FaceRespository(IConfiguration configuration)
        {
            _configuration = configuration;
            client = Authenticate(_configuration.GetSection("azureendpoint:faceendpoint").Value, _configuration.GetSection("azurekey:facekey").Value);
            
        }
        private IFaceClient Authenticate(string endpoint, string key)
        {
            return new FaceClient(new ApiKeyServiceClientCredentials(key)) { Endpoint = endpoint };

        }
        public async Task<int> GetFaceDetectAsync(Stream image)
        {
            var faces = await client.Face.DetectWithStreamAsync(image, true, false, recognitionModel: RecognitionModel.Recognition04);
            return faces.Count;
        }
        public async Task<string> CreatePersonGroupAsync(string GroupName)
        {
            string personGroupId = Guid.NewGuid().ToString();
            await client.PersonGroup.CreateAsync(personGroupId, GroupName, GroupName, recognitionModel: RecognitionModel.Recognition04);
            return personGroupId;
        }
        public async Task<IList<PersonGroup>> GetPersonGroupsListAsync()
        {
            return await client.PersonGroup.ListAsync();
        }
        public async Task<Person> CreatePersonGroupPersonAsync(int ID)
        {
            Person person = null;
            IList<Person> person_list = await client.PersonGroupPerson.ListAsync(personGroupId);
            person = person_list.Where(p => p.Name == ID.ToString()).FirstOrDefault();
            if (person == null)
            {
                person = await client.PersonGroupPerson.CreateAsync(personGroupId: personGroupId, name: ID.ToString());
            }
            return person;
        }
        public async Task<PersistedFace> AddPersonGroupPersonFaceAsync(Guid Id,string name ,Stream stream)
        {
            return await client.PersonGroupPerson.AddFaceFromStreamAsync(personGroupId, Id, stream, name);
        }
        public async Task DeletePersonGroupPersonAsync(Guid personId)
        {
            var a = await client.PersonGroupPerson.DeleteWithHttpMessagesAsync(personGroupId: personGroupId, personId);
        }
        public async Task TrainingPersonGroupAsync()
        {
            await client.PersonGroup.TrainAsync(personGroupId);
            // Wait until the training is completed.
            while (true)
            {
                await Task.Delay(1000);
                var trainingStatus = await client.PersonGroup.GetTrainingStatusAsync(personGroupId);
                if (trainingStatus.Status == TrainingStatusType.Succeeded) { break; }
            }
        }
    }
}
