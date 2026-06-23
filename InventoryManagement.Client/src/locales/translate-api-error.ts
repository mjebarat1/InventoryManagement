import type { TFunction } from 'i18next';

import { ApiError } from 'src/api';

export function translateApiError(error: unknown, t: TFunction): string {
  if (!(error instanceof ApiError)) {
    return t('common.error');
  }

  if (!error.code) {
    return error.message;
  }

  return t(`errors.${error.code}`, {
    ...error.parameters,
    defaultValue: t('common.error'),
  });
}
