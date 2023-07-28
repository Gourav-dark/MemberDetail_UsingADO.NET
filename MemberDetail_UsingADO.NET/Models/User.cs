using System.ComponentModel.DataAnnotations;

namespace MemberDetail_UsingADO.NET.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="Name Field is Required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Phone Number is Required")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Phone Number should be 10 characters")]
        public string PhoneNumber { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required(ErrorMessage = "Address is Required")]
        public string Address { get; set; }
        [Required]
        public IFormFile Image { get; set; }
        public string ImageUrl { get; set; }
    }
}
