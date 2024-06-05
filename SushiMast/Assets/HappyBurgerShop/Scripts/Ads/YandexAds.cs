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

        //Mouse of touch?
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
                    AddMoneyExtern(100, 30);

                    StartCoroutine(reactiveTap());

                    break;
            }
        }
    }
    public void AddMoneyOrTime(string values)
    {
        // ��������� ������ �� ��� ��������
        string[] splitValues = values.Split(',');
        int value1 = int.Parse(splitValues[0]);
        int value2 = int.Parse(splitValues[1]);

        if (isSaveMe)
        {
            MainGameController.startTime = (int)Time.time + value2;
            MainGameController.gameIsFinished = false;
            GameObject egp = GameObject.FindGameObjectWithTag("EndGamePlane");
            if (egp)
            {
                egp.SetActive(false);
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
