using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Dot.Net.WebApi.Domain
{
    public class User : IdentityUser
    {
        [Key]
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}