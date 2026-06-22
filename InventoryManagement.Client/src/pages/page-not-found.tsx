import { CONFIG } from 'src/config-global';
import { useTranslate } from 'src/locales';

import { NotFoundView } from 'src/sections/error';

export default function Page() {
  const { t } = useTranslate();

  return (
    <>
      <title>{`${t('errors.notFoundTitle')} - ${CONFIG.appName}`}</title>

      <NotFoundView />
    </>
  );
}
