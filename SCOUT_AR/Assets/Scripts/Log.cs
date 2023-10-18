using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Threading.Tasks;
using System;
#if WINDOWS_UWP
using Windows.Storage;
#endif

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

		LoadLog(filePath);
	}

	public Log(string logName, string logContentText, string dateTime) {
		this.logName = logName;
		this.logContentText = logContentText;
		this.dateTime = dateTime;
	}

	private async void LoadLog(string filePath) {
#if WINDOWS_UWP
		StorageFolder storageFolder = await ApplicationData.Current.LocalFolder.GetFolderAsync("Logs");

		StorageFile sampleFile = await storageFolder.GetFileAsync(filePath);

		var stream = await sampleFile.OpenAsync(FileAccessMode.Read);
		ulong size = stream.Size;

		using (var inputStream = stream.GetInputStreamAt(0)) {
			using (var dataReader = new Windows.Storage.Streams.DataReader(inputStream)) {
				dataReader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
                dataReader.ByteOrder = Windows.Storage.Streams.ByteOrder.LittleEndian;
				uint numBytesLoaded = await dataReader.LoadAsync((uint)size);
				string text = dataReader.ReadString(numBytesLoaded);
				logContentText = text;
				logName = "test";
			}
		}
#endif
#if UNITY_EDITOR
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
#endif
	}

	public void UpdateContent(string logName, string logContentText, string dateTime) {
		this.logName = logName;
		this.logContentText = logContentText;
		this.dateTime = dateTime;
	}
}
