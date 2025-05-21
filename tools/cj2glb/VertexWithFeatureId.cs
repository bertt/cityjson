using SharpGLTF.Geometry.VertexTypes;
using SharpGLTF.Memory;
using SharpGLTF.Schema2;
using System.Numerics;

namespace cj2glb;

[System.Diagnostics.DebuggerDisplay("𝐔𝐕:{TexCoord} 𝐁𝐈:{BatchId}")]
public struct VertexWithFeatureId1 : IVertexCustom
{
    public const string CUSTOMATTRIBUTENAME = "_FEATURE_ID_0";

    public float BatchId;
    public Vector2 TexCoord;

    public VertexWithFeatureId1(float batchId, Vector2 texCoord)
    {
        BatchId = batchId;
        TexCoord = texCoord;
    }

    public static implicit operator VertexWithFeatureId1(float batchId)
    {
        return new VertexWithFeatureId1(batchId, Vector2.Zero);
    }

    public IEnumerable<KeyValuePair<string, AttributeFormat>> GetEncodingAttributes()
    {
        yield return new KeyValuePair<string, AttributeFormat>(CUSTOMATTRIBUTENAME, new AttributeFormat(DimensionType.SCALAR));
        yield return new KeyValuePair<string, AttributeFormat>("TEXCOORD_0", new AttributeFormat(DimensionType.VEC2));
    }

    public int MaxColors => 0;
    public int MaxTextCoords => 1;

    public IEnumerable<string> CustomAttributes
    {
        get
        {
            yield return CUSTOMATTRIBUTENAME;
        }
    }

    public Vector2 GetTexCoord(int index)
    {
        if (index == 0) return TexCoord;
        throw new ArgumentOutOfRangeException(nameof(index));
    }

    public void SetTexCoord(int index, Vector2 coord)
    {
        if (index == 0) TexCoord = coord;
        else throw new ArgumentOutOfRangeException(nameof(index));
    }

    public Vector4 GetColor(int index) => throw new ArgumentOutOfRangeException(nameof(index));
    public void SetColor(int setIndex, Vector4 color) { }

    public void Validate() { }

    public object GetCustomAttribute(string attributeName)
    {
        if (attributeName == CUSTOMATTRIBUTENAME) return BatchId;
        return null;
    }

    public bool TryGetCustomAttribute(string attributeName, out object value)
    {
        if (attributeName == CUSTOMATTRIBUTENAME)
        {
            value = BatchId;
            return true;
        }

        value = null;
        return false;
    }

    public void SetCustomAttribute(string attributeName, object value)
    {
        if (attributeName == CUSTOMATTRIBUTENAME)
        {
            BatchId = Convert.ToSingle(value);
        }
        else
        {
            throw new ArgumentException($"Unknown attribute: {attributeName}");
        }
    }

    public VertexMaterialDelta Subtract(IVertexMaterial baseValue) => default;
    public void Add(in VertexMaterialDelta delta) { }
}