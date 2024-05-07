using UnityEngine;
using System.Collections;

public class DrinkController : MonoBehaviour {

	/// <summary>
	/// Drinks are special ingredients that have their own range of ID (Starting from 101 ...)
	/// and they are also not a part of bigger order. A customer who asks for a drink, just looks for the same 
	/// drink ID and doesn't accpet anything else. A customer can order only one drink at a time.
	/// </summary>

	public int drinkID;						//Drink ID
	private float delayTime;				//after this delay, we let player to be able to choose another drink again
	private bool canTap;                    //cutome flag to prevent double picking
    public bool isLocked;                  //check if this ingrediennt is locked or available

    public GameObject lockGo;				//lock prefab

    //Reference to game objects
    private GameObject deliveryPlate;
	private GameObject currentCustomer;
	private GameObject buttonCheck;

	//AudioClips
	public AudioClip wrongPick;
	public AudioClip correctPick;
	public AudioClip successfulDelivery;


	/// <summary>
	/// Init
	/// </summary>
	void Awake (){
		delayTime = 0.15f;
		canTap = true;
		isLocked = true;
		deliveryPlate = GameObject.FindGameObjectWithTag ("serverPlate");
		buttonCheck = GameObject.FindGameObjectWithTag("buttonHold");
	}

    private void Start()
    {
        //check if this ingredient is available for this level
        int ai = PlayerPrefs.GetInt("availableDrinks");
        for (int i = 0; i < ai; i++)
        {
            if (drinkID == PlayerPrefs.GetInt("careerDrink_" + i))
            {
                isLocked = false;
                break;
            }
        }

        //check if this is locked on open
        if (isLocked)
        {
            GetComponent<BoxCollider>().enabled = false;
            GetComponent<Renderer>().material.color = new Color(1, 1, 1, 0.5f);
            GameObject lck = Instantiate(lockGo, transform.position + new Vector3(0, 0, -0.1f), Quaternion.Euler(0, 180, 0)) as GameObject;
            lck.name = "Lock";
        }
        else
        {
            GetComponent<BoxCollider>().enabled = true;
            GetComponent<Renderer>().material.color = new Color(1, 1, 1, 1);
        }
    }


    /// <summary>
    /// FSM
    /// </summary>
    void Update (){

		if(canTap && !MainGameController.deliveryQueueIsFull && customerIsAvailable())
			monitorTap();

	}


	private RaycastHit hitInfo;
	private Ray ray;
	void monitorTap (){
		
		//Mouse or touch?
		if(	Input.touches.Length > 0 && Input.touches[0].phase == TouchPhase.Moved)  
			ray = Camera.main.ScreenPointToRay(Input.touches[0].position);
		else if(Input.GetMouseButtonDown(0))
			ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		else
			return;

		if(Physics.Raycast(ray, out hitInfo)) {
			GameObject objectHit = hitInfo.transform.gameObject;
			if(objectHit.tag == "drink" && objectHit.name == gameObject.name) {
				StartCoroutine(updateOrderQuoue(gameObject));
			}
		}
	}


	/// <summary>
	/// when we click on an ingredient, it should match the exact id of the product the current customer wants.
	/// So we first need to check if the player is tapping on the right ingredient/drink.
	/// If tap is correct, we will add the ingredient to the delivery quoue
	/// if not, we delete the current delivery, and player needs to start from scratch
	/// </summary>
	IEnumerator updateOrderQuoue (GameObject selectedIngredient){
		if(!MainGameController.gameIsFinished && !MainGameController.deliveryQueueIsFull) {

			//prevent double tap
			canTap = false;
			StartCoroutine(reactivate());

			//check tapped ingredient with customer's order
			int dqi = MainGameController.deliveryQueueItems;

			//prevent array size error
			if (dqi >= CustomerController.orderIngredientsIDs.Length) {
				MainGameController.deliveryQueueIsFull = true;
				print ("deliveryQueueIsFull");
				yield break;
			}

			if (drinkID != CustomerController.orderIngredientsIDs [dqi]) {

				//player tapped on a wrong drink.

				//play wrong pick sfx
				playSfx(wrongPick);

				//delete delivery items on the plater
				deliveryPlate.GetComponent<PlateController> ().deleteDelivery ();

				//clear main delivery arrays
				MainGameController.deliveryQueueItems = 0;
				MainGameController.deliveryQueueIsFull = false;
				MainGameController.deliveryQueueItemsContent.Clear();

				yield break;
			}

			//add this drink to delivery quoue
			MainGameController.deliveryQueueItems++;
			MainGameController.deliveryQueueItemsContent.Add(drinkID);

			buttonCheck.GetComponent<ButtonCompl>().isOrderReady = true;

            //play ingredient pick sound
            playSfx (correctPick);

			//check if order is finished and completed
			if (MainGameController.deliveryQueueItems == CustomerController.orderIngredientsIDs.Length) {
				//order is complete!
				print ("Order is done!");
				//wait
				yield return new WaitForSeconds (0.2f);
				playSfx (successfulDelivery);
				//tell customer to settle and leave
				GameObject c = GameObject.FindGameObjectWithTag ("customer");
				c.GetComponent<CustomerController> ().settle ();
			}

            buttonCheck.GetComponent<ButtonCompl>().isOrderReady = false;

            StartCoroutine(reactivate());

			//debug
			/*
			print ("Delivery size: " + MainGameController.deliveryQueueItems);
			for (int i = 0; i < MainGameController.deliveryQueueItemsContent.Count; i++) {
				print ("Ing[" + i + "]: " + MainGameController.deliveryQueueItemsContent [i]);
			}
			*/

		}
	}


	/// <summary>
	/// Make this ingredient draggable again
	/// </summary>
	IEnumerator reactivate (){
		yield return new WaitForSeconds(delayTime);
		canTap = true;
	}


	/// <summary>
	/// Check if there is any customer inside the shop (which is ready to order)
	/// </summary>
	/// <returns><c>true</c>, if there is a customer in shop<c>false</c> otherwise.</returns>
	bool customerIsAvailable() {
		GameObject c = GameObject.FindGameObjectWithTag ("customer");
		if (c != null) {
			if(CustomerController.isCustomerReady)
				return true;
			else
				return false;
		} else
			return false;
	}


	/// <summary>
	/// Play AudioClips
	/// </summary>
	/// <param name="_sfx">Sfx.</param>
	void playSfx ( AudioClip _sfx  ){
		GetComponent<AudioSource>().clip = _sfx;
		if(!GetComponent<AudioSource>().isPlaying)
			GetComponent<AudioSource>().Play();
	}

}