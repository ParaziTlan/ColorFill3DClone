using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public static LevelController instance;
    public static event LevelManager.LevelManagingActions GameOverEvent;
    public static event LevelManager.LevelManagingActions LevelPassedEvent;
    public enum TileStates
    {
        empty,
        wall,
        player,
        filled,
        fromBottomForCalculation,
        fromTopForCalculation
    }
    [HideInInspector]
    public GameObject TilesParentObj;
    [HideInInspector]
    public List<GameObject> PlayerTilesList;
    [HideInInspector]
    public PlayerCharacterMover playerCharacterMover;
    [HideInInspector]
    public SerializableState2DArrayClass TileState2DArray;

    public GameObject[] tilePrefabs;

    private bool isGameEnded = false;
    private int fillNeededToPassLevel;
    private int fillCount;

    private void Awake()
    {
        if (instance != null && instance != this) Destroy(this.gameObject);
        else instance = this;

        TilesParentObj = new GameObject("Tiles Parent");
        PlayerTilesList = new List<GameObject>();
    }

    public void UpdateTileVisuals(int posX, int posY, LevelController.TileStates state)
    {
        if (state == LevelController.TileStates.empty | state == LevelController.TileStates.fromBottomForCalculation
            | state == LevelController.TileStates.fromTopForCalculation)
            return;

        GameObject instantiatedObj = Instantiate(tilePrefabs[(int)state - 1], new Vector3(posX - 1, 0, posY - 1), Quaternion.identity, TilesParentObj.transform);
        if (state == LevelController.TileStates.player)
            PlayerTilesList.Add(instantiatedObj);
    }

    private void Start()
    {
        //Calculate fillNeededToPassLevel
        for (int x = 0; x < TileState2DArray.Width - 1; x++)
            for (int y = 0; y < TileState2DArray.Height - 1; y++)
                if (TileState2DArray[x, y] == LevelController.TileStates.empty)
                    fillNeededToPassLevel++;
    }

    public void CheckIsLevelFinished(int filledAmount)
    {
        fillCount += filledAmount;
        //float InGameUILevelProgressBarAmount = (float)fillCount / filledAmount;  //UpdateUI!
        int fillLeftToPass = fillNeededToPassLevel - fillCount;
        if (fillLeftToPass <= 4)
            FillAllLeftTiles();
    }

    private void FillAllLeftTiles()
    {
        for (int x = 0; x < TileState2DArray.Width - 1; x++)
            for (int y = 0; y < TileState2DArray.Height - 1; y++)
                if (TileState2DArray[x, y] == LevelController.TileStates.empty)
                    TileState2DArray[x, y] = LevelController.TileStates.filled;

        LevelPassedEvent?.Invoke();
        PlayerPrefs.SetInt("n_level", PlayerPrefs.GetInt("n_level") + 1);
        StartCoroutine(WaitThenLoadScene());
        Debug.Log("Level PASSED EVENT!");
    }
    private IEnumerator WaitThenLoadScene()
    {
        yield return new WaitForSeconds(0.5f);
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
    public void GameOver(bool loadScene = true)
    {
        if (!isGameEnded)
        {
            isGameEnded = true;
            Debug.Log("GameOver EVENT");
            if (loadScene) StartCoroutine(WaitThenLoadScene());
            GameOverEvent?.Invoke();
        }
    }
    public void StartTileAnimation()
    {
        GameOver(false);
        StartCoroutine(TileAnimation());
    }
    private IEnumerator TileAnimation()
    {
        for (int i = 0; i < PlayerTilesList.Count; i++)
        {
            PlayerTilesList[i].SetActive(false);
            yield return new WaitForSeconds(0.1f);
        }
        StartCoroutine(WaitThenLoadScene());
    }
    public void DisablePlayerTiles()
    {
        foreach (GameObject playerTile in PlayerTilesList)
            playerTile.SetActive(false);

        GameOver();
    }

}
