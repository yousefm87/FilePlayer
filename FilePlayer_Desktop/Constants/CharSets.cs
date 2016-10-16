
namespace FilePlayer.Constants
{
    static class CharSets
    {
        public static string[] charSetABC = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J",
                                                            "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T",
                                                            "", "", "U", "V", "W", "X", "Y", "Z", "", "" };
        public static string[] charSetNonABC = new string[] {"1", "2", "3", "4", "5", "6", "7", "8", "9", "0",
                                                              "", ".", "?", "!", ":", "-", "#","&", "+", "",
                                                              "", "", "(", ")", "\\", "/", "\"", "'", "", "" };

        public static string[][] charSets = new string[][] { CharSets.charSetABC, CharSets.charSetNonABC};
}
}
