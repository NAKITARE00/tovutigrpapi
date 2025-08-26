namespace tovutigrpapi.Models
{
    public class SparePart
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Cost { get; set; }
        public string Status { get; set; }

        public int Station_Id { get; set; }
        public string? Station_Name { get; set; }
        public List<string>? LinkedGadgetNames { get; set; }


    }
}
