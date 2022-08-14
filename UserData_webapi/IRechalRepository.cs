namespace UserData_webapi
{
    public interface IRechalRepository
    {
        IEnumerable<Rachelstate> All { get; }
        void Update(Rachelstate UID);
        bool getstate();
    }
}
