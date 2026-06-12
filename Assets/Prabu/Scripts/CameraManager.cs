using UnityEngine;
using Unity.Cinemachine;
using System.Collections.Generic;

public class CameraManager : MonoBehaviour
{
   public static CameraManager Instance;

    [System.Serializable]
    public class CameraZone
    {
        public string zoneName;
        public CinemachineCamera camera;
    }

    [Header("Daftar Semua Zona Kamera")]
    public List<CameraZone> zones = new List<CameraZone>();

    private CinemachineCamera activeCamera;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        for (int i = 0; i < zones.Count; i++)
        {
            if (zones[i].camera == null) continue;
            zones[i].camera.enabled = (i == 0);
            if (i == 0) activeCamera = zones[i].camera;
        }
    }

    public void SwitchCamera(string zoneName)
    {
        var target = zones.Find(z => z.zoneName == zoneName);

        if (target == null)
        {
            Debug.LogWarning($"[CameraManager] Zona '{zoneName}' tidak ditemukan!");
            return;
        }

        if (target.camera == activeCamera) return;

        ActivateCamera(target.camera);
    }

    private void ActivateCamera(CinemachineCamera newCamera)
    {
        if (newCamera == null) return;

        foreach (var zone in zones)
            if (zone.camera != null) zone.camera.enabled = false;

        activeCamera = newCamera;
        activeCamera.enabled = true;

        Debug.Log($"[CameraManager] Kamera aktif: {activeCamera.name}");
    }

    public CinemachineCamera GetActiveCamera() => activeCamera;
}