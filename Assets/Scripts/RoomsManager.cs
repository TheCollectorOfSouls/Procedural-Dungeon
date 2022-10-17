using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class RoomsManager : MonoBehaviour
{
    public static RoomsManager Instance;

    [SerializeField] private RoomsData roomsData;
    [SerializeField] private int maxRooms;
    [SerializeField] private int minRooms;
    
    private List<Room> _spawningRoomsQueueList = new List<Room>();
    private List<Room> _spawnedRoomsList = new List<Room>();

    private int _roomsWithoutSpawnsLeft = 0;
    private Coroutine _emptyQueueCor;

    public RoomsData RoomsData => roomsData;

    #region Events
    
    public UnityEvent onFinish;
    
    public UnityAction OnMoreRoomsNeeded;

    #endregion
    
    private void Awake()
    {
        Instance = this;
    }

    public void AddSpawningRoomsQueue(Room room)
    {
        if (_spawningRoomsQueueList.Contains(room)) return;
        
        _spawningRoomsQueueList.Add(room);
            
        if(_emptyQueueCor != null)
            StopCoroutine(_emptyQueueCor);

    }

    public void RemoveSpawningRoomsQueue(Room room)
    {
        if(_spawningRoomsQueueList.Contains(room))
            _spawningRoomsQueueList.Remove(room);

        if (_spawningRoomsQueueList.Count > 0) return;
        
        if (_spawnedRoomsList.Count < minRooms)
        {
            if(_emptyQueueCor != null)
                StopCoroutine(_emptyQueueCor);
            
            _emptyQueueCor = StartCoroutine(EmptyQueueRoutine());
        }
        else
        {
            onFinish?.Invoke();
            print($"{_spawnedRoomsList.Count} rooms generated");
        }
    }

    public void RoomCannotSpawn()
    {
        _roomsWithoutSpawnsLeft++;

        if (_roomsWithoutSpawnsLeft >= _spawnedRoomsList.Count)
        {
            onFinish?.Invoke();
            print($"Failed to generate at least {minRooms} rooms, {_spawnedRoomsList.Count} rooms generated {_roomsWithoutSpawnsLeft}");
        }
    }

    public void AddSpawnedRoom(Room room)
    {
        if(!_spawnedRoomsList.Contains(room))
            _spawnedRoomsList.Add(room);
    }

    public bool CanSpawnRoom()
    {
        return _spawnedRoomsList.Count < maxRooms;
    }

    public GameObject GetRandomRoom()
    {
        return roomsData.rooms[Random.Range(0, roomsData.rooms.Count)];
    }
    
    //small delay to wait every room properly finish spawns
    private IEnumerator EmptyQueueRoutine()
    {
        yield return new WaitForSeconds(0.01f);
        
        OnMoreRoomsNeeded?.Invoke();
    }
}
