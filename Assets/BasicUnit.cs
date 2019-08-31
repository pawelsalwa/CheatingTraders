﻿using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public enum ControlledBy
{
    Player,
    AI
}

[RequireComponent(typeof(Animator))]
public class BasicUnit : MonoBehaviour, Initable
{
    public HealthComponent hp;
    public string animDieKey;
    public int deadBodyTimeout = 2000;

    private bool isAlive = true;

    private Animator _animator;

    private Animator animator => _animator == null ? _animator = GetComponent<Animator>() : _animator;

    public void Init()
    {
        hp.OnHpDropBelowZero += Die;
    }

    private void Die()
    {
        isAlive = false;
        animator.SetBool(animDieKey, true);
        foreach (var childCollider in GetComponentsInChildren<Collider>())
            childCollider.enabled = false;
        
        RemoveLoosersBody();
    }

    private async void RemoveLoosersBody()
    {
        await Task.Delay(deadBodyTimeout);
        Destroy(gameObject);
    }
}