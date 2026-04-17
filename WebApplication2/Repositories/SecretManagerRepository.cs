using Google.Cloud.SecretManager.V1;
using System.Text.Json;

namespace WebApplication2.Repositories
{

    public class SecretManagerRepository
    {
        public string GetSecret(string projectId, string key, string secretId = "secrets_class_demo", string versionId = "1")
        {
            // Create the Secret Manager client
            SecretManagerServiceClient client = SecretManagerServiceClient.Create();
            // Build the resource name of the secret version
            SecretVersionName secretVersionName = new SecretVersionName(projectId, secretId, versionId);
            // Access the secret version
            AccessSecretVersionResponse result = client.AccessSecretVersion(secretVersionName);
            // Return the secret payload as a string
            string keysInBlock = result.Payload.Data.ToStringUtf8();

            Newtonsoft.Json.JsonSerializer myDeserializer = new Newtonsoft.Json.JsonSerializer();

            
            dynamic keysInBlockAsObject = 
                myDeserializer.Deserialize<dynamic>(new Newtonsoft.Json.JsonTextReader(new System.IO.StringReader(keysInBlock)));

            string value = keysInBlockAsObject[key].ToString();

            return value;

        }
    }
}
