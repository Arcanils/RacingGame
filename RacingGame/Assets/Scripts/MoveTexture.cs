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
                    currentOffset += Time.deltaTime * VecSpeed;
                    if (currentOffset.y > subValue.y)
                        currentOffset -= subValue;

                    _mat.SetTextureOffset("_MainTex", currentOffset);
                    yield return null;
                }
            }
        }
    }
}
