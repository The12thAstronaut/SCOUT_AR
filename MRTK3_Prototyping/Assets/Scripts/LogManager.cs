using Microsoft.MixedReality.Toolkit.UX;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class LogManager : MonoBehaviour
{
	public VirtualizedScrollRectList logScrollList;
	public TextMeshProUGUI logReader;

	public int activeLogIndex { get; set; }
	public Log activeLog { get; private set; }
	public int numLogs { get; set; }

	private List<Log> logs = new List<Log>();
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
        
    }

	public void LoadLogs() {

		string path = FileHelper.MakePath("Assets", "Data", "Logs");
		string[] files = Directory.GetFiles(path, "*.txt");

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
			//button.stepText.text = logs[index - 1].currentStep + "/" + logs[index - 1].totalSteps;
		}
	}

	private void DepopulateLogButton(GameObject obj, int index) {

	}
}
