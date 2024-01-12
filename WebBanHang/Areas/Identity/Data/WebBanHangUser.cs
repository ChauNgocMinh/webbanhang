using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace WebBanHang.Areas.Identity.Data;

// Add profile data for application users by adding properties to the WebBanHangUser class
public class WebBanHangUser : IdentityUser
{
    [DataType(DataType.Text)]
    [Display(Name = "First Name")]
    public string FirstName { get; set; }


    [DataType(DataType.Text)]
    [Display(Name = "Last Name")]
    public string LastName { get; set; }
}

