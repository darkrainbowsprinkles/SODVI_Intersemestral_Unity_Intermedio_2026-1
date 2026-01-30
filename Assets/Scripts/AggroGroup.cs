using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AggroGroup : MonoBehaviour
{
    [SerializeField] UnityEvent onGroupDead;
    Dictionary<Health, UnityAction> deathListeners = new();
    int initialCount;

    public event Action OnChange;

    public int GetTotalCount()
    {
        return initialCount;
    }

    public int GetAliveCount()
    {
        return deathListeners.Count;
    }

    void Awake()
    {
        FillDeathListeners();
        initialCount = deathListeners.Count;
    }

    void FillDeathListeners()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            Health enemyHealth = enemy.GetComponent<Health>();
            void listener() => OnEnemyDead(enemyHealth);
            deathListeners[enemyHealth] = listener;
            enemyHealth.onDie.AddListener(listener);
        }
    }

    void OnEnemyDead(Health enemy)
    {
        if (deathListeners.TryGetValue(enemy, out var listener))
        {
            enemy.onDie.RemoveListener(listener);
            deathListeners.Remove(enemy);
        }

        if (deathListeners.Count == 0)
        {
            onGroupDead?.Invoke();
        }

        OnChange?.Invoke();
    }
}
