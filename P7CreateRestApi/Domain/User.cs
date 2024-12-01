using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Dot.Net.WebApi.Domain
{
    public class User : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
    }
}