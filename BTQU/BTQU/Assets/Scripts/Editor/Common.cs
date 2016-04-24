using BTQLib;
using Newtonsoft.Json;
using System.IO;

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
    }
}
