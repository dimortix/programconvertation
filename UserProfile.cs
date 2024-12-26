using System.Collections.Generic;

namespace ConverterApp
{
    public class UserProfile
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public List<string> FavoriteConversions { get; set; } = new List<string>();
        public Dictionary<string, double> CustomRates { get; set; } = new Dictionary<string, double>();
    }
}