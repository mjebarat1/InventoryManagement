import Box from '@mui/material/Box';
import Card from '@mui/material/Card';
import Alert from '@mui/material/Alert';
import Table from '@mui/material/Table';
import Button from '@mui/material/Button';
import Tooltip from '@mui/material/Tooltip';
import TableRow from '@mui/material/TableRow';
import TableBody from '@mui/material/TableBody';
import TableCell from '@mui/material/TableCell';
import TableHead from '@mui/material/TableHead';
import Typography from '@mui/material/Typography';
import TableContainer from '@mui/material/TableContainer';

import { useTranslate } from 'src/locales';
import { DashboardContent } from 'src/layouts/dashboard';

import { Iconify } from 'src/components/iconify';
import { Scrollbar } from 'src/components/scrollbar';

const columnKeys = [
  'articles.reference',
  'articles.name',
  'articles.type',
  'articles.priceExcludingTax',
  'articles.priceIncludingTax',
  'articles.vat',
  'articles.stock',
  'articles.status',
  'articles.actions',
] as const;

export function ArticlesView() {
  const { t } = useTranslate();

  return (
    <DashboardContent>
      <Box sx={{ mb: 4, display: 'flex', alignItems: 'center', gap: 2 }}>
        <Box sx={{ flexGrow: 1 }}>
          <Typography variant="h4">{t('articles.title')}</Typography>
          <Typography sx={{ mt: 1, color: 'text.secondary' }}>
            {t('articles.description')}
          </Typography>
        </Box>

        <Tooltip title={t('articles.addArticleUnavailable')}>
          <span>
            <Button
              disabled
              variant="contained"
              color="inherit"
              startIcon={<Iconify icon="mingcute:add-line" />}
            >
              {t('articles.addArticle')}
            </Button>
          </span>
        </Tooltip>
      </Box>

      <Alert severity="info" sx={{ mb: 3 }}>
        {t('articles.apiUnavailable')}
      </Alert>

      <Card>
        <Scrollbar>
          <TableContainer sx={{ overflow: 'unset' }}>
            <Table sx={{ minWidth: 1100 }}>
              <TableHead>
                <TableRow>
                  {columnKeys.map((columnKey) => (
                    <TableCell key={columnKey}>{t(columnKey)}</TableCell>
                  ))}
                </TableRow>
              </TableHead>
              <TableBody>
                <TableRow>
                  <TableCell colSpan={columnKeys.length} align="center" sx={{ py: 10 }}>
                    <Typography color="text.secondary">{t('articles.noData')}</Typography>
                  </TableCell>
                </TableRow>
              </TableBody>
            </Table>
          </TableContainer>
        </Scrollbar>
      </Card>
    </DashboardContent>
  );
}
