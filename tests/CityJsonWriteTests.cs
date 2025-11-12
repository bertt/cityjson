using System.IO;
using System.Linq;
using CityJSON.Geometry;
using Newtonsoft.Json;
using NUnit.Framework;

namespace CityJSON.Tests
{
    public class CityJsonWriteTests
    {
        [Test]
        public void WriteMinimalCityJson()
        {
            // Create a minimal CityJSON document
            var document = new CityJsonDocument
            {
                Type = "CityJSON",
                Version = "2.0",
                Transform = new Transform
                {
                    Scale = new double[] { 1.0, 1.0, 1.0 },
                    Translate = new double[] { 0.0, 0.0, 0.0 }
                },
                CityObjects = new System.Collections.Generic.Dictionary<string, CityObject>(),
                Vertices = new System.Collections.Generic.List<Vertex>()
            };

            // Write to JSON
            var json = CityJsonWriter.Write(document);

            // Verify it's valid JSON
            Assert.That(json, Is.Not.Null);
            Assert.That(json, Does.Contain("\"type\": \"CityJSON\""));
            Assert.That(json, Does.Contain("\"version\": \"2.0\""));
        }

        [Test]
        public void RoundTripMinimalCityJson()
        {
            // Read the minimal fixture
            var originalJson = File.ReadAllText("fixtures/minimal.city.json");
            var document = JsonConvert.DeserializeObject<CityJsonDocument>(originalJson);

            // Write it back
            var writtenJson = CityJsonWriter.Write(document);

            // Read it again
            var roundTrippedDocument = JsonConvert.DeserializeObject<CityJsonDocument>(writtenJson);

            // Verify key properties
            Assert.That(roundTrippedDocument.Type, Is.EqualTo(document.Type));
            Assert.That(roundTrippedDocument.Version, Is.EqualTo(document.Version));
            Assert.That(roundTrippedDocument.Transform.Scale, Is.EqualTo(document.Transform.Scale));
            Assert.That(roundTrippedDocument.Transform.Translate, Is.EqualTo(document.Transform.Translate));
            Assert.That(roundTrippedDocument.CityObjects.Count, Is.EqualTo(document.CityObjects.Count));
            Assert.That(roundTrippedDocument.Vertices.Count, Is.EqualTo(document.Vertices.Count));
        }

        [Test]
        public void RoundTripCubeGeometry()
        {
            // Read the cube fixture with geometry
            var originalJson = File.ReadAllText("./fixtures/simplegeom/v2.0/cube.city.json");
            var document = JsonConvert.DeserializeObject<CityJsonDocument>(originalJson);

            // Write it back
            var writtenJson = CityJsonWriter.Write(document);

            // Read it again
            var roundTrippedDocument = JsonConvert.DeserializeObject<CityJsonDocument>(writtenJson);

            // Verify document properties
            Assert.That(roundTrippedDocument.Type, Is.EqualTo(document.Type));
            Assert.That(roundTrippedDocument.Version, Is.EqualTo(document.Version));
            Assert.That(roundTrippedDocument.CityObjects.Count, Is.EqualTo(document.CityObjects.Count));
            Assert.That(roundTrippedDocument.Vertices.Count, Is.EqualTo(document.Vertices.Count));

            // Verify geometry
            var originalCityObject = document.CityObjects.First().Value;
            var roundTrippedCityObject = roundTrippedDocument.CityObjects.First().Value;
            
            Assert.That(roundTrippedCityObject.Type, Is.EqualTo(originalCityObject.Type));
            Assert.That(roundTrippedCityObject.Geometry.Count, Is.EqualTo(originalCityObject.Geometry.Count));
            
            var originalGeometry = originalCityObject.Geometry.First();
            var roundTrippedGeometry = roundTrippedCityObject.Geometry.First();
            
            Assert.That(roundTrippedGeometry.Type, Is.EqualTo(originalGeometry.Type));
            Assert.That(roundTrippedGeometry.Lod, Is.EqualTo(originalGeometry.Lod));
            
            // Verify Solid geometry boundaries
            var originalSolid = (SolidGeometry)originalGeometry;
            var roundTrippedSolid = (SolidGeometry)roundTrippedGeometry;
            
            Assert.That(roundTrippedSolid.Boundaries.Length, Is.EqualTo(originalSolid.Boundaries.Length));
        }

        [Test]
        public void RoundTripMultiSurfaceGeometry()
        {
            // Read a file with MultiSurface geometry
            var originalJson = File.ReadAllText("./fixtures/building_with_innerring.city.json");
            var document = JsonConvert.DeserializeObject<CityJsonDocument>(originalJson);

            // Write it back
            var writtenJson = CityJsonWriter.Write(document);

            // Read it again
            var roundTrippedDocument = JsonConvert.DeserializeObject<CityJsonDocument>(writtenJson);

            // Verify document properties
            Assert.That(roundTrippedDocument.Type, Is.EqualTo(document.Type));
            Assert.That(roundTrippedDocument.Version, Is.EqualTo(document.Version));
            Assert.That(roundTrippedDocument.CityObjects.Count, Is.EqualTo(document.CityObjects.Count));
            Assert.That(roundTrippedDocument.Vertices.Count, Is.EqualTo(document.Vertices.Count));

            // Verify geometry type
            var originalCityObject = document.CityObjects.First().Value;
            var roundTrippedCityObject = roundTrippedDocument.CityObjects.First().Value;
            
            Assert.That(roundTrippedCityObject.Geometry.Count, Is.EqualTo(originalCityObject.Geometry.Count));
            
            var originalGeometry = originalCityObject.Geometry.First();
            var roundTrippedGeometry = roundTrippedCityObject.Geometry.First();
            
            Assert.That(roundTrippedGeometry.Type, Is.EqualTo(originalGeometry.Type));
        }

