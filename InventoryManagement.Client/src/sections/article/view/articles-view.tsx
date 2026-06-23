import type { ArticleKind, SortDirection, ArticleSummary, ArticleSortField, ArticleActivityFilter } from 'src/api';

import { useNavigate } from 'react-router-dom';
import { useState, useEffect, useCallback } from 'react';

import Box from '@mui/material/Box';
import Chip from '@mui/material/Chip';
import Card from '@mui/material/Card';
import Alert from '@mui/material/Alert';
import Table from '@mui/material/Table';
import Dialog from '@mui/material/Dialog';
import Button from '@mui/material/Button';
import Select from '@mui/material/Select';
import MenuItem from '@mui/material/MenuItem';
import Snackbar from '@mui/material/Snackbar';
import TableRow from '@mui/material/TableRow';
import TextField from '@mui/material/TextField';
import TableBody from '@mui/material/TableBody';
import TableCell from '@mui/material/TableCell';
import TableHead from '@mui/material/TableHead';
import InputLabel from '@mui/material/InputLabel';
import Typography from '@mui/material/Typography';
import FormControl from '@mui/material/FormControl';
import DialogTitle from '@mui/material/DialogTitle';
import DialogContent from '@mui/material/DialogContent';
import DialogActions from '@mui/material/DialogActions';
import TableSortLabel from '@mui/material/TableSortLabel';
import TableContainer from '@mui/material/TableContainer';
import TablePagination from '@mui/material/TablePagination';
import CircularProgress from '@mui/material/CircularProgress';

import { useTranslate } from 'src/locales';
import { DashboardContent } from 'src/layouts/dashboard';
import { ApiError, searchArticles, deactivateArticle } from 'src/api';

import { Iconify } from 'src/components/iconify';
import { Scrollbar } from 'src/components/scrollbar';

type Filters = { searchTerm: string; type: '' | ArticleKind; activityFilter: ArticleActivityFilter };
const initialFilters: Filters = { searchTerm: '', type: '', activityFilter: 'Active' };

