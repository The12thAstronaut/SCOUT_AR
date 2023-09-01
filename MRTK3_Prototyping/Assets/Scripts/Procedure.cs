using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class Procedure
{
	public int index;
	public string procedureName { get; private set; }
	public int currentStep;
	public int totalSteps { get; private set; }
	public List<ProcedureStep> steps;

	public Procedure(string filePath) {

		string[] info = filePath.Substring(filePath.LastIndexOf('\\') + 1).Split('_');
		info[info.Length - 1] = info[info.Length - 1].Remove(info[info.Length - 1].Length - 4);

		currentStep = 0;

		index = int.Parse(info[0]);

		for (int i = 1; i < info.Length - 1; i++) {
			procedureName += info[i] + " ";
		}
		procedureName.TrimEnd();

		totalSteps = int.Parse(info[info.Length - 1]);
		steps = new List<ProcedureStep>();
		

		using (StreamReader reader = new StreamReader(filePath)) {

			string[] fileLines = new string[File.ReadAllLines(filePath).Length];
			string line;
			int i = 0;
			//int prevTabLvl = 0;
			//ProcedureStep lastProcedure = null;
			//List<ProcedureStep> lastParent = null;
			//List<ProcedureStep> lastHigherParent = null;

			int currentStep = 0;

			while ((line = reader.ReadLine()) != null) {
				fileLines[i] = line;

				int tabLevel = 0;
				foreach (Char c in line) {
					if (c == '\t') tabLevel++;
				}

				if (tabLevel == 0) {
					steps.Add(new ProcedureStep(line.Trim()));
					currentStep++;
				} else if (tabLevel == 1) {
					steps[currentStep - 1].substeps.Add(new ProcedureStep(line.Trim()));
				} else if (tabLevel == 2) {
					steps[currentStep - 1].substeps[steps[currentStep - 1].substeps.Count - 1].substeps.Add(new ProcedureStep(line.Trim()));
				}

				/*if (i == 0) {
					steps.Add(new ProcedureStep(line.Trim()));
					lastProcedure = steps[i];
					lastParent = steps;
				}

				if (tabLevel > prevTabLvl) {
					lastProcedure.substeps.Add(new ProcedureStep(line.Trim()));
					lastProcedure = lastProcedure.substeps[lastProcedure.substeps.Count - 1];
					lastHigherParent = lastParent;
					lastParent = lastProcedure.substeps;
				} else if (tabLevel == prevTabLvl) {
					lastParent.Add(new ProcedureStep(line.Trim()));
					lastProcedure = lastParent[lastParent.Count - 1];
				} else if (tabLevel < prevTabLvl) {
					lastHigherParent.Add(new ProcedureStep(line.Trim()));
					lastProcedure = lastHigherParent[lastHigherParent.Count - 1];
					lastParent = lastHigherParent;

				}*/


				//ParseLogic(steps[].text);
				//prevTabLvl = tabLevel;
				i++;
			}
		}
	}

	
}

public class ProcedureStep {
	public string text;
	public string instruction;
	public List<ProcedureStep> substeps;

	public ProcedureStep(string text) {
		this.text = text;
		substeps = new List<ProcedureStep>();

		ParseLogic();
	}

	private void ParseLogic() {
		string logic = text.Remove(text.IndexOf(">") + 1).Trim();
		instruction = text.Substring(text.IndexOf(">") + 1).Trim();

		char[] charsToTrim = { '<', '>' };
		string[] logicCommands = logic.Trim(charsToTrim).Split(", ");

		foreach (string cmd in logicCommands) {
			if (cmd.Contains('=')) {
				string[] split = cmd.Split('=');
				switch (split[0]) {
					case "RETURN_TO":

					case "SKIP_TO":

					case "REPEAT":

					default: break;
				}
			} else {
				switch (cmd) {
					case "END":

					case "HIDDEN":

					default: break;
				}
			}
		}
	}
}