using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Bagsik/Character Data")]
public class CharacterData : ScriptableObject
{
    [Header("Character Info")]
    public string characterName;
    public Sprite characterIcon;
    public GameObject characterPrefab;

    [Header("Animation")]
    public RuntimeAnimatorController animatorController;
    public Avatar avatar;

    [Header("Character Stats")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float jumpHeight = 2f;
}