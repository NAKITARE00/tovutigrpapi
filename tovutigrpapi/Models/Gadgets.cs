namespace tovutigrpapi.Models
{
    public class Gadgets
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; } 

        //Foreign keys
        public int Station_Id { get; set; }
      


    }
}