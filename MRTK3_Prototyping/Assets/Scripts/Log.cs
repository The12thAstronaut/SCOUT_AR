using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Log
{
	public int index;
	public string logName { get; private set; }

	public Log(string filePath) {
		string[] info = filePath.Substring(filePath.LastIndexOf('\\') + 1).Split('_');
		info[info.Length - 1] = info[info.Length - 1].Remove(info[info.Length - 1].Length - 4);

		index = int.Parse(info[0]);

		for (int i = 1; i < info.Length; i++) {
			logName += info[i] + " ";
		}
		logName.TrimEnd();
	}
}
