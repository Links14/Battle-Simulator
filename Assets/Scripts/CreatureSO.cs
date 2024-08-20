using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CreatureSO", menuName = "Creatures/Create new Creature", order = 0)]
[System.Serializable]
public class CreatureSO : ScriptableObject
{
    [SerializeField] private string creatureName;
    [SerializeField] private Sprite sprite;
    [SerializeField] private List<MoveSO> moves;

    // combat stats
    [SerializeField] private float maxHpStat;
    [SerializeField] private float attackStat;
    [SerializeField] private float defenseStat;
    [SerializeField] private float speedStat;

    public CreatureSO (string _name, Sprite _sprite, float _maxHp, float _atk, float _dmg, float _spd)
    {
        creatureName = _name;
        sprite = _sprite;
        maxHpStat = _maxHp;

        attackStat = _atk;
        defenseStat = _dmg;
        speedStat = _spd;
    }

    public MoveSO GetMove(int _index)
    {
        return moves[_index];
    }

    public string Name { get { return creatureName; } }
    public Sprite Sprite { get { return sprite; } }
    
    public float MaxHp { get { return maxHpStat; } }
    public float AttackStat { get {  return attackStat; } }
    public float DefenseStat { get { return defenseStat; } }
    public float SpeedStat { get { return speedStat; } }
}
