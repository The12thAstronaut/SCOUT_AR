using Microsoft.MixedReality.Toolkit.UX;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LogEntrySelectorButton : MonoBehaviour {

	public TextMeshProUGUI nameText;
	//public TextMeshProUGUI stepText;
	public TextMeshProUGUI dateTimeText;
	public int logIndex { get; set; }

	public LogManager logManager { get; set; }

	// Start is called before the first frame update
	void Start() {
		transform.localRotation = Quaternion.identity;
	}

	// Update is called once per frame
	void Update()
    {
        
    }

	public void SelectLog() {
		logManager.activeLogIndex = logIndex;

		foreach (Transform child in logManager.logToggleCollection) {
			if (child.GetComponent<LogEntrySelectorButton>().logIndex != logIndex) child.GetComponent<PressableButton>().ForceSetToggled(false);
		}
	}
}
