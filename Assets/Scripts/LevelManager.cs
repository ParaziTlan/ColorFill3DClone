using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[DefaultExecutionOrder(-1)]
public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public delegate void OnWidthAndHeightSetDelegate(int width, int height);
    public static event OnWidthAndHeightSetDelegate OnWidthAndHeightSet;
    public delegate void LevelManagingActions();
    public static event LevelManagingActions LevelStarted;

    [HideInInspector]
    public Object[] Levels;
    [HideInInspector]
    public bool canLoadLevels = true; // sadece level editör scripti kullanıyor
    [HideInInspector]
    public GameObject EnemyParent;

    public int n_level = 0;
    public GameObject EnemyPrefab;

    private void Awake()
    {
        if (instance != null && instance != this) Destroy(this.gameObject);
        else instance = this;

        GetLevels();

        n_level = PlayerPrefs.GetInt("n_level");
    }

    public void GetLevels()
    {
        Levels = Resources.LoadAll("Levels", typeof(LevelScriptableObject));
    }

    private void Start()
    {
        EnemyParent = new GameObject("Enemy Parent");
        if (canLoadLevels)
            LoadLevel();
    }

    public void LoadLevel(bool latestForEditor = false) // bool latest sadece level editör scripti için gerekli
    {
        int levelIndex = n_level;
        if (latestForEditor)
            levelIndex = Levels.Length - 1;

        if (Levels.Length == 0)
        {
            Debug.LogWarning("Kayıtlı herhangi bir level yok");
            return;
        }
        if (levelIndex >= Levels.Length)
        {
            Debug.LogWarning(levelIndex + ". Level kayıtlı değil.\nKayıtlı ilk level yükleniyor");
            PlayerPrefs.SetInt("n_level", 0);
            levelIndex = 0;
        }

        LevelScriptableObject currentLevelScriptable = Levels[levelIndex] as LevelScriptableObject;
        LevelController.instance.TileState2DArray = new SerializableState2DArrayClass(currentLevelScriptable.Width, currentLevelScriptable.Height);

        //Set everyState 1 by 1 making SerializableState2DArrayClass instantiate tile visuals
        for (int x = 0; x < currentLevelScriptable.Width; x++)
        {
            for (int y = 0; y < currentLevelScriptable.Height; y++)
            {
                LevelController.instance.TileState2DArray[x, y] = currentLevelScriptable.State2DArray[x, y];
            }
        }

        if (latestForEditor)
            for (int i = 0; i < EnemyParent.transform.childCount; i++) Destroy(EnemyParent.transform.GetChild(i).gameObject);  // Destroy active enemies beforeLoading

        LoadEnemies(currentLevelScriptable);
        OnWidthAndHeightSet?.Invoke(currentLevelScriptable.Width, currentLevelScriptable.Height);

        //check closed Fill available   ....
        //FloodFill.ChooseSideAndFill(); // game designer ın tasarım hatası. level tasarlarken kapalı alanlar bırakmamalı. bırakması durumunda , o kapalı alanı doldurması için "//" kaldırmak yeterli

        if (!latestForEditor)
            LevelStarted?.Invoke();   // MOVE ENEMIES!
    }

    private void LoadEnemies(LevelScriptableObject levelScriptable)
    {
        for (int i = 0; i < levelScriptable.EnemyArray.Length; i++)
        {
            EnemyController createdEnemy = Instantiate(EnemyPrefab, Vector3.zero, Quaternion.identity, EnemyParent.transform).GetComponent<EnemyController>();
            createdEnemy.FragmentCoords = new List<Vector2>(levelScriptable.EnemyArray[i].fragmentCoords);
            createdEnemy.PatrolWhere = levelScriptable.EnemyArray[i].patrolWhere;
            createdEnemy.Speed = levelScriptable.EnemyArray[i].Speed;
            createdEnemy.LoadFragments();
        }
    }

}
