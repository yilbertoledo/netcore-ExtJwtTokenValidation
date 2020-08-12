using System.Collections.Generic;

namespace ExtJwtTokenValidation.Dto
{
    public class UserDto
    {
        public long Id { get; set; }
        public string EmployeeId { get; set; }
        public string Name { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public ICollection<string> Groups { get; set; }

        public UserDto()
        {
            Groups = new List<string>();
        }
    }
}