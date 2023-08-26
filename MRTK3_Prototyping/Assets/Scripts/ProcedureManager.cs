using Microsoft.MixedReality.Toolkit.UX;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Schema;
using UnityEngine;

public class ProcedureManager : MonoBehaviour
{
	public int numProcedures;
	public VirtualizedScrollRectList procedureScrollList;

	private float startTime;
	private bool isSettingScroll;
	private List<Procedure> procedures = new List<Procedure>();

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
		procedureScrollList.SetItemCount(numProcedures);

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
		}

		ProcedureSelectorButton button = obj.GetComponent<ProcedureSelectorButton>();
		button.nameText.text = index.ToString();
	}

	private void DepopulateProcedureButton(GameObject obj, int index) {

	}
}