using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class HUDStaminaShade : MonoBehaviour {

	[SerializeField] private Image shadeImage;
	[SerializeField] private float shadeLoosingTimeout = 1f;
	[SerializeField, Range(0f, 3f)] private float shadeLoosingPerSec = 0.6f;

	private bool loosingShade = false;
	
	private float _currentStamina = 100f;
	
	private float maxStamina => GM.projectConstants.unit.stamina.maxStamina;
	
	public float currentStamina {
		get {
			return _currentStamina;
		}
		set {
			_currentStamina = value;
			
			if (loosingShade)
				return;
			
			if (shadeImage.fillAmount < _currentStamina / maxStamina) {
				shadeImage.fillAmount = value / maxStamina;
				return;
			}
			
			loosingShade = false;
			Invoke("EnableLoosingShade", shadeLoosingTimeout);
		}
	}

	private void EnableLoosingShade() {
		loosingShade = true;
	}

	private void Update() {
		if (!loosingShade) return;

		shadeImage.fillAmount -= Time.deltaTime * shadeLoosingPerSec;
		if (shadeImage.fillAmount < _currentStamina / maxStamina) {
			loosingShade = false;
		}
	}
}
