using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloodFill : MonoBehaviour
{
    public delegate void AnimationActions();
    public static event AnimationActions FloodFinished;

    private static int totalSum = 0;

    /// <summary>
    /// Tile Arrayinin başından ya da sonundan, empty tile arayıp bulur. Bulunca bu tile ın index ini döndürür, Bulamayınca -1 döndürür
    /// </summary>
    /// <param name="startFromBottom"></param>
    /// <returns></returns>
    private static Vector2 GetEmptyCoord(bool startFromBottom)
    {
        for (int x = 0; x < LevelController.instance.TileState2DArray.Width - 2; x++)
            for (int y = 0; y < LevelController.instance.TileState2DArray.Height - 2; y++)
                if (startFromBottom & LevelController.instance.TileState2DArray[x + 1, y + 1] == LevelController.TileStates.empty)
                    return new Vector2(x + 1, y + 1);
                else if (!startFromBottom & LevelController.instance.TileState2DArray[LevelController.instance.TileState2DArray.Width - 2 - x
                            , LevelController.instance.TileState2DArray.Height - 2 - y] == LevelController.TileStates.empty)
                    return new Vector2(LevelController.instance.TileState2DArray.Width - 2 - x, LevelController.instance.TileState2DArray.Height - 2 - y);

        return new Vector2(-1, -1); // -1 means: Empty Not Found! 
    }

    public static void ChooseSideAndFill()
    {
        if (LevelController.instance.playerCharacterMover != null)
            LevelController.instance.playerCharacterMover.canFill = false;

        //liste başından empty tile bul, koordinatını al
        totalSum = 0;
        Vector2 fromBottomCoord = GetEmptyCoord(true);
        if (fromBottomCoord.x == -1) // eğer empty yoksa!
        {
            FillPlayerTiles();
            return;
        }
        // empty tile ı calculation tile bottom ile dolduruyoruz. Toplam kaç adet doldurduysak bottomSum a kaydediyoruz
        Flood_fill((int)fromBottomCoord.x, (int)fromBottomCoord.y, LevelController.TileStates.empty, LevelController.TileStates.fromBottomForCalculation);
        int bottomSum = totalSum;

        //liste sonundan empty tile bul, koordinatını al
        totalSum = 0;
        Vector2 fromTopCoord = GetEmptyCoord(false);
        //// eğer empty yoksa .. Empty alanı 1 parça halindedir. yani sahnede Emptyler 2 ye bölünmemiştir! Sadece player tile ları fille ! 
        if (fromTopCoord.x == -1)
        {
            Flood_fill((int)fromBottomCoord.x, (int)fromBottomCoord.y, LevelController.TileStates.fromBottomForCalculation, LevelController.TileStates.empty);
            Debug.Log("player tile fillle");
        }
        else // 1 den fazla empty parçası var ise 
        {
            // empty tile ı calculation tile top ile dolduruyoruz. Toplam kaç adet doldurduysak topsum a kaydediyoruz
            Flood_fill((int)fromTopCoord.x, (int)fromTopCoord.y, LevelController.TileStates.empty, LevelController.TileStates.fromTopForCalculation);
            int topSum = totalSum;

            // Daha az olan empty tile alanınıı filled tile dolduruyoruz. Diğer alanı tekrar empty tile a çekiyoruz
            if (bottomSum < topSum)
            {
                Flood_fill((int)fromBottomCoord.x, (int)fromBottomCoord.y, LevelController.TileStates.fromBottomForCalculation, LevelController.TileStates.filled);
                Flood_fill((int)fromTopCoord.x, (int)fromTopCoord.y, LevelController.TileStates.fromTopForCalculation, LevelController.TileStates.empty);
                Debug.Log("Bottom Küçük  2 li \ntop: " + topSum + " bot " + bottomSum);
                LevelController.instance.CheckIsLevelFinished(bottomSum);
            }
            else
            {
                Flood_fill((int)fromBottomCoord.x, (int)fromBottomCoord.y, LevelController.TileStates.fromBottomForCalculation, LevelController.TileStates.empty);
                Flood_fill((int)fromTopCoord.x, (int)fromTopCoord.y, LevelController.TileStates.fromTopForCalculation, LevelController.TileStates.filled);
                Debug.Log("Top Küçük 2 li " + topSum + " bot " + bottomSum);
                LevelController.instance.CheckIsLevelFinished(topSum);
            }
        }

        FillPlayerTiles();
    }

    private static void FillPlayerTiles()
    {
        //Her halükarda player tile ı filliyoruz
        LevelController.instance.CheckIsLevelFinished(ChangePlayerTilesToFilled());

        // animasyonlar için event tetikliyoruz 
        FloodFinished?.Invoke();
    }

    private static int ChangePlayerTilesToFilled()
    {
        int filled = 0;
        foreach (GameObject playerObj in LevelController.instance.PlayerTilesList)
        {
            Vector2 normalizedPos = new Vector2(Mathf.RoundToInt(playerObj.transform.position.x) + 1, Mathf.RoundToInt(playerObj.transform.position.z) + 1);
            Destroy(playerObj);
            LevelController.instance.TileState2DArray[(int)normalizedPos.x, (int)normalizedPos.y] = LevelController.TileStates.filled;
            filled++;
        }
        LevelController.instance.PlayerTilesList = new List<GameObject>();
        return filled;
    }

    private static void Flood_fill(int pos_x, int pos_y, LevelController.TileStates target_state, LevelController.TileStates to_state)
    {
        if (LevelController.instance.TileState2DArray[pos_x, pos_y] == LevelController.TileStates.wall || LevelController.instance.TileState2DArray[pos_x, pos_y] == to_state) // if there is wall or already have to_state tile return
            return;

        if (LevelController.instance.TileState2DArray[pos_x, pos_y] != target_state) // if it's not targetState return
            return;

        LevelController.instance.TileState2DArray[pos_x, pos_y] = to_state; // set Tile to to_state
        totalSum++;

        Flood_fill(pos_x + 1, pos_y, target_state, to_state);  // then i can either go east
        Flood_fill(pos_x - 1, pos_y, target_state, to_state);  // or west
        Flood_fill(pos_x, pos_y + 1, target_state, to_state);  // or north
        Flood_fill(pos_x, pos_y - 1, target_state, to_state);  // or south

        return;
    }
}
