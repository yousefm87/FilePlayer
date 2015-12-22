using System.Collections.Generic;

namespace ObjectDAL
{
    public class Itemlist
    {
        public string name { get; set; }
        public string file { get; set; }
    }

    public class Console
    {
        public string consolename { get; set; }
        public string apppath { get; set; }
        public string filespath { get; set; }
        public string extension { get; set; }
        public List<Itemlist> itemlist { get; set; }
        public string titlesubstring { get; set; }
        public string maxandfocusscript { get; set; }
        public string arguments { get; set; }
    }

    public class RootObject
    {
        public List<Console> consoles { get; set; }
    }
}
