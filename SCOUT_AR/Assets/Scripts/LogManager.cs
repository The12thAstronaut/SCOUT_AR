using Microsoft.MixedReality.Toolkit.UX;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using TMPro;
using UnityEditor;
using UnityEngine;

public class LogManager : MonoBehaviour
{
	public VirtualizedScrollRectList logScrollList;
	public TextMeshProUGUI logReader;
	public TMP_InputField logSubjectInputField;
	public TMP_InputField logContentInputField;
	public TextMeshProUGUI logDateTimeDisplay;
	public Transform logToggleCollection;
	public PressableButton closeCreatorButton;

	public int activeLogIndex { get; set; }
	public Log activeLog { get; private set; }
	public int numLogs { get; set; }
	public bool isEdittingLog { get; set; } = false;

	private List<Log> logs = new List<Log>();
	private string dateTime = "";
	private bool updateReader = false;

	// Start is called before the first frame update
	void Start()
    {
		logScrollList.OnVisible = PopulateLogButton;
		logScrollList.OnInvisible = DepopulateLogButton;

		LoadLogs();
	}

    // Update is called once per frame
    void Update()
    {
        if (!isEdittingLog && logDateTimeDisplay.gameObject.activeInHierarchy) {
			dateTime = DateTime.Now.ToString("yyyy-MM-dd");
			dateTime += " " + DateTime.Now.ToString("HH-mm-ss");
			logDateTimeDisplay.text = dateTime;
		}

		if (updateReader && logReader.gameObject.activeInHierarchy) {
			SelectLog(activeLogIndex);

			foreach (Transform child in logToggleCollection) {
				LogEntrySelectorButton button = child.GetComponent<LogEntrySelectorButton>();
				if (button.logIndex == activeLogIndex) {
					button.nameText.text = activeLog.logName;
				}
			}

			updateReader = false;
		}
    }

	public void LoadLogs() {

		string path = FileHelper.MakePath("Assets", "Data", "Logs");
		string[] files = Directory.GetFiles(path, "*.txt");

		logs.Clear();
		numLogs = Directory.GetFiles(path).Length / 2;
		logScrollList.SetItemCount(numLogs + 1);

		foreach (string file in files) {
			logs.Add(new Log(file));
		}
	}

	private void PopulateLogButton(GameObject obj, int index) {

		if (index == 0) { // Scroll views are set up with a "fake" first element to fix top element pop-in.
			obj.SetActive(false);
			return;
		} else {
			obj.SetActive(true);

			LogEntrySelectorButton button = obj.GetComponent<LogEntrySelectorButton>();
			button.logManager = this;
			button.logIndex = index - 1;
			button.nameText.text = logs[index - 1].logName;
			string[] dateTime = logs[index - 1].dateTime.Split(' ');
			button.dateTimeText.text = dateTime[0] + " " + dateTime[1].Replace('-', ':');

			if (activeLogIndex != default(int) && button.logIndex == activeLogIndex) {
				obj.GetComponent<PressableButton>().ForceSetToggled(true);
			}
		}
	}

	private void DepopulateLogButton(GameObject obj, int index) {
		obj.GetComponent<PressableButton>().ForceSetToggled(false);
		obj.transform.Translate(0, 30000, 0);
	}

	public void SaveLogEntry() {
#if WINDOWS_UWP
		string path = Application.persistentDataPath + "/Logs/" + logDateTimeDisplay.text.Replace(' ', '_').Replace(':', '-') + ".txt";

		StreamWriter writer = new StreamWriter(path, false);

		writer.WriteLine(logSubjectInputField.text);
		writer.WriteLine(logContentInputField.text);

		writer.Close();
#endif
#if UNITY_EDITOR
		string path = FileHelper.MakePath("Assets", "Data", "Logs", logDateTimeDisplay.text.Replace(' ', '_').Replace(':', '-') + ".txt");

		if (!Directory.Exists(FileHelper.MakePath("Assets", "Data", "Logs"))) {
			Directory.CreateDirectory(FileHelper.MakePath("Assets", "Data", "Logs"));
		}

		StreamWriter writer = new StreamWriter(path, false);

		writer.WriteLine(logSubjectInputField.text);
		writer.WriteLine(logContentInputField.text);

		writer.Close();

		AssetDatabase.ImportAsset(path);
#endif
		if (!isEdittingLog) {
			numLogs++;
			logs.Add(new Log(logSubjectInputField.text, logContentInputField.text, logDateTimeDisplay.text));
			logScrollList.SetItemCount(numLogs + 1);
		} else {
			activeLog.UpdateContent(logSubjectInputField.text, logContentInputField.text, logDateTimeDisplay.text);
			updateReader = true;
			isEdittingLog = false;
		}
	}

	public void SelectLog(int index) {
		closeCreatorButton.ForceSetToggled(true);

		activeLogIndex = index;
		activeLog = logs[index];

		foreach (Transform child in logToggleCollection) {
			if (child.GetComponent<LogEntrySelectorButton>().logIndex != index) child.GetComponent<PressableButton>().ForceSetToggled(false);
		}

		dateTime = DateTime.Now.ToString("yyyy-MM-dd");
		dateTime += " " + DateTime.Now.ToString("HH:mm:ss");

		string readerText = "";

		readerText += $"<u><size=6><color=#A4A4A4><indent=4%>Logged: {activeLog.dateTime}<indent=50%>Opened: {dateTime}</indent></indent></color></size></u>";

		readerText += $"\n\n<u><color=#FFFFFF>Subject: <size=140%><b>{activeLog.logName}</b></size></u>";

		readerText += $"\n{activeLog.logContentText}";

		logReader.text = readerText;
	}

	public void EditLog() {
		isEdittingLog = true;

		logSubjectInputField.text = activeLog.logName;
		logContentInputField.text = activeLog.logContentText;
		logDateTimeDisplay.text = activeLog.dateTime;
	}
}
