import { CONFIG } from 'src/config-global';
import { useTranslate } from 'src/locales';

import { ArticleCreateView } from 'src/sections/article/view';

export default function Page() {
  const { t } = useTranslate();
  return (
    <>
      <title>{`${t('articleCreate.title')} - ${CONFIG.appName}`}</title>
      <ArticleCreateView />
    </>
  );
}
