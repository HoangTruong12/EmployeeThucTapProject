using System;
using System.ComponentModel.DataAnnotations;

namespace EmployeeWebApp.Models
{
    public class Employee
    {
        [Key]
        // [Display(Name = "Mã")]
        public Guid Id { get; set; }

        [Required]
        // [Display(Name = "Tài khoản")]
        [MaxLength(100, ErrorMessage = "Username can only be 100 characters long")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email is required")]
        // [StringLength(16, ErrorMessage = "Must be between 5 and 50 characters", MinimumLength = 5)]
        [RegularExpression("^[a-zA-Z0-9_.-]+@[a-zA-Z0-9-]+.[a-zA-Z0-9-.]+$", ErrorMessage = "Must be a valid Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        // [Display(Name = "Password")]
        [RegularExpression("^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])|(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[^a-zA-Z0-9])|(?=.*?[A-Z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])|(?=.*?[a-z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])).{8,}$", ErrorMessage = "Passwords must be at least 8 characters and contain at 3 of 4 of the following: upper case (A-Z), lower case (a-z), number (0-9) and special character (e.g. !@#$%^&*)")]
        public string Password { get; set; }

        [Required]
        // [Display(Name = "Ngày sinh")]
        public string Birthday { get; set; }

        [Required]
        // [Display(Name = "Địa chỉ")]
        [MaxLength(500)]
        public string Address { get; set; }

        [Required(ErrorMessage = "You must provide a phone number")]
        // [Display(Name = "Số điện thoại")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        public string PhoneNumber { get; set; }

        // [Display(Name = "Thời gian tạo")]
        public DateTime CreatedAt { get; set; }

        // [Display(Name = "Thời gian cập nhật")]
        public DateTime UpdatedAt { get; set; }
    }
}
