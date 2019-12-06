using UnityEngine;

[RequireComponent(typeof(BasicUnit))]
public class BotController : MonoBehaviour {
	private enum MovementState { MoveA, MoveD, MoveW, MoveS, None }

	private MovementState _movementState;

	private MovementState movementState {
		get => _movementState;
		set {
			if (_movementState == value)
				return;

			_movementState = value;
		}
	}
	
	private enum CombatState { Attack, ShieldBlock, None }

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
	private CombatComponent combat => _combat == null ? _combat = GetComponent<CombatComponent>() : _combat;

	private CharacterRotationComponent _rotatation;
	private CharacterRotationComponent rotatation => _rotatation == null ? _rotatation = GetComponent<CharacterRotationComponent>() : _rotatation;

	private readonly WeightedRandomObjectsBag<MovementState> movementActionsBag = new WeightedRandomObjectsBag<MovementState>();

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

	private void Awake() {
		InitCombatStatesWeights();
	}

	private void InitCombatStatesWeights() {
		movementActionsBag.AddWeightedObject(MovementState.MoveW, 7210);
		movementActionsBag.AddWeightedObject(MovementState.MoveS, 10);
		movementActionsBag.AddWeightedObject(MovementState.MoveA, 10);
		movementActionsBag.AddWeightedObject(MovementState.MoveD, 10);
	}

	private void Update() {
		if (!IsValid()) {
			SetIdleAction();
			return;
		}

		SeekTarget();

		if (currentTarget == null) {
			SetIdleAction();
			return;
		}

		LookAtTarget();
		UpdateIfInAggroMode();

		if (inAggroMode)
			SetRandomMovementAction();
		else
			Chase();

		ExecuteMovementAction();
		
		UpdateIfInAttackMode();
		SetCombatAction();
		ExecuteCombatAction();
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
		movementState = MovementState.MoveW;
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

	private void SetRandomMovementAction() {
		currentTimeBetweenActions += Time.deltaTime;
		currentTimeBetweenActions = Mathf.Clamp(currentTimeBetweenActions, 0f, timeBetweenAiActionChange);

		if (!Mathf.Approximately(currentTimeBetweenActions, timeBetweenAiActionChange))
			return;

		currentTimeBetweenActions = 0f;
		movementState = movementActionsBag.GetRandomWeightedObject();
	}

	private void SetCombatAction() {
//		combatState = currentTarget.isAttacking ? CombatState.ShieldBlock : CombatState.Attack;
		if (inAttackMode)
			combatState = CombatState.Attack;
		else 
			combatState = CombatState.None;
	}

	private void ExecuteMovementAction() {
		switch (movementState) {
			case MovementState.MoveW:
				if (distanceToTarget >= attackStartDistance)
					movement.MoveW();
				else
					movement.DontMove();
				break;
			case MovementState.MoveS:
				movement.MoveS();
				break;
			case MovementState.MoveA:
				movement.MoveA();
				break;
			case MovementState.MoveD:
				movement.MoveD();
				break;
		}
	}

	private void ExecuteCombatAction() {
		switch (combatState) {
			case CombatState.ShieldBlock:
				combat.SetBlockCommand(true);
				break;
			case CombatState.Attack:
				combat.SetAttackCommand(true);
				break;
			case CombatState.None:
			default:
				combat.SetAttackCommand(false);
				combat.SetBlockCommand(false);
				break;
		}
	}

	private void SetIdleAction() {
		combat.SetAttackCommand(false);
		combat.SetBlockCommand(false);
		movement.DontMove();
	}
}