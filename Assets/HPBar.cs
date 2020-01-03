using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour {
	
	public Image hpBarImg;

	public void SetHpPercentage(float fillPercentage) {
		hpBarImg.fillAmount = fillPercentage;
	}
	
}
