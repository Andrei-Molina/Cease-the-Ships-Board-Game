using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoadSystem : MonoBehaviour
{
    /*
    private const string SaveCountKey = "SaveCount";
    private const string SavePrefix = "Save";
    private const string SaveSlotKey = "CurrentSaveSlot";

    public ShipPieces[,] shipboardPieces;
    private int currentSaveSlot = 1; // Default save slot

    public void SaveGame()
    {
        int saveCount = PlayerPrefs.GetInt(SaveCountKey, 0) + 1;
        PlayerPrefs.SetInt(SaveCountKey, saveCount);
        currentSaveSlot = saveCount;
        PlayerPrefs.SetInt(SaveSlotKey, currentSaveSlot);

        for (int x = 0; x < shipboardPieces.GetLength(0); x++)
        {
            for (int y = 0; y < shipboardPieces.GetLength(1); y++)
            {
                ShipPieces piece = shipboardPieces[x, y];
                if (piece != null)
                {
                    string key = $"{SavePrefix}{currentSaveSlot}_Piece_{x}_{y}";
                    string value = $"{(int)piece.type},{piece.team}";
                    PlayerPrefs.SetString(key, value);
                }
            }
        }

        PlayerPrefs.Save();
        Debug.Log("Game saved to slot: " + currentSaveSlot);
    }

    public void LoadGame(int saveSlot)
    {
        currentSaveSlot = saveSlot;

        ClearBoard(); // Clear existing pieces

        for (int x = 0; x < shipboardPieces.GetLength(0); x++)
        {
            for (int y = 0; y < shipboardPieces.GetLength(1); y++)
            {
                string key = $"{SavePrefix}{currentSaveSlot}_Piece_{x}_{y}";
                if (PlayerPrefs.HasKey(key))
                {
                    string value = PlayerPrefs.GetString(key);
                    string[] data = value.Split(',');
                    ShipPieceType type = (ShipPieceType)int.Parse(data[0]);
                    int team = int.Parse(data[1]);

                    shipboardPieces[x, y] = SpawnSinglePiece(type, team);
                }
            }
        }

        Debug.Log("Game loaded from slot: " + currentSaveSlot);
    }

    private void ClearBoard()
    {
        foreach (ShipPieces piece in shipboardPieces)
        {
            if (piece != null)
                Destroy(piece.gameObject);
        }

        System.Array.Clear(shipboardPieces, 0, shipboardPieces.Length);
    }

    private ShipPieces SpawnSinglePiece(ShipPieceType type, int team)
    {
        ShipPieces sp = Instantiate(GameManager.instance.prefabs[(int)type - 1], transform).GetComponent<ShipPieces>();
        sp.type = type;
        sp.team = team;
        GameManager.instance.listShip.Add(sp);
        return sp;
    }

    public int GetSaveSlotCount()
    {
        return PlayerPrefs.GetInt(SaveCountKey, 0);
    }*/
}
