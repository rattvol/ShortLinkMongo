using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShortLink.sakila
{
    public partial class Log
    {
        [Key]
        public int IdLink { get; set; }
        public int? Count { get; set; }

        public virtual Linktable IdLinkNavigation { get; set; }
    }
}
