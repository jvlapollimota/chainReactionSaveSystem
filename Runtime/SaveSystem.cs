using System.IO;
using UnityEngine;

public static class SaveSystem
{

	private static SaveData localData = new SaveData();
	private const string saveName = "SaveGame";
	private static string savePath = "/savedGame";
	private static bool isInitialized;

	public delegate void OnSaveGameHandler(SaveData saveData);

	public static OnSaveGameHandler OnCleanSaveLoaded;
	public static OnSaveGameHandler OnSaveLoaded;
	public static OnSaveGameHandler OnSaveSucessfully;


	public static void SaveGame()
	{
		if (!isInitialized)
		{
			InitializeSaveSystem();
		}

		string json = JsonUtility.ToJson(localData);
		File.WriteAllText(savePath + "/" + saveName + ".json", json);

		OnSaveSucessfully?.Invoke(localData);
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
			Debug.Log($"<color=magenta> SaveSystem </color> loaded clean save from {savePath + "/"}");
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
		newSaveFile.masterVolume = 1;
		newSaveFile.soundEffectVolume = 0.8f;
		newSaveFile.soundtrackVolume = 0.8f;

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
