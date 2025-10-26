using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.InputSystem;

public class StoryManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI storyText;
    public Image storyImage;

    [Header("Story Content")]
    public StoryBeat[] storyBeats; // Each beat has text + optional image

    [Header("Settings")]
    public float typeSpeed = 0.05f;
    public float finalPauseTime = 2.0f;

    [Header("Scene To Load")]
    public string gameSceneName = "GameScene";

    private bool clickReceived = false;

    void Start()
    {
        if (storyText == null || storyImage == null)
        {
            Debug.LogError("Story UI elements not assigned in the Inspector!");
            return;
        }

        storyImage.color = new Color(1, 1, 1, 0);
        StartCoroutine(RunStorySequence());
    }

    void Update()
    {
        // Detect mouse click with new Input System
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            clickReceived = true;
        }
        
        // Support touch for mobile
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            clickReceived = true;
        }
    }

    IEnumerator RunStorySequence()
    {
        for (int i = 0; i < storyBeats.Length; i++)
        {
            StoryBeat beat = storyBeats[i];

            // Handle image changes
            if (beat.newImage != null)
            {
                // Fade out current image if one is showing
                if (storyImage.color.a > 0)
                {
                    yield return StartCoroutine(FadeOutImage());
                }
                // Fade in new image
                yield return StartCoroutine(FadeInImage(beat.newImage));
            }
            else if (beat.hideImage && storyImage.color.a > 0)
            {
                // Fade out if requested
                yield return StartCoroutine(FadeOutImage());
            }

            // Type the text
            yield return StartCoroutine(TypeText(beat.text));

            // Wait for input (unless it's the last beat)
            if (i < storyBeats.Length - 1)
            {
                yield return StartCoroutine(WaitForClick());
            }
        }

        // Final pause before loading game scene
        yield return new WaitForSeconds(finalPauseTime);

        Debug.Log("Story finished, loading game scene.");
        SceneManager.LoadScene(gameSceneName);
    }

    IEnumerator WaitForClick()
    {
        clickReceived = false;
        yield return new WaitUntil(() => clickReceived);
    }

    IEnumerator TypeText(string line)
    {
        storyText.text = "";
        foreach (char c in line.ToCharArray())
        {
            storyText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }
    }

    IEnumerator FadeInImage(Sprite sprite)
    {
        storyImage.sprite = sprite;
        storyImage.color = new Color(1, 1, 1, 0);

        float t = 0;
        while (t < 1.0f)
        {
            t += Time.deltaTime;
            storyImage.color = new Color(1, 1, 1, t);
            yield return null;
        }
        storyImage.color = new Color(1, 1, 1, 1);
    }

    IEnumerator FadeOutImage()
    {
        float t = 1;
        while (t > 0.0f)
        {
            t -= Time.deltaTime;
            storyImage.color = new Color(1, 1, 1, t);
            yield return null;
        }
        storyImage.color = new Color(1, 1, 1, 0);
    }
}

// Define this class OUTSIDE the StoryManager class (at the bottom of the file)
[System.Serializable]
public class StoryBeat
{
    [TextArea(2, 4)]
    public string text;
    
    [Tooltip("Leave empty to keep current image")]
    public Sprite newImage;
    
    [Tooltip("Check this to fade out the current image")]
    public bool hideImage;
}