using DevExpress.RichEdit.Export;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryProject.DataAccess.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        [Required]
        public string Username { get; set; }
        [RegularExpression(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$", ErrorMessage = "Use at least 8 characters in combination of uppercase, letters, numbers & symbols.")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }
        [RegularExpression(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$", ErrorMessage = "Use at least 8 characters in combination of uppercase, letters, numbers & symbols.")]
        [Required(ErrorMessage = "New Password is required.")]
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }
        [RegularExpression(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$", ErrorMessage = "Use at least 8 characters in combination of uppercase, letters, numbers & symbols.")]
        [Required(ErrorMessage = "Confirm Password is required.")]
        [DataType(DataType.Password)]
        public string? ConfirmPassword { get; set; }
        public bool IsUpdate { get; set; } = false;
        public string? PasswordSalt { get; set; }
        public DateTime DateCreated { get; set; }
        public int CreatedById { get; set; }
        public DateTime? DateModified { get; set; }
        public int? ModifiedById { get; set; }
        [Required(ErrorMessage = "First Name is required.")]
        public string? FirstName { get; set; }
        [Required(ErrorMessage = "Last Name is required.")]
        public string? LastName { get; set; }
        [Required(ErrorMessage = "Age is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Age must be a positive number.")]
        public int? Age { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        public string? Email { get; set; }
        public bool IsAdmin { get; set; }
        public string? ProfilePicture { get; set; }
        public IFormFile? ProfilePictureFile { get; set; }
        [Required(ErrorMessage = "Phone No. is required.")]
        public int? PhoneNumber { get; set; }

    }
}
