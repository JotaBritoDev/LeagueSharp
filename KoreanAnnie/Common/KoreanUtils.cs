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
        public static Func<CommonMenu, string, bool> GetParamBool = (cm, s) => cm.MainMenu.Item(ParamName(cm, s)).GetValue<bool>();
        public static Func<CommonMenu, string, int> GetParamInt = (cm, s) => cm.MainMenu.Item(ParamName(cm, s)).GetValue<Slider>().Value;
        public static Func<CommonMenu, string, bool> GetParamKeyBind = (cm, s) => cm.MainMenu.Item(ParamName(cm, s)).GetValue<KeyBind>().Active;
        public static Func<CommonMenu, string, MenuItem> GetParam = (cm, s) => cm.MainMenu.Item(ParamName(cm, s));
        public static Action<CommonMenu, string, bool> SetValueBool = (cm, s, b) => cm.MainMenu.Item(ParamName(cm, s)).SetValue(b);
    }
}
