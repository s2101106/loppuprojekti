using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurboMapReader
{
	public class PairWriter
	{
		public static void PrintToConsole(string header, string[] pairs, int indentation)
		{
			StringBuilder sb = new StringBuilder();
			if (indentation > 0)
			{
				sb.Append("\t", 0, indentation - 1);
			}
			Console.Write(sb.ToString());
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.WriteLine(header);
			sb.Append("\t");
			for (int i = 0; i <  pairs.Length; i += 2) {
				Console.ResetColor();
				Console.Write(sb.ToString());
				Console.Write(pairs[i] + " : ");
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine(pairs[i + 1]);
			}
			Console.ResetColor();

		}
	}
	public class MapLayer
	{
		public int[] data;  // "data":[. . .]
		public int height;  // "height":20,
		public int id;
		public string name; //  "name":"Ground Layer",
		public string type;
		public bool visible;
		public int width;   //  "width":30,
		public int x;
		public int y;

		public void PrintToConsole(int indent)
		{
			string visibility = visible ? "Yes" : "No";
			string[] pairs = { "Name", name, "ID", Convert.ToString(id), "Type", type, "Size (tiles)", $"{width} x {height}", "Position (tiles)", $"({x},{y})", "Visible", visibility};
			PairWriter.PrintToConsole("Map Layer", pairs, indent);
		}
	}

	/// <summary>
	/// Refers to a .tmx file.
	/// Variable <b>source</b> is the filename of the TMX file
	/// TMX files are represented by class TileSetFile
	/// </summary>
	public class Tileset
	{
		public int firstgid;
		public string source;
	}

	/// <summary>
	/// Represents a TSX file
	/// Variable <b>imageWoPath</b> is the image containing the graphics
	/// </summary>
	public class TilesetFile
	{
		public string version;
		public string tiledversion;
		public string name;
		public int tilewidth;
		public int tileheight;
		public int tilecount;
		public int columns;
		public string image;
		public string imageWoPath;
		public int imagewidth;
		public int imageheight;

		public void PrintToConsole(int indent)
		{
			string[] pairs = { "Name", name, "Tile count", Convert.ToString(tilecount), "Columns", Convert.ToString(columns), "Tile size (px)", $"{tilewidth} x {tileheight}", "Image Path", image, "Image File", imageWoPath, "Size (px)", $"{imagewidth} x {imageheight}"};
			PairWriter.PrintToConsole("Tileset File", pairs, indent);
		}
	}

	/// <summary>
	/// Represents a TSJ (JSON) file
	/// that contains a map made in Tiled
	/// and exported to JSON format.
	/// </summary>
	public class TiledMap
	{
		public int compressionLevel;
		public int height;
		public bool infinite;

		public List<MapLayer> layers;

		public int nextlayerid;
		public int nextobjectid;
		public string orientation;
		public string renderorder;
		public string tiledversion;
		public int tileheight;    //  "tileheight":16,

		public List<Tileset> tilesets;
		public List<TilesetFile> tilesetFiles;

		public int tilewidth;     //  "tilewidth":16,
		public string type;
		public string version;
		public int width;
		public void PrintToConsole()
		{
			string sizeString = infinite ? "Infinite" : $"{width} x {height} tiles.";
			string[] pairs = {"Type", type, "Size", sizeString, "Tile size (px)", $"{tilewidth} x {tileheight}" };
			PairWriter.PrintToConsole("TileMap", pairs, 0);

			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine("\tTile set files:-------");
			int index = 0;
			foreach (TilesetFile tf in tilesetFiles)
			{
				Console.ForegroundColor = ConsoleColor.Cyan;
				Console.Write($"\tTile set {index}:");
				tf.PrintToConsole(2);
				index++;
			}
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine("\tLayers:---------------");
			index = 0;
			foreach (MapLayer lr in layers)
			{
				Console.ForegroundColor = ConsoleColor.Cyan;
				Console.Write($"\tLayer {index}:");
				lr.PrintToConsole(2);
				index++;
			}
		}
	}
}
