using CityJSON;
using SharpGLTF.Schema2;
using SharpGLTF.Schema2.Tiles3D;

namespace cj2glb;
public static class AttributesHandler
{
    public static void AddAttributes(Dictionary<string, CityObject> cityObjects, ModelRoot model)
    {
        Tiles3DExtensions.RegisterExtensions();

        var hasAttributes = cityObjects.Any(x => x.Value.Attributes != null && x.Value.Attributes.Count > 0);

        if (hasAttributes)
        {
            var rootMetadata = model.UseStructuralMetadata();
            var schema = rootMetadata.UseEmbeddedSchema("schema_001");

            var schemaClass = schema.UseClassMetadata("triangles");

            var properties = GetProperties(cityObjects.Values.ToList());

            foreach (var property in properties)
            {
                schemaClass.UseProperty(property).WithStringType();
            }

            var propertyTable = schemaClass
                .AddPropertyTable(cityObjects.Count);

            var dict = new Dictionary<string, List<string>>();
            foreach (var property in properties)
            {
                dict.Add(property, new List<string>());
            }

            foreach (var cityObject in cityObjects)
            {
                foreach (var property in properties)
                {
                    if (cityObject.Value.Attributes != null && cityObject.Value.Attributes.ContainsKey(property))
                    {
                        var attribute = Convert.ToString(cityObject.Value.Attributes[property]);
                        dict[property].Add(attribute);
                    }
                    else
                    {
                        dict[property].Add("");
                    }
                }
            }

            foreach (var property in properties)
            {
                var nameProperty = schemaClass.UseProperty(property).WithStringType();
                var values = dict[property].ToArray();
                propertyTable.UseProperty(nameProperty).SetValues(values);
            }

            foreach (var primitive in model.LogicalMeshes[0].Primitives)
            {
                var featureIdAttribute = new FeatureIDBuilder(cityObjects.Count, 0, propertyTable);
                primitive.AddMeshFeatureIds(featureIdAttribute);
            }
        }
    }
    private static List<string> GetProperties(List<CityObject> cityObjects)
    {
        var properties = new List<string>();
        foreach (var cityObject in cityObjects)
        {
            if (cityObject.Attributes != null)
            {
                properties.AddRange(cityObject.Attributes.Select(x => x.Key).ToList());
            }
        }
        return properties.Distinct().ToList();
    }


}
