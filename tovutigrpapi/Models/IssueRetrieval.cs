namespace tovutigrpapi.Models
{
    public class IssueRetrieval
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Issue { get; set; }

        public string Recommendations { get; set; }
        public int Gadget_Id { get; set; }
        public string Gadget_Name { get; set; } = string.Empty;

        public int Station_Id { get; set; }
        public string Station_Name { get; set; }
        public string Status { get; set; }
    }
}
