using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RollableDice : MonoBehaviour
{
    // [SerializeField] public DiceScriptableObject diceData;
    [SerializeField] protected List<TextMeshPro> _faceTexts;
    [SerializeField] protected List<SpriteRenderer> _faceSprites;
    [SerializeField] protected Renderer _diceRenderer;
    [SerializeField] public Rigidbody _diceRigidbody;
    [SerializeField] protected GameObject faceTextsParent;
    [SerializeField] protected GameObject faceSpritesParent;
    [SerializeField] protected List<Sprite> availableFaceSprites;
    [SerializeField] protected SkillCheckManager _skillCheckManager;

    public bool isLocked;
    public bool isChosen;

    protected readonly Vector3[] faceNormals = new Vector3[]
    {
        Vector3.up,
        Vector3.right,
        Vector3.back,
        Vector3.forward,
        Vector3.left,
        Vector3.down
    };

    void Awake()
    {
        faceTextsParent.SetActive(false);
        faceSpritesParent.SetActive(false);
        isLocked = true;
        isChosen = true;
        if(!_diceRenderer) _diceRenderer = GetComponent<MeshRenderer>();
        if(!_diceRigidbody) _diceRigidbody = GetComponent<Rigidbody>();
    }

    void Start(){
        if (!GameContext.SceneServices.TryGet(out _skillCheckManager))
        {
            Debug.LogError("SkillCheckManager Not Found!");
        }
    }

    public virtual void SetData(DiceScriptableObject data)
    {
    }

    public void SetFaces(List<int> facesString)
    {
        for(int i = 0; i < 6; i++)
        {
            if(facesString[i] != 0){
                _faceTexts[i].text = facesString[i].ToString();
            } else
            {
                _faceTexts[i].text = "";
            }
        }
    }

    public void SetSprites(List<int> facesSprite)
    {
        // Not yet implemented
    }

    public void ToggleLock()
    {
        isLocked = !isLocked;
    }

    public void ToggleChoose()
    {
        isChosen = !isChosen;
    }

    protected int GetTopFaceIndex()
    {
        int topFaceIndex = 0;
        float highestDot = float.MinValue;
        for (int i = 0; i < faceNormals.Length; i++)
        {
            // Convert the local face normal to world space
            Vector3 worldNormal = transform.TransformDirection(faceNormals[i]);

            // Dot product: 1.0 = perfectly aligned with up, -1.0 = pointing down
            float dot = Vector3.Dot(worldNormal, Vector3.up);

            if (dot > highestDot)
            {
                highestDot = dot;
                topFaceIndex = i;
            }
        }

        return topFaceIndex;
    }

    public virtual DiceResult CalculateTopFace(){return new DiceResult{};}
    
    public IEnumerator RollDice(float upForce, float rotationForce)
    {
        _diceRigidbody.AddForce(transform.up * upForce);
        // _diceRigidbody.AddForce(transform.forward * UnityEngine.Random.Range(forwardForce/1.5f, forwardForce));

        yield return new WaitForSeconds(0.01f);

        _diceRigidbody.AddTorque(Vector3.up * UnityEngine.Random.Range(-rotationForce, rotationForce));
        _diceRigidbody.AddTorque(Vector3.left * UnityEngine.Random.Range(-rotationForce, rotationForce));
        _diceRigidbody.AddTorque(Vector3.forward * UnityEngine.Random.Range(-rotationForce, rotationForce));

        yield return new WaitForSeconds(0.01f);
        _skillCheckManager.isRolling = true;
    }
}
