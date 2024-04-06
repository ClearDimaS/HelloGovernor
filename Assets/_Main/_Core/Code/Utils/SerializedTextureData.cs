using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializedTextureData
{
    [HideInInspector] public FilterMode filterMode;
    [HideInInspector] public TextureFormat format;
    [HideInInspector] public Vector2Int size;
    // Start is called before the first frame update
    public Texture2D texture
    {
        get
        {
            if (_texture == null)
            {
                _texture = new Texture2D(size.x, size.y, format, false);
                _texture.filterMode = filterMode;
                _texture.LoadImage(textureBytes);
            }

            return _texture;
        }
        set
        {
            _texture = value;
            _texture.filterMode = filterMode;
            textureBytes = _texture.EncodeToPNG();
        }
    }

    [SerializeField, HideInInspector] private byte[] textureBytes;
    [SerializeField] private Texture2D _texture;
}
