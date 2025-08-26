namespace tovutigrpapi.Models
{
    public class Users
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? HashPassword { get; set; }
        public string? Salt { get; set; }
        public string? Email { get; set; }

        public int RoleId { get; set; }
        public string? UserType { get; set; }  
        public string? RoleName { get; set; }
        public int ClientId { get; set; }
        public int StationId { get; set; }

        public string? ClientName { get; set; }
        public string? StationName { get; set; }
        public string? StationLocation { get; set; }
    }

    public class Role
    {
        public int RoleId { get; set; }
        public string UserType { get; set; }
        public string RoleName { get; set; }
        public ICollection<Users> Users { get; set; }
    }
}

