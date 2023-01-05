using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[Serializable]
public class BattleRecord
{
  public DateTime datetime;

  public int turnCount;
  public int defeatCount;

  public int correctAnswers;
  public int totalAnswers;
  public float avgAnswerTime;
}

[Serializable]
public class SaveData
{
  public List<BattleRecord> battleRecordList;
}

public class UtilSave : MonoBehaviour
{

  // public methods

  public static SaveData UpdateMyValues(SaveData saveData) {
	BattleRecord newRecord = new BattleRecord {
	  datetime = DateTime.Now,
	  
	  turnCount = UnityEngine.Random.Range(0, 101),
	  defeatCount = UnityEngine.Random.Range(0, 101),

	  totalAnswers = UnityEngine.Random.Range(0, 101),
	  correctAnswers = UnityEngine.Random.Range(0, 101),

	  avgAnswerTime = UnityEngine.Random.Range(0f, 101f),
	};

	saveData.battleRecordList.Add(newRecord);

	return saveData;
  }

  public static void SaveGame(SaveData saveData) {
	BinaryFormatter bf = new BinaryFormatter();

	FileStream file = File.Create(Application.persistentDataPath + "/MySaveData.dat");

	bf.Serialize(file, saveData);

	file.Close();
  }

  public static SaveData LoadGame() {
	SaveData saveData = new SaveData {
	  battleRecordList = new List<BattleRecord>(),
	};

	if (!File.Exists(Application.persistentDataPath + "/MySaveData.dat")) {
	  return saveData;
	}

	BinaryFormatter bf = new BinaryFormatter();

	FileStream file = File.Open(Application.persistentDataPath + "/MySaveData.dat", FileMode.Open);
	  
	SaveData data = (SaveData)bf.Deserialize(file);

	file.Close();

	saveData = data;

	return saveData;
  }

}
