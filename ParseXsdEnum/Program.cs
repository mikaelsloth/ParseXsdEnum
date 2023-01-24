// See https://aka.ms/new-console-template for more information
using System.Text.Json;
using System.Xml;
using System.Xml.Schema;

string tempmsg = string.Empty;

XmlSchema? schema = XmlSchema.Read(XmlReader.Create(@"C:\temp\HUF.xsd"),
    (o, e) =>
    {
        tempmsg = "The following messages came from validating against the schema: \r\n";
        switch (e.Severity)
        {
            case XmlSeverityType.Error:
                tempmsg = tempmsg + "\r\n" + "ERROR: " + e.Message;
                break;
            case XmlSeverityType.Warning:
                tempmsg = tempmsg + "\r\n" + "WARNING: " + e.Message;
                break;
        }
    });

if (schema == null)
{
    Console.WriteLine($"Schema cannot be read. Error is {tempmsg}.");
    return;
}

XmlSchemaSet schemaSet = new();
schemaSet.Add(schema);
schemaSet.Compile();

EnumClass ec = new()
{
    EnumeratedTypes = schema.Items.OfType<XmlSchemaSimpleType>()
 .Where(s => (s.Content is XmlSchemaSimpleTypeRestriction str)
     && str.Facets.OfType<XmlSchemaEnumerationFacet>().Any())
 .Select(c => new EnumTypes
 {
     TypeName = c.Name,
     EnumerationValues = (c.Content as XmlSchemaSimpleTypeRestriction)!
            .Facets.OfType<XmlSchemaEnumerationFacet>().Select(d => d.Value)
 })
};

string jsonString = JsonSerializer.Serialize(ec);
Console.WriteLine(jsonString);

public class EnumClass
{
    public IEnumerable<EnumTypes>? EnumeratedTypes { get; set; }
}

public class EnumTypes
{
    public string? TypeName { get; set; }
    public IEnumerable<string?>? EnumerationValues { get; set; }
}
