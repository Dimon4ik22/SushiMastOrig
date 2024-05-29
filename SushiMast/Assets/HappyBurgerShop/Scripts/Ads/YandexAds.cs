using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

public class YandexAds : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void Hello();

    private bool canTap = true;
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
                    Hello();
                    //hide videoAds button by moving it far away
                    transform.position = new Vector3(transform.position.x, transform.position.y, 10000);
                    StartCoroutine(reactiveTap());

                    break;
            }
        }
    }
    IEnumerator reactiveTap()
    {
        yield return new WaitForSeconds(1.0f);
        canTap = true;
    }
}
