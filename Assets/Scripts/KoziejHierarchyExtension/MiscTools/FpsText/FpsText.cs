using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AWI.Test {
	 public class FpsText : MonoBehaviour {
		  public Text text;

		  void Update() {
				text.text = (1 / Time.smoothDeltaTime).ToString("000.00");
		  }
	 }
}
