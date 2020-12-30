using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
public class LevelEditorUIController : MonoBehaviour
{
    public delegate void OnWidthAndHeightSetDelegate(int width, int height);
    public static event OnWidthAndHeightSetDelegate OnWidthAndHeightSet;

    [SerializeField]
    private InputField widthInput, heightInput;
    [SerializeField]
    private Text infoText;
    [SerializeField]
    private Text currentText, enemyCurrentText;
    [SerializeField]
    private GameObject levelPanel, enemyPanel;

    private string[] currentStrings = new string[3] { "<color='white'>Empty</color>", "<color='black'>Wall</color>", "<color='purple'>(debug)</color>" };
    private string[] enemyCurrentString = new string[2] { "<color='white'>Empty</color>", "<color='blue'>Enemy Fragment</color>" };

    private void OnEnable()
    {
        EnemyEditorSpawner.OnEnmySpawnActiveChanged += OnEnmySpawnActiveChanged;
    }
    private void OnDisable()
    {
        EnemyEditorSpawner.OnEnmySpawnActiveChanged -= OnEnmySpawnActiveChanged;
    }

    private void OnEnmySpawnActiveChanged(bool isEnemyEditingActive)
    {
        levelPanel.SetActive(!isEnemyEditingActive);
        enemyPanel.SetActive(isEnemyEditingActive);

    }
    public void UpdateEnemyCurrentText(int index)
    {
        enemyCurrentText.text = "\n<color='red'>Current Tile: </color>" + enemyCurrentString[index];
    }

    public void UpdateCurrentText(int index)
    {
        currentText.text = "\n<color='red'>Current Tile: </color>" + currentStrings[index];
    }

    public void OnOkButtonClicked()
    {
        if (widthInput.text.Length == 0)
        {
            infoText.text = "Please enter width";
            return;
        }
        if (heightInput.text.Length == 0)
        {
            infoText.text = "Please enter height";
            return;
        }

        int width = Mathf.Abs(int.Parse(widthInput.text));
        int height = Mathf.Abs(int.Parse(heightInput.text));

        if (width < 3 | height < 3)
        {
            infoText.text = "Width and height must be higher than 3";
            return;
        }

        OnWidthAndHeightSet?.Invoke(width + 2, height + 2); // kenar duvarlarda eklenince +2 

        levelPanel.SetActive(true);

        DisablePanel();
    }
    public void DisablePanel()
    {
        widthInput.transform.parent.gameObject.SetActive(false);
        levelPanel.SetActive(true);
    }
}
#endif
