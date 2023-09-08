using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Log
{
	public int index;
	public string logName { get; private set; }
	public string logContentText { get; private set; }
	public string dateTime { get; private set; }
	public bool imageAttached { get; private set; }
	public bool videoAttached { get; private set; }

	public Log(string filePath) {
		string[] dateInfo = filePath.Remove(filePath.Length - 4).Substring(filePath.LastIndexOf('\\') + 1).Split('_');

		dateTime = dateInfo[0] + " " + dateInfo[1].Replace('-', ':');

		using (StreamReader reader = new StreamReader(filePath)) {

			string line;
			int i = 0;

			while ((line = reader.ReadLine()) != null) {
				if (i == 0) {
					logName = line.TrimEnd();
				} else if (i == 1) {
					logContentText = line.TrimEnd();
				}

				i++;
			}

		}
	}

	public Log(string logName, string logContentText, string dateTime) {
		this.logName = logName;
		this.logContentText = logContentText;
		this.dateTime = dateTime;
	}
}
