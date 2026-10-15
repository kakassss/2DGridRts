using System;
using System.Collections.Generic;
using UnityEngine;

public class BuildingView : MonoBehaviour, ISoldierProducer, IDamageable
{
    [SerializeField] private HealthBarView healthBar;

    public BuildingDataSO buildingDataSO;

    private readonly HealthModel _health = new HealthModel();

    public event Action<IDamageable> Died;

    public string DisplayName => buildingDataSO.DisplayName;
    public Sprite Icon => buildingDataSO.Icon;
    public IReadOnlyList<SoldierDataSo> Producibles => buildingDataSO.ProducibleSoldiers;

    public Vector2Int OriginCell { get; private set; }
    public Vector2Int Size => buildingDataSO.Size;

    public int CurrentHealth => _health.Current;
    public int MaxHealth => _health.Max;
    public bool IsDead => _health.IsDead;

    public void Initialize(BuildingDataSO data, Vector2Int originCell)
    {
        buildingDataSO = data;
        OriginCell = originCell;
        _health.Reset(data.MaxHealth);

        healthBar.Setup(data.Size.x * 0.8f, data.Size.y);
        healthBar.SetHealth(_health.Current, _health.Max);
    }

    public void TakeDamage(int amount)
    {
        bool died = _health.Take(amount);
        healthBar.SetHealth(_health.Current, _health.Max);

        if (died)
            Died?.Invoke(this);
    }
}