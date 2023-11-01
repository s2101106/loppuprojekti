using Newtonsoft.Json;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace TurboMapReader
{

	/// <summary>
	/// This is the interface to this library.
	/// Contains a static function LoadMapFromFile
	/// </summary>
	public class MapReader
	{
		/// <summary>
		/// Attempts to load a Tiled Map file
		/// from given file.
		/// On success returns a TiledMap instance
		/// that is populated with the data.
		/// If reading fails a Null is returned instead.
		/// </summary>
		/// <param name="filename">Filename of a file. The file must be a .json file that has been exported from Tiled without compression.</param>
		/// <returns>The data in the file wrapped in a TiledMap instance or null if reading fails.</returns>
		public static TiledMap? LoadMapFromFile(string filename)
		{
			StreamReader fileReader = null;
			try
			{
				fileReader = new StreamReader(filename);
			}
			catch(FileNotFoundException e)
			{
				Console.Write("Could not find Tiled map file: ");
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine(filename);
				Console.ResetColor();
				Console.WriteLine("Error message: " + e.Message);
				return null;
			}

			string fileContents = fileReader.ReadToEnd();
			fileReader.Close();

			try
			{
				TiledMap mapData = JsonConvert.DeserializeObject<TiledMap>(fileContents);
				if (mapData.tilesets.Count > 0)
				{
					mapData.tilesetFiles = new List<TilesetFile>();
					foreach (Tileset tileset in mapData.tilesets)
					{
						TilesetFile tilesetFile = new TilesetFile();
						tilesetFile = LoadTileSetFromFile(tileset.source);
						if (tilesetFile != null)
						{
							mapData.tilesetFiles.Add(tilesetFile);
						}
					}
				}
				return mapData;
			}
			catch (JsonReaderException e)
			{
				Console.Write("Could not read Tiled map file: ");
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine(filename);
				Console.ResetColor();
				Console.WriteLine("Error message: " + e.Message);
				return null;
			}
		}

		private static TilesetFile? LoadTileSetFromFile(string filename)
		{
			// The file must have an .xml extension
			// copy the file to temp xml file
			if (File.Exists(filename) == false)
			{
				Console.Write("No such tile set file: ");
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine(filename);
				Console.ResetColor();
				return null;
			}

			TilesetFile loadedData = new TilesetFile();
			// Read data to nicer format
			tileset tilesetData = ReadXml.ReadXmlTo<tileset>(filename); 

			loadedData.version = tilesetData.version;
			loadedData.tiledversion = tilesetData.tiledversion;
			loadedData.name = tilesetData.name;
			loadedData.tilewidth = Convert.ToInt32(tilesetData.tilewidth);
			loadedData.tileheight = Convert.ToInt32(tilesetData.tileheight);
			loadedData.tilecount = Convert.ToInt32(tilesetData.tilecount);
			loadedData.columns = Convert.ToInt32(tilesetData.columns);
			loadedData.image = tilesetData.image.source;
			int lastSlash = loadedData.image.LastIndexOf('/');
			loadedData.imageWoPath = tilesetData.image.source.Substring(lastSlash + 1);
			loadedData.imagewidth = Convert.ToInt32(tilesetData.image.width);
			loadedData.imageheight = Convert.ToInt32(tilesetData.image.height);

			return loadedData;
		}
	}
}