using Xamarin.Forms;

namespace AE_Xamarin.Managers
{
    public class AppTheme
    {
        public Color TextColor { get; private set; }
        public Color NavBarColor { get; private set; }
        public Color AccentColor { get; private set; }
        public Color BackgroundColor { get; private set; }

        /// <summary>
        /// Creates An App Theme Used By The AppThemeManager.
        /// </summary>
        /// <param name="TxtColor">The Text Color From Known Colors.</param>
        /// <param name="NavColor">The Navbar Color From Known Colors.</param>
        /// <param name="AcntColor">The Accent Color From Known Colors.</param>
        /// <param name="BkgdColor">The Background Color From Known Colors.</param>
        public AppTheme(Color TxtColor, Color NavColor, Color AcntColor, Color BkgdColor)
        {
            TextColor = TxtColor;
            NavBarColor = NavColor;
            AccentColor = AcntColor;
            BackgroundColor = BkgdColor;
        }

        /// <summary>
        /// Creates An App Theme Used By The AppThemeManager.
        /// </summary>
        /// <param name="TxtColor">The Text Color From Hex.</param>
        /// <param name="NavColor">The Navbar Color From Hex.</param>
        /// <param name="AcntColor">The Accent Color From Hex.</param>
        /// <param name="BkgdColor">The Background Color From Hex.</param>
        public AppTheme(string TxtColor, string NavColor, string AcntColor, string BkgdColor)
        {
            TextColor = Color.FromHex(TxtColor.Contains("#") ? TxtColor : $"#{TxtColor}");
            NavBarColor = Color.FromHex(NavColor.Contains("#") ? NavColor : $"#{NavColor}");
            AccentColor = Color.FromHex(AcntColor.Contains("#") ? AcntColor : $"#{AcntColor}");
            BackgroundColor = Color.FromHex(BkgdColor.Contains("#") ? BkgdColor : $"#{BkgdColor}");
        }
    }
}
