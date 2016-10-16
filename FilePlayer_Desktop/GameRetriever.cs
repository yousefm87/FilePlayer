using FilePlayer.Model;
using GiantBomb.Api;
using GiantBomb.Api.Model;
using Microsoft.Practices.Prism.PubSubEvents;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;

namespace FilePlayer
{
    public class GameRetriever
    {
        private const string apiToken = "6b2a93c2be2eecb746c2bba7193da92fdf23b5d2";

        public class GameData
        {
            public string GameName { get; set; }
            public string GameDescription { get; set; }
            public string ImageURL { get; set; }
            public string ReleaseDate { get; set; }
            public string PlatformName { get; set; }
            public List<GameData> GameReleases { get; set; }


            public GameData()
            {
                GameName = "";
                GameDescription = "";
                ImageURL = "";
                ReleaseDate = "";
                PlatformName = "";
            }
        }

        public class GameDataSet 
        {
            public List<GameData> DataSet { get; set; }

            public GameDataSet()
            {
                DataSet = new List<GameData>();
            }
        }

        /// <summary>
        /// Gets Sift3 distance
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <param name="maxOffset"></param>
        /// <returns></returns>
        /// Reference: http://siderite.blogspot.com/2007/04/super-fast-and-accurate-string-distance.html
        public static float SiftDistance(string s1, string s2, int maxOffset)
        {
            if (String.IsNullOrEmpty(s1))
            {
                return String.IsNullOrEmpty(s2)
                         ? 0
                         : s2.Length;
            }
            if (String.IsNullOrEmpty(s2))
            {
                return s1.Length;
            }
            int c = 0;
            int offset1 = 0;
            int offset2 = 0;
            int lcs = 0;
            while ((c + offset1 < s1.Length) && (c + offset2 < s2.Length))
            {
                if (s1[c + offset1] == s2[c + offset2])
                {
                    lcs++;
                }
                else
                {
                    offset1 = 0;
                    offset2 = 0;
                    for (int i = 0; i < maxOffset; i++)
                    {
                        if ((c + i < s1.Length) && (s1[c + i] == s2[c]))
                        {
                            offset1 = i;
                            break;
                        }
                        if ((c + i < s2.Length) && (s1[c] == s2[c + i]))
                        {
                            offset2 = i;
                            break;
                        }
                    }
                }
                c++;
            }
            return (s1.Length + s2.Length) / 2 - lcs;
        }

        public int LevenshteinDistance(string source, string target)
        {
            if (String.IsNullOrEmpty(source))
            {
                if (String.IsNullOrEmpty(target)) return 0;
                return target.Length;
            }
            if (String.IsNullOrEmpty(target))
            {
                return source.Length;
            }
            if (source.Length > target.Length)
            {
                var temp = target;
                target = source;
                source = temp;
            }

            var m = target.Length;
            var n = source.Length;
            var distance = new int[2, m + 1];
            // Initialize the distance 'matrix'
            for (var j = 1; j <= m; j++) distance[0, j] = j;

            var currentRow = 0;
            for (var i = 1; i <= n; ++i)
            {
                currentRow = i & 1;
                distance[currentRow, 0] = i;
                var previousRow = currentRow ^ 1;
                for (var j = 1; j <= m; j++)
                {
                    var cost = (target[j - 1] == source[i - 1] ? 0 : 1);
                    distance[currentRow, j] = Math.Min(Math.Min(
                                distance[previousRow, j] + 1,
                                distance[currentRow, j - 1] + 1),
                                distance[previousRow, j - 1] + cost);
                }
            }
            return distance[currentRow, m];
        }


        public void GetItemImage(string gameQuery, string saveDir, bool overwriteFile)
        {
            const int MAX_SEARCH = 5;
            
            GiantBombRestClient giantBomb = new GiantBombRestClient(apiToken);

            IEnumerable<Game> games = giantBomb.SearchForGames(gameQuery);

            int maxCount = (games.Count() > MAX_SEARCH) ? MAX_SEARCH : games.Count();
            float minDistance = Int32.MaxValue;
            int minIndex = -1;

            for (int i = 0; i < maxCount; i++)
            {
                float distance = SiftDistance(gameQuery, games.ElementAt(i).Name, games.ElementAt(i).Name.Length);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    minIndex = i;
                }

            }

            float distanceThreshold = 30;

            if (minDistance <= distanceThreshold)
            {

                string itemImageURL = games.ElementAt(minIndex).Image.MediumUrl;
                string extension = itemImageURL.Split('.').Last();

                string saveToFilePath = saveDir + gameQuery + "." + extension;
                if ((!File.Exists(saveToFilePath)) || (File.Exists(saveToFilePath) && overwriteFile))
                {
                    WebClient webClient = new WebClient();
                    webClient.DownloadFile(itemImageURL, saveToFilePath);
                }

                int a = 0;
                int b = a + a;

            }
        }

