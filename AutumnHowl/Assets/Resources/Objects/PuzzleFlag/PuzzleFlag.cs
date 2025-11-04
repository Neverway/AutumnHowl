using System.Collections;
using UnityEngine;

public class PuzzleFlag : MonoBehaviour
{

    public const string REFERENCE_ID = "PuzzleFlag";
    private bool hasWon = false;
    private bool hasDoneFlagShift = false;

    public void Update()
    {
        IDToObj<PuzzleFlag>.ClearNew();
        IDToObj<PuzzleFlag>.TryAdd(REFERENCE_ID, this);

        if (hasWon && !hasDoneFlagShift)
        {
            transform.position += Vector3.up * 0.3f;
            hasDoneFlagShift = true;
        }
    }

    public void OnDestroy()
    {
        IDToObj<PuzzleFlag>.ClearNew();
    }

    public bool CheckForWin()
    {
        hasWon = Vector3.Distance(transform.position, GameInstance.Playerbody.transform.position) <= 0.5f;
        return hasWon;
    }
}
