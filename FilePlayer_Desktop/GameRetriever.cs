using GiantBomb.Api;
using GiantBomb.Api.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FilePlayer
{
    class GameRetriever
    {

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
            while ((c + offset1 < s1.Length)
                   &&
                   (c + offset2 < s2.Length))
            {
                if (s1[c + offset1] ==
                    s2[c + offset2])
                    lcs++;
                else
                {
                    offset1 = 0;
                    offset2 = 0;
                    for (int i = 0;
                         i < maxOffset;
                         i++)
                    {
                        if ((c + i < s1.Length)
                            &&
                            (s1[c + i] == s2[c]))
                        {
                            offset1 = i;
                            break;
                        }
                        if ((c + i < s2.Length)
                            &&
                            (s1[c] == s2[c + i]))
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
            if (String.IsNullOrEmpty(target)) return source.Length;

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
            const string apiToken = "6b2a93c2be2eecb746c2bba7193da92fdf23b5d2";
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


        public static void GetConsoleData(IEnumerable<string> gamelist, string consoleName, string saveDir, bool overwriteFile)
        {
            string IMAGE_SUBFOLDER = saveDir + "Images\\";

            Directory.CreateDirectory(IMAGE_SUBFOLDER);
            const int MAX_SEARCH = 5;
            const string apiToken = "6b2a93c2be2eecb746c2bba7193da92fdf23b5d2";
            GiantBombRestClient giantBomb = new GiantBombRestClient(apiToken);


            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            JsonWriter writer = new JsonTextWriter(sw);

            writer.Formatting = Formatting.Indented;
            writer.WriteStartObject();
            writer.WritePropertyName("games");
            writer.WriteStartArray();

            for (int gameIndex = 0; gameIndex < 100; gameIndex++) // gamelist.Count(); gameIndex++)
            {
                string gameQuery = gamelist.ElementAt(gameIndex);

                if (gameQuery.ToLower().StartsWith("ad&d"))
                {
                    string newQuery = gameQuery;
                }
                    


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


                float distanceThreshold = 30;



                if (minGameDistance <= distanceThreshold)
                {
                    IEnumerable<Release> releases = giantBomb.GetReleasesForGame(games.ElementAt(minGameIndex).Id);

                    string itemImageURL = "";
                    bool retrievedImage = false;

                    if (releases.Count() > 0)
                    {
                        float minReleasePlatformDistance = Int32.MaxValue;
                        float minReleaseGameDistance = Int32.MaxValue;
                        int minReleaseIndex = -1;
                        for (int releaseNum = 0; releaseNum < (releases.Count() - 1); releaseNum++)
                        {
                            float platformDistance = SiftDistance(consoleName.ToLower(), releases.ElementAt(releaseNum).Platform.Name.ToLower(), releases.ElementAt(releaseNum).Platform.Name.Length);
                            float gameDistance = SiftDistance(gameQuery.ToLower(), releases.ElementAt(releaseNum).Name.ToLower(), releases.ElementAt(releaseNum).Name.Length);

                            if (releases.ElementAt(releaseNum).Image != null)
                            {
                                if (platformDistance <= minReleasePlatformDistance)
                                {
                                    if (gameDistance <= minReleaseGameDistance)
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

                        if (minReleaseIndex != -1)
                        {
                            retrievedImage = true;
                            itemImageURL = releases.ElementAt(minReleaseIndex).Image.MediumUrl;
                        }
                        
                    }

                    if(!retrievedImage)
                    { 
                        if (games.ElementAt(minGameIndex).Image != null)
                        {
                            itemImageURL = games.ElementAt(minGameIndex).Image.MediumUrl;
                            retrievedImage = true;
                        }
                    }




                    
                    string extension = itemImageURL.Split('.').Last();

                    string saveToFilePath = IMAGE_SUBFOLDER + gameQuery + "." + extension;
                    string itemImageLocation = saveToFilePath;
                    if (retrievedImage)
                    {
                        if ((!File.Exists(saveToFilePath)) || (File.Exists(saveToFilePath) && overwriteFile))
                        {
                            WebClient webClient = new WebClient();
                            webClient.DownloadFile(itemImageURL, saveToFilePath);
                        }
                    }
                    else
                    {
                        itemImageLocation = "";
                    }

                    string itemDeck = games.ElementAt(minGameIndex).Deck;
                    string itemDescription = games.ElementAt(minGameIndex).Description;
                    string itemReleaseDate = games.ElementAt(minGameIndex).OriginalReleaseDate.ToString();
                    

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

                }
            }

            writer.WriteEndArray();
            writer.WriteEndObject();

            File.WriteAllText(saveDir + "gameinfo.json", sw.ToString());

        }
    }
}
