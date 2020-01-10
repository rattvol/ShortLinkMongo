using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShortLink.sakila
{
    public partial class Fulltable
    {

        public int Id { get; set; }
        public string Longlink { get; set; }
        public string Shortlink { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? Date { get; set; }
        public sbyte? Deleted { get; set; }
        public int? Count { get; set; }

    }
}
