using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AccessoryItemBtn : MonoBehaviour
{
    public Text text;
    public GameObject cross;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    const float meshSizeMult = 10f;

    const float ROTATE_PREVIEW_SPEED = 5f;

    public AccessoryItem item;

    public bool Dressed { get => cross.activeSelf; set => cross.SetActive(value); }

    Vector3 startScale;

    public void Init(AccessoryItem _item)
    {
        //text.text = item.name;
        item = _item;

        MeshFilter mf = _item.gameObject.GetComponent<MeshFilter>();
        if (mf)
        {
            meshFilter.sharedMesh = mf.sharedMesh;
            meshFilter.transform.localRotation = Quaternion.Euler(0, -90f, -90f);

        }

        MeshRenderer mr = _item.gameObject.GetComponent<MeshRenderer>();
        if (mr)
        {
            var mat = new Material(mr.sharedMaterial);
            mat.renderQueue = 5000;

            meshRenderer.material = mat;
        }

        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        meshRenderer.receiveShadows = false;
        startScale = meshFilter.transform.localScale;
    }

    private void Update()
    {
        meshFilter.gameObject.transform.Rotate(0, Time.deltaTime * ROTATE_PREVIEW_SPEED, 0, Space.Self);
        var scale = startScale / meshFilter.mesh.bounds.size.x * meshSizeMult;
        if(scale.magnitude > 0)
        meshFilter.transform.localScale = scale;

    }

    public void DressOrUndressItBtn()
    {
        Dressed = CustomizeMenu.DressItem(item);
        cross.SetActive(Dressed);
    }
}
