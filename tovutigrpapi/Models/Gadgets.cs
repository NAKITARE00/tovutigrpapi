namespace tovutigrpapi.Models
{
    public class Gadgets
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; } 
        public string Serial_No { get; set; }
        public string Imei1 { get; set; }
        public string Imei2 { get; set; }

        //Foreign keys
        public int Station_Id { get; set; }
        public List<int>? SparePartIds { get; set; } = new List<int>();
    }
}