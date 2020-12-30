using UnityEngine;
using System.Collections;

public class PlayerCharacterMover : MonoBehaviour
{
    public float Speed = 3.5f;

    [HideInInspector]
    public bool canFill = false;

    private Rigidbody rb;
    private Vector2 latestDirection;
    private bool isCommandWaiting;

    private void OnEnable()
    {
        LevelManager.OnWidthAndHeightSet += OnWidthAndHeightSet;
        LevelController.GameOverEvent += GameOver;
    }
    private void OnDisable()
    {
        LevelManager.OnWidthAndHeightSet -= OnWidthAndHeightSet;
        LevelController.GameOverEvent -= GameOver;
    }
    private void GameOver()
    {
        StopAllCoroutines();
        rb.velocity = Vector3.zero;
    }
    private void OnWidthAndHeightSet(int width, int height) // move to startingPos Update tile to filled at startingPos
    {
        int playerStartingPosX = width / 2 - 1;
        transform.position = new Vector3(playerStartingPosX, 0.5f, 0);
        LevelController.instance.TileState2DArray[playerStartingPosX + 1, 1] = LevelController.TileStates.filled;
        LevelController.instance.TilesParentObj.transform.GetChild(LevelController.instance.TilesParentObj.transform.childCount - 1)
            .GetComponent<Renderer>().sharedMaterial = ColorManager.instance.FilledMat;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        LevelController.instance.playerCharacterMover = this;
    }
    public void MovePlayer(Vector2 _joystickDirection)
    {

        if (_joystickDirection == latestDirection * -1f & LevelController.instance.TileState2DArray[Mathf.RoundToInt(transform.position.x) + 1 - (int)latestDirection.x
                , Mathf.RoundToInt(transform.position.z) + 1 - (int)latestDirection.y] == LevelController.TileStates.player) //PlayerTile yönüne doğru tam ters istikamete gidilmesin
            return;
        if (_joystickDirection == latestDirection) // tek yönde 1 kere swipe alsın
            return;

        isCommandWaiting = true;

        UpdateLastTileToPlayerTile();
        StopAllCoroutines();
        StartCoroutine(CheckForNextCommand(_joystickDirection));
    }
    private IEnumerator CheckForNextCommand(Vector2 _joystickDirection)
    {
        float differenceX = Mathf.Abs(Mathf.RoundToInt(transform.position.x) - transform.position.x);
        float differenceZ = Mathf.Abs(Mathf.RoundToInt(transform.position.z) - transform.position.z);

        while (differenceX > 0.1f | differenceZ > 0.1f)
        {
            differenceX = Mathf.Abs(Mathf.RoundToInt(transform.position.x) - transform.position.x);
            differenceZ = Mathf.Abs(Mathf.RoundToInt(transform.position.z) - transform.position.z);
            yield return new WaitForEndOfFrame();
        }

        UpdateLastTileToPlayerTile();

        if (isCommandWaiting)
        {
            isCommandWaiting = false;
            GetNextCommand(_joystickDirection);
        }

        yield return new WaitForFixedUpdate();
        StartCoroutine(CheckForNextCommand(_joystickDirection));
    }
    private void GetNextCommand(Vector2 _joystickDirection)
    {
        rb.velocity = Vector3.zero;
        transform.position = new Vector3(Mathf.RoundToInt(transform.position.x), 0.5f, Mathf.RoundToInt(transform.position.z));
        rb.velocity = new Vector3(_joystickDirection.x, 0, _joystickDirection.y) * Speed;
        latestDirection = _joystickDirection;
    }
    public void StopMoving()
    {
        UpdateLastTileToPlayerTile();
        StopAllCoroutines();
        rb.velocity = Vector3.zero;
        transform.position = new Vector3(Mathf.RoundToInt(transform.position.x), 0.5f, Mathf.RoundToInt(transform.position.z));
    }
    private void UpdateLastTileToPlayerTile()
    {
        int posX = Mathf.RoundToInt(transform.position.x) + 1;
        int posY = Mathf.RoundToInt(transform.position.z) + 1;

        if (posX > 0 & posY > 0 & posX < LevelController.instance.TileState2DArray.Width - 1 & posY < LevelController.instance.TileState2DArray.Height - 1)
            if (LevelController.instance.TileState2DArray[posX, posY] != LevelController.TileStates.player
                & LevelController.instance.TileState2DArray[posX, posY] != LevelController.TileStates.filled)
            {
                LevelController.instance.TileState2DArray[posX, posY] = LevelController.TileStates.player;
                canFill = true;
            }
    }
}
