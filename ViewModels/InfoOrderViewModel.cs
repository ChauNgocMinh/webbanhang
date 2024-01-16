﻿using WebBanHang.Models;

namespace WebBanHang.ViewModels
{
    public class InfoOrderViewModel
    {
        public int Status { get; set; }
        public bool PaymentMethod { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string Address { get; set; }
        public float Total { get; set; }
        public List<OrderItem> OrderItem { get; set; } = new();
    }
}