export function ArticlesView() {
  const { t, currentLanguage } = useTranslate();
  const locale = currentLanguage.value === 'fr' ? 'fr-FR' : 'en-GB';
  const navigate = useNavigate();
  const [items, setItems] = useState<ArticleSummary[]>([]);
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(20);
  const [totalItems, setTotalItems] = useState(0);
  const [sortBy, setSortBy] = useState<ArticleSortField>('Reference');
  const [sortDirection, setSortDirection] = useState<SortDirection>('Asc');
  const [filters, setFilters] = useState<Filters>(initialFilters);
  const [appliedFilters, setAppliedFilters] = useState<Filters>(initialFilters);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);
  const [articleToDeactivate, setArticleToDeactivate] = useState<ArticleSummary | null>(null);
  const [deactivating, setDeactivating] = useState(false);
  const [notification, setNotification] = useState<{ severity: 'success' | 'error'; message: string } | null>(null);

  const loadArticles = useCallback((signal?: AbortSignal) => {
    setLoading(true);
    setError(null);
    return searchArticles({
      pageNumber: page + 1,
      pageSize,
      sortBy,
      sortDirection,
      type: appliedFilters.type || undefined,
      searchTerm: appliedFilters.searchTerm || undefined,
      activityFilter: appliedFilters.activityFilter,
    }, signal)
      .then((result) => {
        setItems(result.items);
        setTotalItems(result.totalItems);
      })
      .catch((caughtError) => {
        if (caughtError instanceof DOMException && caughtError.name === 'AbortError') return;
        setError(caughtError instanceof ApiError ? caughtError.message : t('common.error'));
      })
      .finally(() => setLoading(false));
  }, [appliedFilters, page, pageSize, sortBy, sortDirection, t]);

  useEffect(() => {
    const controller = new AbortController();
    loadArticles(controller.signal);
    return () => controller.abort();
  }, [loadArticles]);

  const applyFilters = () => {
    setPage(0);
    setAppliedFilters(filters);
  };

  const changeSort = (field: ArticleSortField) => {
    setPage(0);
    if (sortBy === field) setSortDirection((current) => current === 'Asc' ? 'Desc' : 'Asc');
    else {
      setSortBy(field);
      setSortDirection('Asc');
    }
  };

  const confirmDeactivation = async () => {
    if (!articleToDeactivate) return;
    setDeactivating(true);
    try {
      await deactivateArticle(articleToDeactivate.id);
      setArticleToDeactivate(null);
      setNotification({ severity: 'success', message: t('articleDeactivate.success') });
      if (items.length === 1 && page > 0) setPage((current) => current - 1);
      else await loadArticles();
    } catch (caughtError) {
      setNotification({
        severity: 'error',
        message: caughtError instanceof ApiError ? caughtError.message : t('common.error'),
      });
    } finally {
      setDeactivating(false);
    }
  };

  const sortableHeader = (label: string, field: ArticleSortField) => (
    <TableCell sortDirection={sortBy === field ? (sortDirection === 'Asc' ? 'asc' : 'desc') : false}>
      <TableSortLabel active={sortBy === field} direction={sortDirection === 'Asc' ? 'asc' : 'desc'} onClick={() => changeSort(field)}>
        {label}
      </TableSortLabel>
    </TableCell>
  );

  const formatPrices = (article: ArticleSummary, field: 'vatRate' | 'priceIncludingTax') => article.prices
    .map((price) => {
      const prefix = price.saleMode ? `${t(`saleModes.${price.saleMode === 'TakeAway' ? 'takeAway' : 'onSite'}`)}: ` : '';
      const value = field === 'vatRate'
        ? new Intl.NumberFormat(locale, { style: 'percent', maximumFractionDigits: 1 }).format(price.vatRate)
        : new Intl.NumberFormat(locale, { style: 'currency', currency: 'EUR' }).format(price.priceIncludingTax);
      return `${prefix}${value}`;
    })
    .join(' / ');

  return (
    <DashboardContent>
      <Snackbar open={notification !== null} autoHideDuration={4000} onClose={() => setNotification(null)}>
        <Alert severity={notification?.severity ?? 'success'} onClose={() => setNotification(null)}>{notification?.message}</Alert>
      </Snackbar>
      <Box sx={(theme) => ({ mb: 4, p: { xs: 2.5, md: 3.5 }, display: 'flex', alignItems: 'center', gap: 2, borderRadius: 3, color: 'common.white', background: `linear-gradient(135deg, ${theme.vars.palette.primary.main} 0%, ${theme.vars.palette.secondary.dark} 100%)`, boxShadow: theme.vars.customShadows.primary })}>
        <Box sx={{ flexGrow: 1 }}><Typography variant="h4">{t('articles.title')}</Typography><Typography sx={{ mt: 1, color: 'rgba(255,255,255,0.78)' }}>{t('articles.description')}</Typography></Box>
        <Button variant="contained" startIcon={<Iconify icon="mingcute:add-line" />} onClick={() => navigate('/articles/new')} sx={{ color: 'primary.dark', bgcolor: 'common.white', '&:hover': { bgcolor: 'primary.lighter' } }}>
          {t('articles.addArticle')}
        </Button>
      </Box>

      <Card sx={{ p: 2.5, mb: 3, borderTop: 3, borderTopColor: 'secondary.main' }}>
        <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', md: '1fr 220px 220px auto' }, gap: 2 }}>
          <TextField size="small" label={t('articles.searchTerm')} value={filters.searchTerm} onChange={(event) => setFilters({ ...filters, searchTerm: event.target.value })} />
          <FormControl size="small"><InputLabel id="filter-type-label">{t('articles.type')}</InputLabel><Select labelId="filter-type-label" label={t('articles.type')} value={filters.type} onChange={(event) => setFilters({ ...filters, type: event.target.value as Filters['type'] })}>
            <MenuItem value="">{t('common.all')}</MenuItem><MenuItem value="Food">{t('articleTypes.food')}</MenuItem><MenuItem value="NonFood">{t('articleTypes.nonFood')}</MenuItem>
          </Select></FormControl>
          <FormControl size="small"><InputLabel id="filter-activity-label">{t('articles.activity')}</InputLabel><Select labelId="filter-activity-label" label={t('articles.activity')} value={filters.activityFilter} onChange={(event) => setFilters({ ...filters, activityFilter: event.target.value as ArticleActivityFilter })}>
            <MenuItem value="Active">{t('articleStatuses.active')}</MenuItem><MenuItem value="Inactive">{t('articleStatuses.inactive')}</MenuItem><MenuItem value="All">{t('common.all')}</MenuItem>
          </Select></FormControl>
          <Button variant="outlined" onClick={applyFilters}>{t('common.search')}</Button>
        </Box>
      </Card>

      {error && <Alert severity="error" sx={{ mb: 3 }}>{error}</Alert>}
      <Card>
        <Scrollbar><TableContainer sx={{ overflow: 'unset' }}><Table sx={{ minWidth: 1100 }}>
          <TableHead><TableRow>
            {sortableHeader(t('articles.reference'), 'Reference')}{sortableHeader(t('articles.name'), 'Name')}{sortableHeader(t('articles.type'), 'Type')}{sortableHeader(t('articles.priceExcludingTax'), 'PriceExcludingTax')}
            <TableCell>{t('articles.priceIncludingTax')}</TableCell><TableCell>{t('articles.vat')}</TableCell><TableCell>{t('articles.stock')}</TableCell><TableCell>{t('articles.status')}</TableCell><TableCell>{t('articles.actions')}</TableCell>
          </TableRow></TableHead>
          <TableBody>
            {loading ? (
              <TableRow>
                <TableCell colSpan={9} align="center" sx={{ py: 10 }}>
                  <CircularProgress aria-label={t('common.loading')} />
                </TableCell>
              </TableRow>
            ) : items.length === 0 ? <TableRow><TableCell colSpan={9} align="center" sx={{ py: 10 }}><Typography color="text.secondary">{t('articles.noData')}</Typography></TableCell></TableRow> : items.map((article) => (
              <TableRow hover key={article.id} sx={{ '&:hover': { bgcolor: 'primary.lighter' } }}>
                <TableCell sx={{ fontWeight: 600, color: 'primary.dark' }}>{article.reference}</TableCell><TableCell sx={{ fontWeight: 600 }}>{article.name}</TableCell><TableCell><Chip size="small" label={t(`articleTypes.${article.type === 'Food' ? 'food' : 'nonFood'}`)} color={article.type === 'Food' ? 'success' : 'secondary'} /></TableCell>
                <TableCell>{new Intl.NumberFormat(locale, { style: 'currency', currency: 'EUR' }).format(article.priceExcludingTax)}</TableCell>
                <TableCell>{formatPrices(article, 'priceIncludingTax')}</TableCell><TableCell>{formatPrices(article, 'vatRate')}</TableCell><TableCell><Chip size="small" label={article.totalStock} color={article.totalStock > 0 ? 'success' : 'default'} /></TableCell><TableCell><Chip size="small" label={t(`articleStatuses.${article.isActive ? 'active' : 'inactive'}`)} color={article.isActive ? 'success' : 'default'} variant={article.isActive ? 'filled' : 'outlined'} /></TableCell>
                <TableCell><Box sx={{ display: 'flex', gap: 1 }}><Button size="small" onClick={() => navigate(`/articles/${article.id}`)}>{t('articles.view')}</Button><Button size="small" color="error" disabled={!article.isActive} onClick={() => setArticleToDeactivate(article)}>{t('articles.deactivate')}</Button></Box></TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table></TableContainer></Scrollbar>
        <TablePagination
          component="div" count={totalItems} page={page} rowsPerPage={pageSize}
          onPageChange={(_, newPage) => setPage(newPage)}
          onRowsPerPageChange={(event) => { setPageSize(Number(event.target.value)); setPage(0); }}
          rowsPerPageOptions={[10, 20, 50, 100]}
          labelRowsPerPage={t('articles.rowsPerPage')}
        />
      </Card>
      <Dialog open={articleToDeactivate !== null} onClose={() => !deactivating && setArticleToDeactivate(null)} maxWidth="sm" fullWidth>
        <DialogTitle>{t('articleDeactivate.title')}</DialogTitle>
        <DialogContent>
          <Typography>{t('articleDeactivate.message')}</Typography>
          <Typography sx={{ mt: 2 }} color="text.secondary">{t('articleDeactivate.details')}</Typography>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setArticleToDeactivate(null)} disabled={deactivating}>{t('common.cancel')}</Button>
          <Button color="error" variant="contained" onClick={confirmDeactivation} disabled={deactivating}>{deactivating ? t('common.loading') : t('articleDeactivate.confirm')}</Button>
        </DialogActions>
      </Dialog>
    </DashboardContent>
  );
}
