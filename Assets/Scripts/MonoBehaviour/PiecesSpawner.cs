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
        NetworkManager.OnClientStarted += InitPieces;
    }

    private void InitPieces()
    {
        SpawnPieces(bottomLeftP1);
        SpawnPieces(bottomLeftP2);
    }

    private void SpawnPieces(Transform _bottomLeft)
    {
        for (int y = 0; y < 2; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                Vector3 position = new Vector3(_bottomLeft.position.x + tileSize * 0.5f + tileSize * x, _bottomLeft.position.y, _bottomLeft.position.z + tileSize * 0.5f + tileSize * y);
                NetworkManager.Instance.Instantiate(piecePrefab.name, position, Quaternion.identity, _bottomLeft);
            }
        }
    }

    int index = 0;
    private void ConfigurePiece(GameObject piece)
    {
        if (index < 16)
            piece.GetComponent<Piece>().SetModel(whiteModels.Find(w => w.index == index).model);
        else
            piece.GetComponent<Piece>().SetModel(blackModels.Find(b => b.index == index).model);
        index++;
    }
}

[System.Serializable]
public struct ModelByIndex
{
    public int index;
    public GameObject model;
}
