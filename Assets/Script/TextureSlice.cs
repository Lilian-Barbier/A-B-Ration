using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TextureSlice : MonoBehaviour
{
    GameObject cutLine;
    Bounds cutLineBounds;

    Sprite sprite;
    Bounds spriteBounds;

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>().sprite;
        spriteBounds = sprite.bounds;

        Debug.Log("Sprite bounds size x : " + spriteBounds.size.x);
        Debug.Log("Sprite bounds size y : " + spriteBounds.size.y);
        Debug.Log("Sprite bounds center : " + spriteBounds.center);
        Debug.Log("Sprite bounds min : " + spriteBounds.min);
        Debug.Log("Sprite bounds max : " + spriteBounds.max);
     
        cutLine = GameObject.FindWithTag("CutLine");

        Sprite cutLineSprite = cutLine.GetComponent<SpriteRenderer>().sprite;
        cutLineBounds = cutLineSprite.bounds;
    }

    void Update(){

        cutLineBounds.center = cutLine.transform.position;
        spriteBounds.center = transform.position;

        if(Input.GetKeyDown(KeyCode.Space)){
            StartCoroutine(Cut());
        }
    }

    Vector3 drawSpherePosition;

    IEnumerator Cut(){

        //Vérification de si la ligne de coupe est sur le sprite
        if(!cutLineBounds.Intersects(spriteBounds)) yield break;

        Texture2D texture = sprite.texture;

        //Création d'une nouvelle texture pour la partie de gauche
        Texture2D leftTexture = new Texture2D(texture.width, texture.height);

        // Copy the pixels from the original texture to the left texture
        for (int y = 0; y < texture.height; y++)
        {
    
            for (int x = 0; x < texture.width; x++)
            {
                //on vérifie si on n'a pas encore atteint le sprite de la découpe, si on l'atteint on casse la boucle
                
                //Fonctionne pas
                Vector3 pixelWorldPosition = new Vector3(x, y, 0) + transform.position - new Vector3(spriteBounds.extents.x, spriteBounds.extents.y, 0);
                if(cutLineBounds.Contains(pixelWorldPosition)){
                    break;
                }
                
                //Draw gizmos
                drawSpherePosition = pixelWorldPosition;
                yield return new WaitForSeconds(0.1f);


                Color pixel = texture.GetPixel(x, y);
                leftTexture.SetPixel(x, y, pixel);
            }
        }

        var leftPart = new GameObject("TexLeftPart");
        leftPart.transform.position = new Vector3(-7, 0, 0);
        leftPart.AddComponent<SpriteRenderer>();
        leftPart.GetComponent<SpriteRenderer>().sprite = Sprite.Create(leftTexture, new Rect(0, 0, leftTexture.width, leftTexture.height), new Vector2(0.5f, 0.5f));

        // var rightPart = new GameObject("TexRightPart");
        // rightPart.transform.position = new Vector3(7, 0, 0);
        // rightPart.AddComponent<SpriteRenderer>();
        // rightPart.GetComponent<SpriteRenderer>().sprite = Sprite.Create(texture, new Rect(texture.width / 2, 0, texture.width / 2, texture.height), new Vector2(0.5f, 0.5f));
   
    }


    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, spriteBounds.size);
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(cutLine.transform.position, cutLineBounds.size);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(drawSpherePosition, 0.1f);
    }
}
