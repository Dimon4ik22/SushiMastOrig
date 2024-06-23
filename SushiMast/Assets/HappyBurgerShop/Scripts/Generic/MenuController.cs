using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public static string gameBundleName = "com.yourcompany.yourgamename";
    public static string gameName = "gameName";

    private float buttonAnimationSpeed = 9;
    private bool canTap = true;

    public GameObject playerMoney;
    private int availableMoney;

    public AudioClip tapSfx;

    private GameObject _muteButton;
    private GameObject _unMuteButton;
    private AudioSource audioSource;

    void Awake()
    {
        Time.timeScale = 1.0f;

        availableMoney = PlayerPrefs.GetInt("PlayerMoney");
        playerMoney.GetComponent<TextMesh>().text = "Δενόγθ: " + availableMoney;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.LoadMuteState();
        }
    }

    void Start()
    {
        _muteButton = GameObject.Find("Button-Mute");
        _unMuteButton = GameObject.Find("Button-UnMute");

        audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        {
            AudioManager.Instance?.RegisterAudioSource(audioSource);
        }

        UpdateMuteButtons();
    }

    private void OnDestroy()
    {
        if (audioSource != null)
        {
            AudioManager.Instance?.UnregisterAudioSource(audioSource);
        }
    }

    void Update()
    {
        if (canTap)
        {
            StartCoroutine(tapManager());
        }
    }

    private void UpdateMuteButtons()
    {
        if (AudioManager.Instance != null && AudioManager.Instance.GetMuteState())
        {
            _muteButton.SetActive(false);
            _unMuteButton.SetActive(true);
        }
        else
        {
            _muteButton.SetActive(true);
            _unMuteButton.SetActive(false);
        }
    }

    private RaycastHit hitInfo;
    private Ray ray;
    IEnumerator tapManager()
    {
        if (Input.touches.Length > 0 && Input.touches[0].phase == TouchPhase.Ended)
            ray = Camera.main.ScreenPointToRay(Input.touches[0].position);
        else if (Input.GetMouseButtonUp(0))
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        else
            yield break;

        if (Physics.Raycast(ray, out hitInfo))
        {
            GameObject objectHit = hitInfo.transform.gameObject;
            switch (objectHit.name)
            {
                case "Button-01":
                    playSfx(tapSfx);
                    StartCoroutine(animateButton(objectHit));
                    yield return new WaitForSeconds(1.0f);
                    SceneManager.LoadScene("LevelSelection");
                    break;

                case "Button-02":
                    playSfx(tapSfx);
                    StartCoroutine(animateButton(objectHit));
                    yield return new WaitForSeconds(1.0f);
                    SceneManager.LoadScene("Shop");
                    break;

                case "Button-03":
                    playSfx(tapSfx);
                    StartCoroutine(animateButton(objectHit));
                    yield return new WaitForSeconds(1.0f);
                    Application.OpenURL("market://details?id=" + gameBundleName);
                    break;

                case "Button-VideoAds":
                    playSfx(tapSfx);
                    StartCoroutine(animateButton(objectHit));
                    yield return new WaitForSeconds(1.0f);
                    break;

                case "Button-Mute":
                    playSfx(tapSfx);
                    StartCoroutine(animateButton(objectHit));
                    if (AudioManager.Instance != null)
                    {
                        AudioManager.Instance.SetMuteState(true);
                    }
                    UpdateMuteButtons();
                    yield return new WaitForSeconds(1.0f);
                    break;

                case "Button-UnMute":
                    playSfx(tapSfx);
                    StartCoroutine(animateButton(objectHit));
                    if (AudioManager.Instance != null)
                    {
                        AudioManager.Instance.SetMuteState(false);
                    }
                    UpdateMuteButtons();
                    yield return new WaitForSeconds(1.0f);
                    break;
            }
        }
    }

    IEnumerator animateButton(GameObject _btn)
    {
        canTap = false;
        Vector3 startingScale = _btn.transform.localScale;
        Vector3 destinationScale = startingScale * 0.85f;

        float t = 0.0f;
        while (t <= 1.0f)
        {
            t += Time.deltaTime * buttonAnimationSpeed;
            _btn.transform.localScale = new Vector3(Mathf.SmoothStep(startingScale.x, destinationScale.x, t),
                                                    Mathf.SmoothStep(startingScale.y, destinationScale.y, t),
                                                    _btn.transform.localScale.z);
            yield return 0;
        }

        float r = 0.0f;
        if (_btn.transform.localScale.x >= destinationScale.x)
        {
            while (r <= 1.0f)
            {
                r += Time.deltaTime * buttonAnimationSpeed;
                _btn.transform.localScale = new Vector3(Mathf.SmoothStep(destinationScale.x, startingScale.x, r),
                                                        Mathf.SmoothStep(destinationScale.y, startingScale.y, r),
                                                        _btn.transform.localScale.z);
                yield return 0;
            }
        }

        if (r >= 1)
            canTap = true;
    }

    void playSfx(AudioClip _clip)
    {
        if (audioSource != null)
        {
            audioSource.clip = _clip;
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
    }
}
