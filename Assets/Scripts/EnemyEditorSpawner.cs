using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
#if UNITY_EDITOR
public class EnemyEditorSpawner : MonoBehaviour
{
    public delegate void OnEnemySpawnActive(bool isEnemy);
    public static event OnEnemySpawnActive OnEnmySpawnActiveChanged;
    [HideInInspector]
    public List<EnemyController> enemyControllerList = new List<EnemyController>();

    [SerializeField]
    private GameObject EnemyPrefab;
    [SerializeField]
    private Material previouslySpawnedEnemyMat;

    private bool isEnemyEditingActive = false;
    private bool isSpawningFragment = false;
    private GameObject lastInstantiatedEnemy = null;
    private EnemyController lastInstantiatedController;
    private LevelEditorController levelEditor;
    private LevelEditorUIController levelUI;

    private void Start()
    {
        levelEditor = GetComponent<LevelEditorController>();
        levelUI = GetComponent<LevelEditorUIController>();
    }
    void Update()
    {
        if (levelEditor.isEditingStarted)
            if (Input.GetKeyDown(KeyCode.N))
            {
                isEnemyEditingActive = true;
                OnEnmySpawnActiveChanged?.Invoke(isEnemyEditingActive);
                SpawnNewEnemy();
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (lastInstantiatedEnemy != null)
                    SetEnemyToPreviouslySpawned(lastInstantiatedEnemy);
                isEnemyEditingActive = false;
                OnEnmySpawnActiveChanged?.Invoke(isEnemyEditingActive);
                Selection.activeObject = null;
            }
            else if (isEnemyEditingActive)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    isSpawningFragment = false;
                    levelUI.UpdateEnemyCurrentText(0);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    isSpawningFragment = true;
                    levelUI.UpdateEnemyCurrentText(1);
                }
                else if (Input.GetMouseButton(0))
                {
                    RaycastHit hit;
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 2000f))
                    {
                        Vector2 normalizedPos = new Vector2(Mathf.RoundToInt(hit.point.x), Mathf.RoundToInt(hit.point.z));
                        if (normalizedPos.x >= 0 & normalizedPos.y >= 0 & normalizedPos.x < levelEditor.activeState2DArray.Width - 1 & normalizedPos.y < levelEditor.activeState2DArray.Height - 1) // hit point grid içinde olmalı !
                            UpdateEnemyFragment(normalizedPos, hit.transform.gameObject);
                    }
                }
            }
    }
    private void UpdateEnemyFragment(Vector2 normalizedPos, GameObject hitObj)
    {
        if (hitObj.CompareTag("Enemy") & !isSpawningFragment) // hitted a spawned Enemy
        {
            normalizedPos = new Vector2(hitObj.transform.position.x, hitObj.transform.position.z); // NormalizedPos mathf.RoundToInt ile Geldiğinden 3 boyutta kübe çarpmış ama farklı bir int'e yuvarlanmış olabilme ihtimali var.

            lastInstantiatedController.FragmentCoords.Remove(normalizedPos);
            Destroy(hitObj);
        }
        else if (hitObj.CompareTag("Untagged") & isSpawningFragment)  // spawn
        {
            if (lastInstantiatedController.FragmentCoords.IndexOf(new Vector2((int)normalizedPos.x, (int)normalizedPos.y)) != -1)
                return;

            lastInstantiatedController.FragmentCoords.Add(new Vector2((int)normalizedPos.x, (int)normalizedPos.y));
            lastInstantiatedController.LoadFragments(true);
        }
    }

    private void OnEnable()
    {
        Selection.selectionChanged += changed;
    }
    private void OnDisable()
    {
        Selection.selectionChanged -= changed;
    }

    void changed()
    {
        //keep select ActiveEnemy to FORCE LEVEL DESIGNER TO Edit enemyController's variables!
        if (isEnemyEditingActive & Selection.activeObject != lastInstantiatedEnemy)
        {
            Debug.LogWarning("Fill EnemyController Variables!!!");
            StartCoroutine(WaitThenSelect());
        }
    }
    IEnumerator WaitThenSelect()
    {
        yield return new WaitForSeconds(0.2f);
        Selection.activeObject = lastInstantiatedEnemy;
    }
    void SpawnNewEnemy()
    {
        if (lastInstantiatedEnemy != null)
            SetEnemyToPreviouslySpawned(lastInstantiatedEnemy);

        lastInstantiatedEnemy = Instantiate(EnemyPrefab, LevelManager.instance.EnemyParent.transform);
        lastInstantiatedController = lastInstantiatedEnemy.GetComponent<EnemyController>();
        enemyControllerList.Add(lastInstantiatedController);

        Selection.activeObject = lastInstantiatedEnemy;
    }

    private void SetEnemyToPreviouslySpawned(GameObject prevEnemy)
    {
        for (int j = 0; j < prevEnemy.transform.childCount; j++) // hafif flu yap
        {
            prevEnemy.transform.GetChild(j).tag = "Wall";
            prevEnemy.transform.GetChild(j).GetComponent<Renderer>().sharedMaterial = previouslySpawnedEnemyMat;
        }
    }
    public void LoadEnemies()
    {
        for (int i = 0; i < LevelManager.instance.EnemyParent.transform.childCount; i++)
        {
            EnemyController activeEnemy = LevelManager.instance.EnemyParent.transform.GetChild(i).GetComponent<EnemyController>();
            enemyControllerList.Add(activeEnemy);

            SetEnemyToPreviouslySpawned(activeEnemy.gameObject);
        }
    }
}
#endif
