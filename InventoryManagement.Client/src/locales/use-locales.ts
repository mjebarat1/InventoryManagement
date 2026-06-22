import { useCallback } from 'react';
import { useTranslation } from 'react-i18next';

import { allLanguages, fallbackLanguage } from './config-locales';

import type { LanguageValue } from './config-locales';

export function useTranslate() {
  const { t, i18n } = useTranslation();

  const currentLanguage =
    allLanguages.find((language) => language.value === i18n.resolvedLanguage) ??
    allLanguages.find((language) => language.value === fallbackLanguage)!;

  const onChangeLanguage = useCallback(
    (language: LanguageValue) => i18n.changeLanguage(language),
    [i18n]
  );

  return { t, currentLanguage, onChangeLanguage };
}
