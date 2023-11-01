using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TurboMapReader
{
	[Serializable]
	public class tileset
	{
		[XmlAttribute]
		public string version { get; set; }
		[XmlAttribute]
		public string tiledversion { get; set; }
		[XmlAttribute]
		public string name { get; set; }
		[XmlAttribute]
		public string tilewidth { get; set; }
		[XmlAttribute]
		public string tileheight { get; set; }
		[XmlAttribute]
		public string tilecount { get; set; }
		[XmlAttribute]
		public string columns { get; set; }
		[XmlElement]
		public image image { get; set; }
	}
	[Serializable]
	public class image
	{
		[XmlAttribute]
		public string source { get; set; }
		[XmlAttribute]
		public string width { get; set; }
		[XmlAttribute]
		public string height { get; set; }
	}
}
