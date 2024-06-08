using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ButtonCompl : MonoBehaviour
{
    private GameObject customers;
    private GameObject ingredients;
    public bool isOrderReady = false;
    private bool isButtonPressed = false;

    public Slider progressSlider;
    private Image _fillImage;

    private float buttonPressTime = 0f;
    private const float REQUIRED_PRESS_TIME = 2f; // 5 секунд

    private void OnEnable()
    {
        MainGameController.OnCustomerSpawned += HandleCustomerSpawned;
    }

    private void OnDisable()
    {
        MainGameController.OnCustomerSpawned -= HandleCustomerSpawned;
    }

    private void HandleCustomerSpawned(GameObject customer)
    {
        // Сохраняем ссылку на созданный объект customer
        customers = customer;
    }
    private void Awake()
    {
        _fillImage = progressSlider.fillRect.GetComponent<Image>();
    }
    // Start is called before the first frame update
    void Update()
    {
        // Проверяем условия для обновления прогресса
        if (isButtonPressed
            && customers != null
            && customers.GetComponent<CustomerController>() != null 
            && customers.GetComponent<CustomerController>().isOnSeat
            && MainGameController.deliveryQueueItems == CustomerController.orderIngredientsIDs.Length)
        {
            progressSlider.gameObject.SetActive(true);
            _fillImage.enabled = true;
            buttonPressTime += Time.deltaTime;
            progressSlider.value = buttonPressTime / REQUIRED_PRESS_TIME;
            

            if (buttonPressTime >= REQUIRED_PRESS_TIME)
            {
                SetOrderReady();
                isButtonPressed = false;
                buttonPressTime = 0f;
                progressSlider.value = 0f;
                progressSlider.gameObject.SetActive(false);
            }
            else if(PlayerPrefs.GetInt("shopItem-3") == 1)
            {
                SetOrderReady();
                progressSlider.gameObject.SetActive(false);
            }
        }
    }
    public void ButtonPressed()
    {
        isButtonPressed = true;
        buttonPressTime = 0f;
    }

    public void ButtonReleased()
    {
        isButtonPressed = false;
        _fillImage.enabled = false;
    }

    public void SetOrderReady()
    {
        ingredients = GameObject.FindGameObjectWithTag("ingredient");

        if (customers != null && customers.GetComponent<CustomerController>() != null && customers.GetComponent<CustomerController>().isOnSeat)
        {
            isOrderReady = true;

            //check if order is finished and completed
            if (MainGameController.deliveryQueueItems == CustomerController.orderIngredientsIDs.Length)
            {
                //order is complete!
                print("Order is done!");
                ingredients.GetComponent<IngredientsController>().playSfx(ingredients.GetComponent<IngredientsController>().successfulDelivery);
                customers.GetComponent<CustomerController>().settle();
            }
        }
    }
}
