using System;
using System.Collections.Generic;

namespace ShortLink.sakila
{
    public partial class Linktable
    {
        public Linktable()
        {
            Log = new HashSet<Log>();
        }

        public int Id { get; set; }
        public string Longlink { get; set; }
        public string Shortlink { get; set; }
        public DateTime? Date { get; set; }
        public sbyte? Deleted { get; set; }

        public virtual ICollection<Log> Log { get; set; }
    }
}
