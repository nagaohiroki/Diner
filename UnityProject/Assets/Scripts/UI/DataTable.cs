using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using System.Collections;
using Unity.EditorCoroutines.Editor;
using UnityEngine.Networking;
#endif
public class DataTable : MonoBehaviour
{
	Dictionary<string, Dictionary<string, string>> mTextDict;
	[SerializeField]
	string mText;
#if UNITY_EDITOR
	[SerializeField]
	string sheetId;
	[SerializeField]
	int page;
	[ContextMenu("Load")]
	public void Load()
	{
		var url = $"https://docs.google.com/spreadsheets/d/{sheetId}/export?format=tsv&gid={page}";
		EditorCoroutineUtility.StartCoroutine(GetSheets(url), this);
	}
	IEnumerator GetSheets(string inURL)
	{
		Debug.Log($"Start : {inURL}");
		var request = UnityWebRequest.Get(inURL);
		yield return request.SendWebRequest();
		mText = request.downloadHandler.text;
		Debug.Log($"Done : {inURL}");
	}
#endif
	public string GetText(string inLanguage, string inId)
	{
		if(!mTextDict.TryGetValue(inId, out var dict))
		{
			return $"N/A:{inId}";
		}
		if(dict.TryGetValue(inLanguage, out var text))
		{
			return text;
		}
		if(dict.TryGetValue("en", out var enText))
		{
			return enText;
		}
		return $"N/A:{inId}";
	}
	void Build(string inText, string inSeparator = "\t", string inComment = "#")
	{
		mTextDict = new Dictionary<string, Dictionary<string, string>>();
		var rowList = inText.Split("\r\n");
		var header = rowList[0].Split(inSeparator);
		for(int row = 1; row < rowList.Length; ++row)
		{
			var rowText = rowList[row];
			if(rowText.StartsWith(inComment))
			{
				continue;
			}
			var rowData = rowText.Split(inSeparator);
			var table = new Dictionary<string, string>();
			for(int col = 1; col < header.Length; col++)
			{
				table.Add(header[col], rowData[col]);
			}
			mTextDict.Add(rowData[0], table);
		}
	}
}
