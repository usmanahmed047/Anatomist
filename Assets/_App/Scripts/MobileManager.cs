using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class MobileManager : MonoBehaviour
{
    public static MobileManager Instance;
    string savepath;
    void Awake()
    {
        Instance = this;
    }
    public static void ShareBragImage(Texture2D image)
    {
        Debug.Log("ShareBragImage called");
        //string savepath = Application.persistentDataPath + "/" + "screenshare.png";
        //System.IO.File.WriteAllBytes(savepath, image.GetRawTextureData());
        NativeShare.ShareTexture(image, "Anatomist", LocalizationManager.localization.bigDeal);
      // Instance.Invoke("DoShare", 0.1f);

       // UM_ShareUtility.ShareMedia("Brag to friends!", "Yeah, Im kind of a big deal on @AnatomistApp! Try beating me -- www.anatomist.us", image);
    }

    //public void DoShare()
    //{
       
    //   NativeShare.ShareTexture("Yeah, I'm kind of a big deal on @AnatomistApp! Try beating me -- www.anatomist.us", savepath, null, "Brag to friends!", chooser:true);
    //}
	
}
