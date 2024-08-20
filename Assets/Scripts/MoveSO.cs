using System;
using UnityEngine;

[CreateAssetMenu(fileName = "MoveSO", menuName = "Creatures/Create new Move", order = 1)]
[Serializable]
public class MoveSO : ScriptableObject
{
    public string moveName;
    public float movePower;

    public string moveDescription;

    public MoveSO(string _name, float _power)
    {
        moveName = _name;
        movePower = _power;
    }
    public string Name { get { return moveName; } }
    public float Power { get {  return movePower; } }
    public string Description { get { return moveDescription; } }
}
