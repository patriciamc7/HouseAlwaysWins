using UnityEngine;

public class TestIris : MonoBehaviour
{
    [SerializeField] private IrisTransition iris;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            iris.CloseIris();

        if (Input.GetKeyDown(KeyCode.Return))
            iris.OpenIris();
    }
}