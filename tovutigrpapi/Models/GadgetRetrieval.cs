namespace tovutigrpapi.Models
{
    public class GadgetRetrieval
    {
        public int GadgetId { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public int Station_Id { get; set; }
        public string StationName { get; set; }
        public int Client_Id { get; set; }
        public string Location { get; set; }
        public string Serial_No { get; set; }
        public string Imei1 { get; set; }
        public string Imei2 { get; set; }
        public List<string>? SparePartNames { get; set; }
        

    }
}
