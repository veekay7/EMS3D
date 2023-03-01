using UnityEngine;

// Reference: https://kylewbanks.com/blog/create-fullscreen-background-image-in-unity2d-with-spriterenderer
[RequireComponent(typeof(SpriteRenderer))]
public class FullscreenSprite : MonoBehaviour
{
    private SpriteRenderer m_SpriteRenderer;
    private Camera m_mainCamera;


    private void Awake()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        m_mainCamera = Camera.main;

        float cameraHeight = m_mainCamera.orthographicSize * 2;
        Vector2 cameraSize = new Vector2(m_mainCamera.aspect * cameraHeight, cameraHeight);
        Vector2 spriteSize = m_SpriteRenderer.sprite.bounds.size;

        Vector2 scale = transform.localScale;
        if (cameraSize.x >= cameraSize.y)
        {
            // Landscape (or equal)
            scale *= cameraSize.x / spriteSize.x;
        }
        else
        {
            // Portrait
            scale *= cameraSize.y / spriteSize.y;
        }

        transform.position = Vector2.zero; // Optional
        transform.localScale = scale;
    }

    private void OnValidate()
    {
        if (m_SpriteRenderer == null)
        {
            m_SpriteRenderer = GetComponent<SpriteRenderer>();
            if (m_SpriteRenderer == null)
                m_SpriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }
    }
}
