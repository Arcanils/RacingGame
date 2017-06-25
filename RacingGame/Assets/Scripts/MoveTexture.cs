using UnityEngine;
using System.Collections;

public class MoveTexture : MonoBehaviour {

    public Vector2 VecSpeed;
    

    public void Start()
    {
        StartCoroutine(Move());
    }

    public IEnumerator Move()
    {
        var mesh = GetComponent<MeshRenderer>();
        if (mesh != null)
        {
            var _mat = mesh.material;

            if (_mat != null)
            {
                Vector2 currentOffset = Vector2.zero;
                Vector2 subValue = new Vector2(0.0f, 1.0f);
                while (true)
                {
                    currentOffset = new Vector2(0f, Mathf.Repeat(Time.deltaTime * VecSpeed.y + currentOffset.y, 1));

                    _mat.SetTextureOffset("_MainTex", currentOffset);
                    yield return null;
                }
            }
        }
    }
}
