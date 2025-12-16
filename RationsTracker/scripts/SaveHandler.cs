using System.Linq;
using System.Linq;
using Godot;

namespace Handlers
{
	public static class SaveLoadHandler
	{
		static public string[] CreateSetsNameList()
		{
			string[] setsList = new string[] { };

			if (DirAccess.Open(Globals.Paths.SaveSetsPlot) != null)
			{
				DirAccess dirAccess = DirAccess.Open(Globals.Paths.SaveSetsPlot);
				setsList = dirAccess.GetFiles();
			}

			return setsList.Select(
				fileName =>
					{
						int startIndex = fileName.IndexOf('_') + 1;
						int endIndex = fileName.LastIndexOf('.');
						return fileName.Substring(startIndex, endIndex - startIndex);
					}
				).ToArray();
		}
		static public void SaveSet(Godot.Collections.Array<Portion> portions, string name = "")
		{
			PortionsSetRes portionsSetRes = new PortionsSetRes();
			portionsSetRes.SetName = name;

			foreach (Portion portion in portions)
			{
				portionsSetRes.PortionsResList.Add(portion.Info);
			}
			string file_path = System.IO.Path.Combine(Globals.Paths.SaveSetsPlot, $"set_{name}.tres");
			ResourceSaver.Save(portionsSetRes, file_path);
		}
		static public PortionsSetRes LoadSet(string name)
		{
			string file_path = System.IO.Path.Combine(Globals.Paths.SaveSetsPlot, $"set_{name}.tres");
			if (!ResourceLoader.Exists(file_path))
			{
				return null;
				return null;
			}

			PortionsSetRes portionsSetRes = (PortionsSetRes)ResourceLoader.Load(file_path, cacheMode: ResourceLoader.CacheMode.Ignore);
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
			static public ConfigFile LoadMainConfig()
			{
				string configFilePath = System.IO.Path.Combine(Globals.Paths.SaveConfigs, $"main_config.cfg");
				ConfigFile config = new ConfigFile();
				Error err = config.Load(configFilePath);
				if (err != Error.Ok) { return null; }

				return config;
				return config;
			}
			static public void CreateMainConfig()
			{
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

		}