        public static void GetAllPlatformsData(ItemLists itemLists, IEventAggregator iEventAggregator)
        {
            for (int currConsole = 0; currConsole < itemLists.GetConsoleCount(); currConsole++)
            {
                IEnumerable<string> currList = itemLists.GetItemNames(currConsole);
                string currPlatform = itemLists.GetConsoleName(currConsole);
                string currIndex = (currConsole).ToString();
                string currCount = itemLists.GetConsoleCount().ToString();
                string currPercentage = ((currConsole * 100) / itemLists.GetConsoleCount()).ToString();
                string currSaveDir = itemLists.GetConsoleFilePath(currConsole);
                bool overwriteFile = true;

                iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("GIANTBOMB_PLATFORM_UPLOAD_START",
                                                                                new string[] { currPlatform, currIndex, currCount, currPercentage }));
                GetPlatformData(currList, currPlatform, currSaveDir, overwriteFile, iEventAggregator);

                iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("GIANTBOMB_PLATFORM_UPLOAD_FINISH",
                                                                                new string[] { currPlatform, (currConsole + 1).ToString(),
                                                                                               currList.Count().ToString(), ((currConsole + 1)/currList.Count()).ToString() }));
            }
        }


        public static void GetPlatformData(IEnumerable<string> gamelist, string platformName, string saveDir, bool overwriteFile, IEventAggregator iEventAggregator)
        {
            string IMAGE_SUBFOLDER = saveDir + "Images\\";
            Directory.CreateDirectory(IMAGE_SUBFOLDER);

            int GIANTBOMB_API_RATE_LIMIT = 1100; //Giantbomb uses a 1 sec/request rate limit. Set to 1.1 for cushion

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            JsonWriter writer = new JsonTextWriter(sw);

            writer.Formatting = Formatting.Indented;
            writer.WriteStartObject();
            writer.WritePropertyName("games");
            writer.WriteStartArray();


            Stopwatch watch;

            //Giantbomb API has a limit of 15 min/400 requests. This is 2.25 sec/1 req. I'll make it 2.3 sec/req adding a small cushion
            for (int gameIndex = 0; gameIndex < gamelist.Count(); gameIndex++) // gamelist.Count(); gameIndex++)
            {
                string currGame = gamelist.ElementAt(gameIndex);
                string currGameIndex = (gameIndex).ToString();
                string currPlatformGameCount = gamelist.Count().ToString();
                string currPercentage = ((gameIndex * 100) / gamelist.Count()).ToString();
                iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("GIANTBOMB_GAME_UPLOAD_START", 
                                                                                new string[] { currGame, currGameIndex, currPlatformGameCount, currPercentage }));
                watch = new Stopwatch();

                GetGameData(currGame, platformName, IMAGE_SUBFOLDER, overwriteFile, writer);

                if(watch.ElapsedMilliseconds < GIANTBOMB_API_RATE_LIMIT) //May sleep in order to maintain giantbomb rate limit
                {
                    Thread.Sleep(GIANTBOMB_API_RATE_LIMIT - Convert.ToInt32(watch.ElapsedMilliseconds));
                }
                iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("GIANTBOMB_GAME_UPLOAD_FINISH",
                                                                                new string[] { currGame, (gameIndex + 1).ToString(),
                                                                                                gamelist.Count().ToString(), ((gameIndex + 1)/gamelist.Count()).ToString() }));
            }

            writer.WriteEndArray();
            writer.WriteEndObject();

            File.WriteAllText(saveDir + "gameinfo.json", sw.ToString());
        }


        public static bool GetGameData(string gameQuery, string platformName, string imageFolderPath, bool overwriteFile, JsonWriter writer)
        {
            const int MAX_SEARCH = 10;
            
            GiantBombRestClient giantBomb = new GiantBombRestClient(apiToken);

            try
            {
                IEnumerable<Game> games = giantBomb.SearchForGames(gameQuery);

                int maxCount = (games.Count() > MAX_SEARCH) ? MAX_SEARCH : games.Count();
                float minGameDistance = Int32.MaxValue;
                int minGameIndex = -1;

                for (int i = 0; i < maxCount; i++)
                {
                    float distance = SiftDistance(gameQuery, games.ElementAt(i).Name, games.ElementAt(i).Name.Length);

                    if (distance < minGameDistance)
                    {
                        minGameDistance = distance;
                        minGameIndex = i;
                    }

                }


                float distanceThreshold = gameQuery.Length; 

                if (minGameDistance <= distanceThreshold)
                {
                    IEnumerable<Release> releases = giantBomb.GetReleasesForGame(games.ElementAt(minGameIndex).Id);

                    string itemImageURL = "";
                    string itemDeck = "";
                    string itemDescription = "";
                    string itemReleaseDate = "";
                    bool retrievedImage = false;

                    if (releases.Count() > 0)
                    {
                        float minReleasePlatformDistance = Int32.MaxValue;
                        float minReleaseGameDistance = Int32.MaxValue;
                        int minReleaseIndex = -1;

                        for (int releaseNum = 0; releaseNum < (releases.Count() - 1); releaseNum++)
                        {
                            float platformDistance = SiftDistance(platformName.ToLower(), releases.ElementAt(releaseNum).Platform.Name.ToLower(), releases.ElementAt(releaseNum).Platform.Name.Length);
                            float gameDistance = SiftDistance(gameQuery.ToLower(), releases.ElementAt(releaseNum).Name.ToLower(), releases.ElementAt(releaseNum).Name.Length);


                            if (releases.ElementAt(releaseNum).Image != null)
                            {
                                if (platformDistance <= minReleasePlatformDistance) // At or below minimum platform distance - note: cares about game over platform
                                {
                                    if (gameDistance <= minReleaseGameDistance) // At or below game distance
                                    {
                                        if ((platformDistance == minReleasePlatformDistance) && (gameDistance == minReleaseGameDistance))
                                        { }
                                        else  //set unless both distances are the same
                                        {
                                            minReleasePlatformDistance = platformDistance;
                                            minReleaseGameDistance = gameDistance;
                                            minReleaseIndex = releaseNum;
                                        }
                                    }
                                }
                            }
                        }

                        if (minReleaseIndex != -1) // First try to get image URL from Game Release object
                        {
                            if (releases.ElementAt(minReleaseIndex).Image.MediumUrl != null)
                            {
                                itemImageURL = releases.ElementAt(minReleaseIndex).Image.MediumUrl;
                            }

                            if (releases.ElementAt(minReleaseIndex).Deck != null)
                            {
                                itemDeck = releases.ElementAt(minReleaseIndex).Deck;
                            }

                            if (releases.ElementAt(minReleaseIndex).Description != null)
                            {
                                itemDescription = releases.ElementAt(minReleaseIndex).Description;
                            }

                            if (releases.ElementAt(minReleaseIndex).ReleaseDate != null)
                            {
                                itemReleaseDate = releases.ElementAt(minReleaseIndex).ReleaseDate.ToString();
                            }

                            retrievedImage = (itemImageURL != "");
                        }

                    }

                    //Get game information from Game Release object if not set
                    if (games.ElementAt(minGameIndex).Image != null)
                    {
                        if (games.ElementAt(minGameIndex).Image.MediumUrl != null)
                        {
                            itemImageURL = (itemImageURL != "") ? itemImageURL : games.ElementAt(minGameIndex).Image.MediumUrl;
                        }
                    }
                        
                    if (games.ElementAt(minGameIndex).Deck != null)
                    {
                        itemDeck = (itemDeck != "") ? itemDeck : games.ElementAt(minGameIndex).Deck;
                    }

                    if (games.ElementAt(minGameIndex).Description != null)
                    {
                        itemDescription = (itemDescription != "") ? itemDescription : games.ElementAt(minGameIndex).Description;
                    }

                    if (games.ElementAt(minGameIndex).OriginalReleaseDate != null)
                    {
                        itemReleaseDate = (itemReleaseDate != "") ? itemReleaseDate : games.ElementAt(minGameIndex).OriginalReleaseDate.ToString();
                    }
                      
                    
                    string extension = itemImageURL.Split('.').Last();

                    if (extension.Length > 4)
                    {
                        extension = "jpg";
                    }

                    string itemImageLocation = imageFolderPath + gameQuery + "." + extension;

                    if (itemImageURL != "")//download image if needed
                    {
                        if ((!File.Exists(itemImageLocation)) || (File.Exists(itemImageLocation) && overwriteFile))
                        {
                            WebClient webClient = new WebClient();
                            try
                            {
                                webClient.DownloadFile(itemImageURL, itemImageLocation);
                            }
                            catch (WebException)
                            {
                                itemImageLocation = "";
                            }

                        }
                    }
                    else
                    {
                        itemImageLocation = "";
                    }
                    

                    writer.WriteStartObject();

                    writer.WritePropertyName("Name");
                    writer.WriteValue(gameQuery);

                    writer.WritePropertyName("Description");
                    writer.WriteValue(itemDescription);

                    writer.WritePropertyName("ShortDescription");
                    writer.WriteValue(itemDeck);

                    writer.WritePropertyName("ReleaseDate");
                    writer.WriteValue(itemReleaseDate);

                    writer.WritePropertyName("ImageLocation");
                    writer.WriteValue(itemImageLocation);

                    writer.WriteEndObject();

                    return true;
                }

            }
            catch (Exception)
            {
                
            }
            return false;
        }

        public static bool HasProperty(object b, string property)
        {
            Type t = b.GetType();
            PropertyInfo p = t.GetProperty(property);

            bool hasProperty = (p != null);

            return hasProperty;
        }


        public static ObservableCollection<GameData> GetGameDataSet(string gameQuery)
        {
            const int MAX_SEARCH = 5;
            
            GiantBombRestClient giantBomb = new GiantBombRestClient(apiToken);
            ArrayList gameDataSet = new ArrayList();
            try
            {
                IEnumerable<Game> games = giantBomb.SearchForGames(gameQuery);

                int maxCountGames = (games.Count() > MAX_SEARCH) ? MAX_SEARCH : games.Count();

                ObservableCollection<GameData> currGames = new ObservableCollection<GameData>();

                for (int i = 0; i < maxCountGames; i++)
                {
                    GameData currGame = new GameData();
                    currGame.GameName = games.ElementAt(i).Name;
                    currGame.GameDescription = games.ElementAt(i).Deck;
                    if (games.ElementAt(i).Image != null)
                    {
                        if (games.ElementAt(i).Image.SuperUrl != null)
                        {
                            currGame.ImageURL = games.ElementAt(i).Image.SuperUrl;
                        }
                        else
                        {
                            if (games.ElementAt(i).Image.MediumUrl != null)
                            {
                                currGame.ImageURL = games.ElementAt(i).Image.MediumUrl;
                            }
                            else
                            {
                                if (games.ElementAt(i).Image.SmallUrl != null)
                                {
                                    currGame.ImageURL = games.ElementAt(i).Image.SmallUrl;
                                }
                                else
                                {
                                    currGame.ImageURL = "";
                                }
                            }
                        }
                    }
                    else
                    {
                        currGame.ImageURL = "";
                    }
                    currGame.ReleaseDate = games.ElementAt(i).OriginalReleaseDate.ToString();

                    IEnumerable<Release> releases = giantBomb.GetReleasesForGame(games.ElementAt(i));
                    int maxCountReleases = (releases.Count() > MAX_SEARCH) ? MAX_SEARCH : releases.Count();
                    List<GameData> currGameReleases = new List<GameData>();

                    for (int j = 0; j < maxCountReleases; j++)
                    {

                        GameData currRelease = new GameData();
                        currRelease.GameName = releases.ElementAt(j).Name;
                        if (releases.ElementAt(j).Deck != null)
                        {
                            currRelease.GameDescription = releases.ElementAt(j).Deck;
                        }
                        else
                        {
                            currRelease.GameDescription = games.ElementAt(i).Deck;
                        }



                        if (releases.ElementAt(j).Image != null)
                        {
                            if (releases.ElementAt(j).Image.SuperUrl != null)
                            {
                                currRelease.ImageURL = releases.ElementAt(j).Image.SuperUrl;
                            }
                            else
                            {
                                if (releases.ElementAt(j).Image.MediumUrl != null)
                                {
                                    currRelease.ImageURL = releases.ElementAt(j).Image.MediumUrl;
                                }
                                else
                                {
                                    if (releases.ElementAt(j).Image.SmallUrl != null)
                                    {
                                        currRelease.ImageURL = releases.ElementAt(j).Image.SmallUrl;
                                    }
                                    else
                                    {
                                        currRelease.ImageURL = "";
                                    }
                                }
                            }
                        }
                        else
                        {
                            currRelease.ImageURL = "";
                        }

                        if (currRelease.ImageURL.Equals(""))
                        {
                            currRelease.ReleaseDate = releases.ElementAt(j).ReleaseDate.ToString();
                            currRelease.PlatformName = releases.ElementAt(j).Platform.Name;

                            currGameReleases.Add(currRelease);
                        }
                    }

                    currGame.GameReleases = currGameReleases;

                    if (currGame.ImageURL.Equals("") && (currGame.GameReleases.Count() == 0))
                    { }
                    else
                    {
                        currGames.Add(currGame);
                    }
                }

                return currGames;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static bool ItemImageExists(string imageURL)
        {
            if (imageURL.Equals(""))
            {
                return false;
            }

            var request = (HttpWebRequest)WebRequest.Create(imageURL);
            request.Method = "HEAD";

            try
            {
                using (var response = request.GetResponse())
                {
                    bool isImageValid = response.ContentType.ToLower(CultureInfo.InvariantCulture).StartsWith("application/octet-stream", StringComparison.OrdinalIgnoreCase);
                    bool isImageValid2 = response.ContentType.ToLower(CultureInfo.InvariantCulture).StartsWith("image/", StringComparison.OrdinalIgnoreCase);
                    return isImageValid || isImageValid2;
                }

            }
            catch (WebException)
            {
                return false;
            }
        }

        public static ObservableCollection<List<GameData>> GetGameDataSetLists(string gameQuery)
        {
            const int MAX_SEARCH = 10;
            
            GiantBombRestClient giantBomb = new GiantBombRestClient(apiToken);

            try
            {
                IEnumerable<Game> games = giantBomb.SearchForGames(gameQuery);

                int maxCountGames = (games.Count() > MAX_SEARCH) ? MAX_SEARCH : games.Count();

                ObservableCollection<List<GameData>> currGames = new ObservableCollection<List<GameData>>();

                for (int i = 0; i < maxCountGames; i++)
                {
                    List<GameData> currGameList = new List<GameData>(); 

                    GameData currGame = new GameData();

                    currGame = new GameData();
                    currGame.GameName = games.ElementAt(i).Name;
                    currGame.GameDescription = games.ElementAt(i).Deck;
                    currGame.ReleaseDate = games.ElementAt(i).OriginalReleaseDate.ToString();

                    if (games.ElementAt(i).Image != null)
                    {
                        if (games.ElementAt(i).Image.SuperUrl != null)
                        {
                            currGame.ImageURL = games.ElementAt(i).Image.SuperUrl;
                        }
                        else
                        {
                            if (games.ElementAt(i).Image.MediumUrl != null)
                            {
                                currGame.ImageURL = games.ElementAt(i).Image.MediumUrl;
                            }
                            else
                            {
                                if (games.ElementAt(i).Image.SmallUrl != null)
                                {
                                    currGame.ImageURL = games.ElementAt(i).Image.SmallUrl;
                                }
                                else
                                {
                                    currGame.ImageURL = null;
                                }
                            }
                        }
                    }
                    else
                    {
                        currGame.ImageURL = null;
                    }

                    
                    if (currGame.ImageURL != null)
                    {
                        if (ItemImageExists(currGame.ImageURL))
                        {
                            currGameList.Add(currGame);
                        }
                    }
                    

                    IEnumerable<Release> releases = giantBomb.GetReleasesForGame(games.ElementAt(i));
                    int maxCountReleases = (releases.Count() > MAX_SEARCH) ? MAX_SEARCH : releases.Count();


                    for (int j = 0; j < maxCountReleases; j++)
                    {
                        GameData currRelease = new GameData();
                        currRelease.GameName = releases.ElementAt(j).Name;
                        currRelease.ReleaseDate = releases.ElementAt(j).ReleaseDate.ToString();
                        currRelease.PlatformName = releases.ElementAt(j).Platform.Name;
                        if (releases.ElementAt(j).Deck != null)
                        {
                            currRelease.GameDescription = releases.ElementAt(j).Deck;
                        }
                        else
                        {
                            currRelease.GameDescription = games.ElementAt(i).Deck;
                        }
                        
                        if (releases.ElementAt(j).Image != null)
                        {
                            if (releases.ElementAt(j).Image.SuperUrl != null)
                            {
                                currRelease.ImageURL = releases.ElementAt(j).Image.SuperUrl;
                            }
                            else
                            {
                                if (releases.ElementAt(j).Image.MediumUrl != null)
                                {
                                    currRelease.ImageURL = releases.ElementAt(j).Image.MediumUrl;
                                }
                                else
                                {
                                    if (releases.ElementAt(j).Image.SmallUrl != null)
                                    {
                                        currRelease.ImageURL = releases.ElementAt(j).Image.SmallUrl;
                                    }
                                    else
                                    {
                                        currRelease.ImageURL = null;
                                    }
                                }
                            }
                        }
                        else
                        {
                            currRelease.ImageURL = null;
                        }

                        if (currRelease.ImageURL != null)
                        {
                            if (ItemImageExists(currGame.ImageURL))
                            {
                                currGameList.Add(currRelease);
                            }
                        }
                    }
                    
                    if (currGameList.Count() > 0)
                    {     
                        currGames.Add(currGameList);
                    }
                }

                return currGames;
            }
            catch (Exception )
            {
                return null;
            }
        }
    }
}

    




