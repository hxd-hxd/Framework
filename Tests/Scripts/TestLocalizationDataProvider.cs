using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Localization;
using Framework.LocalizationSimple;

namespace Framework.Test
{
    public class TestLocalizationDataProvider : MonoBehaviour
    {
        public LocalizationDataOnlyLangProvider<LocalizationDataString> _stringProvider;
        public LocalizationDataOnlyLangProvider<LocalizationDataSprite> _spriteProvider;

        // Start is called before the first frame update
        void Start()
        {
        
        }

    }
}
