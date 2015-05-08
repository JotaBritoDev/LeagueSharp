using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;

namespace KoreanAnnie
{
    static class KoreanUtils
    {
        public static Func<CommonMenu, string, string> ParamName = (cm, s) => string.Format("{0}.{1}", cm.MainMenu.Name, s);
        public static Func<CommonMenu, string, MenuItem> GetParam = (cm, s) => cm.MainMenu.Item(ParamName(cm, s));

        public static Func<CommonMenu, string, bool> GetParamBool = (cm, s) => cm.MainMenu.Item(ParamName(cm, s)).GetValue<bool>();
        public static Func<CommonMenu, string, int> GetParamSlider = (cm, s) => cm.MainMenu.Item(ParamName(cm, s)).GetValue<Slider>().Value;
        public static Func<CommonMenu, string, int> GetParamInt = (cm, s) => cm.MainMenu.Item(ParamName(cm, s)).GetValue<int>();
        public static Func<CommonMenu, string, bool> GetParamKeyBind = (cm, s) => cm.MainMenu.Item(ParamName(cm, s)).GetValue<KeyBind>().Active;
        public static Func<CommonMenu, string, int> GetParamStringList = (cm, s) => cm.MainMenu.Item(ParamName(cm, s)).GetValue<StringList>().SelectedIndex;

        public static Action<CommonMenu, string, bool> SetValueBool = (cm, s, b) => cm.MainMenu.Item(ParamName(cm, s)).SetValue(b);
        public static Action<CommonMenu, string, Slider> SetValueSlider = (cm, s, slider) => cm.MainMenu.Item(ParamName(cm, s)).SetValue<Slider>(slider);
        public static Action<CommonMenu, string, int> SetValueInt = (cm, s, i) => cm.MainMenu.Item(ParamName(cm, s)).SetValue<int>(i);
    }
}
