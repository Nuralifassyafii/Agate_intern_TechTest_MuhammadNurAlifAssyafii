using UnityEngine;
using Unity.Cinemachine;
public class CamSwitch : MonoBehaviour
{
//    // -----------------------------------------------------------------
//    // Inspector Variables
//    // -----------------------------------------------------------------

//    [Header("Pengaturan Zona")]
//    [Tooltip("Nama zona tujuan. Harus sama persis dengan zoneName di CameraManager.")]
//    public string targetZoneName;

//    [Header("Cooldown")]
//    [Tooltip("Jeda waktu (detik) sebelum OnTriggerExit diproses. " +
//             "Naikkan nilai ini jika kamera masih bolak-balik. Default: 0.3")]
//    public float exitCooldown = 0.3f;

//    [Header("Debug")]
//    public bool showDebugLog = true;

//    // -----------------------------------------------------------------
//    // Private Variables
//    // -----------------------------------------------------------------

//    private BoxCollider zoneCollider;
//    private bool isPlayerInside = false;

//    // Timer yang mencatat kapan terakhir kali player MASUK zona ini.
//    // OnTriggerExit hanya akan diproses jika waktu sejak masuk
//    // sudah melebihi exitCooldown.
//    private float enterTime = -999f;


//    // -----------------------------------------------------------------
//    // Unity Lifecycle
//    // -----------------------------------------------------------------

//    private void Awake()
//    {
//        zoneCollider = GetComponent<BoxCollider>();
//        zoneCollider.isTrigger = true;
//    }

//    private void Start()
//    {
//        if (string.IsNullOrEmpty(targetZoneName))
//            Debug.LogError($"[CamSwitch: {gameObject.name}] 'Target Zone Name' masih kosong!");

//        if (CameraManager.Instance == null)
//            Debug.LogError($"[CamSwitch: {gameObject.name}] CameraManager.Instance tidak ditemukan!");
//    }


//    // -----------------------------------------------------------------
//    // Trigger Events
//    // -----------------------------------------------------------------

//    private void OnTriggerEnter(Collider other)
//    {
//        if (!other.CompareTag("Player")) return;
//        if (isPlayerInside) return;

//        isPlayerInside = true;

//        // Catat waktu saat player masuk.
//        // Time.time = waktu total sejak game dimulai (dalam detik).
//        enterTime = Time.time;

//        if (showDebugLog)
//            Debug.Log($"[CamSwitch] Player MASUK zona: '{targetZoneName}' pada t={enterTime:F2}s");

//        CameraManager.Instance?.SwitchCamera(targetZoneName);
//    }

//    private void OnTriggerExit(Collider other)
//    {
//        if (!other.CompareTag("Player")) return;
//        if (!isPlayerInside) return;

//        // KUNCI SOLUSI:
//        // Hitung selisih waktu antara saat masuk dan saat keluar.
//        // Jika player keluar terlalu cepat (kurang dari exitCooldown),
//        // kemungkinan besar ini adalah trigger dari zona yang overlap,
//        // bukan player yang benar-benar ingin keluar.
//        // Kita ABAIKAN exit ini.
//        float timeInsideZone = Time.time - enterTime;

//        if (timeInsideZone < exitCooldown)
//        {
//            if (showDebugLog)
//                Debug.Log($"[CamSwitch] Exit DIABAIKAN di zona '{targetZoneName}'. " +
//                          $"Waktu di dalam: {timeInsideZone:F2}s (kurang dari cooldown {exitCooldown}s)");
//            return;
//        }

//        // Jika waktu di dalam zona sudah cukup, proses exit secara normal.
//        isPlayerInside = false;

//        if (showDebugLog)
//            Debug.Log($"[CamSwitch] Player KELUAR zona: '{targetZoneName}'. " +
//                      $"Waktu di dalam: {timeInsideZone:F2}s");

//        CameraManager.Instance?.
//    }

//    private void OnTriggerStay(Collider other)
//    {
//        if (!other.CompareTag("Player")) return;

//        if (!isPlayerInside)
//        {
//            isPlayerInside = true;
//            enterTime = Time.time;
//            CameraManager.Instance?.SwitchCamera(targetZoneName);
//        }
//    }


//    // -----------------------------------------------------------------
//    // Gizmos
//    // -----------------------------------------------------------------

//    private void OnDrawGizmos()
//    {
//        if (zoneCollider == null)
//            zoneCollider = GetComponent<BoxCollider>();
//        if (zoneCollider == null) return;

//        Gizmos.color = isPlayerInside
//            ? new Color(1f, 1f, 0f, 0.25f)
//            : new Color(0f, 1f, 0f, 0.1f);

//        Gizmos.matrix = transform.localToWorldMatrix;
//        Gizmos.DrawCube(zoneCollider.center, zoneCollider.size);

//        Gizmos.color = isPlayerInside
//            ? new Color(1f, 1f, 0f, 0.9f)
//            : new Color(0f, 1f, 0f, 0.5f);
//        Gizmos.DrawWireCube(zoneCollider.center, zoneCollider.size);

//#if UNITY_EDITOR
//        UnityEditor.Handles.Label(
//            transform.TransformPoint(
//                zoneCollider.center + Vector3.up * (zoneCollider.size.y / 2f + 0.2f)
//            ),
//            $"Zone: {targetZoneName}"
//        );
//#endif
//    }
}