        [Test]
        public void RoundTripWithTextures()
        {
            // Read a file with textures
            var originalJson = File.ReadAllText("./fixtures/minimal_textures.city.json");
            var document = JsonConvert.DeserializeObject<CityJsonDocument>(originalJson);

            // Write it back
            var writtenJson = CityJsonWriter.Write(document);

            // Read it again
            var roundTrippedDocument = JsonConvert.DeserializeObject<CityJsonDocument>(writtenJson);

            // Verify appearance/textures
            Assert.That(roundTrippedDocument.Appearance, Is.Not.Null);
            Assert.That(roundTrippedDocument.Appearance.Textures.Count, Is.EqualTo(document.Appearance.Textures.Count));
            
            var originalTexture = document.Appearance.Textures.First();
            var roundTrippedTexture = roundTrippedDocument.Appearance.Textures.First();
            
            Assert.That(roundTrippedTexture.Image, Is.EqualTo(originalTexture.Image));
            Assert.That(roundTrippedTexture.ImageType, Is.EqualTo(originalTexture.ImageType));
            
            // Verify vertices-texture
            Assert.That(roundTrippedDocument.Appearance.VerticesTexture.Length, 
                Is.EqualTo(document.Appearance.VerticesTexture.Length));
        }

        [Test]
        public void RoundTripDenHaag()
        {
            // Read a larger file
            var originalJson = File.ReadAllText("fixtures/denhaag.json");
            var document = JsonConvert.DeserializeObject<CityJsonDocument>(originalJson);

            // Write it back
            var writtenJson = CityJsonWriter.Write(document);

            // Read it again
            var roundTrippedDocument = JsonConvert.DeserializeObject<CityJsonDocument>(writtenJson);

            // Verify document properties
            Assert.That(roundTrippedDocument.Version, Is.EqualTo(document.Version));
            Assert.That(roundTrippedDocument.CityObjects.Count, Is.EqualTo(document.CityObjects.Count));
            Assert.That(roundTrippedDocument.Vertices.Count, Is.EqualTo(document.Vertices.Count));

            // Verify first city object
            var originalFirstCityObject = document.CityObjects.First().Value;
            var roundTrippedFirstCityObject = roundTrippedDocument.CityObjects.First().Value;
            
            Assert.That(roundTrippedFirstCityObject.Type, Is.EqualTo(originalFirstCityObject.Type));
            Assert.That(roundTrippedFirstCityObject.Attributes.Count, Is.EqualTo(originalFirstCityObject.Attributes.Count));
            Assert.That(roundTrippedFirstCityObject.Geometry.Count, Is.EqualTo(originalFirstCityObject.Geometry.Count));
        }

        [Test]
        public void RoundTripWithAddress()
        {
            // Read a file with address property
            var originalJson = File.ReadAllText("fixtures/euromast.json");
            var document = JsonConvert.DeserializeObject<CityJsonDocument>(originalJson);

            // Write it back
            var writtenJson = CityJsonWriter.Write(document);

            // Read it again
            var roundTrippedDocument = JsonConvert.DeserializeObject<CityJsonDocument>(writtenJson);

            // Verify city objects count
            Assert.That(roundTrippedDocument.CityObjects.Count, Is.EqualTo(document.CityObjects.Count));

            // Verify address property
            var bagObject = roundTrippedDocument.CityObjects["BAG_0599100000661084"];
            Assert.That(bagObject.Address, Is.Not.Null);
            
            string postalCode = bagObject.Address["PostalCode"]?.ToString();
            Assert.That(postalCode, Is.EqualTo("3016GM"));
        }

        [Test]
        public void WriteToFileThenReadBack()
        {
            // Create a test document
            var document = new CityJsonDocument
            {
                Type = "CityJSON",
                Version = "2.0",
                Transform = new Transform
                {
                    Scale = new double[] { 1.0, 1.0, 1.0 },
                    Translate = new double[] { 0.0, 0.0, 0.0 }
                },
                CityObjects = new System.Collections.Generic.Dictionary<string, CityObject>(),
                Vertices = new System.Collections.Generic.List<Vertex>()
            };

            var tempFile = Path.GetTempFileName();
            try
            {
                // Write to file
                CityJsonWriter.WriteToFile(document, tempFile);

                // Verify file exists
                Assert.That(File.Exists(tempFile), Is.True);

                // Read it back
                var readJson = File.ReadAllText(tempFile);
                var readDocument = JsonConvert.DeserializeObject<CityJsonDocument>(readJson);

                // Verify
                Assert.That(readDocument.Type, Is.EqualTo(document.Type));
                Assert.That(readDocument.Version, Is.EqualTo(document.Version));
            }
            finally
            {
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }
            }
        }
    }
}
