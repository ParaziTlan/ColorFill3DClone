using UnityEngine;

[RequireComponent(typeof(PlayerCharacterMover))]
public class InputController : MonoBehaviour
{
    public delegate void LevelManagingActions();
    public static event LevelManagingActions GameStarted;

    private PlayerCharacterMover playerCMover;
    private Vector2 swipeDirection;
    private float swipeMinMove = 50f; // kaç piksel sonra swipe algılansın    ... 1920 ye 1080 ekrana göre ayarlı !  
    private Vector2 startingSwipePos;
    private bool isGameEnded;
    private bool isGameStarted; // ilk input geldikten sonra enemyler hareket etsin diye entegre ettim ama, orjinal oyunda input beklemeden harekete başlıyorlar.

    private void Start()
    {
        playerCMover = GetComponent<PlayerCharacterMover>();
        //Piksel miktarını her ekrana oranlamak için ! 
        swipeMinMove = swipeMinMove.Remap(0, 1080, 0, Screen.width);
    }
    private void OnEnable()
    {
        LevelController.GameOverEvent += GameEnded;
        LevelController.LevelPassedEvent += GameEnded;
    }
    private void OnDisable()
    {
        LevelController.GameOverEvent -= GameEnded;
        LevelController.LevelPassedEvent -= GameEnded;
    }
    private void GameEnded()
    {
        isGameEnded = true;
        TouchFinished();
    }
    void Update()
    {
        if ((Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor) && (Input.GetMouseButton(0) || Input.GetMouseButtonUp(0)))
        {
            if (Input.GetMouseButtonDown(0))
                TouchStarted();

            if (!isGameEnded & isGameStarted) Touching();

            if (Input.GetMouseButtonUp(0))
                TouchFinished();
        }
        if ((Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) && Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
                TouchStarted();

            if (!isGameEnded & isGameStarted) Touching();

            if (Input.GetTouch(0).phase == TouchPhase.Ended)
                TouchFinished();
        }
    }


    private void TouchStarted()
    {
        if (!isGameEnded)
        {
            if (!isGameStarted)
            {
                isGameStarted = true;
                GameStarted?.Invoke();
            }
            startingSwipePos = Input.mousePosition;
        }
        else // game over dan veya nextlevel dan sonra herhangi bir touch halinde ne yapılsın ?
        {

        }
    }
    private void Touching()
    {
        GetInputAndMoveIfSwiped();
    }

    private void GetInputAndMoveIfSwiped()
    {
        Vector2 currentSwipePos = Input.mousePosition;
        Vector2 offset = (currentSwipePos - startingSwipePos);

        if (Mathf.Abs(offset.x) > Mathf.Abs(offset.y))
        {
            if (Mathf.Abs(offset.x) < swipeMinMove) return;
            swipeDirection = offset.x > 0 ? new Vector2(1f, 0) : new Vector2(-1f, 0);
        }
        else
        {
            if (Mathf.Abs(offset.y) < swipeMinMove) return;
            swipeDirection = offset.y > 0 ? new Vector2(0, 1f) : new Vector2(0, -1f);

        }
        MovePlayer();
    }

    private void MovePlayer()
    {
        playerCMover.MovePlayer(swipeDirection);
        startingSwipePos = Input.mousePosition; // reset swipe
    }

    private void TouchFinished()
    {

    }
}
