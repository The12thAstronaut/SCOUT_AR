using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class Procedure
{
	public int index;
	public string procedureName;
	public int currentStep;
	public int totalSteps;

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

		

		using (StreamReader reader = new StreamReader(filePath)) {

			string[] fileLines = new string[File.ReadAllLines(filePath).Length];
			string line;
			int i = 0;

			while ((line = reader.ReadLine()) != null) {
				fileLines[i] = line;
				i++;
			}
		}
	}
}

public class Step {
	public string text;
	public Step substep;


}