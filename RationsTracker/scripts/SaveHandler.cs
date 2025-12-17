using System.Linq;
using Godot;

namespace Handlers
{
	public static class SaveLoadHandler
	{
		static public Godot.Collections.Array<string> CreateSetsNameList()
		{
			string[] setsList = [];

			if (DirAccess.Open(Globals.Paths.SaveSetsPlot) != null)
			{
				DirAccess dirAccess = DirAccess.Open(Globals.Paths.SaveSetsPlot);
				setsList = dirAccess.GetFiles();
			}

			setsList = [.. setsList.Select(
				fileName =>
					{
						int startIndex = fileName.IndexOf('_') + 1;
						int endIndex = fileName.LastIndexOf('.');
						return fileName[startIndex..endIndex].Replace("_", " ");
					}
				)];

			return [.. setsList];
		}
		static public void SaveSet(PortionsSetRes portionsSetRes)
		{
			string fileName = portionsSetRes.SetName.Replace(" ", "_");
			string filePath = System.IO.Path.Combine(Globals.Paths.SaveSetsPlot, $"set_{fileName}.tres");
			ResourceSaver.Save(portionsSetRes, filePath);
		}
		static public PortionsSetRes LoadSet(string name)
		{
			string fileName = name.Replace(" ", "_");
			string filePath = System.IO.Path.Combine(Globals.Paths.SaveSetsPlot, $"set_{fileName}.tres");
			if (!ResourceLoader.Exists(filePath))
			{
				return null;
			}

			PortionsSetRes portionsSetRes = (PortionsSetRes)ResourceLoader.Load(filePath, cacheMode: ResourceLoader.CacheMode.Ignore);
			return portionsSetRes;
		}
		static public void RemoveSet(string name)
		{
			string filePath = System.IO.Path.Combine(Globals.Paths.SaveSetsPlot, $"set_{name}.tres");
			if (!ResourceLoader.Exists(filePath))
				return;

			DirAccess.RemoveAbsolute(filePath);
		}
		static public void ChangeSetName(string oldName, string newName)
		{
			string oldFileName = oldName.Replace(" ", "_");
			string oldFilePath = System.IO.Path.Combine(Globals.Paths.SaveSetsPlot, $"set_{oldFileName}.tres");
			if (!ResourceLoader.Exists(oldFilePath))
				return;

			string newFileName = newName.Replace(" ", "_");
			string newFilePath = System.IO.Path.Combine(Globals.Paths.SaveSetsPlot, $"set_{newFileName}.tres");
			DirAccess.RenameAbsolute(oldFilePath, newFilePath);
		}
		static public ConfigFile LoadMainConfig()
		{
			string configFilePath = System.IO.Path.Combine(Globals.Paths.SaveConfigs, $"main_config.cfg");
			ConfigFile config = new ConfigFile();
			Error err = config.Load(configFilePath);
			if (err != Error.Ok) { return null; }

			return config;
		}
		static public void CreateMainConfig()
		{
			if (DirAccess.Open(Globals.Paths.SaveConfigs) != null)
			{
				return;
			}
			DirAccess dirAccess = DirAccess.Open("user://");
			dirAccess.MakeDir("configs");

			string configFilePath = System.IO.Path.Combine(Globals.Paths.SaveConfigs, $"main_config.cfg");
			ConfigFile config = new ConfigFile();




			config.Save(configFilePath);
		}
		static public void CreateSaveSetsDir()
		{
			if (DirAccess.Open(Globals.Paths.SaveSetsPlot) != null)
			{
				return;
			}
			DirAccess dirAccess = DirAccess.Open("user://");
			dirAccess.MakeDir("sets");
		}
	}
}