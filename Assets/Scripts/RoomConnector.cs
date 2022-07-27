using System;
using UnityEngine;
using UnityEngine.Events;

public class RoomConnector : MonoBehaviour
{
    [SerializeField] private Direction myDirection = default;
    [SerializeField] private CollisionCheck collisionCheck = default;
    
    private RoomsManager RoomsManager => RoomsManager.Instance;
    public Direction MyDirection => myDirection;
    
    public Room MyRoom { get; private set; }
    public Direction OppositeDirection { get; private set; }

    public enum Direction
    {
        Top,
        Bottom,
        Left,
        Right
    }

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
            SetOppositeDirection();
            Spawn();   
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

    private void Spawn()
    {
        var roomPref = RoomsManager.GetRandomRoom(OppositeDirection);
        var roomGo = Instantiate(roomPref);
        roomGo.GetComponent<Room>().SpawnedSetup(transform.position, this, out var success);
        
        if(success) onConnectedRoomSpawned?.Invoke();
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

    private void SetOppositeDirection()
    {
        OppositeDirection = myDirection switch
        {
            Direction.Top => Direction.Bottom,
            Direction.Bottom => Direction.Top,
            Direction.Left => Direction.Right,
            Direction.Right => Direction.Left,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}