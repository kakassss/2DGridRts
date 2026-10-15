using System;
using System.Collections.Generic;
using UnityEngine;

public class SoldierView : MonoBehaviour, IMovable, IDamageable, IAttacker
{
    [SerializeField] private HealthBarView healthBar;
    
    private static readonly IReadOnlyList<SoldierDataSo> NoProducibles = Array.Empty<SoldierDataSo>();
    public SoldierDataSo Data { get; private set; }
    public Vector2Int Cell { get; private set; }

    public string DisplayName => Data.DisplayName;
    public Sprite Icon => Data.Icon;
    public IReadOnlyList<SoldierDataSo> Producibles => NoProducibles;
    public Queue<Vector2Int> Path { get; } = new Queue<Vector2Int>();
    public Vector2Int Destination { get; set; }
    public GameObject GameObject => gameObject;
    public float MoveSpeed => Data.MoveSpeed;
    private readonly HealthModel _health = new HealthModel();

    public event Action<IDamageable> Died;

    public int CurrentHealth => _health.Current;
    public int MaxHealth => _health.Max;
    public bool IsDead => _health.IsDead;
    public int Damage => Data.Damage;
    public float AttackInterval => Data.AttackInterval;
    public int AttackRange => Data.AttackRange;
    

    public void Initialize(SoldierDataSo data, Vector2Int cell, Vector3 worldPosition)
    {
        Data = data;
        _health.Reset(data.MaxHealth);
        Cell = cell;
        Path.Clear();
        transform.position = worldPosition;

        healthBar.Setup(0.8f, 1f);                       
        healthBar.SetHealth(_health.Current, _health.Max);
    }

    public void TakeDamage(int amount)
    {
        bool died = _health.Take(amount);
        healthBar.SetHealth(_health.Current, _health.Max);
        if (died) Died?.Invoke(this);
    }

    public void SetCell(Vector2Int cell) => Cell = cell;

    public void Dead()
    {
        
    }
}