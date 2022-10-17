using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Room : MonoBehaviour
{
    private RoomsManager _roomsManager => RoomsManager.Instance;

    [SerializeField] private List<RoomConnector> activeConnectorList = new List<RoomConnector>();
    [SerializeField] private List<RoomConnector> alternativeConnectorList = new List<RoomConnector>();
    [SerializeField] private List<CollisionCheck> collisionCheckList = new List<CollisionCheck>();

    private bool _spawnedByRoom;
    private bool _finishedSpawns;
    
    
    private void OnTriggerEnter(Collider other)
    {
        print("DOUBLE ROOM");
    }
    
    private void Start()
    {
        SetListeners();
        
        if (_spawnedByRoom) return;
        
        StartCoroutine(SetSpawnersRoutine());
    }

    private void OnDisable()
    {
        UnsetListeners();
    }

    private void SetListeners()
    {
        _roomsManager.OnMoreRoomsNeeded += SetAltConnector;
    }

    private void UnsetListeners()
    {
        _roomsManager.OnMoreRoomsNeeded -= SetAltConnector;
    }
    
    private void SetAltConnector()
    {
        if (alternativeConnectorList.Count <= 0)
        {
            _roomsManager.RoomCannotSpawn();
            _roomsManager.OnMoreRoomsNeeded -= SetAltConnector;

            return;
        }

        var connector = alternativeConnectorList[Random.Range(0, alternativeConnectorList.Count)];
        activeConnectorList.Add(connector);
        alternativeConnectorList.Remove(connector);
        
        StartCoroutine(SetSpawnersRoutine());
    }

    public void SpawnedSetup(Vector3 position, RoomConnector otherConnector, out bool success)
    {
        _spawnedByRoom = true;
        
        RoomConnector connectorConnected = null;
        
        if(activeConnectorList.Count > 0)
            connectorConnected = activeConnectorList[Random.Range(0, activeConnectorList.Count)];

        if (!connectorConnected)
        {
            success = false;
            
            Debug.LogError($"Room {gameObject.name} active spawner is missing");
            Destroy(gameObject);
            return;
        }

        Transform tf = transform;
        
        Quaternion rotDiff = otherConnector.transform.rotation * Quaternion.Inverse(connectorConnected.transform.rotation);
        tf.rotation = rotDiff * tf.rotation;
        tf.rotation = Quaternion.LookRotation(-tf.forward, tf.up);


        Vector3 currentPos = tf.position;
        var newPosition = (currentPos - connectorConnected.transform.position)+position;

        if (!CanMoveToPosition(newPosition))
        {
            success = false;
            
            Destroy(gameObject);
            return;
        }
        
        success = true;

        connectorConnected.JoinedConnection();
        
        tf.position = newPosition;
        
        Physics.SyncTransforms();
        
        _roomsManager.AddSpawnedRoom(this);
        
        StartCoroutine(SetSpawnersRoutine(otherConnector.MyRoom));
    }

    private bool CanMoveToPosition(Vector3 position)
    {
        foreach (var check in collisionCheckList)
        {
            if (!check.IsColliding(position)) continue;
            return false;
        }

        return true;
    }

    private IEnumerator SetSpawnersRoutine(Room generatorRoom = null)
    {
        _finishedSpawns = false;
        
        _roomsManager.AddSpawningRoomsQueue(this);

        if(generatorRoom != null)
            yield return new WaitUntil(() => generatorRoom._finishedSpawns);

        foreach (var spawner in activeConnectorList)
        {
            spawner.Setup(this);
        }
        
        activeConnectorList.Clear();
        
        _finishedSpawns = true;
        
        _roomsManager.RemoveSpawningRoomsQueue(this);
    }
}
