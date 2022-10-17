using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Rooms", menuName = "RoomsData", order = 0)]
public class RoomsData : ScriptableObject
{
    public List<GameObject> rooms;
}
