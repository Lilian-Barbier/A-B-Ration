using System;
using System.Diagnostics.Contracts;
using UnityEngine;

public class Cuttable : MonoBehaviour
{
    SpriteMask spriteMask;
    [SerializeField] private int mass;

    void Start()
    {
        spriteMask = GetComponentInChildren<SpriteMask>();
    }

    // cuttingPosition is the world position of the cutting line
    internal int[] Cut(Vector3 cuttingPosition, SpriteRenderer partLeft, SpriteRenderer partRight)
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Sprite currentSprite = spriteRenderer.sprite;
        Bounds spriteBounds = currentSprite.bounds;

        // Get the width of the left part in world units : 
        float cuttingPositionInUnit = spriteBounds.extents.x + cuttingPosition.x - transform.position.x;

        // The width and left with are in world units, so we need to convert them to pixels
        float partLeftWidthUnit = cuttingPositionInUnit;
        float partRightWidthUnit = spriteBounds.size.x - cuttingPositionInUnit;

        int pixelTexWidth = currentSprite.texture.width;
        int pixelTexHeight = currentSprite.texture.height;

        int newPixelLeftWidth = (int)(partLeftWidthUnit / spriteBounds.size.x * pixelTexWidth);
        int newPixelRightWidth = (int)(partRightWidthUnit / spriteBounds.size.x * pixelTexWidth);

        // Create the textures for the left and right parts
        Texture2D leftTexture = new Texture2D(newPixelLeftWidth, pixelTexHeight);
        Texture2D rightTexture = new Texture2D(newPixelRightWidth, pixelTexHeight);

        int leftPixelCount = 0;
        int rightPixelCount = 0;

        // Copy the pixels from the original texture to the left texture
        for (int x = 0; x < newPixelLeftWidth; x++)
        {
            for (int y = 0; y < pixelTexHeight; y++)
            {
                Color pixel = currentSprite.texture.GetPixel(x, y);
                if (pixel.a != 0)
                {
                    leftPixelCount++;
                }
                leftTexture.SetPixel(x, y, pixel);
            }
        }

        // Copy the pixels from the original texture to the right texture
        for (int x = newPixelLeftWidth; x < pixelTexWidth; x++)
        {
            for (int y = 0; y < pixelTexHeight; y++)
            {
                Color pixel = currentSprite.texture.GetPixel(x, y);
                if (pixel.a != 0)
                {
                    rightPixelCount++;
                }
                rightTexture.SetPixel(x - newPixelLeftWidth, y, pixel);
            }
        }

        int totalPixelCount = leftPixelCount + rightPixelCount;

        int leftMass = (int)((float)leftPixelCount / totalPixelCount * mass);
        int rightMass = mass - leftMass;

        leftTexture.Apply();
        rightTexture.Apply();

        partLeft.sprite = Sprite.Create(leftTexture, new Rect(0, 0, newPixelLeftWidth, pixelTexHeight), new Vector2(0.5f, 0.5f));
        partRight.sprite = Sprite.Create(rightTexture, new Rect(0, 0, newPixelRightWidth, pixelTexHeight), new Vector2(0.5f, 0.5f));

        partLeft.transform.position = new Vector3(transform.position.x - spriteBounds.extents.x + partLeftWidthUnit / 2, transform.position.y, transform.position.z);
        partRight.transform.position = new Vector3(transform.position.x + spriteBounds.extents.x - partRightWidthUnit / 2, transform.position.y, transform.position.z);

        spriteRenderer.enabled = false;


        partLeft.enabled = true;
        partRight.enabled = true;

        return new int[] { leftMass, rightMass };
    }
}