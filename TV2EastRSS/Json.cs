using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TV2EastRSS
{
    class Json
    {

        public class JObject
        {
            public string version { get; set; }
            public string xmlnsatom { get; set; }
            public Channel channel { get; set; }
        }

        public class Channel
        {
            public AtomLink atomlink { get; set; }
            public Title title { get; set; }
            public Description description { get; set; }
            public string link { get; set; }
            public Item[] item { get; set; }
        }

        public class AtomLink
        {
            public string href { get; set; }
            public string rel { get; set; }
            public string type { get; set; }
        }

        public class Title
        {
            public string cdatasection { get; set; }
        }

        public class Description
        {
            public string cdatasection { get; set; }
        }


        public class Item
        {
            public string guid { get; set; }
            public Title title { get; set; }
            public Description description { get; set; }
            public string link { get; set; }
            public Category category { get; set; }
            public Pubdate pubDate { get; set; }
            public Enclosure enclosure { get; set; }
        }

        public class Category
        {
            public string cdatasection { get; set; }
        }

        public class Pubdate
        {
            public string cdatasection { get; set; }
        }

        public class Enclosure
        {
            public string url { get; set; }
            public string length { get; set; }
            public string type { get; set; }
        }
    }
}
