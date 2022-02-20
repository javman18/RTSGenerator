using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteDrawer { 
    static Sprite squareSprite;
    public static void DrawQuad(Rect position, Color color, GUIContent content = null)
    {

        if (!squareSprite)
        {
            squareSprite = Resources.Load<Sprite>("WhiteSquare");
        }
    }
}
