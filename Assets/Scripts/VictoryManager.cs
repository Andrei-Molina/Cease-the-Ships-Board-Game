using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryManager : MonoBehaviour
{
    private static VictoryManager instance;
    [SerializeField] private GameObject victoryCanvas;
    [SerializeField] private GameObject victoryScreen;

    public Shipboard shipboard;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Make this object persist between scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
    }

    // Victory Logic
    public void Checkmate(int team)
    {
        DisplayVictory(team);
    }
    private void DisplayVictory(int winningTeam)
    {
        victoryCanvas.SetActive(true);
        victoryScreen.SetActive(true);
        victoryScreen.transform.GetChild(winningTeam).gameObject.SetActive(true);
    }
    public void OnResetButton()
    {
        /*
        //UI Reset
        victoryCanvas.SetActive(false);
        victoryScreen.SetActive(false);
        victoryScreen.transform.GetChild(0).gameObject.SetActive(false);
        victoryScreen.transform.GetChild(1).gameObject.SetActive(false);
        victoryScreen.transform.GetChild(2).gameObject.SetActive(false);
        victoryScreen.transform.GetChild(3).gameObject.SetActive(false);

        //Fields Reset
        shipboard.SetCurrentlyDragging(null);
        shipboard.ClearAvailableMoves();
        shipboard.GetMoveList().Clear();

        //Clean Up
        for (int x = 0; x < shipboard.GetTileCountX(); x++)
        {
            for (int y = 0; y < shipboard.GetTileCountY(); y++)
            {
                if (shipboard.GetShipboardPieces()[x, y] != null)
                    Destroy(shipboard.GetShipboardPieces()[x, y].gameObject);

                shipboard.GetShipboardPieces()[x, y] = null;
            }
        }

        for (int i = 0; i < shipboard.GetDeadPlayer1().Count; i++)
            Destroy(shipboard.GetDeadPlayer1()[i].gameObject);
        for (int i = 0; i < shipboard.GetDeadPlayer2().Count; i++)
            Destroy(shipboard.GetDeadPlayer2()[i].gameObject);

        shipboard.GetDeadPlayer1().Clear();
        shipboard.GetDeadPlayer2().Clear();

        shipboard.RespawnAllPieces();
        shipboard.PositionAllPieces();

        shipboard.HandleCalamitySpawn();
        shipboard.SetIsPlayer1Turn(true);*/

        //Just reload the entire scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void OnExitButton()
    {
        SceneManager.LoadScene("Deployment");
    }
}
