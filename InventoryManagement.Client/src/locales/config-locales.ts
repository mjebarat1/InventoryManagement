export type LanguageValue = 'fr' | 'en';

export const fallbackLanguage: LanguageValue = 'fr';
export const supportedLanguages: LanguageValue[] = ['fr', 'en'];

export const allLanguages = [
  {
    value: 'fr' as const,
    labelKey: 'languages.fr',
    icon: '/assets/icons/flags/ic-flag-fr.svg',
  },
  {
    value: 'en' as const,
    labelKey: 'languages.en',
    icon: '/assets/icons/flags/ic-flag-en.svg',
  },
];
