using System;
using System.IO;
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


	public static void SaveGame(bool invokeEvent = true)
	{
		if (!isInitialized)
		{
			InitializeSaveSystem();
		}

		localData.lastSavedTime = ($"{DateTime.UtcNow}");
		string json = JsonUtility.ToJson(localData);
		File.WriteAllText(savePath + "/" + saveName + ".json", json);
		if (invokeEvent)
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
		}

		if (Directory.Exists(savePath) && File.Exists(savePath + "/" + saveName + ".json"))
		{
			string json = File.ReadAllText(savePath + "/" + saveName + ".json");
			localData = JsonUtility.FromJson<SaveData>(json);
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
			string jsonFile = JsonUtility.ToJson(localData);
			File.WriteAllText(savePath + "/" + saveName + ".json", jsonFile);
			Debug.Log($"<color=magenta> SaveSystem </color> loaded <color=white>clean</color> save from {savePath + "/"}");
			localData.saveFileLocation = $"{savePath}/{saveName}.json";
			SaveGame(false);
			OnCleanSaveLoaded?.Invoke(localData);
		}
	}

	public static void ClearSavedData()
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
				Debug.Log($"<color=magenta> SaveSystem </color> Loading a Clean save...");
				File.Delete(savePath + "/" + saveName + ".json");
			}
		}

		LoadGame();
	}

	public static string GetSaveDataString()
	{
		return JsonUtility.ToJson(localData);
	}

	public static void OverrideSaveData(string jsonSaveDataType)
	{
		File.WriteAllText(savePath + "/" + saveName + ".json", jsonSaveDataType);
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
