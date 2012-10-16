using UnityEngine;

class CameraBehaviour : MonoBehaviour
{
    const float AspectRatio = 0.9f;

    public bool IsFogEnabled;

    void Start()
    {
        // determine the game window's current aspect ratio
        float windowAspect = camera.aspect;

        // current viewport height should be scaled by this amount
        float scaleHeight = windowAspect / AspectRatio;

        // if scaled height is less than current height, add letterbox
        if (scaleHeight < 1.0f)
        {
            Rect rect = camera.rect;

            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f;

            camera.rect = rect;
        }
        else // add pillarbox
        {
            float scalewidth = 1.0f / scaleHeight;

            Rect rect = camera.rect;

            rect.width = scalewidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scalewidth) / 2.0f;
            rect.y = 0;

            camera.rect = rect;
        }
    }

    void OnPreRender()
    {
        RenderSettings.fog = IsFogEnabled;
    }
}
