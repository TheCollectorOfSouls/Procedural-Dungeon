using System;
using UnityEngine;
using UnityEngine.Events;

public class RoomConnector : MonoBehaviour
{
    [SerializeField] private CollisionCheck collisionCheck = default;
    
    private RoomsManager RoomsManager => RoomsManager.Instance;

    public Room MyRoom { get; private set; }

    #region Events

    public UnityEvent onConnectionFailed;
    public UnityEvent onConnectedRoomSpawned;
    public UnityEvent onJoinedConnection;

    #endregion


    public void Setup(Room room)
    {
        MyRoom = room;

        if (CanSpawn())
        {
            SpawnRoom();
        }
        else
        {
            onConnectionFailed?.Invoke();
            
            gameObject.SetActive(false);
        }
    }

    public void JoinedConnection()
    {
        onJoinedConnection?.Invoke();
        gameObject.SetActive(false);
    }

    private void SpawnRoom()
    {
        var roomPref = RoomsManager.GetRandomRoom();
        var roomGo = Instantiate(roomPref);
        roomGo.GetComponent<Room>().SpawnedSetup(transform.position, this, out var success);

        if (success)
        {
            onConnectedRoomSpawned?.Invoke();
        }
        else
        {
            onConnectionFailed?.Invoke();
        }

        gameObject.SetActive(false);
    }

    private bool CanSpawn()
    {
        
        if (!RoomsManager.CanSpawnRoom()) return false;

        return !collisionCheck.IsColliding();
    }
}