using System.Text;
using Yubico.YubiKey;
using Yubico.YubiKey.Piv;

public static class Program
{
    public static void Main()
    {
        // The SDK may return zero, one, or more YubiKeys. If there
        // was more than one, let's simply use the first.
        Console.WriteLine("Look for YubiKey Devise");

        IYubiKeyDevice yubiKey = YubiKeyDevice.FindAll().First();
        
        Console.WriteLine(string.Format("Found {0}", yubiKey.SerialNumber));      

        var collectorObj = new Simple39KeyCollector();

         // Open a sesion to the PIV application on the YubiKey that
        // we selected in the previous step.
        using (PivSession piv = new PivSession(yubiKey))
        {

            piv.KeyCollector = collectorObj.Simple39KeyCollectorDelegate;

            var publicKey = piv.GenerateKeyPair(
                PivSlot.CardAuthentication, 
                PivAlgorithm.Rsa2048, 
                PivPinPolicy.Never, 
                PivTouchPolicy.Never);  
                
            Console.WriteLine(string.Format("Public Key {0}.", publicKey.YubiKeyEncodedPublicKey.ToString()));

            byte[] signature1 = piv.Sign(PivSlot.CardAuthentication, Helper.GetDigestData(PivAlgorithm.Rsa2048));
            
            Console.WriteLine(string.Format("Signature {0}.", signature1[0]));

            Console.WriteLine("Write some content");

            var metacoIntent = new MetacoIntent("MyMetacoIntentPayload");

            using (var pivSession = new PivSession(yubiKey))
            {
                pivSession.KeyCollector = collectorObj.Simple39KeyCollectorDelegate;;

                pivSession.WriteObject(metacoIntent);
            }

            using (var pivSession = new PivSession(yubiKey))
            {
                pivSession.KeyCollector = collectorObj.Simple39KeyCollectorDelegate;;

                var storedMetacoIntent = pivSession.ReadObject<MetacoIntent>();

                Console.WriteLine(string.Format("Stored Data: {0}.", storedMetacoIntent.Payload));
            }
        }
    }
}