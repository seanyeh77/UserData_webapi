namespace UserData_webapi
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Cursors
    {
        public string before { get; set; }
        public string after { get; set; }
    }

    public class Datum
    {
        public List<Image> images { get; set; }
        public string id { get; set; }
    }

    public class Image
    {
        public int height { get; set; }
        public string source { get; set; }
        public int width { get; set; }
    }

    public class Paging
    {
        public Cursors cursors { get; set; }
        public string next { get; set; }
    }

    public class Photos
    {
        public List<Datum> data { get; set; }
        public Paging paging { get; set; }
    }

    public class facebookimageclass
    {
        public Photos photos { get; set; }
        public string id { get; set; }
    }
    public class Data
    {
        public string url { get; set; }
    }

    public class Picture
    {
        public Data data { get; set; }
    }

    public class iconurl
    {
        public Picture picture { get; set; }
        public string id { get; set; }
    }
    public class teamabout
    {
        public string about { get; set; }
        public string description { get; set; }
        public string id { get; set; }
    }



}
