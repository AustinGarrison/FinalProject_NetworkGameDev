using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    private GameObject[] players;
    public Canvas earth;
    public Canvas gameOver;
    private TextMeshProUGUI mtext;

    private void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player");

    }

    public void EndGame(PlayerType winner)
    {
        mtext = gameOver.GetComponentInChildren<TextMeshProUGUI>();

        if (winner == PlayerType.defender)
        {
            mtext.text = "Game Over! - Defender wins!";
            gameOver.enabled = true;
        }
        else
        {
            Destroy(earth);
            mtext.text = "Game Over! - Attacker wins!";
            gameOver.enabled = true;
        }
            
    }


}
