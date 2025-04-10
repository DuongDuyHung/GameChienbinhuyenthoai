using Cinemachine;
using Fusion;
using UnityEngine;

public class AutoAssignCamera : MonoBehaviour
{
    private void Start()
    {
        // Tìm NetworkObject cha
        NetworkObject parentNetworkObject = GetComponentInParent<NetworkObject>();
        if (parentNetworkObject != null && parentNetworkObject.HasInputAuthority)
        {
            CinemachineVirtualCamera virtualCamera = GetComponent<CinemachineVirtualCamera>();
            if (virtualCamera != null)
            {
                // Gán Follow và LookAt cho đối tượng cha
                virtualCamera.Follow = parentNetworkObject.transform;
                virtualCamera.LookAt = parentNetworkObject.transform;
                virtualCamera.Priority = 10; // Kích hoạt camera này
            }
        }
        else
        {
            // Hạ Priority nếu không phải LocalPlayer
            CinemachineVirtualCamera virtualCamera = GetComponent<CinemachineVirtualCamera>();
            if (virtualCamera != null)
            {
                virtualCamera.Priority = 0; // Vô hiệu hóa camera
            }
        }
    }
}
