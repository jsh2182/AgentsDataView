using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace AgentsDataView.Entities.DtoModels
{
    public class LoginRequest
    {
        [Required]
        public required string Username { get; set; }

        [Required]
        [SwaggerSchema(Format = "password")]
        public  required string Password { get; set; }
    }
}
