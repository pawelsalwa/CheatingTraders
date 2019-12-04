using UnityEngine;

[RequireComponent(typeof(BasicUnit))]
public class BotController : MonoBehaviour {
	private enum CombatState {
		MoveA,
		MoveD,
		MoveW,
		MoveS
	}

	private CombatState _combatState;

	private CombatState combatState {
		get => _combatState;
		set {
			if (_combatState == value)
				return;

			_combatState = value;
		}
	}	
	
	private BasicUnit _thisUnit;
	private BasicUnit thisUnit => _thisUnit == null ? _thisUnit = GetComponent<BasicUnit>() : _thisUnit;

	private MovementComponent _movement;
	private MovementComponent movement => _movement == null ? _movement = GetComponent<MovementComponent>() : _movement;

	private CombatComponent _combat;
	private CombatComponent Combat => _combat == null ? _combat = GetComponent<CombatComponent>() : _combat;

	private CharacterRotationComponent _rotatation;
	private CharacterRotationComponent rotatation => _rotatation == null ? _rotatation = GetComponent<CharacterRotationComponent>() : _rotatation;

	private WeightedRandomObjectsBag<CombatState> objectsBag = new WeightedRandomObjectsBag<CombatState>();

	[SerializeField]
	private BasicUnit currentTarget;

	[Header("AI Config")]
	[SerializeField]
	private float combatModeStartDistance = 5f;

	[SerializeField]
	private float combatModeQuitDistance = 8f;

	[SerializeField]
	private float timeBetweenAiActionChange = 1f;
	[SerializeField]
	private float enemyDetectionDistance = 14f;
	[SerializeField]
	private LayerMask Mask;

	[SerializeField, Range(0.1f, 2.5f)]
	private float attackStartDistance = 0.7f;
	[SerializeField, Range(1f, 3f)]
	private float attackQuitDistance = 1.5f;

	private float currentTimeBetweenActions = 0f;
	private float distanceToTarget;

	private bool inAggroMode = false;
	private bool inAttackMode = false;

	private Transform player => GM.player?.transform;
	private Vector3 thisPos => transform.position + Vector3.up;
	private Vector3 targetPos => player.position + Vector3.up;
	private Vector3 targetDir => targetPos - thisPos;

	private bool shiftPressed = false;

	private void Awake() {
		InitCombatStatesWeights();
	}

	private void InitCombatStatesWeights() {
		objectsBag.AddWeightedObject(CombatState.MoveW, 10);
		objectsBag.AddWeightedObject(CombatState.MoveS, 1);
		objectsBag.AddWeightedObject(CombatState.MoveA, 1);
		objectsBag.AddWeightedObject(CombatState.MoveD, 1);
	}

	private void Update() {
		if (!IsValid()) {
			Combat.SetAttackCommand(false);
			return;
		}

		SeekTarget();

		if (currentTarget == null) {
			movement.DontMove();
			return;
		}

		LookAtTarget();
		UpdateIfInAggroMode();

		if (inAggroMode)
			SetRandomCombatAction();
		else
			Chase();

		ExecuteCombatAction();
		AttackIfInRange();
	}

	private bool IsValid() {
		return !GM.isGamePaused && thisUnit.isAlive;
	}

	private void SeekTarget() {
		if (player == null) return;

		Debug.DrawRay(thisPos, targetDir, Color.white, Time.deltaTime, true);

		if (!Physics.Raycast(thisPos, targetDir, out var hitInfo, Mathf.Infinity, Mask)) {
			currentTarget = null;
			return;
		}

		if (!hitInfo.transform.CompareTag("Player")) {
			currentTarget = null;
			return;
		}

		if ((distanceToTarget = hitInfo.distance) > enemyDetectionDistance) {
			currentTarget = null;
			return;
		}

		currentTarget = hitInfo.transform.GetComponentInParent<BasicUnit>(); //TODO: legitny system łapania targetu (moze nawet bez tagow)
		currentTarget = currentTarget.isAlive ? currentTarget : null;
	}


	private void Chase() {
		combatState = CombatState.MoveW;
	}

	private void Strafe() {
		movement.MoveD();
	}

	private void LookAtTarget() {
		if (!currentTarget.isAlive) return;
		rotatation.LookAt(player.position - transform.position);
	}

	private void UpdateIfInAggroMode() { // hysteresis
		if (distanceToTarget < combatModeStartDistance) inAggroMode = true;
		if (distanceToTarget > combatModeQuitDistance) inAggroMode = false;
	}
	
	private void UpdateIfInAttackMode() { // hysteresis
		if (distanceToTarget < attackStartDistance) inAttackMode = true;
		if (distanceToTarget > attackQuitDistance) inAttackMode = false;
	}

	private void SetRandomCombatAction() {
		currentTimeBetweenActions += Time.deltaTime;
		currentTimeBetweenActions = Mathf.Clamp(currentTimeBetweenActions, 0f, timeBetweenAiActionChange);

		if (!Mathf.Approximately(currentTimeBetweenActions, timeBetweenAiActionChange))
			return;

		currentTimeBetweenActions = 0f;
		combatState = objectsBag.GetRandomWeightedObject();

//		combatState = (CombatState) (Random.Range(0, 1000) % 2) + 2;
//		shiftPressed = Random.Range(0, 2) == 1;
	}

	private void ExecuteCombatAction() {
		switch (combatState) {
			case CombatState.MoveW:
				if (distanceToTarget >= attackStartDistance)
					movement.MoveW(shiftPressed);
				else
					movement.DontMove();
				timeBetweenAiActionChange = 1f;
				break;
			case CombatState.MoveS:
				movement.MoveS(shiftPressed);
				timeBetweenAiActionChange = 1f;
				break;
			case CombatState.MoveA:
				movement.MoveA(shiftPressed);
				timeBetweenAiActionChange = 1f;
				break;
			case CombatState.MoveD:
				movement.MoveD(shiftPressed);
				timeBetweenAiActionChange = 1f;
				break;
		}
	}

	private void AttackIfInRange() {
		UpdateIfInAttackMode();
		Combat.SetAttackCommand(inAttackMode);
	}
}