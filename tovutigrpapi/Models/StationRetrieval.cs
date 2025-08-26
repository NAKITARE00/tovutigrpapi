namespace tovutigrpapi.Models
{
    public class StationRetrieval
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Station { get; set; }
        public string Location { get; set; }
        public DateTime Date_Created { get; set; }
        public string Status { get; set; }
        // Client info
        public int Client_Id { get; set; }
        public string ClientName { get; set; }
        public string ClientCountry { get; set; }
    }

}
