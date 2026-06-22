import { CONFIG } from 'src/config-global';
import { useTranslate } from 'src/locales';

import { ArticleDetailsView } from 'src/sections/article/view';

export default function Page() {
  const { t } = useTranslate();
  return (
    <>
      <title>{`${t('articleDetails.title')} - ${CONFIG.appName}`}</title>
      <ArticleDetailsView />
    </>
  );
}
