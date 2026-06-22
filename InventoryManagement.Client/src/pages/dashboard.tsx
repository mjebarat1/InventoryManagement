import { CONFIG } from 'src/config-global';
import { useTranslate } from 'src/locales';

import { OverviewAnalyticsView as DashboardView } from 'src/sections/overview/view';

export default function Page() {
  const { t } = useTranslate();

  return (
    <>
      <title>{`${t('navigation.dashboard')} - ${CONFIG.appName}`}</title>
      <meta name="description" content={t('app.description')} />

      <DashboardView />
    </>
  );
}
