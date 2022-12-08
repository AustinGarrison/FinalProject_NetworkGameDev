using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class HealthBar : MonoBehaviour
{

    public NetworkVariable<Vector3> HealthRemaining = new NetworkVariable<Vector3>();

    [SerializeField] Health healthManager = null;
    [SerializeField] RectTransform forground = null;
    public Canvas rootCanvas = null;

    private void Update()
    {
        if (healthManager.currentHitPoints <= 0)
        {
            rootCanvas.enabled = false;
            return;
        }

        rootCanvas.enabled = true;
        forground.localScale = new Vector3(healthManager.GetFraction(), 1, 1);
        RequestUpdateHealthServerRpc(new Vector3(healthManager.GetFraction(), 1, 1));
        HealthRemaining.OnValueChanged += OnHealthChanged;
    }

    private void OnHealthChanged(Vector3 previous, Vector3 current)
    {
        UpdateHealthBar();
    }


    [ServerRpc]
    public void RequestUpdateHealthServerRpc(Vector3 value)
    {
        HealthRemaining.Value = value;
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        forground.localScale = HealthRemaining.Value;
    }
}