using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiecesSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform bottomLeftP1;
    [SerializeField] private Transform bottomLeftP2;
    [SerializeField] private float tileSize;
    [SerializeField] private GameObject piecePrefab;
    [SerializeField] private List<ModelByIndex> whiteModels = new List<ModelByIndex>();
    [SerializeField] private List<ModelByIndex> blackModels = new List<ModelByIndex>();

    private List<Piece> whitePieces = new List<Piece>();
    private List<Piece> blackPieces = new List<Piece>();


    // Start is called before the first frame update
    void Start()
    {
        NetworkManager.OnInstantiate += ConfigurePiece;
        NetworkManager.OnNewClient += LookFor2Players;
    }

    private void LookFor2Players(int _connectionID)
    {
        if(NetworkManager.Instance.GetNumberOfConnectedClients() >= 2)
        {
            InitPieces();
        }
    }

    private void InitPieces()
    {
        SpawnPieces(bottomLeftP1, false);
        SpawnPieces(bottomLeftP2, true);
    }

    private void SpawnPieces(Transform _bottomLeft, bool invert)
    {
        for (int y = 0; y < 2; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                Vector3 position;
                if (!invert)
                {
                    position = new Vector3(_bottomLeft.position.x + tileSize * 0.5f + tileSize * x, _bottomLeft.position.y, _bottomLeft.position.z + tileSize * 0.5f + tileSize * y);
                }
                else
                {
                    position = new Vector3(_bottomLeft.position.x - tileSize * 0.5f - tileSize * x, _bottomLeft.position.y, _bottomLeft.position.z - tileSize * 0.5f - tileSize * y);
                }
                NetworkManager.Instance.Instantiate(piecePrefab.name, position, Quaternion.identity, _bottomLeft.name);
            }
        }
    }

    int pieceIndex = 0;
    int modelIndex = 0;
    private void ConfigurePiece(GameObject instance)
    {
        if (instance.GetComponent<Piece>() != null)
        {
            if (pieceIndex < 16)
            {
                string modelName = whiteModels.Find(w => w.index == pieceIndex).model.name;
                instance.name = $"Piece.{pieceIndex}";
                Debug.Log($"{instance.name} is spawned. Spawnwing a {modelName} for it.", instance);

                if(NetworkManager.Instance.GetClientID() == 1)
                    NetworkManager.Instance.Instantiate($"PieceModels/{modelName}", instance.transform.position, Quaternion.identity, null);
            }
            else
            {
                string modelName = blackModels.Find(b => b.index == pieceIndex).model.name;
                instance.name = $"Piece.{pieceIndex}";
                Debug.Log($"{instance.name} is spawned. Spawnwing a {modelName} for it.", instance);

                if (NetworkManager.Instance.GetClientID() == 1)
                    NetworkManager.Instance.Instantiate($"PieceModels/{modelName}", instance.transform.position, Quaternion.identity, null);
            }
            pieceIndex++;
        }
        else if(instance.GetComponent<PieceModel>() != null)
        {
            instance.name = $"PieceModel.{modelIndex}";
            instance.GetComponent<PieceModel>().AttachToPiece(GameObject.Find($"Piece.{modelIndex}").GetComponent<Piece>());
            modelIndex++;
        }

    }
}

[System.Serializable]
public struct ModelByIndex
{
    public int index;
    public GameObject model;
}
