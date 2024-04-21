using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

// Fixes UI layout placement errors
public class LayoutRefreshOnStart : MonoBehaviour
{

    void Start()
    {
		foreach (Canvas canvas in transform.GetComponentsInChildren<Canvas>()) {
            canvas.scaleFactor = 0.9999f;
        }
        Refresh();
    }

    private async void Refresh() {
        await Task.Delay(200);
		foreach (Canvas canvas in transform.GetComponentsInChildren<Canvas>()) {
            canvas.scaleFactor = 1.0f;
		}
	}
}
