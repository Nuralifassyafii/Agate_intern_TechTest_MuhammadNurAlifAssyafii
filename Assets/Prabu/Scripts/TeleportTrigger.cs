using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Pasang script ini pada setiap BoxCollider trigger di pintu/portal antar zona.
/// Satu trigger = satu arah perpindahan.
/// </summary>
public class TeleportTrigger : MonoBehaviour
{
    [Header("Zona Tujuan")]
    [Tooltip("Nama zona kamera tujuan. Harus sama dengan zoneName di CameraManager.")]
    public string targetZoneName;

    [Header("Posisi Spawn Player Setelah Teleport")]
    [Tooltip("Transform tempat player akan di-spawn setelah teleport. " +
             "Buat GameObject kosong di dalam area tujuan sebagai penanda posisi.")]
    public Transform spawnPoint;

    [Header("Debug")]
    public bool showDebugLog = true;

    // Guard flag: mencegah trigger terpicu lebih dari sekali
    // selama proses teleportasi berlangsung
    private bool isProcessing = false;

    private void Start()
    {
        // Validasi setup di Inspector
        if (string.IsNullOrEmpty(targetZoneName))
            Debug.LogError($"[TeleportTrigger: {gameObject.name}] targetZoneName kosong!");

        if (spawnPoint == null)
            Debug.LogError($"[TeleportTrigger: {gameObject.name}] spawnPoint belum di-assign!");

        var col = GetComponent<BoxCollider>();
        if (col == null || !col.isTrigger)
            Debug.LogError($"[TeleportTrigger: {gameObject.name}] " +
                           "Butuh BoxCollider dengan isTrigger = true!");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (isProcessing) return;
        if (spawnPoint == null) return;

        isProcessing = true;

        if (showDebugLog)
            Debug.Log($"[TeleportTrigger] Player masuk trigger → teleport ke '{targetZoneName}'");

        // Jalankan teleportasi via TeleportManager
        TeleportManager.Instance?.Teleport(
            other.gameObject,
            spawnPoint,
            targetZoneName,
            OnTeleportComplete
        );
    }

    private void OnTeleportComplete()
    {
        // Beri jeda satu frame sebelum trigger bisa digunakan lagi.
        // Ini mencegah trigger zona baru langsung terpicu
        // karena player di-spawn terlalu dekat dengan trigger lain.
        Invoke(nameof(ResetProcessing), 0.5f);
    }

    private void ResetProcessing()
    {
        isProcessing = false;
    }

    // Gizmos: visualisasi di Scene View
    private void OnDrawGizmos()
    {
        var col = GetComponent<BoxCollider>();
        if (col == null) return;

        Gizmos.color = new Color(0f, 0.8f, 1f, 0.15f);
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(col.center, col.size);

        Gizmos.color = new Color(0f, 0.8f, 1f, 0.8f);
        Gizmos.DrawWireCube(col.center, col.size);

        if (spawnPoint != null)
        {
            // Gambar garis dari trigger ke spawn point
            Gizmos.color = Color.yellow;
            Gizmos.matrix = Matrix4x4.identity;
            Gizmos.DrawLine(
                transform.TransformPoint(col.center),
                spawnPoint.position
            );
            Gizmos.DrawSphere(spawnPoint.position, 0.2f);
        }

#if UNITY_EDITOR
        UnityEditor.Handles.Label(
            transform.TransformPoint(col.center + Vector3.up * (col.size.y / 2f + 0.2f)),
            $"→ {targetZoneName}"
        );
#endif
    }
}