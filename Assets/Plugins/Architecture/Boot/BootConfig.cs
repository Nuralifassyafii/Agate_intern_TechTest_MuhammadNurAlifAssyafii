using UnityEngine;

[CreateAssetMenu(fileName = "New Boot Config", menuName = "Config/Boot Config")]
public class BootConfig : ScriptableObject
{
    public PersistentInstallerBase PersistentInstaller;
}
