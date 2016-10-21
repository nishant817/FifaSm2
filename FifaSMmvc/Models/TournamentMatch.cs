using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FifaSMmvc.Models
{
    public class TournamentMatch
    {
        public int Id { get; set; }
        public int TId { get; set; }
        public string MType { get; set; }
        public int P1 { get; set; }
        public string T1 { get; set; }
        public int G1 { get; set; }
        public int P2 { get; set; }
        public string T2 { get; set; }
        public int G2 { get; set; }
        public string Details { get; set; }
    }
}