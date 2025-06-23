namespace tovutigrpapi.Models
{
    public class IssueRetrieval
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Issue { get; set; }
        public int Gadget_Id { get; set; }
        public string GadgetName { get; set; } = string.Empty;
    }
}
