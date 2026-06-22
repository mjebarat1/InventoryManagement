import { useState, useEffect } from 'react';

import Box from '@mui/material/Box';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import CircularProgress from '@mui/material/CircularProgress';

import { pingApi } from 'src/api';
import { useTranslate } from 'src/locales';

const RETRY_DELAY_MS = 2000;

type ApiAvailabilityGateProps = {
  children: React.ReactNode;
};

export function ApiAvailabilityGate({ children }: ApiAvailabilityGateProps) {
  const { t } = useTranslate();
  const [isAvailable, setIsAvailable] = useState(false);

  useEffect(() => {
    const abortController = new AbortController();
    let isDisposed = false;
    let retryTimer: ReturnType<typeof setTimeout> | undefined;

    const checkAvailability = async () => {
      try {
        await pingApi(abortController.signal);
        if (!isDisposed) setIsAvailable(true);
      } catch {
        if (!isDisposed) {
          retryTimer = setTimeout(checkAvailability, RETRY_DELAY_MS);
        }
      }
    };

    checkAvailability();

    return () => {
      isDisposed = true;
      abortController.abort();
      if (retryTimer) clearTimeout(retryTimer);
    };
  }, []);

  if (isAvailable) return children;

  return (
    <Box
      sx={{
        px: 3,
        minHeight: '100vh',
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
      }}
    >
      <Stack spacing={3} alignItems="center" sx={{ maxWidth: 520, textAlign: 'center' }}>
        <CircularProgress />
        <Typography variant="h4">{t('apiAvailability.title')}</Typography>
        <Typography sx={{ color: 'text.secondary' }}>
          {t('apiAvailability.description')}
        </Typography>
      </Stack>
    </Box>
  );
}
