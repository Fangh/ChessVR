using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceModel : SyncMonoBehaviour
{
    public void AttachToPiece(Piece _piece)
    {
        _piece.AttachModel(gameObject);
    }
}
