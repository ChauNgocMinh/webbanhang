﻿using System.ComponentModel.DataAnnotations;

#nullable disable

namespace WebBanHang.ViewModels
{
    public class RoleVM
    {
        [Required]
        public string RoleId { get; set; }
        [Required]
        public string RoleName { get; set; }
    }
}
