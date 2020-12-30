using UnityEngine;

public class ColorManager : MonoBehaviour
{
    public static ColorManager instance;
    public Material FilledMat;

    [SerializeField]
    private Material playerMat, playerTileMat;
    [SerializeField]
    private Color playerTileColor;
    private float prevT = -99;

    private void Awake()
    {
        if (instance != null && instance != this) Destroy(this.gameObject);
        else instance = this;
    }

    public void ChangeColor(float t)
    {
        if (t == prevT) return;
        prevT = t;

        playerTileMat.color = Color.Lerp(playerTileColor, playerMat.color, t);
        if (t == 1)
            playerTileMat.color = playerTileColor;
    }
}
