using UnityEngine;

public abstract class AIStateBase {

	protected abstract void Update();
	
	protected abstract void OnStateEnter();
	
	protected abstract void OnStateExit();
	
}