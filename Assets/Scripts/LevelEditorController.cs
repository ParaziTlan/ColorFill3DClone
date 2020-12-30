using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR

public class LevelEditorController : MonoBehaviour
{
    [HideInInspector]
    public SerializableState2DArrayClass activeState2DArray;
    [HideInInspector]
    public bool isEditingStarted = false;


    private LevelEditorUIController editorUI;
    private int activeIndex = 0; // 0,1 olabilir... 
    private int width, height;
    private bool isLevelEditingActive = true;
    private EnemyEditorSpawner enemyEditor;

    private void OnEnable()
    {
        LevelEditorUIController.OnWidthAndHeightSet += OnWidthAndHeightSet;
        LevelManager.OnWidthAndHeightSet += OnWidthAndHeightSet;
        EnemyEditorSpawner.OnEnmySpawnActiveChanged += OnEnemySpawnActiveChanged;
    }
    private void OnDisable()
    {
        LevelEditorUIController.OnWidthAndHeightSet -= OnWidthAndHeightSet;
        LevelManager.OnWidthAndHeightSet -= OnWidthAndHeightSet;
        EnemyEditorSpawner.OnEnmySpawnActiveChanged -= OnEnemySpawnActiveChanged;
    }

    private void OnWidthAndHeightSet(int _width, int _height)
    {
        width = _width;
        height = _height;
        activeState2DArray = new SerializableState2DArrayClass(_width, _height);

        //Set WALLS
        for (int i = 0; i < activeState2DArray.Height; i++)
        {
            activeState2DArray[0, i] = LevelController.TileStates.wall;
            activeState2DArray[activeState2DArray.Width - 1, i] = LevelController.TileStates.wall;
        }
        for (int i = 0; i < activeState2DArray.Width; i++)
        {
            activeState2DArray[i, 0] = LevelController.TileStates.wall;
            activeState2DArray[i, activeState2DArray.Height - 1] = LevelController.TileStates.wall;
        }
        isEditingStarted = true;
    }
    private void OnEnemySpawnActiveChanged(bool isEnemyEditingActive)
    {
        isLevelEditingActive = !isEnemyEditingActive;
    }
    void UpdateIndex(int index)
    {
        activeIndex = index;
        editorUI.UpdateCurrentText(index);
    }
    void Update()
    {
        if (isEditingStarted & isLevelEditingActive)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                UpdateIndex(0);
            else if (Input.GetKeyDown(KeyCode.Alpha2))
                UpdateIndex(1);
            //else if (Input.GetKeyDown(KeyCode.Alpha3))  // For DEBUG
            //    UpdateIndex(2);

            if (Input.GetMouseButton(0))
            {
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 2000f))
                {
                    Vector2 normalizedPos = new Vector2(Mathf.RoundToInt(hit.point.x) + 1, Mathf.RoundToInt(hit.point.z) + 1);
                    //Debug.Log(normalizedPos);
                    if (normalizedPos.x > 0 & normalizedPos.y > 0 & normalizedPos.x < activeState2DArray.Width - 1 & normalizedPos.y < activeState2DArray.Height - 1) // hit point grid içinde olmalı !
                        UpdateTile(normalizedPos, hit.transform.gameObject);
                }
            }
            if (Input.GetKeyDown(KeyCode.S))
                SaveNewLevelScriptableObject();
            if (Input.GetKeyDown(KeyCode.L))
                LoadLatestLevel();
        }
    }

    private void Awake()
    {
        LevelManager.instance.canLoadLevels = false;
    }
    private void Start()
    {
        editorUI = GetComponent<LevelEditorUIController>();
        enemyEditor = GetComponent<EnemyEditorSpawner>();
    }
    public void LoadLatestLevel()
    {
        if (LevelManager.instance.Levels.Length > 0)
        {
            //Erase CurrentLevel
            for (int i = 0; i < LevelController.instance.TilesParentObj.transform.childCount; i++)
                Destroy(LevelController.instance.TilesParentObj.transform.GetChild(i).gameObject);

            //LoadLevel
            LevelManager.instance.canLoadLevels = true;
            LevelManager.instance.LoadLevel(true);
            //Get Level variables
            LevelScriptableObject currentLevel = LevelManager.instance.Levels[LevelManager.instance.Levels.Length - 1] as LevelScriptableObject;

            activeState2DArray = new SerializableState2DArrayClass(currentLevel.Width, currentLevel.Height);
            for (int x = 0; x < activeState2DArray.Width; x++)
            {
                for (int y = 0; y < activeState2DArray.Height; y++)
                {
                    activeState2DArray[x, y] = currentLevel.State2DArray[x, y];
                }
            }
            isLevelEditingActive = true;
            enemyEditor.LoadEnemies();
            editorUI.DisablePanel();
        }
        else Debug.LogWarning("Kayıtlı Level YOK!");
    }
    private void UpdateTile(Vector2 normalizedPos, GameObject hitObj)
    {
        if (hitObj.CompareTag("Tile")) // NOT EMPTY TILE // tag lı değilse herhangi bir tile dır
        {
            normalizedPos = new Vector2(hitObj.transform.position.x + 1, hitObj.transform.position.z + 1); // NormalizedPos mathf.RoundToInt ile Geldiğinden 3 boyutta kübe çarpmış ama farklı bir int'e yuvarlanmış olabilme ihtimali var.

            if (normalizedPos.x <= 0 | normalizedPos.y <= 0 | normalizedPos.x >= activeState2DArray.Width - 1
                | normalizedPos.y >= activeState2DArray.Height - 1) return;  // hitObj Pozisyonu duvarlar içinde değilse return

            // no need to update Tile
            if (activeIndex.ToString()[0] == hitObj.name[0])
                return;

            //Destroy old tile
            Destroy(hitObj);
        }
        if (!hitObj.CompareTag("Enemy"))
            //Update2DArray
            activeState2DArray[(int)normalizedPos.x, (int)normalizedPos.y] = (LevelController.TileStates)activeIndex;
    }

    private void SaveNewLevelScriptableObject()
    {
        LevelScriptableObject levelScriptable = ScriptableObject.CreateInstance<LevelScriptableObject>();
        levelScriptable.name = "Level " + LevelManager.instance.Levels.Length.ToString("000") + "    NEW!!!";

        // Set levelScriptable Object Variables
        levelScriptable.State2DArray = activeState2DArray;
        levelScriptable.Width = width;
        levelScriptable.Height = height;
        GetAndSetEnemyVariables(levelScriptable);

        //Save levelScriptabl to project 
        AssetDatabase.CreateAsset(levelScriptable, "Assets/Resources/Levels/" + levelScriptable.name + ".asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        LevelManager.instance.GetLevels();
    }

    private void GetAndSetEnemyVariables(LevelScriptableObject levelScriptable)
    {
        if (enemyEditor.enemyControllerList.Count == 0) return;

        levelScriptable.EnemyArray = new SerializedEnemyVariables[enemyEditor.enemyControllerList.Count];
        for (int i = 0; i < enemyEditor.enemyControllerList.Count; i++)
        {
            levelScriptable.EnemyArray[i] = new SerializedEnemyVariables
            {
                fragmentCoords = enemyEditor.enemyControllerList[i].FragmentCoords.ToArray(),
                patrolWhere = enemyEditor.enemyControllerList[i].PatrolWhere,
                Speed = enemyEditor.enemyControllerList[i].Speed
            };
        }
    }
}
#endif
