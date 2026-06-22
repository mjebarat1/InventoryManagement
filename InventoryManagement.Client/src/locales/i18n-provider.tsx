import i18n from 'i18next';
import { useEffect } from 'react';
import LanguageDetector from 'i18next-browser-languagedetector';
import { useTranslation, I18nextProvider, initReactI18next } from 'react-i18next';

import en from './translations/en.json';
import fr from './translations/fr.json';
import { fallbackLanguage, supportedLanguages } from './config-locales';

i18n
  .use(LanguageDetector)
  .use(initReactI18next)
  .init({
    resources: {
      fr: { translation: fr },
      en: { translation: en },
    },
    fallbackLng: fallbackLanguage,
    supportedLngs: supportedLanguages,
    interpolation: { escapeValue: false },
    detection: {
      order: ['localStorage', 'navigator'],
      caches: ['localStorage'],
    },
  });

type I18nProviderProps = {
  children: React.ReactNode;
};

function DocumentLanguageSync() {
  const { i18n: i18nInstance } = useTranslation();

  useEffect(() => {
    document.documentElement.lang = i18nInstance.resolvedLanguage ?? fallbackLanguage;
  }, [i18nInstance.resolvedLanguage]);

  return null;
}

export function I18nProvider({ children }: I18nProviderProps) {
  return (
    <I18nextProvider i18n={i18n}>
      <DocumentLanguageSync />
      {children}
    </I18nextProvider>
  );
}
