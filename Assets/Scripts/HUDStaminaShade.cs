using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class HUDStaminaShade : MonoBehaviour {

	[SerializeField]
	private Image shadeImage;

	private bool loosingShade = false;
	
	private float _currentStamina = 100f;

	public float currentStamina {
		get {
			return _currentStamina;
		}
		set {
			_currentStamina = value;
			
			if (loosingShade)
				return;
			
			if (shadeImage.fillAmount < _currentStamina / 100f) {
				shadeImage.fillAmount = value / 100f;
				return;
			}
			
			loosingShade = false;
			Invoke("EnableLoosingShade", 1f);
		}
	}

	private void EnableLoosingShade() {
		loosingShade = true;
	}

	private void Update() {
		if (!loosingShade) return;

		shadeImage.fillAmount -= Time.deltaTime * 0.3f;
		if (shadeImage.fillAmount < _currentStamina / 100f) {
			loosingShade = false;
		}
	}
}
