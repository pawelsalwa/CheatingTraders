using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ControlledBy {
    Player,
    AI
}

public class BasicUnit : MonoBehaviour, Initable {
    public ControlledBy controlledBy = ControlledBy.Player;

    public MovementComponent movement;
    public HealthComponent hp;
    public AttackComponent attack;
    public void Init() {
        
    }
}
