using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class TeleportManager : MonoBehaviour
{
    public static TeleportManager Instance { get; private set; }

    [Tooltip("Durasi sebelum player boleh di-teleport lagi setelah teleportasi selesai.")]
    public float globalCooldown = 0.5f;

    private bool isTeleporting = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    public void Teleport(
        GameObject player,
        Transform destination,
        string targetZoneName,
        Action onComplete = null)
    {
        if (isTeleporting) return;
        StartCoroutine(TeleportRoutine(player, destination, targetZoneName, onComplete));
    }

    private IEnumerator TeleportRoutine(
        GameObject player,
        Transform destination,
        string targetZoneName,
        Action onComplete)
    {
        isTeleporting = true;

        var agent = player.GetComponent<NavMeshAgent>();

        // --- Step 1: Hentikan pergerakan NavMesh ---
        if (agent != null)
        {
            agent.ResetPath();       // Batalkan path yang sedang berjalan
            agent.velocity = Vector3.zero;
            agent.enabled = false;   // Disable dulu agar bisa pindah posisi
        }

        // --- Step 2: Pindahkan posisi player ---
        player.transform.position = destination.position;
        player.transform.rotation = destination.rotation;

        // --- Step 3: Tunggu 1 frame agar physics & NavMesh update ---
        yield return null;

        // --- Step 4: Re-enable NavMeshAgent ---
        if (agent != null)
            agent.enabled = true;

        // --- Step 5: Switch kamera ---
        // CameraManager sudah ada di project kamu, kita pakai langsung
        CameraManager.Instance?.SwitchCamera(targetZoneName);

        // --- Step 6: Cooldown global ---
        yield return new WaitForSeconds(globalCooldown);

        isTeleporting = false;

        // Beritahu TeleportTrigger bahwa proses selesai
        onComplete?.Invoke();

        Debug.Log($"[TeleportManager] Teleport selesai → zona: '{targetZoneName}'");
    }

    // Properti untuk cek status dari luar jika dibutuhkan
    public bool IsTeleporting => isTeleporting;
}