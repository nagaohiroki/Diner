using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
[System.Serializable]
public class TestType
{
	public enum Cost
	{
		Meat
	}
	public string id;
	public string name;
	public string type;
	public Cost cost;
	public override string ToString()
	{
		return JsonUtility.ToJson(this, true);
	}
}
[System.Serializable]
public class TestTypeList
{
	public List<TestType> data;
}
public class SpreadSheet : MonoBehaviour
{
	void Start()
	{
		var sheet = "1zNdlU6Y8eumEI9GXs4DSIh-gh3TvNJJt8uX83iT0pBA";
		var page = "0";
		var url = $"https://docs.google.com/spreadsheets/d/{sheet}/export?format=tsv&gid={page}";
		StartCoroutine(GetSheets(url));
	}
	IEnumerator GetSheets(string inURL)
	{
		var request = UnityWebRequest.Get(inURL);
		yield return request.SendWebRequest();
		var text = request.downloadHandler.text;
		var list = TSVtoObject<TestType>(text);
	}
	List<T> TSVtoObject<T>(string inText, string inSeparator = "\t", string inComment = "#")
	{
		var result = new List<T>();
		var rowList = inText.Split("\r\n");
		var header = rowList[0].Split('\t');
		for(int row = 1; row < rowList.Length; ++row)
		{
			var rowText = rowList[row];
			if(rowText.StartsWith(inComment))
			{
				continue;
			}
			var rowData = rowText.Split(inSeparator);
			var jsonStr = "{";
			for(int col = 0; col < header.Length; col++)
			{
				var key = header[col];
				if(!key.StartsWith(inComment))
				{
					jsonStr += $"\"{key}\":\"{rowData[col]}\"";
				}
				if(col < header.Length - 1)
				{
					jsonStr += ",";
				}
			}
			jsonStr += "}";
			result.Add(JsonUtility.FromJson<T>(jsonStr));
		}
		return result;
	}
}
