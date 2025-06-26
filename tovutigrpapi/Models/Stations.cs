namespace tovutigrpapi.Models

{
    public class Stations
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Station { get; set; }
        public string Location { get; set; }

        // Foreign Key
        public int Client_Id { get; set; }
    }

}
