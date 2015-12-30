using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;


namespace FilePlayer.Model
{
    public class ItemLists
    {
        public JObject consoles = null;
        public int CurrConsole { get; set; }

        public ItemLists(String jsonFilePath)
        {
            consoles = JObject.Parse(File.ReadAllText(jsonFilePath));
            UpdateItemLists();

            if (consoles["consoles"] != null)
            {
                CurrConsole = 0;
            }

        }

        public bool SetConsoleNext()
        {
            if((CurrConsole + 1) < consoles["consoles"].Count())
            {
                CurrConsole++;
                return true;
            }
            return false;
        }

        public bool SetConsolePrevious()
        {
            if ((CurrConsole - 1) >= 0)
            {
                CurrConsole--;
                return true;
            }
            return false;
        }

        public string GetConsoleMaxAndFocus(int consoleIndex)
        {
            JToken appPath = consoles["consoles"][consoleIndex]["maxandfocusscript"];

            return appPath.Value<String>();
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

        public IEnumerable<string> GetItemFilePaths(int consoleIndex)
        {
            JToken currConsoleItemList = consoles["consoles"][consoleIndex]["itemlist"];
            return currConsoleItemList.Children()["file"].Values<String>();
        }

        public void UpdateItemLists()
        {
            if (consoles != null)
            {
                for (int i = 0; i < consoles["consoles"].Count(); i++) //for each console
                {
                    String filesPath = (String)consoles["consoles"][i]["filespath"];
                    String extension = (String)consoles["consoles"][i]["extension"];
                    String[] files = Directory.GetFiles(filesPath, "*." + extension);

                    JArray itemList = new JArray();
                    for (int j = 0; j < files.Count(); j++)
                    {
                        JObject currItem = new JObject();
                        String currFile = files[j];

                        String currItemName = currFile.Split('\\').Last().Split('.').First().Trim();
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


