using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace FifaSMmvc.Models
{
    public class FifaRepository
    {
        public static string connStrName = ConfigurationManager.AppSettings["connStrName"].ToString();
        public static string connStr = ConfigurationManager.ConnectionStrings[connStrName].ToString();
        //public static SqlConnection conn = new SqlConnection(connStr);

        public static List<Player> GetAllPlayers()
        {
            List<Player> players = new List<Player>();
            DataTable dt = new DataTable();
            string sql = "select * from players";
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    conn.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                    foreach (DataRow dr in dt.Rows)
                    {
                        var pl = new Player();
                        pl.Id = Convert.ToInt32(dr[0]);
                        pl.Name = dr[1].ToString();
                        players.Add(pl);
                    }                    
                }
            }
            return players;
        }

        public static List<FmScore> GetFriendlyMatchScores()
        {
            List<FmScore> fmScores = new List<FmScore>();
            DataTable dt = new DataTable();
            var sql = "select * from FmScores order by Id desc";

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    conn.Open();
                                        
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                    foreach (DataRow dr in dt.Rows)
                    {
                        var score = new FmScore();
                        score.Id = Convert.ToInt32(dr[0]);
                        if (dr[1] != DBNull.Value)
                        {
                            score.MatchDt = Convert.ToDateTime(dr[1]);
                        }
                        score.Player1 = Convert.ToInt32(dr["Player1"]);
                        score.Team1 = Convert.ToInt32(dr[3]);
                        score.Goals1 = Convert.ToInt32(dr[4]);
                        score.Player2 = Convert.ToInt32(dr["Player2"]);
                        score.Team2 = Convert.ToInt32(dr[6]);                        
                        score.Goals2 = Convert.ToInt32(dr[7]);
                        fmScores.Add(score);
                    }
                }
            }

            return fmScores;
        }

        internal static object GetFriendlyReports(List<Player> players)
        {
            throw new NotImplementedException();
        }

        public static int AddFriendlyMatchScore(FmScore newScore)
        {
            //if (scores == null || scores.Length == 0) { return -1; }
            if (newScore == null) return -1;

            StringBuilder sqlSb = new StringBuilder();
            sqlSb.Append("Insert into FmScores (MatchDt, Player1, Team1, Goals1, Player2, Team2, Goals2) values ");

            int count = 0;
            //for (int i = 0; i < scores.Length; i++)
            //{
            if (newScore.Player1 > 0 && newScore.Player2 > 0 && newScore.Goals1 >= 0 && newScore.Goals2 >= 0)
            {
                if (count != 0)
                {
                    sqlSb.Append(", ");
                }
                if (!newScore.MatchDt.HasValue)
                    newScore.MatchDt = DateTime.Now;
                sqlSb.Append(string.Format("('{0}', {1}, '{2}', {3}, {4}, '{5}', {6})", newScore.MatchDt, newScore.Player1, newScore.Team1, newScore.Goals1, newScore.Player2, newScore.Team2, newScore.Goals2));
                count++;
            }
            //}

            if (count == 0)
            {
                //There is nothing to insert
                return -1;
            }

            string sql = sqlSb.ToString();

            int result = -1;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    conn.Open();
                    result = cmd.ExecuteNonQuery();
                }
            }

            return result;
        }

        public static int UpdateFriendlyMatchScore(FmScore score)
        {
            if (score == null) return -1;

            if (!score.MatchDt.HasValue)
                score.MatchDt = DateTime.Now;

            var sql = string.Format("Update FmScores set MatchDt = '{0}', Player1 = {1}, Team1 = {2}, Goals1 = {3}, Player2 = {4}, Team2 = {5}, Goals2 = {6} where Id = {7}",
                                    score.MatchDt,
                                    score.Player1, score.Team1, score.Goals1,
                                    score.Player2, score.Team2, score.Goals2,
                                    score.Id);
                        
            int result = -1;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    conn.Open();
                    result = cmd.ExecuteNonQuery();
                }
            }

            return result;
        }

        public static List<Tournament> GetTournaments()
        {
            List<Tournament> tournaments = new List<Tournament>();
            DataTable dt = new DataTable();
            string sql = "select * from tournaments";
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    conn.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                    foreach (DataRow dr in dt.Rows)
                    {
                        var t = new Tournament();
                        t.Id = Convert.ToInt32(dr["ID"]);
                        t.Name = dr["NAME"].ToString();
                        t.Date = Convert.ToDateTime(dr["DATE"]);
                        t.R1Id = dr.IsNull("R1ID") ? 0 : Convert.ToInt32(dr["R1ID"]);
                        t.R2Id = dr.IsNull("R2ID") ? 0 : Convert.ToInt32(dr["R2ID"]);
                        t.R3Id = dr.IsNull("R3ID") ? 0 : Convert.ToInt32(dr["R3ID"]);
                        t.R4Id = dr.IsNull("R4ID") ? 0 : Convert.ToInt32(dr["R4ID"]);
                        t.R5Id = dr.IsNull("R5ID") ? 0 : Convert.ToInt32(dr["R5ID"]);
                        t.R6Id = dr.IsNull("R6ID") ? 0 : Convert.ToInt32(dr["R6ID"]);
                        t.R7Id = dr.IsNull("R7ID") ? 0 : Convert.ToInt32(dr["R7ID"]);
                        t.Details = dr["DETAILS"].ToString();
                        t.HasResult = dr.IsNull("HASRESULT") ? false : (Convert.ToInt32(dr["HASRESULT"]) == 1 ? true : false);

                        tournaments.Add(t);
                    }
                }
            }
            return tournaments;
        }

        public static List<TournamentMatch> GetTournamentMatches(int tournamentId)
        {
            var tMatches = new List<TournamentMatch>();
            DataTable dt = new DataTable();
            string sql = string.Format("select * from TournamentMatches where TId = {0} order by Id", tournamentId);
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    conn.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                    foreach (DataRow dr in dt.Rows)
                    {
                        var tm = new TournamentMatch();

                        tm.Id = Convert.ToInt32(dr["Id"]);
                        tm.TId = tournamentId;
                        tm.MType = dr["MType"].ToString();
                        tm.P1 = dr.IsNull("P1") ? 0 : Convert.ToInt32(dr["P1"]);
                        tm.T1 = dr["T1"].ToString();
                        tm.G1 = dr.IsNull("G1") ? 0 : Convert.ToInt32(dr["G1"]);
                        tm.P2 = dr.IsNull("P2") ? 0 : Convert.ToInt32(dr["P2"]);
                        tm.T2 = dr["T2"].ToString();
                        tm.G2 = dr.IsNull("G2") ? 0 : Convert.ToInt32(dr["G2"]);
                        tm.Details = dr["DETAILS"].ToString();

                        tMatches.Add(tm);
                    }
                }
            }
            return tMatches;
        }

        public static int AddTournament(Tournament newT)
        {
            string sql = string.Format("Insert into TOURNAMENTS (NAME, DATE, DETAILS) values ('{0}', '{1}', '{2}')", newT.Name, newT.Date, newT.Details);

            int result = -1;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    conn.Open();
                    result = cmd.ExecuteNonQuery();
                }
            }

            return result;
        }

        public static int AddTournamentMatch(TournamentMatch tMatch)
        {
            string sql = string.Format("insert into TOURNAMENTMATCHES (TID, MTYPE, DETAILS, P1, T1, G1, P2, T2, G2) values ({0}, '{1}', '{2}', {3}, '{4}', {5}, {6}, '{7}', {8})",
                                        tMatch.TId, tMatch.MType, tMatch.Details,
                                        tMatch.P1, tMatch.T1, tMatch.G1,
                                        tMatch.P2, tMatch.T2, tMatch.G2);

            int result = -1;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    conn.Open();
                    result = cmd.ExecuteNonQuery();
                }
            }

            return result;
        }

        public static int AddTournamentResult(Tournament tToAdd)
        {
            string sql = string.Format(@"update tournaments 
                                         set R1Id = {0}, R2Id = {1}, R3Id = {2}, R4Id = {3}, R5Id = {4}, R6Id = {5}, R7Id = {6}, details = '{7}', hasResult = {8} 
                                         where Id = {9} ",
                                         tToAdd.R1Id, tToAdd.R2Id, tToAdd.R3Id, tToAdd.R4Id, tToAdd.R5Id, tToAdd.R6Id, tToAdd.R7Id, tToAdd.Details, 1, tToAdd.Id);

            int result = -1;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    conn.Open();
                    result = cmd.ExecuteNonQuery();
                }
            }

            return result;

        }
    }
}