using Microsoft.MixedReality.Toolkit.UX;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Schema;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ProcedureManager : MonoBehaviour
{
	public int numProcedures;
	public VirtualizedScrollRectList procedureScrollList;
	public TextMeshProUGUI stepReader;
	public Color activeStepColor = Color.yellow;
	
	public string activeStepInstruction { get; set; }
	public int activeStepIndex { get; set; }
	public Procedure activeProcedure { get; private set; }
	public Scrollbar stepScrollbar;
	public GameObject stepReaderMenu;

	private float startTime;
	private bool isSettingScroll;
	private List<Procedure> procedures = new List<Procedure>();
	List<Tuple<string, int>> activeSteps = new List<Tuple<string, int>>();
	private List<string> activeRichText = new List<string>();

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

		activeProcedure = procedures[index];

		activeSteps.Clear();
		activeRichText.Clear();

		foreach (ProcedureStep step in procedures[index].steps) {
			activeSteps.Add(new Tuple<string, int>(step.instruction, 0));
			if (step.substeps.Count != 0) activeSteps.AddRange(RetrieveSubsteps(step, 1));
		}

		for (int i = 0; i < activeSteps.Count; i++) {
			GenerateRichText(i);
		}

		UpdateReaderText();
	}

	public void NextStep() {
		GoToStep(activeProcedure.currentStep + 1);
	}

	public void PrevStep() {
		GoToStep(activeProcedure.currentStep - 1);
	}

	public void GoToStep(int stepNum) {
		if (stepNum >= 0 && stepNum < activeProcedure.totalSteps) {
			int temp = activeProcedure.currentStep;
			activeProcedure.currentStep = stepNum;
			GenerateRichText(temp);
			GenerateRichText(stepNum);

			UpdateReaderText();

			// Updates step reader's scroll bar as you progress through tasks such that it's always visible.
			float linesOnReader = 12;

			if (activeProcedure.currentStep / linesOnReader >= 0.5f && (activeProcedure.totalSteps - activeProcedure.currentStep) / linesOnReader >= 0.5f) {

				float perc = (activeProcedure.currentStep - linesOnReader / 2) / (activeProcedure.totalSteps - linesOnReader);
				stepScrollbar.value = Mathf.Clamp01(1 - perc);
			}
		}
	}

	private void GenerateRichText(int index) {
		string line = "";
		if (index != 0) {
			line += '\n';
		}

		if (index == activeProcedure.currentStep) {
			line += "<color=#" + activeStepColor.ToHexString() + ">";
		}

		line += (index + 1) + "<indent=9%>|<indent=14%>";

		line += new String(' ', activeSteps[index].Item2 * 5) + "> " + activeSteps[index].Item1;

		line += "</indent></indent>";

		if (index == activeProcedure.currentStep) {
			line += "</color>";
		}

		if (activeRichText.Count - 1 >= index) {
			activeRichText[index] = line;
		} else {
			activeRichText.Add(line);
		}
	}

	private void UpdateReaderText() {

		string readerText = "";

		foreach (string line in activeRichText) {
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