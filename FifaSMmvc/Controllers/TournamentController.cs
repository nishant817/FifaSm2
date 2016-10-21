using FifaSMmvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FifaSMmvc.Controllers
{
    public class TournamentController : Controller
    {
        public ActionResult Tournament()
        {
            return View();
        }

        public JsonResult GetTournaments()
        {
            var tournaments = FifaRepository.GetTournaments();
            //if (tournaments != null && tournaments.Count > 0)
            //{
            //    Session["tournaments"] = tournaments;
            //}
            var jPlayers = Json(tournaments, JsonRequestBehavior.AllowGet);
            return jPlayers;
        }

        public JsonResult GetTournamentMatches(int tournamentId)
        {
            var matches = FifaRepository.GetTournamentMatches(tournamentId);
            var jMatches = Json(matches, JsonRequestBehavior.AllowGet);
            return jMatches;
        }

        [HttpPost]
        public JsonResult AddTournament(Tournament tournamentToAdd)
        {
            var id = FifaRepository.AddTournament(tournamentToAdd);
            
            return GetTournaments();
        }

        [HttpPost]
        public JsonResult AddTournamentMatch(TournamentMatch matchToAdd)
        {
            var id = FifaRepository.AddTournamentMatch(matchToAdd);

            return GetTournamentMatches(matchToAdd.TId);
        }

        [HttpPost]
        public JsonResult AddTournamentResult(Tournament selectedTournament)
        {
            var id = FifaRepository.AddTournamentResult(selectedTournament);

            return GetTournaments();
        }
    }
}