using FifaSMmvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace FifaSMmvc.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Friendly()
        {            
            return View();
        }

        public ActionResult FriendlyReports()
        {
            return View();
        }

        public JsonResult GetPlayers()
        {
            var players = FifaRepository.GetAllPlayers();
            if(players != null && players.Count > 0)
            {
                Session["players"] = players;
            }
            var jPlayers = Json(players, JsonRequestBehavior.AllowGet);
            return jPlayers;

            //using (var db = new FsmDBEntities())
            //{
            //    var players = db.Players.Select(p=> new {p.Id, p.Name }).ToList();
            //    var jPlayers = Json(players, JsonRequestBehavior.AllowGet);
            //    return jPlayers;
            //}
        }

        public JsonResult GetFriendlyScores()
        {
            var scores = FifaRepository.GetFriendlyMatchScores();
            if(scores!=null && scores.Count > 0)
            {
                Session["scores"] = scores;
            }
            var jfScores = Json(scores, JsonRequestBehavior.AllowGet);
            return jfScores;

            //using (var db = new FsmDBEntities())
            //{
            //    var fScores = db.FmScores.ToList();
            //    var jfScores = Json(fScores, JsonRequestBehavior.AllowGet);
            //    return jfScores;
            //}
        }

        [HttpPost]
        public JsonResult AddOrUpdateFriendlyScore(FmScore newScore)
        {
            if (newScore.IsUpdate && newScore.Id > 0)
            {
                var scoresAdded = FifaRepository.UpdateFriendlyMatchScore(newScore);
                var jfScores = Json(scoresAdded, JsonRequestBehavior.AllowGet);
                return GetFriendlyScores();
            }
            else
            {
                var scoresAdded = FifaRepository.AddFriendlyMatchScore(newScore);
                var jfScores = Json(scoresAdded, JsonRequestBehavior.AllowGet);
                return GetFriendlyScores();
            }
        }

        public JsonResult GetFriendlyMatchesReports()
        {
            var players = Session["players"] as List<Player>;
            if(players == null || players.Count == 0)
            {
                players = FifaRepository.GetAllPlayers();
                Session["players"] = players;
            }

            var scores = Session["scores"] as List<FmScore>;
            if (scores == null || scores.Count == 0)
            {
                scores = FifaRepository.GetFriendlyMatchScores();
                Session["scores"] = scores;
            }

            var reportsDict = new Dictionary<int, FriendlyReport>();

            if (players != null && players.Count != 0 && scores != null && scores.Count != 0)
            {
                foreach (var pl in players)
                {
                    var fReport = new FriendlyReport();
                    fReport.PlayerId = pl.Id;
                    fReport.PlayerName = pl.Name;
                    fReport.Played = 0;
                    fReport.Won = 0;
                    fReport.Draw = 0;
                    fReport.Lost = 0;
                    fReport.GoalShot = 0;
                    fReport.GoalFaced = 0;

                    reportsDict.Add(pl.Id, fReport);
                }

                foreach (var sc in scores)
                {
                    if (!reportsDict.ContainsKey(sc.Player1) || !reportsDict.ContainsKey(sc.Player2))
                    {
                        continue;
                    }

                    reportsDict[sc.Player1].Played++;
                    reportsDict[sc.Player2].Played++;

                    reportsDict[sc.Player1].GoalShot += sc.Goals1;
                    reportsDict[sc.Player2].GoalShot += sc.Goals2;

                    reportsDict[sc.Player1].GoalFaced += sc.Goals2;
                    reportsDict[sc.Player2].GoalFaced += sc.Goals1;

                    if (sc.Goals1 > sc.Goals2)
                    {
                        reportsDict[sc.Player1].Won++;
                        reportsDict[sc.Player2].Lost++;
                    }
                    else if (sc.Goals1 < sc.Goals2)
                    {
                        reportsDict[sc.Player1].Lost++;
                        reportsDict[sc.Player2].Won++;
                    }
                    else
                    {
                        reportsDict[sc.Player1].Draw++;
                        reportsDict[sc.Player2].Draw++;
                    }
                }

                foreach (var pl in players)
                {
                    if(reportsDict[pl.Id].Played == 0)
                    {
                        reportsDict[pl.Id].WinPc = 0;
                        reportsDict[pl.Id].LostPc = 0;
                        reportsDict[pl.Id].DrawPc = 0;
                        reportsDict[pl.Id].PointsPerMatch = 0;
                        continue;
                    }
                    reportsDict[pl.Id].WinPc = Math.Round((decimal)(reportsDict[pl.Id].Won * 100 / reportsDict[pl.Id].Played), 2);
                    reportsDict[pl.Id].LostPc = Math.Round((decimal)(reportsDict[pl.Id].Lost * 100 / reportsDict[pl.Id].Played), 2);
                    reportsDict[pl.Id].DrawPc = Math.Round((decimal)(reportsDict[pl.Id].Draw * 100 / reportsDict[pl.Id].Played), 2);

                    reportsDict[pl.Id].PointsPerMatch = Math.Round(((decimal)(3*reportsDict[pl.Id].Won + reportsDict[pl.Id].Draw)/ reportsDict[pl.Id].Played), 2);
                }
            }

            var reports = reportsDict.Values.ToList();

            var jfReports = Json(reports, JsonRequestBehavior.AllowGet);
            return jfReports;
        }
    }
}