using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideIfUnlocked : MonoBehaviour
{
	void Update ()
    {
        if (Purchaser.Instance.IsInitialized())
        {
            if (Purchaser.Instance.GetPurchasedState(Purchaser.PRODUCT_FULL_APP))
            {
                gameObject.SetActive(false);
            }
        }
	}
}
