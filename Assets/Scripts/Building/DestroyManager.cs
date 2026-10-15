using UnityEngine;

public class DestroyManager
{
    private readonly GridManager _grid;
    private readonly SoldierPoolManager _soldierPools;
    private readonly MoveManager _move;
    private readonly AttackManager _attack;
    private readonly SelectionService _selection;

    public DestroyManager(GridManager grid, SoldierPoolManager soldierPools,
        MoveManager move, AttackManager attack, SelectionService selection)
    {
        _grid = grid;
        _soldierPools = soldierPools;
        _move = move;
        _attack = attack;
        _selection = selection;
    }

    public void Track(IDamageable damageable)
    {
        damageable.Died -= OnDied;
        damageable.Died += OnDied;
    }

    private void OnDied(IDamageable dead)
    {
        dead.Died -= OnDied;

        _attack.CancelTargeting(dead);

        if (_selection.Current == dead)
            _selection.Clear();

        switch (dead)
        {
            case SoldierView soldier:
                _attack.Cancel(soldier);
                _move.Stop(soldier);
                _soldierPools.Despawn(soldier);
                break;

            case BuildingView building:
                ClearBuildingCells(building);
                Object.Destroy(building.gameObject);
                break;
        }
    }

    private void ClearBuildingCells(BuildingView b)
    {
        for (int x = 0; x < b.Size.x; x++)
        for (int y = 0; y < b.Size.y; y++)
            _grid.ClearOccupant(b.OriginCell.x + x, b.OriginCell.y + y, b);
    }
}