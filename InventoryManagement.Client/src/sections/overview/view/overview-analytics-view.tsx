import Card from '@mui/material/Card';
import Stack from '@mui/material/Stack';
import Button from '@mui/material/Button';
import Typography from '@mui/material/Typography';

import { RouterLink } from 'src/routes/components';

import { useTranslate } from 'src/locales';
import { DashboardContent } from 'src/layouts/dashboard';

import { Iconify } from 'src/components/iconify';

export function OverviewAnalyticsView() {
  const { t } = useTranslate();

  return (
    <DashboardContent sx={{ justifyContent: 'center' }}>
      <Card sx={{ width: 1, maxWidth: 800, mx: 'auto', p: { xs: 3, md: 5 } }}>
        <Stack spacing={3} alignItems="flex-start">
          <Typography variant="h4">{t('dashboard.title')}</Typography>
          <Typography sx={{ color: 'text.secondary', maxWidth: 720 }}>
            {t('dashboard.description')}
          </Typography>
          <Button
            component={RouterLink}
            href="/articles"
            variant="contained"
            startIcon={<Iconify icon="solar:cart-3-bold" />}
          >
            {t('dashboard.openArticles')}
          </Button>
        </Stack>
      </Card>
    </DashboardContent>
  );
}
