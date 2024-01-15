using System;
using System.Collections.Generic;

namespace WebBanHang.Models
{
    public partial class Rom
    {
        public Rom()
        {
            DetailRoms = new HashSet<DetailRom>();
        }

        public Guid Id { get; set; }
        public int? Capacity { get; set; }

        public virtual ICollection<DetailRom> DetailRoms { get; set; }
    }
}
