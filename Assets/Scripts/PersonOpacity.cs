using UnityEngine;
using UnityEngine.UIElements;

public class PersonOpacity : MonoBehaviour
{
    public Transform playerTF;
    public Transform parentTF;
    void Update()
    {
        Vector3 lookAt = new Vector3(playerTF.position.x, this.gameObject.transform.position.y, playerTF.position.z);
        parentTF.LookAt(lookAt);
    }
}
