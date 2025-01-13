using Leapfrog.Core.Enums.RadioConfigEnums;

namespace Leapfrog.Core.Entities
{
    public class SecurityModel
    {
        public readonly string DEFAULT_PASSWORD = "EncomWirelessLeapfrog";
        public uint DevID1 { get; set; }
        public uint DevID2 { get; set; }
        public uint DevID3 { get; set; }
        public int Valid { get; set; }
        public readonly int MaxPasswordLength = 32;
        public string Password { get; set; } = string.Empty;
        public bool LibGood { get; set; } = true;
        public bool HashGood { get; set; } = false;

        public string DeviceID
        {
            get
            {
                return string.Format("{0:X8}", DevID1) + "-" + string.Format("{0:X8}", DevID2) + "-" + string.Format("{0:X8}", DevID3);
            }
        }

        public string ValidString
        {
            get
            {
                return (SecurityValid)Valid switch
                {
                    SecurityValid.INVALID => "Invalid Password",
                    SecurityValid.DEFAULT => "Valid Password (default)",
                    SecurityValid.FORCED => "Password Ignored",
                    SecurityValid.VALID => "Valid Password",
                    _ => "Unknown Password State",
                };
            }
        }

        public string Enabled
        {
            get
            {
                if (LibGood == true && HashGood == true)
                {
                    return "Library Enabled";
                }
                else if (LibGood == true && HashGood == false)
                {
                    return "Library Disabled";
                }
                else
                {
                    return "Library Corrupted";
                }
            }
        }

    }
}
