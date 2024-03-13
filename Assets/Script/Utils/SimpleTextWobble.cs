using TMPro;
using UnityEngine;

public class SimpleTextWobble : MonoBehaviour
{
    public TMP_Text textComponent;

    public float wobbleSpeed = 2f;
    public float wobbleAmount = 5f;

    void Update()
    {
        Wobble();
    }

    void Wobble()
    {
        textComponent.ForceMeshUpdate();

        var textInfo = textComponent.textInfo;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            var charInfo = textInfo.characterInfo[i];

            if (!charInfo.isVisible) continue;

            var verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;

            for (int j = 0; j < 4; j++)
            {
                var orig = verts[charInfo.vertexIndex + j];
                verts[charInfo.vertexIndex + j] = orig + new Vector3(0, Mathf.Sin(Time.time * wobbleSpeed + orig.x * 0.01f) * wobbleAmount, 0);
            }
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            var meshInfo = textInfo.meshInfo[i];
            meshInfo.mesh.vertices = meshInfo.vertices;
            textComponent.UpdateGeometry(meshInfo.mesh, i);
        }
    }
}





