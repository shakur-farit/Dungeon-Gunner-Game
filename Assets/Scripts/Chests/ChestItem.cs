using System.Collections;
using UnityEngine;
using TMPro;
using System;

[RequireComponent(typeof(MaterializeEffect))]
public class ChestItem : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private TextMeshPro _textTMP;
    private MaterializeEffect _materializeEffect;

    [HideInInspector] public bool IsItemmaterialized = false;

    private void Awake()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _textTMP = GetComponentInChildren<TextMeshPro>();
        _materializeEffect = GetComponent<MaterializeEffect>();
    }

    public void Initialize(Sprite sprite, string text, Vector3 spawnPostion, Color materializeColor)
    {
        _spriteRenderer.sprite = sprite;
        transform.position = spawnPostion;

        StartCoroutine(MaterializeItem(materializeColor, text));
    }

    private IEnumerator MaterializeItem(Color materializeColor, string text)
    {
        SpriteRenderer[] spriteRenderersArray = new SpriteRenderer[]
        {
            _spriteRenderer
        };

        yield return StartCoroutine(_materializeEffect
            .MaterializeRoutine(GameResources.Instance.materializeShader,
            materializeColor, 1f, spriteRenderersArray, GameResources.Instance.litMaterial));

        IsItemmaterialized = true;

        _textTMP.text = text;
    }
}
