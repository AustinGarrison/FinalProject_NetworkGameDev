using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BulletSpawner : NetworkBehaviour
{
    public NetworkVariable<int> BulletDamage = new NetworkVariable<int>(5);
    public Rigidbody bullet;
    private int maxDamage = 20;
    public Quaternion rotation = new Quaternion();

    private Player player;
    private GameObject gameManager;

    private void Start()
    {
        player = GetComponentInParent<Player>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager");

    }

    public void IncreaseDamage()
    {
        if(BulletDamage.Value == 1)
        {
            BulletDamage.Value = 5;
        }
        else
        {
            BulletDamage.Value += 5;
        }

        if(BulletDamage.Value > maxDamage)
        {
            BulletDamage.Value = maxDamage;
        }
    }

    public bool IsAtMaxDamage()
    {
        return BulletDamage.Value == maxDamage;
    }

    [ServerRpc]
    public void FireServerRpc(ServerRpcParams rpcParams = default)
    {
        transform.rotation = rotation;
        Rigidbody newBullet = Instantiate(bullet, transform.position, rotation);

        if (player.CurrentPlayerType == PlayerType.defender)
        {
            newBullet.velocity = player.transform.up * 80f;
        }
        
        if(player.CurrentPlayerType == PlayerType.attacker)
        {
            newBullet.velocity = -player.transform.up * 20f;
        }

        newBullet.gameObject.GetComponent<NetworkObject>().SpawnWithOwnership(rpcParams.Receive.SenderClientId);
        newBullet.GetComponent<Bullet>().Damage.Value = BulletDamage.Value;
        if(player.CurrentPlayerType == PlayerType.attacker)
        {
            newBullet.GetComponent<Bullet>().isAttackerBullet = true;
        }
        Destroy(newBullet.gameObject, 30);
    }
}
