namespace UserData_webapi
{
    interface IFaceRepository
    {
        public void CreateFaceSet();
        public void AddFaceAsync();
        public void RemoveFaceAsync();
    }
}
