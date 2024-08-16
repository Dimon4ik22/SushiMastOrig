using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class YandexAds : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void AddMoneyExtern(int value1, int value2);

    private bool canTap = true;

    public bool isSaveMe = false;

    void Update()
    {
        if (canTap)
            touchManager();
    }

    private RaycastHit hitInfo;
    private Ray ray;

    void touchManager()
    {
        //Mouse or touch?
        if (Input.touches.Length > 0 && Input.touches[0].phase == TouchPhase.Ended)
            ray = Camera.main.ScreenPointToRay(Input.touches[0].position);
        else if (Input.GetMouseButtonUp(0))
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        else
            return;

        if (Physics.Raycast(ray, out hitInfo))
        {
            GameObject objectHit = hitInfo.transform.gameObject;
            switch (objectHit.name)
            {
                case "Button-VideoAds":
                    canTap = false;
                    AddMoneyExtern(100, 45);
                    StartCoroutine(reactiveTap());
                    break;
            }
        }
    }

    public void AddMoneyOrTime(string values)
    {
        // Split the string into two values
        string[] splitValues = values.Split(',');
        int value1 = int.Parse(splitValues[0]);
        int value2 = int.Parse(splitValues[1]);

        if (isSaveMe)
        {
            MainGameController.startTime += value2;
            MainGameController.gameIsFinished = false;
            GameObject egp = GameObject.FindGameObjectWithTag("EndGamePlane");
            if (egp)
            {
                egp.SetActive(false);
                UnmuteAudio();
                AudioSource audioSource = GetComponent<AudioSource>();
                audioSource.Play(); // Начинаем воспроизведение заново
            }
        }
        else
        {
            PlayerPrefs.SetInt("PlayerMoney", PlayerPrefs.GetInt("PlayerMoney") + value1);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void MuteAudio()
    {
        AudioListener.volume = 0;
    }

    public void UnmuteAudio()
    {
        AudioListener.volume = 1;
    }

    IEnumerator reactiveTap()
    {
        yield return new WaitForSeconds(1.0f);
        canTap = true;
    }
}
