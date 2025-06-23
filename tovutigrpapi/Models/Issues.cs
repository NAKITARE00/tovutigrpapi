namespace tovutigrpapi.Models
{
    public class Issues
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Issue { get; set; }
        //Foreign Key 
        public int Gadget_Id { get; set; }
    }
}
