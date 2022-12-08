using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Bullet : NetworkBehaviour
{
    public NetworkVariable<int> Damage = new NetworkVariable<int>(1);
    public bool isAttackerBullet;

    public void OnTriggerEnter(Collider collision)
    {

        if (collision.gameObject.CompareTag("Bullet"))
        {
            if (isAttackerBullet && collision.GetComponent<Bullet>().isAttackerBullet)
            {
                return;
            }
            else
            {
                HandleBulletCollision(collision.gameObject);
            }
        }

        if(collision.gameObject.CompareTag("Earth"))
        {
            HandleEarthCollision(collision.gameObject);
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            HandlePlayerCollision(collision.gameObject);
        }

    }

    private void HandleBulletCollision(GameObject bullet)
    {
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (isAttackerBullet && bulletScript.isAttackerBullet)
        {
            return;
        }
        else
        {
            Destroy(bullet);
            Destroy(gameObject);
        }
            
        //Score.Value -= bulletScript.Damage.Value;

        //ulong ownerClientId = bullet.GetComponent<NetworkObject>().OwnerClientId;
        //Player otherPlayer = NetworkManager.Singleton.ConnectedClients[ownerClientId].PlayerObject.GetComponent<Player>();
        //otherPlayer.Score.Value += bulletScript.Damage.Value;


    }

    private void HandlePlayerCollision(GameObject player)
    {
        if (isAttackerBullet == false)
        {
            //ulong ownerClientId = player.GetComponent<NetworkObject>().OwnerClientId;
            //Player otherPlayer = NetworkManager.Singleton.ConnectedClients[ownerClientId].PlayerObject.GetComponent<Player>();
            //player.GetComponentInParent<Health>().ApplyDamage(Damage.Value);
            Health health = player.GetComponentInParent<Health>();
            health.ApplyDamage(1f);

            Destroy(gameObject);
        }
        
    }

    private void HandleEarthCollision(GameObject earth)
    {
        earth.GetComponent<Health>().ApplyDamage(Damage.Value);

        Destroy(gameObject);
    }

}
