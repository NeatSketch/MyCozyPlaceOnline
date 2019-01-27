using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShotter : MonoBehaviour
{
    public List<Transform> transforms = new List<Transform>();
    public float camDist = 2f;

    IEnumerator Start()
    {
        for (int i = 0; i < transforms.Count; i++)
        {
            Transform t = (Transform)transforms[i];
            var pos = t.position;
            pos.z += camDist;
            transform.position = pos;

            yield return new WaitForEndOfFrame();

            ScreenCapture.CaptureScreenshot(Application.dataPath + "/Resources/Previews/" + t.name + ".jpg");
            
        }
    }

    void Update()
    {
        
        
    }
}
