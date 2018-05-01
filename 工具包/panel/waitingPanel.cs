using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class waitingPanel : MonoBehaviour {
    Text showText;
	// Use this for initialization
	void Awake () {
        showText = transform.Find("showText").GetComponent<Text>();

    }
    public void ShowInfo(string showMessage) {
        showText.text = showMessage;
    }
    // Update is called once per frame
    void Update () {
		
	}
}
