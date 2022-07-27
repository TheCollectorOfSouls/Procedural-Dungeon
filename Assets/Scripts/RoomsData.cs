using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Rooms", menuName = "RoomsData", order = 0)]
public class RoomsData : ScriptableObject
{
    public GameObject starterRoom;
    public List<GameObject> topRooms;
    public List<GameObject> bottomRooms;
    public List<GameObject> leftRooms;
    public List<GameObject> rightRooms;
}
