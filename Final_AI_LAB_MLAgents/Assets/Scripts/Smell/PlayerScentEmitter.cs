using UnityEngine;

public class PlayerScentEmitter : MonoBehaviour
{
    //variables
    public float dropInterval = 0.5f;
    public float scentIntensity = 1;
    public float scentDecayRate = 0.2f;
    public float scentPradius = 1;
    public float scentLifetime = 5;
    public LayerMask scentLayer;

    //extra 
    private float dropPointTimer;

    private void Update()
    {
        //timer to drop points periodicaly 
        dropPointTimer += Time.deltaTime;

        if (dropPointTimer >= dropInterval)
        {
            DropScentPoint();//point creation
            dropPointTimer = 0f;
            Debug.Log("Dropping scent point");
        }
    }

    void DropScentPoint()
    {
        //create object on player pos
        GameObject scentPointOBJ = new GameObject("ScentPoint");
        scentPointOBJ.transform.position = transform.position;

        //set scent colider
        SphereCollider SPcolider = scentPointOBJ.AddComponent<SphereCollider>();
        SPcolider.isTrigger = true;
        SPcolider.radius = scentPradius;

        //set layer
        scentPointOBJ.layer = LayerMask.NameToLayer(LayerMaskLayerName());
       
       //scent point create and asign atributes
       ScentPoint scentPoint = scentPointOBJ.AddComponent<ScentPoint>();
        scentPoint.intensity = scentIntensity;
        scentPoint.decayRate = scentDecayRate;

        //delete obj when lifetime is over
        Destroy(scentPointOBJ, scentLifetime);
    }

    //pass from layer mask(number) to layer name
    string LayerMaskLayerName()
    {
        int mask = scentLayer.value;
        for (int i = 0; i < 32; i++)
        { 
            if ((mask & (1 << i)) != 0)
            {
                return LayerMask.LayerToName(i);
            }
        }

        return "Default";
    }
}

