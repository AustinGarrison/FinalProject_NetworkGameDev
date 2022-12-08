using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Events;
using System;
using Unity.Netcode;

public class Health : MonoBehaviour
{
    public int maxHitPoints;
    public float currentHitPoints;

    protected float m_timeSinceLastHit = 0.0f;
    protected Collider m_Collider;
    public AudioClip damage;



    private void Start()
    {
        currentHitPoints = maxHitPoints;
        m_Collider = GetComponent<Collider>();
    }

    public void SetColliderState(bool enabled)
    {
        m_Collider.enabled = enabled;
    }

    public float GetFraction()
    {
        return currentHitPoints / maxHitPoints;
    }

    public void ApplyDamage(float damageAmount)
    {
        //GetComponent<AudioSource>().PlayOneShot(damage);
        currentHitPoints -= damageAmount;
        CheckForDead();
    }

    private void CheckForDead()
    {
        if (currentHitPoints <= 0)
        {
            Destroy(gameObject);
            EndGame();
        }
    }

    public void EndGame()
    {
        if(GetComponent<Player>()!=null)
        GameObject.Find("SceneManager").GetComponent<SceneManager>().EndGame(PlayerType.defender);
    }
}