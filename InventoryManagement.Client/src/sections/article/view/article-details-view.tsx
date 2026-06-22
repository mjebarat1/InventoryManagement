import type { ArticlePrice, ArticleDetails } from 'src/api';

import { useState, useEffect } from 'react';
import { useParams, useLocation, useNavigate } from 'react-router-dom';

import Box from '@mui/material/Box';
import Card from '@mui/material/Card';
import Alert from '@mui/material/Alert';
import Table from '@mui/material/Table';
import Button from '@mui/material/Button';
import Snackbar from '@mui/material/Snackbar';
import TableRow from '@mui/material/TableRow';
import TableBody from '@mui/material/TableBody';
import TableCell from '@mui/material/TableCell';
import TableHead from '@mui/material/TableHead';
import Typography from '@mui/material/Typography';
import TableContainer from '@mui/material/TableContainer';
import CircularProgress from '@mui/material/CircularProgress';

import { useTranslate } from 'src/locales';
import { ApiError, getArticleById } from 'src/api';
import { DashboardContent } from 'src/layouts/dashboard';

const formatMoney = (value: number, locale: string) => new Intl.NumberFormat(locale, { style: 'currency', currency: 'EUR' }).format(value);
const formatVat = (value: number, locale: string) => new Intl.NumberFormat(locale, { style: 'percent', maximumFractionDigits: 1 }).format(value);

