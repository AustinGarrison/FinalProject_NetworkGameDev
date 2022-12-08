using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    
    public Player playerPrefab;
    public GameObject spawnPoints;
    public GameObject planetCenter;
    
    public float attackerBulletSpeed;
    public float defenderBulletSpeed;
    public float attaclerBulletDelay;
    public float defenderBulletdelay;

    private int spawnIndex = 0;
    private List<Vector3> availableSpawnPositions = new List<Vector3>();

    public void Awake()
    {
        RefreshSpawnPoints();
    }

    public override void OnNetworkSpawn()
    {
        if(IsHost)
        {
            SpawnPlayers();
        }
    }

    private void RefreshSpawnPoints( )
    {
        Transform[] allPoints = spawnPoints.GetComponentsInChildren<Transform>();
        availableSpawnPositions.Clear();
        foreach (Transform point in allPoints)
        {
            if (point != spawnPoints.transform)
            {
                availableSpawnPositions.Add(point.localPosition);
            }
        }
    }

    public Vector3[] GetNextSpawnLocation(PlayerInfo pi)
    {
        Vector3[] spawnPositions = new Vector3[2];

        var newPosition = planetCenter.transform.position;
        var spritePosition = availableSpawnPositions[spawnIndex];

        newPosition.z = -940f;

        spawnIndex += 1;

        if (spawnIndex > availableSpawnPositions.Count - 1)
        {
            spawnIndex = 0;
        }

        spawnPositions[0] = newPosition;
        spawnPositions[1] = spritePosition;

        return spawnPositions;
    }

    private void SpawnPlayers()
    {
        foreach (PlayerInfo pi in GameData.Instance.allPlayers)
        {
            Vector3[] spawnLocations = GetNextSpawnLocation(pi);

            Player playerSpawn = Instantiate(playerPrefab, spawnLocations[0], Quaternion.identity);


            playerSpawn.GetComponent<NetworkObject>().SpawnAsPlayerObject(pi.clientId);
            playerSpawn.PlayerColor.Value = pi.color;
            playerSpawn.PlayerSprite.Value = pi.playerSprite;

            playerSpawn.playerSprite.transform.position = spawnLocations[1];
            playerSpawn.PlayerPosition.Value = spawnLocations[1];

            playerSpawn.ApplyPlayerShipSprite();
            playerSpawn.ApplyPlayerPosition();
        }
    }
}