using UnityEngine;

[CreateAssetMenu(menuName = "SoldierDataSo", fileName = "Data/SoldierDataSo")]
public class SoldierDataSo : BaseUnitDataSO
{
    [SerializeField] private string displayName;
    [SerializeField] private Sprite icon;
    [SerializeField] private SoldierView prefab;
    [SerializeField, Min(1)] private int maxHealth = 10;
    [SerializeField, Min(0)] private int damage = 10;
    [SerializeField] private float moveSpeed = 3f;

    [Header("Attack")]
    [SerializeField, Min(0.1f)] private float attackInterval = 1f;
    [SerializeField, Min(1)] private int attackRange = 1;

    public string DisplayName => displayName;
    public Sprite Icon => icon;
    public SoldierView Prefab => prefab;
    public int MaxHealth => maxHealth;
    public int Damage => damage;
    public float MoveSpeed => moveSpeed;
    public float AttackInterval => attackInterval;
    public int AttackRange => attackRange;
}