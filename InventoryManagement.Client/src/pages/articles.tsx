import { CONFIG } from 'src/config-global';
import { useTranslate } from 'src/locales';

import { ArticlesView } from 'src/sections/article/view';

export default function Page() {
  const { t } = useTranslate();

  return (
    <>
      <title>{`${t('articles.title')} - ${CONFIG.appName}`}</title>
      <meta name="description" content={t('articles.description')} />

      <ArticlesView />
    </>
  );
}
