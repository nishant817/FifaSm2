using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FifaSMmvc.Models
{
    public class FriendlyReport
    {
        public int PlayerId { get; set; }
        public string PlayerName { get; set; }
        public int Played { get; set; }
        public int Won { get; set; }
        public decimal WinPc { get; set; }
        public int Draw { get; set; }
        public decimal DrawPc { get; set; }
        public int Lost { get; set; }
        public decimal LostPc { get; set; }
        public int GoalShot { get; set; }
        public int GoalFaced { get; set; }
        public decimal AvgGoalPerMatch { get; set; }
        //public float Score { get; set; }
    }
}