using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Localization;

namespace Framework.LocalizationSimple
{
    /// <summary>字符串本地化组件，逻辑功能导向的</summary>
    public class LocalizationCompSelectable : LocalizationCompBase
    {
        public LocalizationItemSelectable _item;

        public override void SetLanguage(string language)
        {
            base.SetLanguage(language);

            var cur = language;
            var def = _defaultLanguage;

            SetItemLanguage(_item, cur, def);
        }

        public override void SetLanguage(ILanguageProvider languageProvider)
        {
            base.SetLanguage(languageProvider);

            var cur = _currentProvider;
            var def = _defaultProvider;

            SetItemLanguage(_item, cur, def);
        }

    }
}