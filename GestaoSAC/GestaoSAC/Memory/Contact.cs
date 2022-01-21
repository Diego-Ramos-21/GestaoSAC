namespace GestaoSAC.Memory
{
    public class Contact
    {
        public string id { get; set; }
        public string number { get; set; }
        public string avatar { get; set; }
        public string name { get; set; }
        public string status { get; set; }
        public string carrier { get; set; }
        public string region { get; set; }
        public string gender { get; set; }
        public Channel channel { get; set; }
        public object email { get; set; }
        public SocialMedias social_medias { get; set; }
        public Tag tag { get; set; }
        public Binded binded { get; set; }
        public object observation { get; set; }
        public bool blocked { get; set; }
        public bool portable { get; set; }
        public object portable_carrier { get; set; }
        public object portable_date { get; set; }
        public bool imported { get; set; }
        public object impoted_at { get; set; }
        public string created_at { get; set; }
    }
}
