using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrSetMaterial : MonoBehaviour
{
    public Material[] materials;
    public GameObject[] meshes;
    public int materialIndex;

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            if ((meshes != null || meshes.Length > 0) && (materials != null || materials.Length > 0))
            {
                for (int i = 0; i < meshes.Length; i++)
                {
                    meshes[i].GetComponent<MeshRenderer>().material = materials[materialIndex];
                }
            }
            else
            {
                if (meshes == null)
                {
                    Debug.LogWarning(this.name + " - " + gameObject.name + " Missing mesh list");
                }
                if (materials == null)
                {
                    Debug.LogWarning(this.name + " - " + gameObject.name + " Missing materials list");
                }
            }
        }
    }
}
