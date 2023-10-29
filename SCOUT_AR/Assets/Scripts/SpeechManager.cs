using Microsoft.MixedReality.Toolkit.Subsystems;
using Microsoft.MixedReality.Toolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SpeechManager : MonoBehaviour
{
    public KeywordCommand[] voiceCommands;

    private KeywordRecognitionSubsystem keywordRecognitionSubsystem;

    // Start is called before the first frame update
    void Start()
    {
		keywordRecognitionSubsystem = XRSubsystemHelpers.GetFirstRunningSubsystem<KeywordRecognitionSubsystem>();

		if (keywordRecognitionSubsystem != null) {
            foreach (KeywordCommand command in voiceCommands) {
                foreach (string keyword in command.keywords) {
					keywordRecognitionSubsystem.CreateOrGetEventForKeyword(keyword).AddListener(() => command.keywordCalled.Invoke());
				}
            }
		}
	}

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartPhraseRecognition() {
        keywordRecognitionSubsystem.Start();
    }

	public void StopPhraseRecognition() {
		keywordRecognitionSubsystem.Stop();
	}
}

[System.Serializable]
public struct KeywordCommand {
    public string name;
    public string[] keywords;
    public UnityEvent keywordCalled;
}