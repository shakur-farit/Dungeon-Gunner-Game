using System.Collections;
using UnityEngine;

public class MaterializeEffect : MonoBehaviour
{
    public IEnumerator MaterializeRoutine(Shader materializeShader, Color materializeColor,
        float materializeTime, SpriteRenderer[] spriteRendererArray, Material normalMaterial)
    {
        Material materializeMaterial = new Material(materializeShader);

        materializeMaterial.SetColor("_EmissionColor", materializeColor);

        foreach (SpriteRenderer spriteRenderer in spriteRendererArray)
        {
            spriteRenderer.material = materializeMaterial;
        }

        float dissoleAmount = 0f;

        while(dissoleAmount < 1f)
        {
            dissoleAmount += Time.deltaTime / materializeTime;

            materializeMaterial.SetFloat("_DissolveAmount", dissoleAmount);

            yield return null;
        }

        foreach (SpriteRenderer spriteRenderer in spriteRendererArray)
        {
            spriteRenderer.material = normalMaterial;
        }
    }
}
