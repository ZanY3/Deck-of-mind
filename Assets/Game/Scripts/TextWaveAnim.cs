using TMPro;
using UnityEngine;

public class TextWaveAnim : MonoBehaviour
{
    public float amplitude = 5f;     // высота волны
    public float frequency = 3f;     // скорость
    public float letterOffset = 0.4f; // сдвиг между буквами

    TMP_Text text;
    Mesh mesh;
    Vector3[] vertices;

    void Awake()
    {
        text = GetComponent<TMP_Text>();
    }

    void Update()
    {
        text.ForceMeshUpdate();
        mesh = text.mesh;
        vertices = mesh.vertices;

        for (int i = 0; i < text.textInfo.characterCount; i++)
        {
            var charInfo = text.textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

            int index = charInfo.vertexIndex;

            float wave = Mathf.Sin(Time.time * frequency + i * letterOffset) * amplitude;

            vertices[index + 0].y += wave;
            vertices[index + 1].y += wave;
            vertices[index + 2].y += wave;
            vertices[index + 3].y += wave;
        }

        mesh.vertices = vertices;
        text.canvasRenderer.SetMesh(mesh);
    }
}
