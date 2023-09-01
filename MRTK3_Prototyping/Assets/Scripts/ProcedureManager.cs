using Microsoft.MixedReality.Toolkit.UX;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Schema;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class ProcedureManager : MonoBehaviour
{
	public int numProcedures;
	public VirtualizedScrollRectList procedureScrollList;
	public TextMeshProUGUI stepReader;
	public Color activeStepColor = Color.yellow;

	private float startTime;
	private bool isSettingScroll;
	private List<Procedure> procedures = new List<Procedure>();
	private List<string> activeSteps = new List<string>();

	// Start is called before the first frame update
	void Start()
    {
		procedureScrollList.OnVisible = PopulateProcedureButton;
		procedureScrollList.OnInvisible = DepopulateProcedureButton;

		LoadProcedures();
	}

    // Update is called once per frame
    void Update()
    {
		
	}

	public void LoadProcedures() {
		procedureScrollList.SetItemCount(numProcedures + 1);

		string path = FileHelper.MakePath("Assets", "Data", "Procedures");
		string[] files = Directory.GetFiles(path, "*.txt");

		foreach (string file in files) {
			procedures.Add(new Procedure(file));
		}
	}

	private void PopulateProcedureButton(GameObject obj, int index) {

		if (index == 0) { // Scroll views are set up with a "fake" first element to fix top element pop-in.
			obj.SetActive(false);
			return;
		} else {
			obj.SetActive(true);

			ProcedureSelectorButton button = obj.GetComponent<ProcedureSelectorButton>();
			button.procedureManager = this;
			button.procedureIndex = index - 1;
			button.nameText.text = procedures[index - 1].procedureName;
			button.stepText.text = procedures[index - 1].currentStep + "/" + procedures[index - 1].totalSteps;
		}
	}

	private void DepopulateProcedureButton(GameObject obj, int index) {

	}

	public void ActivateProcedure(int index) {
		List<Tuple<string, int>> stepTexts = new List<Tuple<string, int>>();
		List<int> tabLevel = new List<int>();

		foreach (ProcedureStep step in procedures[index].steps) {
			stepTexts.Add(new Tuple<string, int>(step.instruction, 0));
			if (step.substeps.Count != 0) stepTexts.AddRange(RetrieveSubsteps(step, 1));
		}

		activeSteps.Clear();

		for (int i = 0; i < stepTexts.Count; i++) {
			string line = "";
			if (i != 0) {
				line += '\n';
			}

			if (i == procedures[index].currentStep) {
				line += "<color=#" + activeStepColor.ToHexString() + ">";
			}

			line += i + "<indent=9%>|<indent=14%>";

			line += new String(' ', stepTexts[i].Item2 * 5) + "> " + stepTexts[i].Item1;

			line += "</indent></indent>";

			if (i == procedures[index].currentStep) {
				line += "</color>";
			}
			activeSteps.Add(line);
		}

		string readerText = "";

		foreach (string line in activeSteps) {
			readerText += line;
		}

		stepReader.text = readerText;
	}

	private List<Tuple<string, int>> RetrieveSubsteps(ProcedureStep step, int tabLevel) {
		List<Tuple<string, int>> steps = new List<Tuple<string, int>>();

		foreach (ProcedureStep substep in step.substeps) {
			steps.Add(new Tuple<string, int>(substep.instruction, tabLevel));
			if (substep.substeps.Count != 0) steps.AddRange(RetrieveSubsteps(substep, tabLevel + 1));
		}

		return steps;
	}
}