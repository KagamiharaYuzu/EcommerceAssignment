using System;
using System.ComponentModel.DataAnnotations;

namespace StoreClassLibrary
{
    public class Search
    {
        public string Term { get; set; }

        [Range(typeof(DateTime), "1/1/2000", "1/1/2023")]
        public DateTime Date { get; set; }
    }
}