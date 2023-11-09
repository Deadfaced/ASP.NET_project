using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace MvcBlog.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the MvcBlogUser class
    public class MvcBlogUser : IdentityUser
    {
        [PersonalData]
        [Required(ErrorMessage = "First Name is required.")]
        [StringLength(20, ErrorMessage = "First Name cannot be longer than 20 characters.")]
        [RegularExpression(
            @"^[a-zA-Z]+$",
            ErrorMessage = "First Name can only contain alphabetic characters."
        )]
        [Column(TypeName = "nvarchar(20)")]
        public string FirstName { get; set; }

        [PersonalData]
        [Required(ErrorMessage = "Last Name is required.")]
        [StringLength(20, ErrorMessage = "Last Name cannot be longer than 20 characters.")]
        [RegularExpression(
            @"^[a-zA-Z]+$",
            ErrorMessage = "Last Name can only contain alphabetic characters."
        )]
        [Column(TypeName = "nvarchar(20)")]
        public string LastName { get; set; }
    }
}
