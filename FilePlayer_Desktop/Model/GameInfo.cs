using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilePlayer.Model
{
    public class GameInfo
    {
        public JObject games = null;
        public int CurrConsole { get; set; }

        public GameInfo(String jsonFilePath)
        {
            games = JObject.Parse(File.ReadAllText(jsonFilePath));
            

            if (games["consoles"] != null)
            {
                CurrConsole = 0;
            }

        }
        public JToken GetGame(string game)
        {
            JToken gameList = games["games"];

            IEnumerable<JToken> gameMatches = games["games"].Where(g =>
            {
                string currGame = g["Name"].Value<String>();
                return currGame.Equals(game);
            }).ToList<JToken>();

            if (gameMatches.Count() > 0)
            {
                return gameMatches.First();
            }
            return null;
        }

        public string GetGameImageLocation(string game)
        {
            JToken gameToken = GetGame(game);

            if (gameToken != null)
            {
                JToken imageLocation = gameToken["ImageLocation"];
                if (imageLocation != null)
                {
                    return imageLocation.Value<String>();
                }
            }
            return "";
        }

        public string GetGameDescription(string game)
        {
            JToken gameToken = GetGame(game);

            if (gameToken != null)
            {
                JToken description = gameToken["Description"];
                if (description != null)
                {
                    return description.Value<String>();
                }
            }
            return "";
        }

        public string GetGameShortDescription(string game)
        {
            JToken gameToken = GetGame(game);

            if (gameToken != null)
            {
                JToken shortDescription = gameToken["ShortDescription"];
                if (shortDescription != null)
                {
                    return shortDescription.Value<String>();
                }
            }
            return "";
        }

        public string GetGameReleaseDate(string game)
        {
            JToken gameToken = GetGame(game);

            if (gameToken != null)
            {
                JToken releaseDate = gameToken["ReleaseDate"];
                if (releaseDate != null)
                {
                    return releaseDate.Value<String>();
                }
            }
            return "";
        }

    }


}
