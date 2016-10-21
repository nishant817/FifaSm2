using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FifaSMmvc.Models
{
    public class Tournament
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public int R1Id { get; set; }
        public int R2Id { get; set; }
        public int R3Id { get; set; }
        public int R4Id { get; set; }
        public int R5Id { get; set; }
        public int R6Id { get; set; }
        public int R7Id { get; set; }
        public string Details { get; set; }
        public bool HasResult { get; set; }
    }
}