using System;

public interface IDamageable : ISelectable
{
    int CurrentHealth { get; }
    int MaxHealth { get; }
    bool IsDead { get; }
    event Action<IDamageable> Died;

    void TakeDamage(int amount);
    void Dead();
}