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


    // Start is called before the first frame update
    void Start()
    {
        SpawnPieces(bottomLeftP1, true);
        SpawnPieces(bottomLeftP2, false);
    }

    private void SpawnPieces(Transform _bottomLeft, bool _white)
    {
        int index = 0;
        for (int y = 0; y < 2; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                GameObject go = Instantiate(piecePrefab, _bottomLeft);
                go.transform.localPosition = new Vector3(tileSize * 0.5f + tileSize * x, tileSize * 0.5f + tileSize * y, 0);
                go.transform.rotation = Quaternion.identity;
                go.GetComponent<Piece>().SetModel(_white ? whiteModels.Find(w => w.index == index).model : blackModels.Find(b => b.index == index).model);
                index++;
            }
        }
    }
}

[System.Serializable]
public struct ModelByIndex
{
    public int index;
    public GameObject model;
}
