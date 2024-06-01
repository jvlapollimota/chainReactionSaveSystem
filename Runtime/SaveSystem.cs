using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

public static class SaveSystem
{

	private static SaveData localData = new SaveData();
	private static string saveName = "SaveGame";
	private static string savePath = "/savedGame";
	private static bool isInitialized;

	public delegate void OnSaveGameHandler(SaveData saveData);

	public static OnSaveGameHandler OnCleanSaveLoaded;
	public static OnSaveGameHandler OnSaveLoaded;
	public static OnSaveGameHandler OnSaveSucessfully;


	public static void SaveGame(bool invokeListeners = true)
	{
		if (!isInitialized)
		{
			InitializeSaveSystem();
		}

		localData.lastSavedTime = ($"{DateTime.UtcNow}");
		string json = JsonConvert.SerializeObject(localData);
		File.WriteAllText(savePath + "/" + saveName + ".json", json);
		if (invokeListeners)
		{
			Debug.Log($"<color=magenta> SaveSystem </color> Save");
			OnSaveSucessfully?.Invoke(localData);
		}
	}

	public static void LoadGame()
	{
		if (!isInitialized)
		{
			InitializeSaveSystem();
			return;
		}

		if (Directory.Exists(savePath) && File.Exists(savePath + "/" + saveName + ".json"))
		{
			string json = File.ReadAllText(savePath + "/" + saveName + ".json");
			localData = ConvertToSaveData(json);
			Debug.Log($"<color=magenta> SaveSystem </color> loaded existing save from {savePath + "/"}");
			localData.saveFileLocation = $"{savePath}/{saveName}.json";
			OnSaveLoaded?.Invoke(localData);
			
		}
		else
		{
			if (!Directory.Exists(savePath))
			{
				Directory.CreateDirectory(savePath);
			}
			localData = SetupNewSaveFile();
			Debug.Log($"<color=magenta> SaveSystem </color> Creating and loading a <color=white>clean</color> save in {savePath + "/"}");
			localData.saveFileLocation = $"{savePath}/{saveName}.json";
			SaveGame(false);
			OnCleanSaveLoaded?.Invoke(localData);
		}
	}

	public static void ClearSavedData(bool loadCleanSave = true)
	{
		if (!isInitialized)
		{
			InitializeSaveSystem();
		}

		if (Directory.Exists(savePath))
		{
			if (File.Exists(savePath + "/" + saveName + ".json"))
			{
				Debug.Log($"<color=magenta> SaveSystem </color> Deleted Save in {savePath + "/"}");
				File.Delete(savePath + "/" + saveName + ".json");
			}
		}
		if (loadCleanSave)
		{
			Debug.Log($"<color=magenta> SaveSystem </color> Loading a Clean save...");
			LoadGame();
		}
	}

	public static void OverrideSaveData(string jsonSaveDataType)
	{
		File.WriteAllText(savePath + "/" + saveName + ".json", jsonSaveDataType);
		LoadGame();
	}

	public static void OverrideSaveData(SaveData newSaveData)
	{
		localData = newSaveData;
		SaveGame();
		LoadGame();
	}
	
	private static void InitializeSaveSystem()
	{
		savePath = Application.persistentDataPath;
		isInitialized = true;
		LoadGame();
	}

	private static SaveData SetupNewSaveFile()
	{
		SaveData newSaveFile = new SaveData();
		
		//Here you set the default values of any save Data

		return newSaveFile;
	}
	
	public static SaveData ConvertToSaveData(string jsonString)
	{
		return JsonConvert.DeserializeObject<SaveData>(jsonString);
	}
	
	public static string ConvertSaveDataToJson(SaveData saveData = null)
	{
		if (saveData != null)
		{
			return JsonConvert.SerializeObject(saveData);
		}
		
		return JsonConvert.SerializeObject(localData);
	}
	public static SaveData Data
	{
		get
		{
			if (!isInitialized)
			{
				InitializeSaveSystem();
			}

			return localData;
		}
	}
}