export function ArticleDetailsView() {
  const { id } = useParams();
  const { t, currentLanguage } = useTranslate();
  const locale = currentLanguage.value === 'fr' ? 'fr-FR' : 'en-GB';
  const location = useLocation();
  const navigate = useNavigate();
  const [article, setArticle] = useState<ArticleDetails | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [successOpen, setSuccessOpen] = useState(Boolean(location.state?.articleCreated));

  useEffect(() => {
    if (!id) return undefined;
    const controller = new AbortController();
    getArticleById(id, controller.signal)
      .then(setArticle)
      .catch((caughtError) => {
        if (caughtError instanceof DOMException && caughtError.name === 'AbortError') return;
        setError(caughtError instanceof ApiError ? caughtError.message : t('common.error'));
      });
    return () => controller.abort();
  }, [id, t]);

  if (!article && !error) {
    return <DashboardContent><Box sx={{ display: 'flex', justifyContent: 'center', py: 10 }}><CircularProgress /></Box></DashboardContent>;
  }

  if (error || !article) {
    return <DashboardContent><Alert severity="error">{error ?? t('articleDetails.notFound')}</Alert></DashboardContent>;
  }

  const renderPrices = (selector: (price: ArticlePrice) => number, formatter: (value: number) => string) => (
    <Box sx={{ display: 'grid', gap: 0.5 }}>
      {article.prices.map((price) => (
        <Typography key={price.saleMode ?? 'default'} variant="body2">
          {price.saleMode ? `${t(`saleModes.${price.saleMode === 'TakeAway' ? 'takeAway' : 'onSite'}`)}: ` : ''}
          {formatter(selector(price))}
        </Typography>
      ))}
    </Box>
  );

  const cards = [
    { label: t('articleDetails.totalStock'), value: String(article.totalStock) },
    { label: t('articleDetails.sellableStock'), value: article.sellableStock == null ? t('common.unavailable') : String(article.sellableStock) },
    { label: t('articleDetails.nonSellableStock'), value: article.nonSellableStock == null ? t('common.unavailable') : String(article.nonSellableStock) },
    { label: t('articles.priceExcludingTax'), value: formatMoney(article.priceExcludingTax, locale) },
  ];

  return (
    <DashboardContent>
      <Snackbar open={successOpen} autoHideDuration={4000} onClose={() => setSuccessOpen(false)}>
        <Alert severity="success" onClose={() => setSuccessOpen(false)}>{t('articleCreate.success')}</Alert>
      </Snackbar>
      <Box sx={(theme) => ({ mb: 3, p: { xs: 2.5, md: 3.5 }, display: 'flex', alignItems: 'center', gap: 2, borderRadius: 3, color: 'common.white', background: `linear-gradient(135deg, ${theme.vars.palette.primary.main} 0%, ${theme.vars.palette.secondary.dark} 100%)`, boxShadow: theme.vars.customShadows.primary })}>
        <Box sx={{ flexGrow: 1 }}>
          <Typography variant="h4">{article.name}</Typography>
          <Typography sx={{ color: 'rgba(255,255,255,0.78)' }}>{article.reference}</Typography>
        </Box>
        <Button onClick={() => navigate('/articles')} sx={{ color: 'common.white', borderColor: 'rgba(255,255,255,0.5)' }} variant="outlined">{t('articleDetails.backToList')}</Button>
      </Box>

      <Card sx={{ p: 3, mb: 3, borderLeft: 4, borderLeftColor: article.type === 'Food' ? 'success.main' : 'secondary.main' }}>
        <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', md: 'repeat(3, 1fr)' }, gap: 2 }}>
          <Info label={t('articles.reference')} value={article.reference} />
          <Info label={t('articles.name')} value={article.name} />
          <Info label={t('articles.type')} value={t(`articleTypes.${article.type === 'Food' ? 'food' : 'nonFood'}`)} />
          <Info label={t('articles.priceExcludingTax')} value={formatMoney(article.priceExcludingTax, locale)} />
          <Box><Typography variant="caption" color="text.secondary">{t('articles.priceIncludingTax')}</Typography>{renderPrices((price) => price.priceIncludingTax, (value) => formatMoney(value, locale))}</Box>
          <Box><Typography variant="caption" color="text.secondary">{t('articles.vat')}</Typography>{renderPrices((price) => price.vatRate, (value) => formatVat(value, locale))}</Box>
          {article.type === 'Food' && <Info label={t('articleCreate.allowedSaleModes')} value={article.allowedSaleModes.map((mode) => t(`saleModes.${mode === 'TakeAway' ? 'takeAway' : 'onSite'}`)).join(', ')} />}
        </Box>
      </Card>

      <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', sm: 'repeat(2, 1fr)', lg: 'repeat(6, 1fr)' }, gap: 2, mb: 3 }}>
        {cards.map((card, index) => <Card key={card.label} sx={{ p: 2, borderTop: 3, borderTopColor: ['primary.main', 'success.main', 'warning.main', 'secondary.main'][index] }}><Typography variant="caption" color="text.secondary">{card.label}</Typography><Typography variant="h6" sx={{ color: index === 1 ? 'success.dark' : 'text.primary' }}>{card.value}</Typography></Card>)}
        <Card sx={{ p: 2, borderTop: 3, borderTopColor: 'info.main' }}><Typography variant="caption" color="text.secondary">{t('articles.priceIncludingTax')}</Typography>{renderPrices((price) => price.priceIncludingTax, (value) => formatMoney(value, locale))}</Card>
        <Card sx={{ p: 2, borderTop: 3, borderTopColor: 'warning.main' }}><Typography variant="caption" color="text.secondary">{t('articles.vat')}</Typography>{renderPrices((price) => price.vatRate, (value) => formatVat(value, locale))}</Card>
      </Box>

      <Card sx={{ mb: 3, overflow: 'hidden' }}>
        <Typography variant="h6" sx={{ p: 3, pb: 1 }}>{t('articleDetails.movements')}</Typography>
        <TableContainer><Table>
          <TableHead><TableRow>{['date', 'movementType', 'quantity', 'expirationDate', 'packaging', 'comment'].map((key) => <TableCell key={key}>{t(`articleDetails.${key}`)}</TableCell>)}</TableRow></TableHead>
          <TableBody>
            {article.movements.length === 0 ? <TableRow><TableCell colSpan={6} align="center" sx={{ py: 6 }}>{t('articleDetails.noMovements')}</TableCell></TableRow> : article.movements.map((movement) => (
              <TableRow key={movement.id}>
                <TableCell>{new Intl.DateTimeFormat(locale, { dateStyle: 'short', timeStyle: 'short' }).format(new Date(movement.createdAt))}</TableCell>
                <TableCell>{t(`movementTypes.${movement.type}`)}</TableCell>
                <TableCell>{movement.quantity}</TableCell><TableCell>{movement.expirationDate ?? '—'}</TableCell>
                <TableCell>{movement.packagingLevel ? t(`packaging.${movement.packagingLevel}`) : '—'}</TableCell><TableCell>{movement.comment ?? '—'}</TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table></TableContainer>
      </Card>

      <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 2 }}>
        <Button variant="contained" disabled>{t('articleDetails.supply')}</Button>
        <Button variant="contained" disabled>{t('articleDetails.sale')}</Button>
        <Button variant="contained" disabled>{t('articleDetails.inventory')}</Button>
      </Box>
    </DashboardContent>
  );
}

function Info({ label, value }: { label: string; value: string }) {
  return <Box><Typography variant="caption" color="text.secondary">{label}</Typography><Typography>{value}</Typography></Box>;
}
