using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FilePlayer.Model
{
    public class GameInfo
    {
        public JObject games = null;
        public int CurrConsole { get; set; }
        public string JsonFilePath = "";
        public string ImagePath = "";
        public GameInfo(String jsonFilePath, String imgPath)
        {
            JsonFilePath = jsonFilePath;
            ImagePath = imgPath;
            Init();
        }

        public void Init()
        {
            if (! File.Exists(JsonFilePath))
            {
                JArray gamesArr = new JArray();
                JProperty gamesProp = new JProperty("games", gamesArr);

                JObject gamesObj = new JObject(gamesProp);

                string json = JsonConvert.SerializeObject(gamesObj, Formatting.Indented);
                File.WriteAllText(JsonFilePath, json);
            }
            games = JObject.Parse(File.ReadAllText(JsonFilePath));

            if (games["games"] != null)
            {
                CurrConsole = 0;
            }
        }
        /// <summary>
        /// Deletes Game from gameinfo.json and deletes image file from 
        /// </summary>
        /// <param name="game"></param>
        public void DeleteGame(string game)
        {
            JToken currGame = GetGame(game);
            if (currGame != null)
            {
                string imgLocation = GetGameImageLocation(game);
                //delete game image if exists
                if (!imgLocation.Equals(""))
                {
                    File.Delete(imgLocation);
                }
                
                currGame.Remove();

                string json = JsonConvert.SerializeObject(games, Formatting.Indented);
                File.WriteAllText(JsonFilePath, json);

                games = JObject.Parse(File.ReadAllText(JsonFilePath));
            }
        }

        public void AddGame(string game, string description, string releaseDate, string imageLocation)
        {
            JToken currGame = GetGame(game);

            string extension = imageLocation.Split('.').Last();

            if (extension.Length > 4)
            {
                extension = "jpg";
            }

            string saveToFilePath = ImagePath + game + "." + extension;
            string itemImageLocation = saveToFilePath;

            //Create directory if it doesn't exist
            if (!Directory.Exists(ImagePath))
            {
                Directory.CreateDirectory(ImagePath);
            }

            //Delete image file if exists - Always overwrite
            if (File.Exists(itemImageLocation))
            {
                File.Delete(itemImageLocation);
            }

            //Download image file
            WebClient webClient = new WebClient();
            try
            {
                webClient.DownloadFile(imageLocation, saveToFilePath);
            }
            catch (WebException)
            {
                itemImageLocation = "";
            }

            //Add entry for game into gameinfo
            JObject newGame = new JObject();
            
            JProperty nameProp = new JProperty("Name", game);
            JProperty descProp = new JProperty("ShortDescription", description);
            JProperty relDateProp = new JProperty("ReleaseDate", releaseDate);
            JProperty imgLocProp = new JProperty("ImageLocation", itemImageLocation);

            newGame.Add(nameProp);
            newGame.Add(descProp);
            newGame.Add(relDateProp);
            newGame.Add(imgLocProp);

            if (currGame != null)
            {
                currGame.Replace(newGame);
            }
            else
            { 
                if (games["games"].Children().LastOrDefault() != null)
                {
                    games["games"].Children().LastOrDefault().AddAfterSelf(newGame);
                }
                else
                {
                    JArray gamesArr = new JArray(newGame);
                    JProperty gamesProp = new JProperty("games", gamesArr);

                    games["games"] = gamesArr;
                }
            }
            
            //overwrite json file
            string json = JsonConvert.SerializeObject(games, Formatting.Indented);
            File.WriteAllText(JsonFilePath, json);

            games = JObject.Parse(File.ReadAllText(JsonFilePath));
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
