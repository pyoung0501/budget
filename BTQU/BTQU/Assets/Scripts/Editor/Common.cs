using BTQLib;
using Newtonsoft.Json;
using System.IO;
using System;

namespace BTQ
{
    public static class Common
    {
        public const string ProfilesDirectory = "Assets/Data/Profiles";

        public static void SaveProfile(Profile profile)
        {
            Directory.CreateDirectory(Common.ProfilesDirectory);

            using (StreamWriter stream = new StreamWriter(Common.ProfilesDirectory + "/" + profile.Name + ".profile"))
            {
                stream.Write(JsonConvert.SerializeObject(profile, Formatting.Indented, new JsonSerializerSettings(){ DateTimeZoneHandling = DateTimeZoneHandling.Utc }));
            }
        }

        public class ProfileConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(Profile);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                Profile profile = new Profile();

                serializer.Deserialize<>

                return profile;
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }
    }
}
