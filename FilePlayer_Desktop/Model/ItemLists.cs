using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace FilePlayer.Model
{
    public class ItemLists
    {
        public JObject consoles = null;
        public int CurrConsole { get; set; }
        public string JsonFilePath = null;
        public ItemLists(String jsonFilePath)
        {
            JsonFilePath = jsonFilePath;
            Init();
        }

        public void Init()
        {
            if (!File.Exists(JsonFilePath))
            {
                if (! Directory.Exists(Path.GetDirectoryName(JsonFilePath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(JsonFilePath));
                }

                JArray consolesArr = new JArray();
                JProperty consolesProp = new JProperty("consoles", consolesArr);
                JObject consolesObj = new JObject(consolesProp);

                string json = JsonConvert.SerializeObject(consolesObj, Formatting.Indented);
                File.WriteAllText(JsonFilePath, json);
            }
            UpdateConsoles();
        }



        public void UpdateConsoles()
        {
            consoles = JObject.Parse(File.ReadAllText(JsonFilePath));
            UpdateItemLists();

            if (consoles["consoles"] != null)
            {
                CurrConsole = 0;
            }
        }

        public int GetConsoleCount()
        {
            return consoles["consoles"].Count();
        }

        public bool SetConsoleNext()
        {
            if (GetConsoleCount() > 0)
            {
                if (CurrConsole < (GetConsoleCount() - 1))
                {
                    CurrConsole++;
                    return true;
                }
            }

            return false;
        }

        public bool SetConsolePrevious()
        {
            if (GetConsoleCount() > 0)
            {
                if (CurrConsole > 0)
                {
                    CurrConsole--;
                    return true;
                }
            }

            return false;
        }

        public string GetConsoleMaxAndFocus(int consoleIndex)
        {
            JToken appPath = consoles["consoles"][consoleIndex]["maxandfocusscript"];

            return appPath.Value<String>();
        }


        public string GetConsoleFilePath(int consoleIndex)
        {
            JToken filePath = consoles["consoles"][consoleIndex]["filespath"];
            return filePath.Value<String>();
        }

        public string GetConsoleAppPath(int consoleIndex)
        {
            JToken appPath = consoles["consoles"][consoleIndex]["apppath"];

            return appPath.Value<String>();
        }

        public string GetConsoleName(int consoleIndex)
        {
            JToken appPath = consoles["consoles"][consoleIndex]["consolename"];

            return appPath.Value<String>();
        }

        public string GetConsoleArguments(int consoleIndex)
        {
            JToken appPath = consoles["consoles"][consoleIndex]["arguments"];

            return appPath.Value<String>();
        }

        public string GetConsoleTitleSubString(int consoleIndex)
        {
            JToken appPath = consoles["consoles"][consoleIndex]["titlesubstring"];

            return appPath.Value<String>();
        }

        public IEnumerable<string> GetItemNames(int consoleIndex)
        {
            JToken currConsoleItemList = consoles["consoles"][consoleIndex]["itemlist"];
            return currConsoleItemList.Children()["name"].Values<String>();
        }

        public IEnumerable<string> GetItemNames(int consoleIndex, string nameContains)
        {
            JToken currConsoleItemList = consoles["consoles"][consoleIndex]["itemlist"];

            IEnumerable<string> list = currConsoleItemList.Children()["name"].Values<String>();
            list = list.Where(x => x.ToLower().Contains(nameContains.ToLower())).ToList();

            return list;
        }

        public IEnumerable<string> GetItemNames(int consoleIndex, string filterText, string filterType)
        {
            JToken currConsoleItemList = consoles["consoles"][consoleIndex]["itemlist"];

            IEnumerable<string> list = currConsoleItemList.Children()["name"].Values<String>();

            switch (filterType.ToLower())
            {
                case "contains":
                    list = list.Where(x => x.ToLower().Contains(filterText.ToLower())).ToList();
                    break;
                case "ends with":
                    list = list.Where(x => x.ToLower().EndsWith(filterText.ToLower())).ToList();
                    break;
                case "starts with":
                    list = list.Where(x => x.ToLower().StartsWith(filterText.ToLower())).ToList();
                    break;
                default: //default is set to contains
                    list = list.Where(x => x.ToLower().Contains(filterText.ToLower())).ToList();
                    break;

            }
            return list;
        }

        public IEnumerable<string> GetItemFilePaths(int consoleIndex)
        {
            JToken currConsoleItemList = consoles["consoles"][consoleIndex]["itemlist"];
            return currConsoleItemList.Children()["file"].Values<String>();
        }

        public IEnumerable<string> GetItemFilePaths(int consoleIndex, string nameContains)
        {
            JToken currConsoleItemList = consoles["consoles"][consoleIndex]["itemlist"];

            IEnumerable<string> list = currConsoleItemList.Children()["file"].Values<String>();
            list = list.Where(x => x.ToLower().Contains(nameContains.ToLower())).ToList();

            return list;
        }

        public IEnumerable<string> GetItemFilePaths(int consoleIndex, string filterText, string filterType)
        {
            string extension;
            JToken currConsoleItemList = consoles["consoles"][consoleIndex]["itemlist"];

            IEnumerable<string> list = currConsoleItemList.Children()["file"].Values<String>();

            switch (filterType.ToLower())
            {
                case "contains":
                    list = list.Where(x => x.ToLower().Contains(filterText.ToLower())).ToList();
                    break;
                case "ends with":
                    extension = (String)consoles["consoles"][consoleIndex]["extension"];

                    list = list.Where(x =>
                    {
                        String currItemName = x.Split('\\').Last();
                        currItemName = currItemName.Substring(0, currItemName.Length - extension.Length - 1).Trim();
                        return currItemName.ToLower().EndsWith(filterText.ToLower());
                    }).ToList();
                    break;

                case "starts with":
                    extension = (String)consoles["consoles"][consoleIndex]["extension"];

                    list = list.Where(x =>
                    {
                        String currItemName = x.Split('\\').Last();
                        currItemName = currItemName.Substring(0, currItemName.Length - extension.Length - 1).Trim();
                        return currItemName.ToLower().StartsWith(filterText.ToLower());
                    }).ToList();
                    break;                
                default:
                    list = list.Where(x => x.ToLower().Contains(filterText.ToLower())).ToList();
                    break;
            }
            

            return list;
        }



        public void UpdateItemLists()
        {
            if (consoles != null)
            {
                for (int i = 0; i < GetConsoleCount(); i++) //for each console
                {
                    String filesPath = (String)consoles["consoles"][i]["filespath"];
                    String extension = (String)consoles["consoles"][i]["extension"];
                    String[] files = Directory.GetFiles(filesPath, "*." + extension);

                    JArray itemList = new JArray();
                    for (int j = 0; j < files.Count(); j++)
                    {
                        JObject currItem = new JObject();
                        String currFile = files[j];

                        String currItemName = currFile.Split('\\').Last();


                        currItemName = currItemName.Substring(0, currItemName.Length - extension.Length - 1).Trim();
                        currItem.Add("name", currItemName);
                        currItem.Add("file", currFile);

                        itemList.Add(currItem);
                    }

                    if (consoles["consoles"][i]["itemlist"] != null)
                    {
                        consoles["consoles"][i]["itemlist"].Replace(itemList);
                    }
                    else
                    {
                        JProperty newList = new JProperty("itemlist", itemList);
                        JObject currConsole = (JObject)consoles["consoles"][i];
                        currConsole.Property("extension").AddAfterSelf(newList);
                    }

                }

            }
            else
            {
                Console.Write("WARNING: consoles object not initialized");
            }
        }
    }

}


