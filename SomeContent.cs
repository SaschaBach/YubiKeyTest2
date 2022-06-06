using Yubico.Core.Tlv;
using Yubico.YubiKey.Piv.Objects;

public class SomeContent : PivDataObject 
{
    private const int EncodingTag = 0x53;
    private const int NameTag = 0x3E;

    public string Payload {get; private set; }

    /*
    Table 1C: Undefined DataTags
    Number range	            Count	                            PIN required for read
    x005F0000 - 0x005FC100	    over 6 million possible numbers	    No
    0x005FC124 - 0x005FFEFF	    over 6 million possible numbers	    No
    0x005FFF02 - 0x005FFF0F	    14 numbers	                        No
    0x005FFF16 - 0x005FFFFF	    223 numbers	                        No
    
    It is possible for an application to store whatever information it wants on a YubiKey 
    under an undefined DataTag. However, there are space limitations. 
    
    It is possible to store at most approximately 3,052 bytes under any single undefined DataTag, 
    and the total space on a YubiKey for all storage is about 51,000 bytes.
    */
    private int MetacoIntentDataTag = 0x005FC100;

    public SomeContent()
    {
        Console.WriteLine("Create a new empty instance of MetacoIntent.");
        DataTag = MetacoIntentDataTag;
        IsEmpty = true;
    }

    public SomeContent(string Name)
    {
        Console.WriteLine("Create a new instance of MetacoIntent.");
        DataTag = MetacoIntentDataTag;
        IsEmpty = false;

        this.Payload = Name;
    }

    public override byte[] Encode()
    {
        Console.WriteLine("enter Encode()");
        Console.WriteLine(this.Payload);


        var tlvWriter = new TlvWriter();
        ReadOnlySpan<byte> emptySpan = ReadOnlySpan<byte>.Empty;
        using (tlvWriter.WriteNestedTlv(EncodingTag))
            {
                tlvWriter.WriteString(NameTag, this.Payload, System.Text.Encoding.ASCII);
            }

            byte[] returnValue = tlvWriter.Encode();

            Console.WriteLine(returnValue.ToString());

            tlvWriter.Clear();
            return returnValue;
    }

    public override int GetDefinedDataTag() => MetacoIntentDataTag;

    public override bool TryDecode(ReadOnlyMemory<byte> encodedData)
    {
           
        Console.WriteLine("enter TryDecode()");

            if (encodedData.Length == 0)
            {
                Console.WriteLine("no input");

                return true;
            }

            var tlvReader = new TlvReader(encodedData);
            bool isValid = tlvReader.TryReadNestedTlv(out tlvReader, EncodingTag);
            
            isValid = tlvReader.TryReadString(out string storedName, NameTag, System.Text.Encoding.ASCII);
            
            Console.WriteLine(storedName);
            Console.WriteLine(isValid);

            this.Payload = storedName;

            return isValid;
    }
}
 